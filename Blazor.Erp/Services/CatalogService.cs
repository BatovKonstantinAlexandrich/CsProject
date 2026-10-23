using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Catalog;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BlazorErp.Services;

public class CatalogService(ApplicationDbContext db)
{
    public async Task<List<CategoryTreeItem>> GetCategoryTreeAsync()
    {
        var categories = await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryTreeItem
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId
            })
            .ToListAsync();

        return BuildTree(categories, null);
    }

    public Task<List<Category>> GetCategoriesAsync() =>
        db.Categories.OrderBy(c => c.Name).ToListAsync();

    public async Task<List<ProductListItem>> GetProductsAsync(
        int? categoryId = null,
        string? search = null,
        ProductSort sort = ProductSort.Name,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        if (minPrice is < 0)
            minPrice = 0;
        if (maxPrice is < 0)
            maxPrice = 0;
        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            (minPrice, maxPrice) = (maxPrice, minPrice);

        var query = db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && p.StockQuantity > 0);

        if (categoryId.HasValue)
        {
            var categoryIds = await GetCategoryAndDescendantIdsAsync(categoryId.Value);
            query = query.Where(p => categoryIds.Contains(p.CategoryId));
        }

        if (!string.IsNullOrWhiteSpace(search))
            query = await ApplySearchAsync(query, search);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        var products = await query.ToListAsync();
        var productIds = products.Select(p => p.Id).ToList();
        var ratings = await GetProductRatingsAsync(productIds);

        var items = products.Select(p =>
        {
            ratings.TryGetValue(p.Id, out var rating);
            return new ProductListItem
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                AverageRating = rating.Average,
                ReviewCount = rating.Count
            };
        });

        return sort switch
        {
            ProductSort.PriceAsc => items.OrderBy(p => p.Price).ThenBy(p => p.Name).ToList(),
            ProductSort.PriceDesc => items.OrderByDescending(p => p.Price).ThenBy(p => p.Name).ToList(),
            ProductSort.RatingDesc => items.OrderByDescending(p => p.AverageRating).ThenByDescending(p => p.ReviewCount).ThenBy(p => p.Name).ToList(),
            _ => items.OrderBy(p => p.Name).ToList()
        };
    }

    public async Task<ProductDetail?> GetProductDetailAsync(int id)
    {
        var product = await db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null)
            return null;

        var ratings = await GetProductRatingsAsync([product.Id]);
        ratings.TryGetValue(product.Id, out var rating);

        var storeLocations = await db.StoreProducts
            .Include(sp => sp.Store)
            .Where(sp => sp.ProductId == id)
            .OrderBy(sp => sp.Store.Name)
            .Select(sp => new ProductStockLocation
            {
                LocationType = "Магазин",
                Name = sp.Store.Name,
                Address = sp.Store.Address,
                Quantity = sp.Quantity
            })
            .ToListAsync();

        var stockLocations = new List<ProductStockLocation>
        {
            new()
            {
                LocationType = "Склад",
                Name = WarehouseConstants.CentralWarehouseName,
                Address = WarehouseConstants.CentralWarehouseAddress,
                Quantity = product.StockQuantity
            }
        };
        stockLocations.AddRange(storeLocations);

        var reviews = await db.Reviews
            .Include(r => r.Author)
            .Include(r => r.Order).ThenInclude(o => o.Items)
            .Where(r => r.Order.Items.Any(i => i.ProductId == id))
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ProductReviewItem
            {
                Id = r.Id,
                AuthorName = r.Author.FullName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return new ProductDetail
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            AverageRating = rating.Average,
            ReviewCount = rating.Count,
            StockLocations = stockLocations,
            Reviews = reviews
        };
    }

    public Task<Product?> GetProductAsync(int id) =>
        db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    private async Task<IQueryable<Product>> ApplySearchAsync(IQueryable<Product> query, string search)
    {
        var terms = search
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var term in terms)
        {
            var pattern = $"%{term}%";
            var categoryIds = await GetCategoryIdsMatchingNameAsync(term);
            var reviewProductIds = await GetProductIdsMatchingReviewsAsync(term);
            var storeProductIds = await GetProductIdsMatchingStoresAsync(term);
            var matchesCentralWarehouse = MatchesCentralWarehouse(term);
            var price = TryParsePrice(term);

            query = query.Where(p =>
                EF.Functions.Like(p.Name, pattern) ||
                (p.Description != null && EF.Functions.Like(p.Description, pattern)) ||
                EF.Functions.Like(p.Category.Name, pattern) ||
                (p.Brand != null && EF.Functions.Like(p.Brand.Name, pattern)) ||
                categoryIds.Contains(p.CategoryId) ||
                reviewProductIds.Contains(p.Id) ||
                storeProductIds.Contains(p.Id) ||
                (matchesCentralWarehouse && p.StockQuantity > 0) ||
                (price != null && p.Price == price));
        }

        return query;
    }

    private async Task<HashSet<int>> GetCategoryIdsMatchingNameAsync(string term)
    {
        var pattern = $"%{term}%";
        var matchingRootIds = await db.Categories
            .Where(c => EF.Functions.Like(c.Name, pattern))
            .Select(c => c.Id)
            .ToListAsync();

        var result = new HashSet<int>();
        foreach (var id in matchingRootIds)
        {
            var treeIds = await GetCategoryAndDescendantIdsAsync(id);
            foreach (var treeId in treeIds)
                result.Add(treeId);
        }

        return result;
    }

    private async Task<List<int>> GetProductIdsMatchingReviewsAsync(string term)
    {
        var pattern = $"%{term}%";
        var rating = TryParseReviewRating(term);

        var query = db.Reviews
            .Include(r => r.Author)
            .Include(r => r.Order).ThenInclude(o => o.Items)
            .AsQueryable();

        query = query.Where(r =>
            (r.Comment != null && EF.Functions.Like(r.Comment, pattern)) ||
            EF.Functions.Like(r.Author.FullName, pattern) ||
            (rating != null && r.Rating == rating));

        return await query
            .SelectMany(r => r.Order.Items.Select(i => i.ProductId))
            .Distinct()
            .ToListAsync();
    }

    private async Task<List<int>> GetProductIdsMatchingStoresAsync(string term)
    {
        var pattern = $"%{term}%";
        return await db.StoreProducts
            .Include(sp => sp.Store)
            .Where(sp =>
                sp.Quantity > 0 &&
                (EF.Functions.Like(sp.Store.Name, pattern) ||
                 (sp.Store.Address != null && EF.Functions.Like(sp.Store.Address, pattern))))
            .Select(sp => sp.ProductId)
            .Distinct()
            .ToListAsync();
    }

    private static bool MatchesCentralWarehouse(string term)
    {
        var normalized = term.Trim();
        if (normalized.Length < 4)
            return false;

        return WarehouseConstants.CentralWarehouseName.Contains(normalized, StringComparison.OrdinalIgnoreCase)
            || normalized.Contains(WarehouseConstants.CentralWarehouseName, StringComparison.OrdinalIgnoreCase)
            || WarehouseConstants.CentralWarehouseAddress.Contains(normalized, StringComparison.OrdinalIgnoreCase)
            || normalized.Contains(WarehouseConstants.CentralWarehouseAddress, StringComparison.OrdinalIgnoreCase);
    }

    private static int? TryParseReviewRating(string term) =>
        int.TryParse(term, out var rating) && rating is >= 1 and <= 5 ? rating : null;

    private static decimal? TryParsePrice(string term)
    {
        var cleaned = term
            .Replace("₽", "", StringComparison.Ordinal)
            .Replace("руб", "", StringComparison.OrdinalIgnoreCase)
            .Replace(" ", "")
            .Trim();

        return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var price)
            || decimal.TryParse(cleaned, NumberStyles.Number, new CultureInfo("ru-RU"), out price)
            ? price
            : null;
    }

    private async Task<HashSet<int>> GetCategoryAndDescendantIdsAsync(int categoryId)
    {
        var all = await db.Categories.Select(c => new { c.Id, c.ParentId }).ToListAsync();
        var result = new HashSet<int> { categoryId };
        var queue = new Queue<int>();
        queue.Enqueue(categoryId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var child in all.Where(c => c.ParentId == current))
            {
                if (result.Add(child.Id))
                    queue.Enqueue(child.Id);
            }
        }

        return result;
    }

    private async Task<Dictionary<int, (double Average, int Count)>> GetProductRatingsAsync(IReadOnlyList<int> productIds)
    {
        if (productIds.Count == 0)
            return [];

        var reviews = await db.Reviews
            .Include(r => r.Order).ThenInclude(o => o.Items)
            .Where(r => r.Order.Items.Any(i => productIds.Contains(i.ProductId)))
            .ToListAsync();

        return reviews
            .SelectMany(r => r.Order.Items
                .Where(i => productIds.Contains(i.ProductId))
                .Select(i => i.ProductId)
                .Distinct()
                .Select(productId => new { productId, r.Rating, r.Id }))
            .GroupBy(x => x.productId)
            .ToDictionary(
                g => g.Key,
                g => (g.Average(x => (double)x.Rating), g.Select(x => x.Id).Distinct().Count()));
    }

    private static List<CategoryTreeItem> BuildTree(IReadOnlyList<CategoryTreeItem> categories, int? parentId) =>
        categories
            .Where(c => c.ParentId == parentId)
            .Select(c =>
            {
                c.Children = BuildTree(categories, c.Id);
                return c;
            })
            .OrderBy(c => c.Name)
            .ToList();
}
