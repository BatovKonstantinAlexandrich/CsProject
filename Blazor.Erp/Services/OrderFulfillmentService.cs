using BlazorErp.Data;
using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class OrderFulfillmentService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
{
    public async Task<FulfillmentAssignment> ResolveNearestAsync(Order order)
    {
        var items = order.Items.ToList();
        if (items.Count == 0)
            return await CreateCentralWarehouseAssignmentAsync();

        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        var storeProducts = await db.StoreProducts
            .Include(sp => sp.Store)
            .Where(sp => productIds.Contains(sp.ProductId) && sp.Quantity > 0)
            .ToListAsync();

        var candidateStoreIds = storeProducts.Select(sp => sp.StoreId).Distinct();
        Store? bestStore = null;
        var bestScore = -1;

        foreach (var storeId in candidateStoreIds)
        {
            var storeItems = storeProducts.Where(sp => sp.StoreId == storeId).ToList();
            var canFulfillAll = items.All(item =>
                storeItems.Any(sp => sp.ProductId == item.ProductId && sp.Quantity >= item.Quantity));

            if (!canFulfillAll)
                continue;

            var store = storeItems[0].Store;
            var score = AddressProximity.Score(order.DeliveryAddress, store.Address);
            if (bestStore == null || score > bestScore ||
                (score == bestScore && string.Compare(store.Name, bestStore.Name, StringComparison.OrdinalIgnoreCase) < 0))
            {
                bestScore = score;
                bestStore = store;
            }
        }

        if (bestStore != null)
        {
            return new FulfillmentAssignment(
                bestStore.Id,
                bestStore.Name,
                bestStore.Address,
                false,
                [bestStore.OwnerId]);
        }

        return await CreateCentralWarehouseAssignmentAsync();
    }

    public async Task ApplyFulfillmentAsync(Order order, FulfillmentAssignment assignment)
    {
        order.StoreId = assignment.StoreId;

        if (assignment.StoreId == null || assignment.IsCentralWarehouse)
            return;

        foreach (var item in order.Items)
        {
            var product = await db.Products.FindAsync(item.ProductId);
            if (product != null)
                product.StockQuantity += item.Quantity;

            var storeProduct = await db.StoreProducts
                .FirstOrDefaultAsync(sp => sp.StoreId == assignment.StoreId && sp.ProductId == item.ProductId);

            if (storeProduct == null || storeProduct.Quantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Недостаточно товара «{item.Product?.Name ?? "товара"}» в магазине «{assignment.LocationName}».");

            storeProduct.Quantity -= item.Quantity;
        }
    }

    private async Task<FulfillmentAssignment> CreateCentralWarehouseAssignmentAsync()
    {
        var admins = await userManager.GetUsersInRoleAsync(RoleNames.Admin);
        var recipientIds = admins.Select(a => a.Id).ToList();

        return new FulfillmentAssignment(
            null,
            WarehouseConstants.CentralWarehouseName,
            WarehouseConstants.CentralWarehouseAddress,
            true,
            recipientIds);
    }
}
