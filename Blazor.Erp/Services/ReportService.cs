using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class ReportService(ApplicationDbContext db)
{
    public const int AllReportsGroupId = 999;
    public const int FavoritesGroupId = 1000;

    public async Task<List<ReportGroupItem>> GetGroupsAsync()
    {
        var groups = await db.ReportGroups
            .OrderBy(g => g.Name)
            .Select(g => new ReportGroupItem { Id = g.Id, Name = g.Name })
            .ToListAsync();

        groups.Insert(0, new ReportGroupItem { Id = AllReportsGroupId, Name = "!Все" });
        groups.Insert(1, new ReportGroupItem { Id = FavoritesGroupId, Name = "!Избранное" });
        return groups;
    }

    public async Task<List<ReportListItem>> GetReportsAsync(string userId, int groupId, string? titleFilter = null)
    {
        var favoriteIds = await db.ReportUserFavorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ReportId)
            .ToListAsync();

        var query = db.ReportProcedures
            .Include(r => r.Group)
            .Where(r => !r.IsHidden);

        if (groupId == FavoritesGroupId)
            query = query.Where(r => favoriteIds.Contains(r.Id));
        else if (groupId != AllReportsGroupId)
            query = query.Where(r => r.GroupId == groupId);

        if (!string.IsNullOrWhiteSpace(titleFilter))
        {
            var filter = titleFilter.Trim();
            query = query.Where(r => EF.Functions.Like(r.Title, $"%{filter}%"));
        }

        return await query
            .OrderBy(r => r.Title)
            .Select(r => new ReportListItem
            {
                Id = r.Id,
                Title = r.Title,
                GroupId = r.GroupId,
                GroupName = r.Group.Name,
                DiagramType = r.DiagramType,
                IsFavorite = favoriteIds.Contains(r.Id)
            })
            .ToListAsync();
    }

    public Task<ReportProcedure?> GetReportAsync(int reportId) =>
        db.ReportProcedures
            .Include(r => r.Parameters.OrderBy(p => p.SortOrder))
            .Include(r => r.Columns.OrderBy(c => c.SortOrder))
            .FirstOrDefaultAsync(r => r.Id == reportId);

    public async Task ToggleFavoriteAsync(string userId, int reportId)
    {
        var existing = await db.ReportUserFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ReportId == reportId);

        if (existing != null)
            db.ReportUserFavorites.Remove(existing);
        else
            db.ReportUserFavorites.Add(new ReportUserFavorite { UserId = userId, ReportId = reportId });

        await db.SaveChangesAsync();
    }

    public async Task<ReportResult?> ExecuteReportAsync(int reportId, int? storeId, Dictionary<string, string?> parameters)
    {
        var report = await GetReportAsync(reportId);
        if (report == null)
            return null;

        var parsed = ParseParameters(report, parameters);
        var rows = report.ProcedureName switch
        {
            "stat_store_sales_by_day" => await BuildSalesByDayAsync(storeId, parsed),
            "stat_top_products" => await BuildTopProductsAsync(storeId, parsed),
            "stat_inventory" => await BuildInventoryAsync(storeId),
            "stat_cashier_payments" => await BuildCashierPaymentsAsync(storeId, parsed),
            "stat_orders_by_status" => await BuildOrdersByStatusAsync(storeId, parsed),
            _ => []
        };

        return new ReportResult
        {
            ReportTitle = report.Title,
            DiagramType = report.DiagramType,
            ChartXField = report.ChartXField,
            ChartYField = report.ChartYField,
            Columns = report.Columns.Select(c => new ReportColumnInfo
            {
                Name = c.ColumnName,
                Title = c.Title,
                Format = c.Format,
                ShowSum = c.ShowSum
            }).ToList(),
            Rows = rows
        };
    }

    private static Dictionary<string, object?> ParseParameters(ReportProcedure report, Dictionary<string, string?> values)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var param in report.Parameters)
        {
            values.TryGetValue(param.Name, out var raw);
            raw ??= param.DefaultValue;

            result[param.Name] = param.Type switch
            {
                ReportParameterType.Integer => int.TryParse(raw, out var i) ? i : 10,
                ReportParameterType.Float => double.TryParse(raw, out var d) ? d : 0d,
                ReportParameterType.Date => DateTime.TryParse(raw, out var dt) ? dt.Date : DateTime.UtcNow.Date,
                _ => raw ?? string.Empty
            };
        }

        return result;
    }

    private async Task<List<Dictionary<string, object?>>> BuildSalesByDayAsync(int? storeId, Dictionary<string, object?> p)
    {
        var from = (DateTime)(p.GetValueOrDefault("dateFrom") ?? DateTime.UtcNow.Date.AddDays(-30));
        var to = (DateTime)(p.GetValueOrDefault("dateTo") ?? DateTime.UtcNow.Date);

        var ordersQuery = db.Orders.AsQueryable();
        if (storeId.HasValue)
            ordersQuery = ordersQuery.Where(o => o.StoreId == storeId);

        var orders = await ordersQuery
            .Where(o => o.CreatedAt.Date >= from && o.CreatedAt.Date <= to)
            .ToListAsync();

        return orders
            .GroupBy(o => o.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new Dictionary<string, object?>
            {
                ["report_date"] = g.Key,
                ["orders_count"] = g.Count(),
                ["revenue"] = g.Sum(o => o.TotalAmount)
            })
            .ToList();
    }

    private async Task<List<Dictionary<string, object?>>> BuildTopProductsAsync(int? storeId, Dictionary<string, object?> p)
    {
        var topN = p.GetValueOrDefault("topN") is int n ? n : 10;

        var itemsQuery = db.OrderItems
            .Include(i => i.Order)
            .Include(i => i.Product)
            .AsQueryable();

        if (storeId.HasValue)
            itemsQuery = itemsQuery.Where(i => i.Order.StoreId == storeId);

        var grouped = await itemsQuery
            .GroupBy(i => new { i.ProductId, i.Product.Name })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.Name,
                Qty = g.Sum(i => i.Quantity),
                Amount = g.Sum(i => i.UnitPrice * i.Quantity)
            })
            .OrderByDescending(x => x.Amount)
            .Take(topN)
            .ToListAsync();

        return grouped.Select((x, index) => new Dictionary<string, object?>
        {
            ["rank"] = index + 1,
            ["product_name"] = x.Name,
            ["quantity"] = x.Qty,
            ["amount"] = x.Amount
        }).ToList();
    }

    private async Task<List<Dictionary<string, object?>>> BuildInventoryAsync(int? storeId)
    {
        if (!storeId.HasValue)
            return [];

        var items = await db.StoreProducts
            .Include(sp => sp.Product)
            .ThenInclude(p => p!.Brand)
            .Where(sp => sp.StoreId == storeId)
            .OrderBy(sp => sp.Product.Name)
            .ToListAsync();

        return items.Select(sp => new Dictionary<string, object?>
        {
            ["product_name"] = sp.Product.Name,
            ["brand"] = sp.Product.Brand?.Name ?? "—",
            ["quantity"] = sp.Quantity,
            ["price"] = sp.Product.Price,
            ["stock_value"] = sp.Product.Price * sp.Quantity
        }).ToList();
    }

    private async Task<List<Dictionary<string, object?>>> BuildCashierPaymentsAsync(int? storeId, Dictionary<string, object?> p)
    {
        var from = (DateTime)(p.GetValueOrDefault("dateFrom") ?? DateTime.UtcNow.Date.AddDays(-7));
        var to = (DateTime)(p.GetValueOrDefault("dateTo") ?? DateTime.UtcNow.Date);

        var basketsQuery = db.SaleBaskets
            .Include(b => b.Items)
            .AsQueryable();

        if (storeId.HasValue)
            basketsQuery = basketsQuery.Where(b => b.StoreId == storeId);

        var baskets = await basketsQuery
            .Where(b => b.CreatedAt.Date >= from && b.CreatedAt.Date <= to)
            .ToListAsync();

        return baskets
            .GroupBy(b => b.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new Dictionary<string, object?>
            {
                ["report_date"] = g.Key,
                ["baskets_count"] = g.Count(),
                ["paid_amount"] = g.Sum(b => b.PaidAmount),
                ["refund_sum"] = g.Sum(b => b.RefundSum)
            })
            .ToList();
    }

    private async Task<List<Dictionary<string, object?>>> BuildOrdersByStatusAsync(int? storeId, Dictionary<string, object?> p)
    {
        var from = (DateTime)(p.GetValueOrDefault("dateFrom") ?? DateTime.UtcNow.Date.AddDays(-30));
        var to = (DateTime)(p.GetValueOrDefault("dateTo") ?? DateTime.UtcNow.Date);

        var ordersQuery = db.Orders.AsQueryable();
        if (storeId.HasValue)
            ordersQuery = ordersQuery.Where(o => o.StoreId == storeId);

        var orders = await ordersQuery
            .Where(o => o.CreatedAt.Date >= from && o.CreatedAt.Date <= to)
            .ToListAsync();

        return orders
            .GroupBy(o => o.Status)
            .OrderBy(g => g.Key)
            .Select(g => new Dictionary<string, object?>
            {
                ["status_name"] = GetOrderStatusLabel(g.Key),
                ["orders_count"] = g.Count(),
                ["total_amount"] = g.Sum(o => o.TotalAmount)
            })
            .ToList();
    }

    public static string GetOrderStatusLabel(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Ожидает оплаты",
        OrderStatus.Paid => "Оплачен",
        OrderStatus.Processing => "Сборка",
        OrderStatus.Shipped => "Отправлен",
        OrderStatus.ReadyForDelivery => "Готов к выдаче",
        OrderStatus.InDelivery => "В доставке",
        OrderStatus.Delivered => "Доставлен",
        OrderStatus.Cancelled => "Отменён",
        _ => status.ToString()
    };

    public static string GetDiagramLabel(ReportDiagramType type) => type switch
    {
        ReportDiagramType.Line => "Линейная",
        ReportDiagramType.Bar => "Столбчатая",
        ReportDiagramType.Pie => "Круговая",
        _ => "Таблица"
    };
}
