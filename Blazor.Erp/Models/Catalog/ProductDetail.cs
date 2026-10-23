namespace BlazorErp.Models.Catalog;

public class ProductDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<ProductStockLocation> StockLocations { get; set; } = [];
    public List<ProductReviewItem> Reviews { get; set; } = [];
}
