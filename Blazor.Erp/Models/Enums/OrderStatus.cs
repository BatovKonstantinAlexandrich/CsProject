namespace BlazorErp.Models.Enums;

public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Processing = 2,
    Shipped = 3,
    InDelivery = 4,
    Delivered = 5,
    Cancelled = 6,
    ReadyForDelivery = 7
}
