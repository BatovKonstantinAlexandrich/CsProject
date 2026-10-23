namespace BlazorErp.Models;

public class OrderReadyForTransportNotificationDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string PickupLocationName { get; set; } = string.Empty;
    public string? PickupLocationAddress { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ItemsSummary { get; set; } = string.Empty;

    public static OrderReadyForTransportNotificationDto FromOrder(Order order)
    {
        var items = order.Items
            .Select(i => $"{i.Product?.Name ?? "Товар"} × {i.Quantity}")
            .ToList();
        var summary = items.Count <= 3
            ? string.Join(", ", items)
            : string.Join(", ", items.Take(3)) + $" и ещё {items.Count - 3}";

        return new OrderReadyForTransportNotificationDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            PickupLocationName = order.Store?.Name ?? WarehouseConstants.CentralWarehouseName,
            PickupLocationAddress = order.Store?.Address ?? WarehouseConstants.CentralWarehouseAddress,
            DeliveryAddress = order.DeliveryAddress,
            ClientName = order.Client?.FullName ?? "Клиент",
            ItemsSummary = summary
        };
    }
}
