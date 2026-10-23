using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Сервис данных для каждого типа представления Highcharts (делегирует общую реализацию).</summary>
public static class HighchartViewDataServiceRegistry
{
    private static readonly Dictionary<string, IHighchartViewDataService> ByViewType =
        new(StringComparer.Ordinal);

    static HighchartViewDataServiceRegistry()
    {
        var defaultService = new HighchartViewDataService();
        foreach (var viewTypeName in GetHighchartViewTypeNames())
            ByViewType[viewTypeName] = defaultService;
    }

    public static IHighchartViewDataService Get(string viewTypeName)
    {
        if (string.IsNullOrWhiteSpace(viewTypeName))
            return new HighchartViewDataService();
        return ByViewType.TryGetValue(viewTypeName, out var service)
            ? service
            : new HighchartViewDataService();
    }

    public static IHighchartViewDataService Get(HighchartView view) =>
        Get(view?.GetType().Name);

    private static IEnumerable<string> GetHighchartViewTypeNames()
    {
        return typeof(HighchartView).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(HighchartView).IsAssignableFrom(t))
            .Select(t => t.Name);
    }
}
