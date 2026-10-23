using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class OrderService(
    ApplicationDbContext db,
    CartService cartService,
    IOrderNotifier orderNotifier,
    OrderFulfillmentService fulfillmentService)
{
    public async Task<Order?> CreateOrderFromCartAsync(
        string userId,
        DeliveryMethod deliveryMethod,
        string deliveryAddress,
        PaymentMethod paymentMethod,
        int? paymentSourceId = null,
        string? paymentSystemName = null)
    {
        var cartItems = await cartService.GetCartAsync(userId);
        if (cartItems.Count == 0)
            return null;

        foreach (var item in cartItems)
        {
            if (item.Product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Недостаточно товара «{item.Product.Name}» на складе.");
        }

        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        var total = cartItems.Sum(i => i.Product.Price * i.Quantity);

        var order = new Order
        {
            OrderNumber = orderNumber,
            ClientId = userId,
            Status = OrderStatus.Pending,
            DeliveryMethod = deliveryMethod,
            DeliveryAddress = deliveryAddress,
            TotalAmount = total,
            Items = cartItems.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Product.Price
            }).ToList(),
            Payment = new Payment
            {
                OrderUid = Guid.NewGuid(),
                Method = paymentMethod,
                PaymentSourceId = paymentSourceId,
                PaymentSystemName = paymentSystemName,
                Status = PaymentStatus.Pending,
                Amount = total
            },
            Delivery = new Delivery
            {
                Status = OrderStatus.Processing
            }
        };

        foreach (var item in cartItems)
        {
            var product = await db.Products.FindAsync(item.ProductId);
            if (product != null)
                product.StockQuantity -= item.Quantity;
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        await cartService.ClearCartAsync(userId);

        return order;
    }

    public async Task<bool> SavePaymentUrlAsync(int orderId, string userId, string paymentUrl)
    {
        var order = await db.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ClientId == userId);

        if (order?.Payment == null || order.Payment.Status != PaymentStatus.Pending)
            return false;

        order.Payment.PaymentUrl = paymentUrl;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProcessPaymentAsync(int orderId, string userId)
    {
        var order = await db.Orders
            .Include(o => o.Payment)
            .Include(o => o.Client)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ClientId == userId);

        if (order?.Payment == null || order.Payment.Status != PaymentStatus.Pending)
            return false;

        order.Payment.Status = PaymentStatus.Completed;
        order.Payment.TransactionId = $"SBP-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        order.Payment.CompletedAt = DateTime.UtcNow;
        order.Status = OrderStatus.Paid;

        var assignment = await fulfillmentService.ResolveNearestAsync(order);
        await fulfillmentService.ApplyFulfillmentAsync(order, assignment);

        await db.SaveChangesAsync();
        await orderNotifier.NotifyPrepareOrderForDeliveryAsync(order, assignment);
        return true;
    }

    public Task<List<Order>> GetClientOrdersAsync(string userId) =>
        db.Orders
            .Include(o => o.Store)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .Include(o => o.Delivery)
            .Include(o => o.Review)
            .Where(o => o.ClientId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public Task<Order?> GetClientOrderAsync(int orderId, string userId) =>
        db.Orders
            .Include(o => o.Store)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
            .Include(o => o.Payment)
            .Include(o => o.Delivery)
            .Include(o => o.Review)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ClientId == userId);

    public Task<Order?> GetOrderAsync(int orderId) =>
        db.Orders
            .Include(o => o.Client)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .Include(o => o.Delivery)
            .Include(o => o.Review)
            .FirstOrDefaultAsync(o => o.Id == orderId);

    public Task<List<Order>> GetAvailableForTransportAsync() =>
        db.Orders
            .Include(o => o.Client)
            .Include(o => o.Store)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Delivery)
            .Where(o => o.Status == OrderStatus.ReadyForDelivery
                && o.DeliveryMethod == DeliveryMethod.TransportCompany
                && o.TransportCompanyId == null)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();

    public async Task<bool> TakeOrderForDeliveryAsync(int orderId, string transportUserId)
    {
        var order = await db.Orders
            .Include(o => o.Delivery)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null || order.Status != OrderStatus.ReadyForDelivery || order.TransportCompanyId != null)
            return false;

        order.TransportCompanyId = transportUserId;
        order.Status = OrderStatus.InDelivery;
        if (order.Delivery != null)
        {
            order.Delivery.TransportCompanyId = transportUserId;
            order.Delivery.Status = OrderStatus.InDelivery;
            order.Delivery.TrackingNumber = $"TRK-{Random.Shared.Next(100000, 999999)}";
            order.Delivery.PickedUpAt = DateTime.UtcNow;
        }

        var transportCompany = await db.UserContext.FindAsync(transportUserId);

        await db.SaveChangesAsync();
        await orderNotifier.NotifyOrderTakenForDeliveryAsync(order, transportCompany);
        return true;
    }

    public async Task<bool> MarkDeliveredAsync(int orderId, string transportUserId)
    {
        var order = await db.Orders
            .Include(o => o.Delivery)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.TransportCompanyId == transportUserId);

        if (order == null)
            return false;

        order.Status = OrderStatus.Delivered;
        order.DeliveredAt = DateTime.UtcNow;
        if (order.Delivery != null)
        {
            order.Delivery.Status = OrderStatus.Delivered;
            order.Delivery.DeliveredAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<Order>> GetTransportOrdersAsync(string transportUserId) =>
        db.Orders
            .Include(o => o.Client)
            .Include(o => o.Store)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Delivery)
            .Where(o => o.TransportCompanyId == transportUserId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<IReadOnlyDictionary<int, IReadOnlyList<OrderPickupLocation>>> GetPickupLocationsByOrderIdsAsync(
        IEnumerable<int> orderIds)
    {
        var ids = orderIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<int, IReadOnlyList<OrderPickupLocation>>();

        var orders = await db.Orders
            .Include(o => o.Store)
            .Include(o => o.Items)
            .Where(o => ids.Contains(o.Id))
            .ToListAsync();

        var productIds = orders.SelectMany(o => o.Items).Select(i => i.ProductId).Distinct().ToList();
        var storeProducts = productIds.Count == 0
            ? []
            : await db.StoreProducts
                .Include(sp => sp.Store)
                .Where(sp => productIds.Contains(sp.ProductId) && sp.Quantity > 0)
                .ToListAsync();

        var result = new Dictionary<int, IReadOnlyList<OrderPickupLocation>>();

        foreach (var order in orders)
        {
            if (order.Store != null)
            {
                result[order.Id] = [new OrderPickupLocation(order.Store.Name, order.Store.Address)];
                continue;
            }

            var locations = new List<OrderPickupLocation>();
            var seenStoreIds = new HashSet<int>();

            foreach (var item in order.Items)
            {
                var storeProduct = storeProducts
                    .Where(sp => sp.ProductId == item.ProductId && sp.Quantity >= item.Quantity)
                    .OrderByDescending(sp => sp.Quantity)
                    .FirstOrDefault();

                if (storeProduct?.Store != null && seenStoreIds.Add(storeProduct.StoreId))
                    locations.Add(new OrderPickupLocation(storeProduct.Store.Name, storeProduct.Store.Address));
            }

            if (locations.Count == 0)
            {
                locations.Add(new OrderPickupLocation(
                    WarehouseConstants.CentralWarehouseName,
                    WarehouseConstants.CentralWarehouseAddress));
            }

            result[order.Id] = locations;
        }

        return result;
    }

    public Task<List<Order>> GetOrdersAwaitingPreparationForStoreAsync(int storeId) =>
        db.Orders
            .Include(o => o.Client)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.StoreId == storeId
                && o.Status == OrderStatus.Paid
                && o.DeliveryMethod == DeliveryMethod.TransportCompany)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();

    public Task<List<Order>> GetOrdersAwaitingPreparationAtWarehouseAsync() =>
        db.Orders
            .Include(o => o.Client)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.StoreId == null
                && o.Status == OrderStatus.Paid
                && o.DeliveryMethod == DeliveryMethod.TransportCompany)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();

    public async Task<bool> MarkOrderPreparedAsync(int orderId, string userId, bool isAdmin)
    {
        var order = await db.Orders
            .Include(o => o.Delivery)
            .Include(o => o.Store)
            .Include(o => o.Client)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null
            || order.Status != OrderStatus.Paid
            || order.DeliveryMethod != DeliveryMethod.TransportCompany)
            return false;

        if (order.StoreId != null)
        {
            var isOwner = order.Store?.OwnerId == userId;
            var isSeller = await db.StoreSellers
                .AnyAsync(s => s.UserId == userId && s.StoreId == order.StoreId);
            var isCashier = await db.StoreCashiers
                .AnyAsync(c => c.UserId == userId && c.StoreId == order.StoreId);
            if (!isOwner && !isSeller && !isCashier)
                return false;
        }
        else if (!isAdmin)
        {
            return false;
        }

        order.Status = OrderStatus.ReadyForDelivery;
        if (order.Delivery != null)
            order.Delivery.Status = OrderStatus.ReadyForDelivery;

        await db.SaveChangesAsync();
        await orderNotifier.NotifyOrderReadyForTransportAsync(order);
        return true;
    }
}
