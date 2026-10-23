using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid? OrderUid { get; set; }
    public PaymentMethod Method { get; set; }
    public int? PaymentSourceId { get; set; }
    public string? PaymentSystemName { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public decimal RefundSum { get; set; }
    public string? PaymentUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProcessedByUserId { get; set; }
    public ApplicationUser? ProcessedBy { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
