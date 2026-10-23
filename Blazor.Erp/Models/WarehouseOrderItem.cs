namespace BlazorErp.Models;

public class WarehouseOrderItem
{
    public int Id { get; set; }
    public int WarehouseOrderId { get; set; }
    public WarehouseOrder WarehouseOrder { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
