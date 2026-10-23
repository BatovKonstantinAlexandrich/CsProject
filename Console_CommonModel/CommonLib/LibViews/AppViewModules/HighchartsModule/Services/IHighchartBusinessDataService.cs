using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Загрузка показателей из БД аналитики и применение к модели Highcharts.</summary>
public interface IHighchartBusinessDataService
{
    HighchartBusinessDataResult Load(HighchartAnalyticsSelection selection);
    void ApplyToChart(Highchart chart, string chartTypeName, HighchartAnalyticsSelection selection);
}
