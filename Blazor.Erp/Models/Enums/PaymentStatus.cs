namespace BlazorErp.Models.Enums;

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    WaitConfirm = 3,
    PartiallyRefunded = 4,
    Refunded = 5
}
