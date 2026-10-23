using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

/// <summary>Запись кассового журнала (аналог kassa_finance).</summary>
public class CashRegisterEntry
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int? SaleBasketId { get; set; }
    public SaleBasket? SaleBasket { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public int? CashPaymentId { get; set; }
    public CashPayment? CashPayment { get; set; }
    public decimal Amount { get; set; }
    public CashRegisterActionType ActionType { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public string CashierUserId { get; set; } = string.Empty;
    public ApplicationUser Cashier { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
