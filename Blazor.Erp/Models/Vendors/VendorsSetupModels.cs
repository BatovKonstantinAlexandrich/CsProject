namespace BlazorErp.Models.Vendors;

public class VendorPartOffer
{
    public string Brand { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public class VendorCatalogRefreshResult
{
    public int UpdatedCount { get; set; }
    public int SearchedCount { get; set; }
    public HashSet<int> UpdatedProductIds { get; set; } = [];
    public string? Message { get; set; }
}
