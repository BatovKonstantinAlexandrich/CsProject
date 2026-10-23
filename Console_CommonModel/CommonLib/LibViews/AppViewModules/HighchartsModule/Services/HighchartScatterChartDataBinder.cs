using System.Collections;
using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Scatter / bubble: точки <c>[[x,y], …]</c> или <c>[[x,y,z], …]</c>.</summary>
public sealed class HighchartScatterChartDataBinder : IHighchartChartDataBinder
{
    private static readonly string[] ScatterTokens = { "scatter", "dynamicclicktoadd", "bubble" };

    public bool CanBind(string chartTypeName)
    {
        if (string.IsNullOrWhiteSpace(chartTypeName))
            return false;
        var lower = chartTypeName.ToLowerInvariant();
        return ScatterTokens.Any(t => lower.Contains(t));
    }

    public void Apply(Highchart chart, HighchartBusinessDataResult data)
    {
        if (chart is null || data is null || data.Series.Count == 0)
            return;

        var seriesProp = HighchartChartModelReflection.GetSeriesProperty(chart.GetType());
        if (seriesProp is null)
            return;

        var itemType = HighchartChartModelReflection.GetSeriesItemType(seriesProp);
        var dataProp = itemType?.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public);
        if (itemType is null || dataProp is null || !HighchartChartModelReflection.IsNestedPointData(dataProp.PropertyType))
            return;

        var pointDimensions = GetPointDimensions(chart.GetType().Name, dataProp.PropertyType);
        var list = (IList)Activator.CreateInstance(seriesProp.PropertyType);
        var nameProp = itemType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);

        foreach (var entry in data.Series)
        {
            var item = Activator.CreateInstance(itemType);
            nameProp?.SetValue(item, entry.Key);
            dataProp.SetValue(item, CreateScatterPoints(dataProp.PropertyType, entry.Value, pointDimensions));
            list.Add(item);
        }

        seriesProp.SetValue(chart, list);
        HighchartChartModelReflection.SetCategories(chart, data.Categories);
    }

    private static int GetPointDimensions(string chartTypeName, Type dataListType) =>
        chartTypeName.Contains("3d", StringComparison.OrdinalIgnoreCase) ? 3 : 2;

    private static object CreateScatterPoints(Type dataListType, List<float?> values, int dimensions)
    {
        var pointListType = dataListType.GetGenericArguments()[0];
        var coordType = pointListType.GetGenericArguments()[0];
        var list = (IList)Activator.CreateInstance(dataListType);

        for (var i = 0; i < values.Count; i++)
        {
            var v = values[i] ?? 0f;
            var point = (IList)Activator.CreateInstance(pointListType);
            if (dimensions >= 3)
            {
                point.Add(ConvertToCoord(coordType, i));
                point.Add(ConvertToCoord(coordType, 0f));
                point.Add(ConvertToCoord(coordType, v));
            }
            else
            {
                point.Add(ConvertToCoord(coordType, i));
                point.Add(ConvertToCoord(coordType, v));
            }

            list.Add(point);
        }

        return list;
    }

    private static object ConvertToCoord(Type coordType, float value) =>
        coordType == typeof(float?) ? (float?)value : Convert.ChangeType(value, coordType);
}
