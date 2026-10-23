using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorErp.Models.Vendors;
using Microsoft.Extensions.Options;

namespace BlazorErp.Services;

public class VendorsSetupApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly SpsApiOptions _spsOptions;
    private readonly VendorsSetupApiOptions _options;
    private readonly SpsAuthService _authService;
    private readonly ILogger<VendorsSetupApiService> _logger;

    public VendorsSetupApiService(
        HttpClient httpClient,
        IOptions<SpsApiOptions> spsOptions,
        IOptions<VendorsSetupApiOptions> options,
        SpsAuthService authService,
        ILogger<VendorsSetupApiService> logger)
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

    public VendorSupplierMapping? GetMapping(string supplierName) =>
        _options.Suppliers.FirstOrDefault(s =>
            string.Equals(s.SupplierName, supplierName, StringComparison.OrdinalIgnoreCase));

    public async Task<List<VendorPartOffer>> SearchAsync(int vendorId, int shopId, string origNr, CancellationToken ct = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(origNr))
            return [];

        try
        {
            var token = await _authService.GetTokenAsync(ct);
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/vendorssetup/vendor/{vendorId}/shop/{shopId}/test_search?orig_nr={Uri.EscapeDataString(origNr)}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request, ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("VendorsSetup search failed: {Status} {Body}", (int)response.StatusCode, content);
                return [];
            }

            return ParseOffers(content);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "VendorsSetup search error for orig_nr={OrigNr}.", origNr);
            return [];
        }
    }

    private static List<VendorPartOffer> ParseOffers(string json)
    {
        var offers = new List<VendorPartOffer>();

        try
        {
            using var doc = JsonDocument.Parse(json);
            var partsLists = FindPartsLists(doc.RootElement);

            foreach (var partsList in partsLists)
            {
                foreach (var part in partsList.EnumerateArray())
                {
                    var offer = ParsePart(part);
                    if (offer != null)
                        offers.Add(offer);
                }
            }
        }
        catch
        {
            return offers;
        }

        return offers;
    }

    private static IEnumerable<JsonElement> FindPartsLists(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (property.NameEquals("PartsList") && property.Value.ValueKind == JsonValueKind.Array)
                    yield return property.Value;

                foreach (var nested in FindPartsLists(property.Value))
                    yield return nested;
            }
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                foreach (var nested in FindPartsLists(item))
                    yield return nested;
            }
        }
    }

    private static VendorPartOffer? ParsePart(JsonElement part)
    {
        var brand = GetString(part, "brand");
        var partNumber = GetString(part, "partnumber");
        var name = GetString(part, "name");
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(partNumber))
            return null;

        decimal price = 0;
        var stockQty = 0;

        if (part.TryGetProperty("stocks", out var stocks) && stocks.ValueKind == JsonValueKind.Array)
        {
            foreach (var stock in stocks.EnumerateArray())
            {
                var stockPrice = GetDecimal(stock, "price");
                var stockCount = GetInt(stock, "count");
                if (stockPrice > 0)
                    price = stockPrice;
                if (stockCount > stockQty)
                    stockQty = stockCount;
            }
        }

        return new VendorPartOffer
        {
            Brand = brand,
            PartNumber = partNumber,
            Name = name,
            Price = price,
            StockQuantity = stockQty
        };
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
            return string.Empty;

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            _ => string.Empty
        };
    }

    private static int GetInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
            return 0;

        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetInt32(out var number) => number,
            JsonValueKind.String when int.TryParse(value.GetString(), out var parsed) => parsed,
            _ => 0
        };
    }

    private static decimal GetDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
            return 0;

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var number))
            return number;

        if (value.ValueKind == JsonValueKind.String
            && decimal.TryParse(value.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        return 0;
    }
}
