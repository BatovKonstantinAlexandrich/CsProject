using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Catalog;
using BlazorErp.Models.Enums;
using BlazorErp.Models.Vendors;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class SupplierService(
    ApplicationDbContext db,
    VendorsSetupApiService vendorsSetupApi)
{
    public bool IsVendorsSetupConfigured => vendorsSetupApi.IsConfigured;

    public VendorSupplierMapping? GetVendorMapping(string supplierName) =>
        vendorsSetupApi.GetMapping(supplierName);

    public async Task<List<VendorPartOffer>> SearchVendorOffersAsync(string supplierName, string origNr, CancellationToken ct = default)
    {
        var mapping = vendorsSetupApi.GetMapping(supplierName);
        if (mapping == null)
            return [];

        return await vendorsSetupApi.SearchAsync(mapping.VendorId, mapping.ShopId, origNr, ct);
    }

    public async Task<VendorCatalogRefreshResult> RefreshCatalogFromVendorsSetupAsync(
        int supplierId,
        string? origNr = null,
        CancellationToken ct = default)
    {
        if (!vendorsSetupApi.IsConfigured)
        {
            return new VendorCatalogRefreshResult
            {
                Message = "VendorsSetup API не настроен (SpsApi + VendorsSetupApi в appsettings)."
            };
        }

        var supplier = await db.Suppliers
            .Include(s => s.Products).ThenInclude(p => p.Product).ThenInclude(p => p.Brand)
            .FirstOrDefaultAsync(s => s.Id == supplierId, ct);

        if (supplier == null)
        {
            return new VendorCatalogRefreshResult { Message = "Поставщик не найден." };
        }

        var mapping = vendorsSetupApi.GetMapping(supplier.Name);
        if (mapping == null)
        {
            return new VendorCatalogRefreshResult
            {
                Message = $"Для «{supplier.Name}» нет привязки VendorId/ShopId в VendorsSetupApi:Suppliers."
            };
        }

        var updatedProductIds = new HashSet<int>();
        var searched = 0;

        if (!string.IsNullOrWhiteSpace(origNr))
        {
            searched = 1;
            var offers = await vendorsSetupApi.SearchAsync(mapping.VendorId, mapping.ShopId, origNr, ct);
            updatedProductIds.UnionWith(await ApplyOffersToSupplierProductsAsync(supplier, offers, ct));
        }
        else
        {
            foreach (var supplierProduct in supplier.Products)
            {
                var searchTerm = ResolveSearchTerm(mapping, supplierProduct.Product.Name);
                if (string.IsNullOrWhiteSpace(searchTerm))
                    continue;

                searched++;
                var offers = await vendorsSetupApi.SearchAsync(mapping.VendorId, mapping.ShopId, searchTerm, ct);
                updatedProductIds.UnionWith(
                    await ApplyOffersToSupplierProductsAsync(supplier, offers, ct, supplierProduct.ProductId));
            }
        }

        if (updatedProductIds.Count > 0)
            await db.SaveChangesAsync(ct);

        return new VendorCatalogRefreshResult
        {
            UpdatedCount = updatedProductIds.Count,
            SearchedCount = searched,
            UpdatedProductIds = updatedProductIds,
            Message = updatedProductIds.Count > 0
                ? $"Обновлено позиций: {updatedProductIds.Count} (запросов: {searched})."
                : searched == 0
                    ? "Нет артикулов для поиска. Укажите orig_nr или SearchTerms в конфиге."
                    : "Совпадений с каталогом не найдено."
        };
    }

    private static string? ResolveSearchTerm(VendorSupplierMapping mapping, string productName)
    {
        if (mapping.SearchTerms.TryGetValue(productName, out var configured) && !string.IsNullOrWhiteSpace(configured))
            return configured;

        return productName;
    }

    private async Task<HashSet<int>> ApplyOffersToSupplierProductsAsync(
        Supplier supplier,
        List<VendorPartOffer> offers,
        CancellationToken ct,
        int? onlyProductId = null)
    {
        var updatedProductIds = new HashSet<int>();
        if (offers.Count == 0)
            return updatedProductIds;

        var products = onlyProductId.HasValue
            ? supplier.Products.Where(p => p.ProductId == onlyProductId.Value).ToList()
            : supplier.Products;

        foreach (var supplierProduct in products)
        {
            var product = supplierProduct.Product;
            var brandName = product.Brand?.Name ?? string.Empty;
            var offer = offers.FirstOrDefault(o => MatchesProduct(o, product.Name, brandName));
            if (offer == null)
                continue;

            if (offer.Price > 0)
                supplierProduct.UnitPrice = offer.Price;
            if (offer.StockQuantity > 0)
                supplierProduct.StockQuantity = offer.StockQuantity;

            updatedProductIds.Add(supplierProduct.ProductId);
        }

        await Task.CompletedTask;
        return updatedProductIds;
    }

    private static bool MatchesProduct(VendorPartOffer offer, string productName, string brandName)
    {
        if (!string.IsNullOrWhiteSpace(brandName)
            && !string.IsNullOrWhiteSpace(offer.Brand)
            && !brandName.Equals(offer.Brand, StringComparison.OrdinalIgnoreCase))
            return false;

        var productLower = productName.ToLowerInvariant();
        var offerNameLower = offer.Name.ToLowerInvariant();
        return offerNameLower.Contains(productLower)
               || productLower.Contains(offerNameLower)
               || (!string.IsNullOrWhiteSpace(offer.PartNumber)
                   && productLower.Contains(offer.PartNumber.ToLowerInvariant()));
    }

    public Task<Supplier?> GetSupplierByOwnerAsync(string ownerId) =>
        db.Suppliers
            .Include(s => s.Brands).ThenInclude(b => b.Brand)
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId);

    public async Task<List<SupplierProduct>> GetSupplierStockAsync(string ownerId)
    {
        var supplier = await GetSupplierByOwnerAsync(ownerId);
        if (supplier == null)
            return [];

        var brandIds = supplier.Brands.Select(b => b.BrandId).ToHashSet();

        return await db.SupplierProducts
            .Include(sp => sp.Product).ThenInclude(p => p.Brand)
            .Where(sp => sp.SupplierId == supplier.Id && sp.Product.BrandId != null && brandIds.Contains(sp.Product.BrandId.Value))
            .OrderBy(sp => sp.Product.Name)
            .ToListAsync();
    }

    public async Task<List<SupplierOrder>> GetIncomingOrdersAsync(string ownerId) =>
        await db.SupplierOrders
            .Include(o => o.Store)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
            .Where(o => o.Supplier.OwnerId == ownerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<bool> ConfirmOrderAsync(int orderId, string ownerId)
    {
        var order = await db.SupplierOrders
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
            .Include(o => o.Supplier)
            .Include(o => o.Store)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.Supplier.OwnerId == ownerId);

        if (order == null || order.Status != OrderStatus.Pending)
            return false;

        foreach (var item in order.Items)
        {
            var supplierProduct = await db.SupplierProducts
                .FirstOrDefaultAsync(sp => sp.SupplierId == order.SupplierId && sp.ProductId == item.ProductId);

            if (supplierProduct == null || supplierProduct.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Недостаточно «{item.Product?.Name ?? "товара"}» у поставщика.");

            supplierProduct.StockQuantity -= item.Quantity;

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

        order.Status = OrderStatus.Paid;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<SupplierCatalogGroup>> GetCatalogForStoreOwnerAsync()
    {
        var suppliers = await db.Suppliers
            .Include(s => s.Brands)
            .Include(s => s.Products).ThenInclude(p => p.Product).ThenInclude(p => p.Brand)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var groups = new List<SupplierCatalogGroup>();

        foreach (var supplier in suppliers)
        {
            var brandIds = supplier.Brands.Select(b => b.BrandId).ToHashSet();
            var products = supplier.Products
                .Where(sp => sp.StockQuantity > 0
                             && sp.Product.BrandId != null
                             && brandIds.Contains(sp.Product.BrandId.Value))
                .Select(sp => new SupplierCatalogProduct
                {
                    ProductId = sp.ProductId,
                    ProductName = sp.Product.Name,
                    BrandName = sp.Product.Brand!.Name,
                    UnitPrice = sp.UnitPrice,
                    StockQuantity = sp.StockQuantity
                })
                .OrderBy(p => p.ProductName)
                .ToList();

            if (products.Count > 0)
            {
                groups.Add(new SupplierCatalogGroup
                {
                    SupplierId = supplier.Id,
                    SupplierName = supplier.Name,
                    Products = products
                });
            }
        }

        return groups;
    }

    public async Task<SupplierOrder?> CreateStoreOrderAsync(string storeOwnerId, int supplierId, Dictionary<int, int> productQuantities)
    {
        var store = await db.Stores.FirstOrDefaultAsync(s => s.OwnerId == storeOwnerId);
        if (store == null || productQuantities.Count == 0)
            return null;

        var supplier = await db.Suppliers
            .Include(s => s.Brands)
            .FirstOrDefaultAsync(s => s.Id == supplierId);

        if (supplier == null)
            return null;

        var brandIds = supplier.Brands.Select(b => b.BrandId).ToHashSet();
        var items = new List<SupplierOrderItem>();
        decimal total = 0;

        foreach (var (productId, qty) in productQuantities)
        {
            if (qty <= 0) continue;

            var supplierProduct = await db.SupplierProducts
                .Include(sp => sp.Product)
                .FirstOrDefaultAsync(sp => sp.SupplierId == supplierId && sp.ProductId == productId);

            if (supplierProduct == null)
                throw new InvalidOperationException("Товар недоступен у выбранного поставщика.");

            if (supplierProduct.Product.BrandId == null || !brandIds.Contains(supplierProduct.Product.BrandId.Value))
                throw new InvalidOperationException($"Поставщик не работает с брендом «{supplierProduct.Product.Brand?.Name ?? "товара"}».");

            if (supplierProduct.StockQuantity < qty)
                throw new InvalidOperationException($"Недостаточно «{supplierProduct.Product.Name}» на складе поставщика.");

            total += supplierProduct.UnitPrice * qty;
            items.Add(new SupplierOrderItem
            {
                ProductId = productId,
                Quantity = qty,
                UnitPrice = supplierProduct.UnitPrice
            });
        }

        if (items.Count == 0)
            return null;

        var order = new SupplierOrder
        {
            StoreId = store.Id,
            SupplierId = supplierId,
            Status = OrderStatus.Pending,
            TotalAmount = total,
            Items = items
        };

        db.SupplierOrders.Add(order);
        await db.SaveChangesAsync();
        return order;
    }

    public Task<List<SupplierOrder>> GetStoreSupplierOrdersAsync(string storeOwnerId) =>
        db.SupplierOrders
            .Include(o => o.Supplier)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
            .Where(o => o.Store.OwnerId == storeOwnerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
}
