namespace BlazorErp.Models;

public class StoreInventoryItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? BrandName { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal RetailPrice { get; set; }
    public int Quantity { get; set; }
}
