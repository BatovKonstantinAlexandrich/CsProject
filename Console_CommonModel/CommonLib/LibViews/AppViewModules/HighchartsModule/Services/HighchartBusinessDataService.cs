using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

public sealed class HighchartBusinessDataService : IHighchartBusinessDataService
{
    private readonly BusinessAnaliticsService _analytics;

    public HighchartBusinessDataService()
        : this(new BusinessAnaliticsService(new BusinessDbContext()))
    {
    }

    public HighchartBusinessDataService(BusinessAnaliticsService analytics)
    {
        _analytics = analytics ?? throw new ArgumentNullException(nameof(analytics));
    }

    public HighchartBusinessDataResult Load(HighchartAnalyticsSelection selection)
    {
        if (selection is null)
            throw new ArgumentNullException(nameof(selection));

        if (selection.HasResourceAndIndicators)
            return LoadByResourceAndIndicators(selection);

        if (selection.HasDatasetFilter && selection.BusinessResourceId is > 0)
            return LoadByDatasetSlice(selection);

        throw new InvalidOperationException(
            "Укажите ресурс и показатели либо набор данных, периодичность, дату и ресурс.");
    }

    public void ApplyToChart(Highchart chart, string chartTypeName, HighchartAnalyticsSelection selection)
    {
        if (chart is null)
            throw new ArgumentNullException(nameof(chart));
        if (string.IsNullOrWhiteSpace(chartTypeName))
            chartTypeName = chart.GetType().Name;

        var data = Load(selection);
        var binder = HighchartChartDataBinderResolver.Resolve(chartTypeName);
        binder.Apply(chart, data);
    }

    private HighchartBusinessDataResult LoadByResourceAndIndicators(HighchartAnalyticsSelection selection)
    {
        var resource = _analytics.GetResourceById(selection.BusinessResourceId!.Value);
        if (resource is null)
            throw new InvalidOperationException("Ресурс не найден.");

        var indicators = _analytics.GetIndicators()
            .Where(i => selection.BusinessIndicatorIds.Contains(i.Id))
            .ToList();
        if (indicators.Count == 0)
            throw new InvalidOperationException("Показатели не выбраны.");

        var rows = _analytics.GetBusinessData(resource, indicators);
        if (rows.Count == 0)
            throw new InvalidOperationException("Нет данных BusinessData для выбранных критериев.");

        var categories = rows.Select(r => r.BeginDate)
            .Distinct()
            .OrderBy(d => d)
            .Select(d => d.ToString("yyyy-MM-dd"))
            .ToList();

        var series = new Dictionary<string, List<float?>>(StringComparer.Ordinal);
        foreach (var indicator in indicators)
        {
            series[indicator.Name] = categories
                .Select(cat =>
                {
                    var date = DateTime.ParseExact(cat, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    return rows.FirstOrDefault(r =>
                        r.BusinessIndicatorID == indicator.Id && r.BeginDate.Date == date.Date)?.IndValue;
                })
                .ToList();
        }

        return new HighchartBusinessDataResult { Series = series, Categories = categories };
    }

    private HighchartBusinessDataResult LoadByDatasetSlice(HighchartAnalyticsSelection selection)
    {
        var rows = _analytics.GetDatasetValues(
                selection.BusinessResourceId!.Value,
                selection.BusinessDatasetId!.Value,
                selection.BeginDate,
                selection.GranularityId!.Value)
            .ToList();

        if (rows.Count == 0)
            throw new InvalidOperationException("Нет данных BusinessData для выбранных критериев.");

        var indicatorIds = rows.Select(r => r.BusinessIndicatorID).Distinct().ToList();
        var indicators = _analytics.GetIndicators()
            .Where(i => indicatorIds.Contains(i.Id))
            .ToList();

        if (selection.BusinessIndicatorIds is { Count: > 0 })
            indicators = indicators.Where(i => selection.BusinessIndicatorIds.Contains(i.Id)).ToList();

        var categories = new List<string> { selection.BeginDate };
        var series = new Dictionary<string, List<float?>>(StringComparer.Ordinal);
        foreach (var indicator in indicators)
        {
            var value = rows.FirstOrDefault(r => r.BusinessIndicatorID == indicator.Id)?.IndValue;
            series[indicator.Name] = new List<float?> { value };
        }

        return new HighchartBusinessDataResult { Series = series, Categories = categories };
    }
}
