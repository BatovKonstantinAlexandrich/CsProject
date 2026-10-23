using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlazorErp.Services;

public class CashierService(
    ApplicationDbContext db,
    StoreService storeService,
    IPaymentNotifier paymentNotifier,
    IOptions<PaymentOptions> paymentOptions)
{
    public async Task<bool> IsCashierForStoreAsync(string userId, int storeId) =>
        await db.StoreCashiers.AnyAsync(c => c.UserId == userId && c.StoreId == storeId)
        || await db.Stores.AnyAsync(s => s.Id == storeId && s.OwnerId == userId);

    public async Task<Store?> GetCashierStoreAsync(string userId) =>
        await storeService.GetStoreForUserAsync(userId);

    public Task<List<SaleBasket>> GetBasketsAsync(int storeId, bool includeClosed = false) =>
        db.SaleBaskets
            .Include(b => b.Items)
            .Include(b => b.Cashier)
            .Where(b => b.StoreId == storeId && (includeClosed || b.Status == SaleBasketStatus.Open || b.Status == SaleBasketStatus.WaitPayment))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public Task<SaleBasket?> GetBasketAsync(int basketId, int storeId) =>
        db.SaleBaskets
            .Include(b => b.Items).ThenInclude(i => i.Product)
            .Include(b => b.CashPayments)
            .Include(b => b.RegisterEntries)
            .FirstOrDefaultAsync(b => b.Id == basketId && b.StoreId == storeId);

    public async Task<SaleBasket?> CreateBasketAsync(
        int storeId,
        string cashierUserId,
        string clientName,
        string? clientPhone,
        string? clientEmail)
    {
        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return null;

        var number = await GenerateBasketNumberAsync(storeId);
        var basket = new SaleBasket
        {
            OrderUid = Guid.NewGuid(),
            BasketNumber = number,
            StoreId = storeId,
            CashierUserId = cashierUserId,
            ClientName = string.IsNullOrWhiteSpace(clientName) ? "Клиент" : clientName.Trim(),
            ClientPhone = PhoneNormalizer.Normalize(clientPhone) ?? clientPhone?.Trim(),
            ClientEmail = clientEmail?.Trim(),
            Status = SaleBasketStatus.Open
        };

        db.SaleBaskets.Add(basket);
        await db.SaveChangesAsync();
        return basket;
    }

    public async Task<bool> AddProductAsync(int basketId, int storeId, int productId, int quantity, string cashierUserId)
    {
        if (quantity <= 0)
            return false;

        var basket = await GetBasketAsync(basketId, storeId);
        if (basket == null || basket.Status is SaleBasketStatus.Paid or SaleBasketStatus.Cancelled)
            return false;

        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return false;

        var storeProduct = await db.StoreProducts
            .Include(sp => sp.Product)
            .FirstOrDefaultAsync(sp => sp.StoreId == storeId && sp.ProductId == productId);

        if (storeProduct == null || storeProduct.Quantity < quantity)
            throw new InvalidOperationException("Товар недоступен в магазине или недостаточно на складе.");

        var existing = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity += quantity;
            existing.LineTotal = existing.UnitPrice * existing.Quantity;
        }
        else
        {
            basket.Items.Add(new SaleBasketItem
            {
                ProductId = productId,
                ProductName = storeProduct.Product.Name,
                Quantity = quantity,
                UnitPrice = storeProduct.Product.Price,
                LineTotal = storeProduct.Product.Price * quantity
            });
        }

        RecalculateBasketTotal(basket);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveItemAsync(int itemId, int storeId, string cashierUserId)
    {
        var item = await db.SaleBasketItems
            .Include(i => i.SaleBasket)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.SaleBasket.StoreId == storeId);

        if (item?.SaleBasket == null || item.SaleBasket.Status is SaleBasketStatus.Paid or SaleBasketStatus.Cancelled)
            return false;

        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return false;

        db.SaleBasketItems.Remove(item);
        RecalculateBasketTotal(item.SaleBasket);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<CashPayment?> CreateOrRefreshPaymentLinkAsync(int basketId, int storeId, string cashierUserId)
    {
        var basket = await GetBasketAsync(basketId, storeId);
        if (basket == null || basket.TotalAmount <= 0)
            return null;

        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return null;

        var payment = basket.CashPayments
            .Where(p => p.Status is PaymentStatus.Pending or PaymentStatus.WaitConfirm)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefault();

        var url = BuildPaymentUrl(basket.OrderUid);

        if (payment == null)
        {
            payment = new CashPayment
            {
                SaleBasketId = basket.Id,
                ExternalPaymentId = GenerateExternalPaymentId(),
                Amount = basket.TotalAmount - basket.PaidAmount,
                PaymentUrl = url,
                PhoneNumber = basket.ClientPhone,
                Status = PaymentStatus.WaitConfirm,
                Method = PaymentMethod.SBP,
                PaymentSystemName = paymentOptions.Value.DefaultPaymentSystemName,
                ProcessedByUserId = cashierUserId,
                StatusDate = DateTime.UtcNow
            };
            db.CashPayments.Add(payment);
        }
        else
        {
            payment.PaymentUrl = url;
            payment.Amount = basket.TotalAmount - basket.PaidAmount;
            payment.Status = PaymentStatus.WaitConfirm;
            payment.StatusDate = DateTime.UtcNow;
        }

        basket.Status = SaleBasketStatus.WaitPayment;
        await db.SaveChangesAsync();
        return payment;
    }

    public async Task<(bool Success, string? Error)> SendPaymentLinkAsync(
        int basketId,
        int storeId,
        string cashierUserId,
        PaymentLinkChannel channel,
        string destination)
    {
        var payment = await CreateOrRefreshPaymentLinkAsync(basketId, storeId, cashierUserId);
        if (payment?.PaymentUrl == null)
            return (false, "Не удалось создать ссылку на оплату");

        var basket = await GetBasketAsync(basketId, storeId);
        if (basket == null)
            return (false, "Корзина не найдена");

        var (success, error) = await paymentNotifier.SendPaymentLinkAsync(
            channel,
            destination,
            payment.PaymentUrl,
            payment.Amount,
            basket.BasketNumber);

        db.PaymentLinkLogs.Add(new PaymentLinkLog
        {
            SaleBasketId = basketId,
            CashPaymentId = payment.Id,
            Channel = channel,
            Destination = destination.Trim(),
            PaymentUrl = payment.PaymentUrl,
            IsSuccess = success,
            ErrorMessage = error,
            SentByUserId = cashierUserId
        });

        if (channel == PaymentLinkChannel.Sms)
        {
            payment.PhoneNumber = PhoneNormalizer.Normalize(destination) ?? destination;
            basket.ClientPhone = payment.PhoneNumber;
        }
        else if (success)
        {
            basket.ClientEmail = destination.Trim();
        }

        await db.SaveChangesAsync();
        return (success, error);
    }

    public async Task<CashRegisterEntry?> ProcessPaymentAsync(
        int basketId,
        int storeId,
        string cashierUserId,
        PaymentMethod method,
        decimal? amount = null)
    {
        var basket = await GetBasketAsync(basketId, storeId);
        if (basket == null || basket.Status is SaleBasketStatus.Cancelled)
            return null;

        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return null;

        var due = basket.TotalAmount - basket.PaidAmount;
        if (due <= 0)
            return null;

        var payAmount = amount ?? due;
        if (payAmount <= 0 || payAmount > due)
            throw new InvalidOperationException("Некорректная сумма оплаты");

        foreach (var item in basket.Items)
        {
            var storeProduct = await db.StoreProducts
                .FirstOrDefaultAsync(sp => sp.StoreId == storeId && sp.ProductId == item.ProductId);
            if (storeProduct == null || storeProduct.Quantity < item.Quantity)
                throw new InvalidOperationException($"Недостаточно «{item.ProductName}» на складе магазина.");
        }

        basket.PaidAmount += payAmount;
        if (basket.PaidAmount >= basket.TotalAmount)
        {
            basket.Status = SaleBasketStatus.Paid;
            basket.PaidAt = DateTime.UtcNow;

            foreach (var item in basket.Items)
            {
                var storeProduct = await db.StoreProducts
                    .FirstOrDefaultAsync(sp => sp.StoreId == storeId && sp.ProductId == item.ProductId);
                if (storeProduct != null)
                    storeProduct.Quantity -= item.Quantity;
            }
        }

        var cashPayment = new CashPayment
        {
            SaleBasketId = basket.Id,
            ExternalPaymentId = GenerateExternalPaymentId(),
            Amount = payAmount,
            Status = PaymentStatus.Completed,
            Method = method,
            PaymentSystemName = method switch
            {
                PaymentMethod.Cash => "Наличные",
                PaymentMethod.Card => "Банковская карта",
                _ => paymentOptions.Value.DefaultPaymentSystemName
            },
            ProcessedByUserId = cashierUserId,
            CompletedAt = DateTime.UtcNow,
            StatusDate = DateTime.UtcNow,
            PhoneNumber = basket.ClientPhone
        };
        db.CashPayments.Add(cashPayment);

        var entry = await AddRegisterEntryAsync(
            storeId,
            basket.Id,
            null,
            cashPayment.Id,
            payAmount,
            CashRegisterActionType.Sale,
            method,
            cashierUserId,
            $"Оплата корзины {basket.BasketNumber}");

        await db.SaveChangesAsync();
        return entry;
    }

    public async Task<CashRegisterEntry?> ProcessRefundAsync(
        int cashPaymentId,
        int storeId,
        string cashierUserId,
        decimal amount,
        string? notes = null)
    {
        if (amount <= 0)
            return null;

        var payment = await db.CashPayments
            .Include(p => p.SaleBasket)
            .FirstOrDefaultAsync(p => p.Id == cashPaymentId);

        if (payment?.SaleBasket == null || payment.SaleBasket.StoreId != storeId)
            return null;

        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return null;

        var available = payment.Amount - payment.RefundSum;
        if (amount > available)
            throw new InvalidOperationException("Сумма возврата превышает доступный остаток");

        payment.RefundSum += amount;
        payment.Status = payment.RefundSum >= payment.Amount
            ? PaymentStatus.Refunded
            : PaymentStatus.PartiallyRefunded;
        payment.StatusDate = DateTime.UtcNow;

        var basket = payment.SaleBasket;
        basket.RefundSum += amount;

        var entry = await AddRegisterEntryAsync(
            storeId,
            basket.Id,
            null,
            payment.Id,
            -amount,
            CashRegisterActionType.Refund,
            payment.Method,
            cashierUserId,
            notes ?? $"Возврат по платежу {payment.ExternalPaymentId}");

        await db.SaveChangesAsync();
        return entry;
    }

    public Task<List<CashPayment>> GetOnlinePaymentsAsync(int storeId) =>
        db.CashPayments
            .Include(p => p.SaleBasket)
            .Include(p => p.Order).ThenInclude(o => o!.Client)
            .Where(p =>
                (p.SaleBasket != null && p.SaleBasket.StoreId == storeId)
                || (p.Order != null && p.Order.StoreId == storeId))
            .Where(p => p.Status == PaymentStatus.WaitConfirm
                || p.Status == PaymentStatus.Pending
                || p.Status == PaymentStatus.PartiallyRefunded
                || p.Status == PaymentStatus.Completed)
            .OrderByDescending(p => p.CreatedAt)
            .Take(200)
            .ToListAsync();

    public async Task<CashRegisterEntry?> PrintReceiptAsync(
        int basketId,
        int storeId,
        string cashierUserId)
    {
        var basket = await GetBasketAsync(basketId, storeId);
        if (basket == null || basket.Status != SaleBasketStatus.Paid)
            return null;

        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return null;

        if (basket.IsReceiptPrinted)
        {
            return basket.RegisterEntries
                .Where(e => e.ActionType == CashRegisterActionType.Sale)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault();
        }

        var lastPayment = basket.CashPayments
            .Where(p => p.Status == PaymentStatus.Completed)
            .OrderByDescending(p => p.CompletedAt)
            .FirstOrDefault();

        var entry = await AddRegisterEntryAsync(
            storeId,
            basket.Id,
            null,
            lastPayment?.Id,
            basket.PaidAmount,
            CashRegisterActionType.Sale,
            lastPayment?.Method ?? PaymentMethod.Cash,
            cashierUserId,
            $"Фискальный чек по корзине {basket.BasketNumber}");

        basket.IsReceiptPrinted = true;
        await db.SaveChangesAsync();
        return entry;
    }

    public Task<List<Order>> GetPendingWebOrdersAsync(int storeId) =>
        db.Orders
            .Include(o => o.Client)
            .Include(o => o.Payment)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.StoreId == storeId && o.Payment != null && o.Payment.Status == PaymentStatus.Pending)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<CashPayment?> CreatePaymentLinkForOrderAsync(int orderId, int storeId, string cashierUserId)
    {
        var order = await db.Orders
            .Include(o => o.Payment)
            .Include(o => o.Client)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.StoreId == storeId);

        if (order?.Payment == null || order.Payment.Status != PaymentStatus.Pending)
            return null;

        if (!await IsCashierForStoreAsync(cashierUserId, storeId))
            return null;

        order.Payment.OrderUid ??= Guid.NewGuid();
        order.Payment.PaymentUrl = BuildPaymentUrl(order.Payment.OrderUid.Value);
        order.Payment.Status = PaymentStatus.WaitConfirm;
        order.Payment.PhoneNumber = PhoneNormalizer.Normalize(order.Client.PhoneNumber) ?? order.Client.PhoneNumber;

        var existing = await db.CashPayments
            .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status != PaymentStatus.Completed);

        if (existing == null)
        {
            existing = new CashPayment
            {
                OrderId = order.Id,
                ExternalPaymentId = GenerateExternalPaymentId(),
                Amount = order.TotalAmount,
                PaymentUrl = order.Payment.PaymentUrl,
                PhoneNumber = order.Payment.PhoneNumber,
                Status = PaymentStatus.WaitConfirm,
                Method = order.Payment.Method,
                PaymentSystemName = paymentOptions.Value.DefaultPaymentSystemName,
                ProcessedByUserId = cashierUserId,
                StatusDate = DateTime.UtcNow
            };
            db.CashPayments.Add(existing);
        }
        else
        {
            existing.PaymentUrl = order.Payment.PaymentUrl;
            existing.Amount = order.TotalAmount;
            existing.Status = PaymentStatus.WaitConfirm;
            existing.StatusDate = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return existing;
    }

    public async Task<(bool Success, string? Error)> SendOrderPaymentLinkAsync(
        int orderId,
        int storeId,
        string cashierUserId,
        PaymentLinkChannel channel,
        string destination)
    {
        var payment = await CreatePaymentLinkForOrderAsync(orderId, storeId, cashierUserId);
        if (payment?.PaymentUrl == null)
            return (false, "Не удалось создать ссылку");

        var order = await db.Orders.FindAsync(orderId);
        var reference = order?.OrderNumber ?? orderId.ToString();

        var (success, error) = await paymentNotifier.SendPaymentLinkAsync(
            channel, destination, payment.PaymentUrl, payment.Amount, reference);

        db.PaymentLinkLogs.Add(new PaymentLinkLog
        {
            OrderId = orderId,
            CashPaymentId = payment.Id,
            Channel = channel,
            Destination = destination.Trim(),
            PaymentUrl = payment.PaymentUrl,
            IsSuccess = success,
            ErrorMessage = error,
            SentByUserId = cashierUserId
        });

        await db.SaveChangesAsync();
        return (success, error);
    }

    public Task<List<StoreProduct>> GetStoreProductsAsync(int storeId) =>
        db.StoreProducts
            .Include(sp => sp.Product)
            .Where(sp => sp.StoreId == storeId && sp.Quantity > 0)
            .OrderBy(sp => sp.Product.Name)
            .ToListAsync();

    private Task<CashRegisterEntry> AddRegisterEntryAsync(
        int storeId,
        int? basketId,
        int? orderId,
        int? cashPaymentId,
        decimal amount,
        CashRegisterActionType actionType,
        PaymentMethod method,
        string cashierUserId,
        string notes)
    {
        var receiptNumber = $"R-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
        var entry = new CashRegisterEntry
        {
            StoreId = storeId,
            SaleBasketId = basketId,
            OrderId = orderId,
            CashPaymentId = cashPaymentId,
            Amount = amount,
            ActionType = actionType,
            PaymentMethod = method,
            ReceiptNumber = receiptNumber,
            CashierUserId = cashierUserId,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
        db.CashRegisterEntries.Add(entry);
        return Task.FromResult(entry);
    }

    private static void RecalculateBasketTotal(SaleBasket basket)
    {
        basket.TotalAmount = basket.Items.Sum(i => i.LineTotal);
    }

    private async Task<string> GenerateBasketNumberAsync(int storeId)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"B-{today}-";
        var count = await db.SaleBaskets.CountAsync(b => b.StoreId == storeId && b.BasketNumber.StartsWith(prefix));
        return $"{prefix}{(count + 1):D4}";
    }

    private static string GenerateExternalPaymentId() =>
        Random.Shared.Next(100000, 9999999).ToString();

    private string BuildPaymentUrl(Guid orderUid)
    {
        var baseUrl = paymentOptions.Value.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/pay/{orderUid:N}";
    }
}
