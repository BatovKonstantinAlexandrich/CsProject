namespace BlazorErp.Models;

public class OrderPreparationNotificationDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string? LocationAddress { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ItemsSummary { get; set; } = string.Empty;
    public bool IsCentralWarehouse { get; set; }

    public static OrderPreparationNotificationDto FromOrder(Order order, FulfillmentAssignment assignment)
    {
        var items = order.Items
            .Select(i => $"{i.Product?.Name ?? "Товар"} × {i.Quantity}")
            .ToList();
        var summary = items.Count <= 3
            ? string.Join(", ", items)
            : string.Join(", ", items.Take(3)) + $" и ещё {items.Count - 3}";

        return new OrderPreparationNotificationDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            LocationName = assignment.LocationName,
            LocationAddress = assignment.LocationAddress,
            DeliveryAddress = order.DeliveryAddress,
            ClientName = order.Client?.FullName ?? "Клиент",
            ItemsSummary = summary,
            IsCentralWarehouse = assignment.IsCentralWarehouse
        };
    }

    public string ToMessageSubject() => $"Сборка заказа {OrderNumber}";

    public string ToMessageBody()
    {
        var location = string.IsNullOrWhiteSpace(LocationAddress)
            ? LocationName
            : $"{LocationName}, {LocationAddress}";

        return $"""
            Оплачен заказ {OrderNumber}.

            Клиент: {ClientName}
            Адрес доставки: {DeliveryAddress}
            Место сборки: {location}

            Товары: {ItemsSummary}

            Соберите заказ и отметьте готовность в разделе «Мой магазин» → «Сборка заказов».
            """;
    }
}
