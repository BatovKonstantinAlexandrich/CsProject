namespace BlazorErp.Shared.Views.Models;

public sealed class ViewChartData
{
    public string Title { get; set; } = string.Empty;
    public ViewChartType ChartType { get; set; } = ViewChartType.Bar;
    public string[] Labels { get; set; } = [];
    public IList<ViewChartSeries> Series { get; set; } = [];
}

public sealed class ViewChartSeries
{
    public string Name { get; set; } = string.Empty;
    public double[] Values { get; set; } = [];
}
