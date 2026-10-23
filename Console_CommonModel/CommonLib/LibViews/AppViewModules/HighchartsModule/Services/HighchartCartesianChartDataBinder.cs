using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Линейные, столбчатые, областные и комбинированные диаграммы.</summary>
public sealed class HighchartCartesianChartDataBinder : IHighchartChartDataBinder

{
    private static readonly string[] PieTokens = { "pie", "donut", "drilldown" };
    private static readonly string[] ScatterTokens = { "scatter", "dynamicclicktoadd", "bubble" };

    public bool CanBind(string chartTypeName)
    {
        if (string.IsNullOrWhiteSpace(chartTypeName))
            return true;
        var lower = chartTypeName.ToLowerInvariant();
        if (PieTokens.Any(t => lower.Contains(t)))
            return false;
        return !ScatterTokens.Any(t => lower.Contains(t));
    }

    public void Apply(Highchart chart, HighchartBusinessDataResult data)
    {
        if (chart is null || data is null)
            return;

        var seriesProp = HighchartChartModelReflection.GetSeriesProperty(chart.GetType());
        if (seriesProp is null)
            return;

        var itemType = HighchartChartModelReflection.GetSeriesItemType(seriesProp);
        if (itemType is null)
            return;

        var list = HighchartChartModelReflection.CreateSeriesList(itemType, data.Series);
        seriesProp.SetValue(chart, list);
        HighchartChartModelReflection.SetCategories(chart, data.Categories);
    }
}
