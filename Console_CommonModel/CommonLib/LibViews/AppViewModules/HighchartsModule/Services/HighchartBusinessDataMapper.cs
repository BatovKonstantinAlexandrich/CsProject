using System.Globalization;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Строит ряды диаграммы из строк <see cref="BusinessData"/> (поле <see cref="BusinessData.IndValue"/>).</summary>
internal static class HighchartBusinessDataMapper
{
    public static HighchartBusinessDataResult FromRows(
        IReadOnlyList<BusinessData> rows,
        BusinessAnaliticsService analytics = null)
    {
        if (rows is null || rows.Count == 0)
            return new HighchartBusinessDataResult();

        analytics ??= new BusinessAnaliticsService();

        var indicatorIds = rows.Select(r => r.BusinessIndicatorID).Distinct().ToList();
        var indicators = analytics.GetIndicators()
            .Where(i => indicatorIds.Contains(i.Id))
            .ToDictionary(i => i.Id, i => i.Name ?? $"#{i.Id}");

        var categories = rows.Select(r => r.BeginDate)
            .Distinct()
            .OrderBy(d => d)
            .Select(d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            .ToList();

        var series = new Dictionary<string, List<float?>>(StringComparer.Ordinal);
        foreach (var indicatorId in indicatorIds)
        {
            var name = indicators.TryGetValue(indicatorId, out var indicatorName)
                ? indicatorName
                : $"#{indicatorId}";

            series[name] = categories
                .Select(cat =>
                {
                    var date = DateTime.ParseExact(cat, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    return rows.FirstOrDefault(r =>
                        r.BusinessIndicatorID == indicatorId && r.BeginDate.Date == date.Date)?.IndValue;
                })
                .ToList();
        }

        return new HighchartBusinessDataResult { Series = series, Categories = categories };
    }
}
