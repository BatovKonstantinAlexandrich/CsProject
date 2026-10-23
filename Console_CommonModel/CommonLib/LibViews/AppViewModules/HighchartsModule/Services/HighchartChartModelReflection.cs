using System.Collections;
using System.Reflection;

using Newtonsoft.Json;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

internal static class HighchartChartModelReflection
{
    public static PropertyInfo GetSeriesProperty(Type chartType) =>
        chartType.GetProperty("Series", BindingFlags.Instance | BindingFlags.Public);

    public static void SetCategories(object chart, IList<string> categories)
    {
        if (categories is null || categories.Count == 0)
            return;

        foreach (var prop in chart.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!string.Equals(prop.Name, "XAxis", StringComparison.OrdinalIgnoreCase))
                continue;

            var axis = prop.GetValue(chart);
            if (axis is null)
                continue;

            if (TrySetCategoriesOnObject(axis, categories))
                return;
        }
    }

    private static bool TrySetCategoriesOnObject(object axis, IList<string> categories)
    {
        foreach (var prop in axis.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var jp = prop.GetCustomAttribute<JsonPropertyAttribute>();
            if (jp?.PropertyName == "categories" || prop.Name == "Categories")
            {
                if (!typeof(IList).IsAssignableFrom(prop.PropertyType))
                    continue;

                var list = CreateStringList(prop.PropertyType, categories);
                prop.SetValue(axis, list);
                return true;
            }
        }

        return false;
    }

    private static object CreateStringList(Type listType, IList<string> categories)
    {
        if (!listType.IsGenericType)
            return categories.ToList();

        var elementType = listType.GetGenericArguments()[0];
        var list = (IList)Activator.CreateInstance(listType);
        foreach (var cat in categories)
        {
            if (elementType == typeof(string))
                list.Add(cat);
            else
                list.Add(Convert.ChangeType(cat, elementType));
        }

        return list;
    }

    public static bool IsNestedPointData(Type dataListType) =>
        dataListType.IsGenericType
        && dataListType.GetGenericArguments()[0].IsGenericType;

    public static IList CreateSeriesList(Type itemType, Dictionary<string, List<float?>> series)
    {
        var listType = typeof(List<>).MakeGenericType(itemType);
        var list = (IList)Activator.CreateInstance(listType);
        var nameProp = itemType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
        var dataProp = itemType.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public);
        if (dataProp is null || IsNestedPointData(dataProp.PropertyType))
            return list;

        foreach (var entry in series)
        {
            var item = Activator.CreateInstance(itemType);
            nameProp?.SetValue(item, entry.Key);
            dataProp.SetValue(item, CreateNumericDataList(dataProp.PropertyType, entry.Value));
            list.Add(item);
        }

        return list;
    }

    private static object CreateNumericDataList(Type dataListType, List<float?> values)
    {
        if (!dataListType.IsGenericType)
            return values;

        var elementType = dataListType.GetGenericArguments()[0];
        var list = (IList)Activator.CreateInstance(dataListType);
        foreach (var v in values)
        {
            if (v is null && elementType.IsClass)
            {
                list.Add(null);
                continue;
            }

            list.Add(v is null ? Activator.CreateInstance(elementType) : Convert.ChangeType(v, elementType));
        }

        return list;
    }

    public static Type GetSeriesItemType(PropertyInfo seriesProperty)
    {
        var listType = seriesProperty.PropertyType;
        return listType.IsGenericType ? listType.GetGenericArguments()[0] : null;
    }

    public static Type GetPiePointType(Type seriesItemType)
    {
        var dataProp = seriesItemType?.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public);
        if (dataProp is null || !dataProp.PropertyType.IsGenericType)
            return null;
        return dataProp.PropertyType.GetGenericArguments()[0];
    }
}
