using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class StoreService(ApplicationDbContext db)
{
    public Task<Store?> GetStoreByOwnerAsync(string ownerId) =>
        db.Stores.FirstOrDefaultAsync(s => s.OwnerId == ownerId);

    public async Task<Store?> GetStoreForUserAsync(string userId)
    {
        var owned = await GetStoreByOwnerAsync(userId);
        if (owned != null)
            return owned;

        return await db.StoreSellers
            .Include(s => s.Store)
            .Where(s => s.UserId == userId)
            .Select(s => s.Store)
            .FirstOrDefaultAsync()
            ?? await db.StoreCashiers
                .Include(c => c.Store)
                .Where(c => c.UserId == userId)
                .Select(c => c.Store)
                .FirstOrDefaultAsync();
    }

    public Task<bool> IsStoreOwnerAsync(string userId) =>
        db.Stores.AnyAsync(s => s.OwnerId == userId);

    public async Task<WarehouseOrder?> CreateWarehouseOrderAsync(string ownerId, Dictionary<int, int> productQuantities)
    {
        var store = await GetStoreByOwnerAsync(ownerId);
        if (store == null || productQuantities.Count == 0)
            return null;

        var productIds = productQuantities.Keys.ToList();
        var products = await db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        decimal total = 0;
        var items = new List<WarehouseOrderItem>();

        foreach (var (productId, qty) in productQuantities)
        {
            if (qty <= 0) continue;
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null || product.StockQuantity < qty)
                throw new InvalidOperationException($"Недостаточно «{product?.Name ?? "товара"}» на складе.");

            total += product.Price * qty;
            items.Add(new WarehouseOrderItem
            {
                ProductId = productId,
                Quantity = qty,
                UnitPrice = product.Price
            });
        }

        if (items.Count == 0)
            return null;

        var order = new WarehouseOrder
        {
            StoreId = store.Id,
            Status = OrderStatus.Pending,
            TotalAmount = total,
            Items = items
        };

        db.WarehouseOrders.Add(order);
        await db.SaveChangesAsync();
        return order;
    }

    public async Task<bool> ConfirmWarehouseOrderAsync(int warehouseOrderId, string ownerId)
    {
        var order = await db.WarehouseOrders
            .Include(w => w.Items)
            .Include(w => w.Store)
            .FirstOrDefaultAsync(w => w.Id == warehouseOrderId && w.Store.OwnerId == ownerId);

        if (order == null || order.Status != OrderStatus.Pending)
            return false;

        foreach (var item in order.Items)
        {
            var product = await db.Products.FindAsync(item.ProductId);
            if (product == null || product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Недостаточно товара на складе.");

            product.StockQuantity -= item.Quantity;
        }

        order.Status = OrderStatus.Paid;

        foreach (var item in order.Items)
        {
            var storeProduct = await db.StoreProducts
                .FirstOrDefaultAsync(sp => sp.StoreId == order.StoreId && sp.ProductId == item.ProductId);

            if (storeProduct == null)
            {
                db.StoreProducts.Add(new StoreProduct
                {
                    StoreId = order.StoreId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }
            else
            {
                storeProduct.Quantity += item.Quantity;
            }
        }

        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<WarehouseOrder>> GetWarehouseOrdersAsync(string ownerId) =>
        db.WarehouseOrders
            .Include(w => w.Items).ThenInclude(i => i.Product)
            .Include(w => w.Store)
            .Where(w => w.Store.OwnerId == ownerId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

    public Task<List<Product>> GetWarehouseStockAsync() =>
        db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public Task<List<StoreInventoryItem>> GetStoreInventoryAsync(int storeId) =>
        db.StoreProducts
            .Include(sp => sp.Product).ThenInclude(p => p.Brand)
            .Include(sp => sp.Product).ThenInclude(p => p.Category)
            .Where(sp => sp.StoreId == storeId)
            .OrderBy(sp => sp.Product.Name)
            .Select(sp => new StoreInventoryItem
            {
                ProductId = sp.ProductId,
                ProductName = sp.Product.Name,
                BrandName = sp.Product.Brand != null ? sp.Product.Brand.Name : null,
                CategoryName = sp.Product.Category.Name,
                RetailPrice = sp.Product.Price,
                Quantity = sp.Quantity
            })
            .ToListAsync();

    public async Task<bool> SetStoreProductQuantityAsync(string ownerId, int productId, int quantity)
    {
        if (quantity < 0)
            throw new InvalidOperationException("Количество не может быть отрицательным.");

        var store = await GetStoreByOwnerAsync(ownerId);
        if (store == null)
            return false;

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
        if (product == null)
            return false;

        var storeProduct = await db.StoreProducts
            .FirstOrDefaultAsync(sp => sp.StoreId == store.Id && sp.ProductId == productId);

        if (quantity == 0)
        {
            if (storeProduct != null)
                db.StoreProducts.Remove(storeProduct);
        }
        else if (storeProduct == null)
        {
            db.StoreProducts.Add(new StoreProduct
            {
                StoreId = store.Id,
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            storeProduct.Quantity = quantity;
        }

        await db.SaveChangesAsync();
        return true;
    }

    public async Task SaveStoreInventoryAsync(string ownerId, IReadOnlyDictionary<int, int> quantities)
    {
        var store = await GetStoreByOwnerAsync(ownerId)
            ?? throw new InvalidOperationException("Магазин не найден.");

        var productIds = quantities.Keys.ToList();
        var activeProductIds = await db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .Select(p => p.Id)
            .ToListAsync();

        var storeProducts = await db.StoreProducts
            .Where(sp => sp.StoreId == store.Id && productIds.Contains(sp.ProductId))
            .ToListAsync();

        foreach (var (productId, quantity) in quantities)
        {
            if (!activeProductIds.Contains(productId))
                continue;

            if (quantity < 0)
                throw new InvalidOperationException("Количество не может быть отрицательным.");

            var storeProduct = storeProducts.FirstOrDefault(sp => sp.ProductId == productId);
            if (quantity == 0)
            {
                if (storeProduct != null)
                    db.StoreProducts.Remove(storeProduct);
            }
            else if (storeProduct == null)
            {
                db.StoreProducts.Add(new StoreProduct
                {
                    StoreId = store.Id,
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            else
            {
                storeProduct.Quantity = quantity;
            }
        }

        await db.SaveChangesAsync();
    }

    public Task<List<Product>> GetActiveProductsAsync() =>
        db.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<bool> UpdateStoreProfileAsync(string ownerId, StoreProfileUpdate update)
    {
        var store = await GetStoreByOwnerAsync(ownerId);
        if (store == null || string.IsNullOrWhiteSpace(update.Name))
            return false;

        store.Name = update.Name.Trim();
        store.Address = string.IsNullOrWhiteSpace(update.Address) ? null : update.Address.Trim();
        store.Description = string.IsNullOrWhiteSpace(update.Description) ? null : update.Description.Trim();
        store.ImageUrl = string.IsNullOrWhiteSpace(update.ImageUrl) ? null : update.ImageUrl.Trim();

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<StoreSalesStatistics> GetSalesStatisticsAsync(int storeId)
    {
        var orders = await db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .Where(o => o.StoreId == storeId
                && o.Payment != null
                && o.Payment.Status == PaymentStatus.Completed)
            .ToListAsync();

        var stats = new StoreSalesStatistics
        {
            TotalOrders = orders.Count,
            CompletedOrders = orders.Count(o => o.Status == OrderStatus.Delivered),
            AwaitingPreparation = orders.Count(o => o.Status == OrderStatus.Paid),
            ReadyForDelivery = orders.Count(o => o.Status == OrderStatus.ReadyForDelivery),
            InDelivery = orders.Count(o => o.Status == OrderStatus.InDelivery),
            TotalRevenue = orders.Sum(o => o.TotalAmount),
            CompletedRevenue = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalAmount)
        };

        stats.TopProducts = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.Product.Name)
            .Select(g => new StoreTopSellingProduct
            {
                ProductName = g.Key,
                QuantitySold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.UnitPrice * i.Quantity)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(10)
            .ToList();

        return stats;
    }
}
