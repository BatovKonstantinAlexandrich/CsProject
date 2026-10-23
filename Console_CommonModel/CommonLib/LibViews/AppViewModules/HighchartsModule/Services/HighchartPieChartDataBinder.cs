using System.Collections;
using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Круговые диаграммы: срез на показатель или ресурс.</summary>
public sealed class HighchartPieChartDataBinder : IHighchartChartDataBinder
{
    private static readonly string[] PieTokens = { "pie", "donut" };

    public bool CanBind(string chartTypeName)
    {
        if (string.IsNullOrWhiteSpace(chartTypeName))
            return false;
        var lower = chartTypeName.ToLowerInvariant();
        return PieTokens.Any(t => lower.Contains(t));
    }

    public void Apply(Highchart chart, HighchartBusinessDataResult data)
    {
        if (chart is null || data is null || data.Series.Count == 0)
            return;

        var seriesProp = HighchartChartModelReflection.GetSeriesProperty(chart.GetType());
        if (seriesProp is null)
            return;

        var itemType = HighchartChartModelReflection.GetSeriesItemType(seriesProp);
        var pointType = HighchartChartModelReflection.GetPiePointType(itemType);
        if (itemType is null || pointType is null)
        {
            new HighchartCartesianChartDataBinder().Apply(chart, data);
            return;
        }

        var seriesListType = seriesProp.PropertyType;
        var seriesList = (IList)Activator.CreateInstance(seriesListType);
        var seriesItem = Activator.CreateInstance(itemType);
        itemType.GetProperty("Name")?.SetValue(seriesItem, "Показатели");

        var dataListType = itemType.GetProperty("Data").PropertyType;
        var points = (IList)Activator.CreateInstance(dataListType);
        var nameProp = pointType.GetProperty("Name");
        var yProp = pointType.GetProperties().First(p => p.Name == "Y") ?? pointType.GetProperties().First(p => p.Name == "y");

        foreach (var entry in data.Series)
        {
            var sum = entry.Value.Where(v => v.HasValue).Select(v => v.Value).DefaultIfEmpty(0).Sum();
            var point = Activator.CreateInstance(pointType);
            nameProp?.SetValue(point, entry.Key);
            yProp?.SetValue(point, Convert.ChangeType(sum, yProp.PropertyType));
            points.Add(point);
        }

        itemType.GetProperty("Data").SetValue(seriesItem, points);
        seriesList.Add(seriesItem);
        seriesProp.SetValue(chart, seriesList);
    }
}
