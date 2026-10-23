namespace BlazorErp.Models.Catalog;

public class SupplierCatalogGroup
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public List<SupplierCatalogProduct> Products { get; set; } = [];
}

public class SupplierCatalogProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsLiveFromVendor { get; set; }
    public DateTime? LiveUpdatedAt { get; set; }
}
