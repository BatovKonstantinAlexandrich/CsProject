using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorErp.Models.BookingApi;
using Microsoft.Extensions.Options;

namespace BlazorErp.Services;

public class StoBookingApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly BookingApiOptions _options;
    private readonly ILogger<StoBookingApiService> _logger;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string? _cachedToken;

    public StoBookingApiService(HttpClient httpClient, IOptions<BookingApiOptions> options, ILogger<StoBookingApiService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
    }

    public bool IsConfigured =>
        _options.Enabled &&
        (!string.IsNullOrWhiteSpace(_options.ApiToken) ||
         (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password)));

    public async Task<List<BookingServiceStation>> GetServiceStationsAsync(CancellationToken ct = default)
    {
        var result = await SendAsync<List<BookingServiceStation>>(HttpMethod.Get, "/api/booking/v2/llm/service_stations", ct);
        return result ?? [];
    }

    public async Task<List<BookingJob>> GetAllJobsAsync(CancellationToken ct = default)
    {
        var result = await SendAsync<List<BookingJob>>(HttpMethod.Get, "/api/booking/v2/llm/jobs", ct);
        return result ?? [];
    }

    public async Task<List<BookingJob>> GetStationJobsAsync(string serviceStationId, CancellationToken ct = default)
    {
        var result = await SendAsync<List<BookingJob>>(
            HttpMethod.Get,
            $"/api/booking/v2/llm/service_stations/id/{Uri.EscapeDataString(serviceStationId)}/jobs",
            ct);
        return result ?? [];
    }

    public async Task<List<BookingFreeSlotDay>> GetFreeSlotsAsync(
        string serviceStationId,
        IEnumerable<string> jobIds,
        List<object>? specificJobParameters = null,
        CancellationToken ct = default)
    {
        var body = new BookingSlotsRequest
        {
            JobIds = jobIds.ToList(),
            SpecificJobParameters = specificJobParameters ?? [],
            StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
            DaysAhead = 14
        };

        var result = await SendAsync<List<BookingFreeSlotDay>>(
            HttpMethod.Post,
            $"/api/booking/v2/llm/service_stations/id/{Uri.EscapeDataString(serviceStationId)}/slots",
            ct,
            body);
        return result ?? [];
    }

    public async Task<List<ClientBooking>> GetClientBookingsAsync(string phone, CancellationToken ct = default)
    {
        var normalized = NormalizePhone(phone);
        var result = await SendAsync<List<ClientBooking>>(
            HttpMethod.Get,
            $"/api/booking/v2/llm/customers/{Uri.EscapeDataString(normalized)}/bookings",
            ct);
        return result ?? [];
    }

    public async Task<CreateBookingResponse?> CreateBookingAsync(string phone, CreateBookingRequest request, CancellationToken ct = default)
    {
        var normalized = NormalizePhone(phone);
        return await SendAsync<CreateBookingResponse>(
            HttpMethod.Post,
            $"/api/booking/v2/llm/customers/{Uri.EscapeDataString(normalized)}/bookings",
            ct,
            request);
    }

    public async Task CancelBookingAsync(string phone, string bookingId, CancellationToken ct = default)
    {
        var normalized = NormalizePhone(phone);
        await SendAsync<object>(
            HttpMethod.Delete,
            $"/api/booking/v2/llm/customers/{Uri.EscapeDataString(normalized)}/bookings/{Uri.EscapeDataString(bookingId)}",
            ct);
    }

    public static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length == 10)
            return "7" + digits;
        if (digits.Length == 11 && digits.StartsWith('8'))
            return "7" + digits[1..];
        return digits;
    }

    public static string FormatApiTime(string hour) =>
        hour.Contains(':') ? hour.Split(':')[0] : hour;

    private async Task<T?> SendAsync<T>(HttpMethod method, string path, CancellationToken ct, object? body = null)
    {
        if (!IsConfigured)
            throw new InvalidOperationException("BookingApi не настроен в appsettings.json (BookingApi:ApiToken или Username/Password).");

        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(ct));

        if (body != null)
            request.Content = JsonContent.Create(body, options: JsonOptions);

        using var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);

        if (response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(content))
                return default;

            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }

        var message = TryParseError(content) ?? $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
        _logger.LogWarning("Booking API {Method} {Path} failed: {Message}", method, path, message);
        throw new BookingApiException(message, (int)response.StatusCode);
    }

    private static string? TryParseError(string content)
    {
        try
        {
            var error = JsonSerializer.Deserialize<BookingApiValidationError>(content, JsonOptions);
            if (error?.Detail.Count > 0)
                return string.Join("; ", error.Detail.Select(d => d.Message).Where(m => !string.IsNullOrWhiteSpace(m)));
        }
        catch { }

        return string.IsNullOrWhiteSpace(content) ? null : content;
    }

    private async Task<string> GetTokenAsync(CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(_options.ApiToken))
            return _options.ApiToken;

        if (!string.IsNullOrWhiteSpace(_cachedToken))
            return _cachedToken;

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrWhiteSpace(_cachedToken))
                return _cachedToken;

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/booking/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["username"] = _options.Username,
                    ["password"] = _options.Password
                })
            };

            using var response = await _httpClient.SendAsync(request, ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
                throw new BookingApiException($"Не удалось получить токен Booking API: {content}", (int)response.StatusCode);

            var token = JsonSerializer.Deserialize<BookingTokenResponse>(content, JsonOptions);
            if (string.IsNullOrWhiteSpace(token?.AccessToken))
                throw new BookingApiException("Booking API вернул пустой токен.", (int)response.StatusCode);

            _cachedToken = token.AccessToken;
            return _cachedToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }
}

public class BookingApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
