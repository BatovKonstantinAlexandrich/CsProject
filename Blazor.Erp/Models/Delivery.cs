using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class Delivery
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string? TransportCompanyId { get; set; }
    public ApplicationUser? TransportCompany { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Processing;
    public string? TrackingNumber { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
}
