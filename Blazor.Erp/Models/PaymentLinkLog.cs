using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class PaymentLinkLog
{
    public int Id { get; set; }
    public int? SaleBasketId { get; set; }
    public SaleBasket? SaleBasket { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public int? CashPaymentId { get; set; }
    public CashPayment? CashPayment { get; set; }
    public PaymentLinkChannel Channel { get; set; }
    public string Destination { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string SentByUserId { get; set; } = string.Empty;
    public ApplicationUser SentBy { get; set; } = null!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
