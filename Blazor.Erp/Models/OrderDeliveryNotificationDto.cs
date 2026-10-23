namespace BlazorErp.Models;

public class OrderDeliveryNotificationDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public string TransportCompanyName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;

    public static OrderDeliveryNotificationDto FromOrder(Order order, ApplicationUser? transportCompany) => new()
    {
        OrderId = order.Id,
        OrderNumber = order.OrderNumber,
        TrackingNumber = order.Delivery?.TrackingNumber,
        TransportCompanyName = transportCompany?.FullName ?? "Транспортная компания",
        DeliveryAddress = order.DeliveryAddress
    };
}
