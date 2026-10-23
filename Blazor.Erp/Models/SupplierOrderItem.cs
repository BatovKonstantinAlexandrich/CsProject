namespace BlazorErp.Models;

public class SupplierOrderItem
{
    public int Id { get; set; }
    public int SupplierOrderId { get; set; }
    public SupplierOrder SupplierOrder { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
