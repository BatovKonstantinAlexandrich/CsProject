namespace BlazorErp.Models;

public class StoreSalesStatistics
{
    public int TotalOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int AwaitingPreparation { get; set; }
    public int ReadyForDelivery { get; set; }
    public int InDelivery { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal CompletedRevenue { get; set; }
    public List<StoreTopSellingProduct> TopProducts { get; set; } = [];
}

public class StoreTopSellingProduct
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}
