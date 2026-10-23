namespace BlazorErp.Models;

public class SupplierProduct
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StockQuantity { get; set; }
    public decimal UnitPrice { get; set; }
}
