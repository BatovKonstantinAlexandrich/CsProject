namespace BlazorErp.Models.Catalog;

public class ProductStockLocation
{
    public string LocationType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int Quantity { get; set; }
}
