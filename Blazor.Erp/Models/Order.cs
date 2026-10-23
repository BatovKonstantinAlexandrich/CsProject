using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public ApplicationUser Client { get; set; } = null!;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DeliveryMethod DeliveryMethod { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
    public string? TransportCompanyId { get; set; }
    public ApplicationUser? TransportCompany { get; set; }
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public Payment? Payment { get; set; }
    public Delivery? Delivery { get; set; }
    public Review? Review { get; set; }
}
