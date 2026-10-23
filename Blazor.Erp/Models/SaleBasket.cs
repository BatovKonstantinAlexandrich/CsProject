using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

/// <summary>Заголовок корзины кассы (аналог M_Basket_number).</summary>
public class SaleBasket
{
    public int Id { get; set; }
    public Guid OrderUid { get; set; } = Guid.NewGuid();
    public string BasketNumber { get; set; } = string.Empty;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string? CashierUserId { get; set; }
    public ApplicationUser? Cashier { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ClientPhone { get; set; }
    public string? ClientEmail { get; set; }
    public SaleBasketStatus Status { get; set; } = SaleBasketStatus.Open;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RefundSum { get; set; }
    public bool IsReceiptPrinted { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public List<SaleBasketItem> Items { get; set; } = new();
    public List<CashPayment> CashPayments { get; set; } = new();
    public List<CashRegisterEntry> RegisterEntries { get; set; } = new();
}
