using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace BlazorErp.Services;

public class SpsAuthService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly SpsApiOptions _options;
    private readonly ILogger<SpsAuthService> _logger;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string? _cachedToken;
    private DateTime _tokenExpiresAt = DateTime.MinValue;

    public SpsAuthService(HttpClient httpClient, IOptions<SpsApiOptions> options, ILogger<SpsAuthService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_options.ApiToken) ||
        (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password));

    public async Task<string?> TryGetTokenAsync(CancellationToken ct = default)
    {
        if (!IsConfigured)
            return null;

        try
        {
            return await GetTokenAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось получить SPS API токен.");
            return null;
        }
    }

    public async Task<string> GetTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(_options.ApiToken))
            return _options.ApiToken;

        if (DateTime.UtcNow < _tokenExpiresAt && !string.IsNullOrWhiteSpace(_cachedToken))
            return _cachedToken;

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (DateTime.UtcNow < _tokenExpiresAt && !string.IsNullOrWhiteSpace(_cachedToken))
                return _cachedToken;

            using var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl)
            {
                Content = JsonContent.Create(new
                {
                    userName = _options.Username,
                    password = _options.Password,
                    roles = _options.Roles
                }, options: JsonOptions)
            };

            using var response = await _httpClient.SendAsync(request, ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"SPS auth failed: {content}");

            var tokenResponse = JsonSerializer.Deserialize<SpsTokenResponse>(content, JsonOptions);
            if (string.IsNullOrWhiteSpace(tokenResponse?.Data?.Token))
                throw new InvalidOperationException("SPS auth returned empty token.");

            var expiresSeconds = tokenResponse.Data.Expires > 0 ? tokenResponse.Data.Expires : 300;
            _tokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresSeconds - 30);
            _cachedToken = tokenResponse.Data.Token;
            return _cachedToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    public async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(
        HttpMethod method,
        string relativeUrl,
        CancellationToken ct = default)
    {
        var token = await GetTokenAsync(ct);
        var request = new HttpRequestMessage(method, relativeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private sealed class SpsTokenResponse
    {
        public SpsTokenData? Data { get; set; }
    }

    private sealed class SpsTokenData
    {
        public string Token { get; set; } = string.Empty;
        public int Expires { get; set; }
    }
}
