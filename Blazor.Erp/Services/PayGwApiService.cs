using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorErp.Models.Enums;
using BlazorErp.Models.PayGw;
using Microsoft.Extensions.Options;

namespace BlazorErp.Services;

public class PayGwApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly SpsApiOptions _spsOptions;
    private readonly PayGwApiOptions _options;
    private readonly SpsAuthService _authService;
    private readonly ILogger<PayGwApiService> _logger;

    public PayGwApiService(
        HttpClient httpClient,
        IOptions<SpsApiOptions> spsOptions,
        IOptions<PayGwApiOptions> options,
        SpsAuthService authService,
        ILogger<PayGwApiService> logger)
    {
        _httpClient = httpClient;
        _spsOptions = spsOptions.Value;
        _options = options.Value;
        _authService = authService;
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_spsOptions.BaseUrl))
            _httpClient.BaseAddress = new Uri(_spsOptions.BaseUrl.TrimEnd('/') + "/");
    }

    public bool IsConfigured => _options.Enabled && _authService.IsConfigured;

    public async Task<List<CheckoutPaymentOption>> GetCheckoutPaymentOptionsAsync(CancellationToken ct = default)
    {
        var options = new List<CheckoutPaymentOption>();

        if (IsConfigured)
        {
            try
            {
                var sources = await GetActiveSourcesAsync(ct);
                foreach (var source in sources)
                {
                    options.Add(new CheckoutPaymentOption
                    {
                        Key = $"source:{source.SourceId}",
                        Name = source.SourceName,
                        Description = BuildSourceDescription(source),
                        Method = MapSourceType(source.Type),
                        SourceId = source.SourceId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "PayGw API недоступен, используются демо-способы оплаты.");
            }
        }

        if (options.Count == 0)
        {
            options.Add(new CheckoutPaymentOption
            {
                Key = "SBP",
                Name = "СБП (Система быстрых платежей)",
                Method = PaymentMethod.SBP
            });
            options.Add(new CheckoutPaymentOption
            {
                Key = "Card",
                Name = "Банковская карта",
                Method = PaymentMethod.Card
            });
        }

        options.Add(new CheckoutPaymentOption
        {
            Key = "Cash",
            Name = "Наличные при получении",
            Method = PaymentMethod.Cash
        });

        return options;
    }

    public async Task<PaymentLinkResult?> GetPaymentLinkAsync(Guid orderUid, int? sourceId, CancellationToken ct = default)
    {
        if (!IsConfigured)
            return null;

        try
        {
            if (sourceId.HasValue)
            {
                var testLink = await SendAsync<PayGwMethodResult<PayGwTestPaymentLinkData>>(
                    HttpMethod.Get,
                    $"/api/paygwmanager/sources/{sourceId.Value}/order/{orderUid}/test",
                    ct);

                if (!string.IsNullOrWhiteSpace(testLink?.Data?.Url))
                {
                    return new PaymentLinkResult
                    {
                        PaymentUrl = testLink.Data.Url,
                        Message = testLink.Message
                    };
                }
            }

            var link = await SendAsync<PaymentsPaymentLinkResponse>(
                HttpMethod.Get,
                $"/api/payments/order/payment_link?order_uid={orderUid}",
                ct);

            if (!string.IsNullOrWhiteSpace(link?.Data?.PaymentUrl))
            {
                return new PaymentLinkResult
                {
                    PaymentUrl = link.Data.PaymentUrl,
                    Message = link.Message
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось получить ссылку на оплату для заказа {OrderUid}.", orderUid);
        }

        return null;
    }

    private async Task<List<LegalEntitySource>> GetActiveSourcesAsync(CancellationToken ct)
    {
        var sources = await SendAsync<List<LegalEntitySourceDto>>(
            HttpMethod.Get,
            $"/api/paygwmanager/legal_entities/{_options.LegalEntityId}/sources",
            ct) ?? [];

        return sources
            .Where(s => s.Active != false)
            .Select(s => new LegalEntitySource
            {
                SourceId = s.SourceId,
                SourceName = s.SourceName,
                Comment = s.Comment,
                Type = s.Type,
                Active = s.Active ?? true,
                Splitting = s.Splitting ?? false
            })
            .OrderBy(s => s.SourceName)
            .ToList();
    }

    private async Task<T?> SendAsync<T>(HttpMethod method, string path, CancellationToken ct)
    {
        var token = await _authService.GetTokenAsync(ct);
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        using var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("PayGw API {Method} {Path} failed: {Status} {Body}", method, path, (int)response.StatusCode, content);
            throw new InvalidOperationException($"PayGw API error: {content}");
        }

        if (string.IsNullOrWhiteSpace(content))
            return default;

        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    private static string BuildSourceDescription(LegalEntitySource source)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(source.Type))
            parts.Add(source.Type);
        if (!string.IsNullOrWhiteSpace(source.Comment))
            parts.Add(source.Comment);
        return parts.Count == 0 ? "Платёжная система из NgAppPayGwManager" : string.Join(" · ", parts);
    }

    private static PaymentMethod MapSourceType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return PaymentMethod.SBP;

        var normalized = type.ToLowerInvariant();
        if (normalized.Contains("card") || normalized.Contains("paykeeper") || normalized.Contains("acquiring"))
            return PaymentMethod.Card;

        return PaymentMethod.SBP;
    }

    private sealed class LegalEntitySourceDto
    {
        [JsonPropertyName("source_id")]
        public int SourceId { get; set; }

        [JsonPropertyName("source_name")]
        public string SourceName { get; set; } = string.Empty;

        public string? Comment { get; set; }
        public string? Type { get; set; }
        public bool? Active { get; set; }
        public bool? Splitting { get; set; }
    }

    private sealed class PayGwMethodResult<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    private sealed class PayGwTestPaymentLinkData
    {
        public bool Result { get; set; }
        public string? Url { get; set; }
    }

    private sealed class PaymentsPaymentLinkResponse
    {
        public PaymentsPaymentLinkData? Data { get; set; }
        public string? Message { get; set; }
        public bool Result { get; set; }
    }

    private sealed class PaymentsPaymentLinkData
    {
        [JsonPropertyName("payment_url")]
        public string? PaymentUrl { get; set; }
    }
}
