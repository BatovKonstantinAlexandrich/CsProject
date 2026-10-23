using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

public sealed class HighchartViewDataService : IHighchartViewDataService
{
    public void ApplyFromAnalytics(HighchartView view, HighchartAnalyticsSelection selection = null)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        view.EnsureModel();
        if (view.Model is null)
            return;

        var sel = selection ?? view.AnalyticsSelection;
        if (sel is null)
            throw new InvalidOperationException("Не заданы параметры выборки аналитики.");

        var data = LoadBusinessDataRows(sel);
        view.AnalyticsSelection = sel;
        view.ApplyBusinessData(data);
    }

    private static List<BusinessData> LoadBusinessDataRows(HighchartAnalyticsSelection sel)
    {
        var bas = new BusinessAnaliticsService();

        if (sel.HasResourceAndIndicators)
        {
            var resource = bas.GetResourceById(sel.BusinessResourceId!.Value);
            if (resource is null)
                throw new InvalidOperationException("Ресурс не найден.");

            var indicators = sel.BusinessIndicatorIds
                .Select(bas.GetIndicatorById)
                .Where(i => i is not null)
                .ToList();
            if (indicators.Count == 0)
                throw new InvalidOperationException("Показатели не выбраны.");

            return bas.GetBusinessData(resource, indicators);
        }

        if (sel.HasDatasetFilter && sel.BusinessResourceId is > 0)
        {
            return bas.GetDatasetValues(
                    sel.BusinessResourceId.Value,
                    sel.BusinessDatasetId!.Value,
                    sel.BeginDate,
                    sel.GranularityId!.Value)
                .ToList();
        }

        throw new InvalidOperationException(
            "Укажите ресурс и показатели либо набор данных, периодичность, дату и ресурс.");
    }
}
