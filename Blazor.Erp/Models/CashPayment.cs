using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

/// <summary>Онлайн/ожидающий платёж (аналог orders_payments).</summary>
public class CashPayment
{
    public int Id { get; set; }
    public int? SaleBasketId { get; set; }
    public SaleBasket? SaleBasket { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public string ExternalPaymentId { get; set; } = string.Empty;
    public string? PaymentUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal Amount { get; set; }
    public decimal RefundSum { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod Method { get; set; } = PaymentMethod.SBP;
    public string? PaymentSystemName { get; set; }
    public string? ProcessedByUserId { get; set; }
    public ApplicationUser? ProcessedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? StatusDate { get; set; }
    public List<PaymentLinkLog> LinkDeliveries { get; set; } = [];
}
