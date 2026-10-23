using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class WarehouseOrder
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<WarehouseOrderItem> Items { get; set; } = [];
}
