using System.Collections.Concurrent;
using System.Reflection;

using Newtonsoft.Json;

using WinFormsDesignerApp.ViewModels.HighCharts;

namespace WinFormsApp.AppComponents.AppControls.Highcharts;

/// <summary>Реестр корневых типов диаграмм Highcharts в проекте.</summary>
public static class HighchartTypeRegistry
{
    private static readonly Lazy<IReadOnlyList<Type>> RootTypes = new(DiscoverRootTypes);
    private static readonly IReadOnlyDictionary<string, Type> ByName =
        RootTypes.Value.ToDictionary(t => t.Name, t => t, StringComparer.Ordinal);

    private static readonly ConcurrentDictionary<string, Highchart> DefaultModels = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, string> DefaultModelJson = new(StringComparer.Ordinal);

    public static IReadOnlyList<string> ChartTypeNames =>
        RootTypes.Value.Select(t => t.Name).OrderBy(n => n).ToList();

    public static IReadOnlyList<Type> RootChartTypes => RootTypes.Value;

    public static Type GetType(string chartTypeName)
    {
        if (string.IsNullOrWhiteSpace(chartTypeName))
            throw new ArgumentException("Chart type name is required.", nameof(chartTypeName));
        if (ByName.TryGetValue(chartTypeName, out var type))
            return type;
        throw new KeyNotFoundException($"Highchart type '{chartTypeName}' was not found.");
    }

    /// <summary>Общий экземпляр модели по умолчанию (только чтение в галерее/превью).</summary>
    public static Highchart GetDefaultModel(string chartTypeName)
    {
        if (string.IsNullOrWhiteSpace(chartTypeName))
            throw new ArgumentException("Chart type name is required.", nameof(chartTypeName));
        return DefaultModels.GetOrAdd(chartTypeName, static name =>
            (Highchart)Activator.CreateInstance(GetType(name))!);
    }

    /// <summary>Независимая копия модели по умолчанию (для редактирования / бизнес-данных).</summary>
    public static Highchart CreateInstance(string chartTypeName)
        => CloneModel(GetDefaultModel(chartTypeName));

    public static bool IsSharedDefault(Highchart model)
    {
        if (model is null)
            return false;
        return DefaultModels.TryGetValue(model.GetType().Name, out var shared)
            && ReferenceEquals(shared, model);
    }

    public static Highchart CloneIfShared(Highchart model)
        => model is null || !IsSharedDefault(model) ? model : CloneModel(model);

    public static Highchart CloneModel(Highchart model)
    {
        if (model is null)
            return null;
        var type = model.GetType();
        var json = IsSharedDefault(model)
            ? GetDefaultModelJson(type.Name)
            : HighchartJsonSerializer.SerializeCore(model);
        return (Highchart)JsonConvert.DeserializeObject(json, type)!;
    }

    internal static bool TryGetDefaultModelJson(Highchart options, out string json)
    {
        if (options is not null
            && DefaultModels.TryGetValue(options.GetType().Name, out var shared)
            && ReferenceEquals(shared, options))
        {
            json = GetDefaultModelJson(options.GetType().Name);
            return true;
        }

        json = null;
        return false;
    }

    internal static string GetDefaultModelJson(string chartTypeName)
    {
        GetDefaultModel(chartTypeName);
        return DefaultModelJson.GetOrAdd(chartTypeName, static name =>
            HighchartJsonSerializer.SerializeCore(DefaultModels[name]));
    }

    public static bool IsRootChartType(Type type)
    {
        if (type is null || !typeof(Highchart).IsAssignableFrom(type) || type == typeof(Highchart))
            return false;
        return RootTypes.Value.Contains(type);
    }

    private static IReadOnlyList<Type> DiscoverRootTypes()
    {
        return typeof(HighchartLineBasic).Assembly
            .GetTypes()
            .Where(t =>
                t.IsClass
                && !t.IsAbstract
                && t.Namespace == typeof(HighchartLineBasic).Namespace
                && typeof(Highchart).IsAssignableFrom(t)
                && t != typeof(Highchart)
                && HasRootChartShape(t))
            .OrderBy(t => t.Name)
            .ToList();
    }

    private static bool HasRootChartShape(Type type)
    {
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var jp = prop.GetCustomAttribute<JsonPropertyAttribute>();
            if (jp is null)
                continue;
            var name = jp.PropertyName ?? prop.Name;
            if (name is "series" or "chart" or "drilldown" or "pane" or "colorAxis")
                return true;
        }
        return false;
    }
}
