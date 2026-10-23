using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Подключение BusinessData к конкретному представлению <see cref="HighchartView"/>.</summary>
public interface IHighchartViewDataService
{
    void ApplyFromAnalytics(HighchartView view, HighchartAnalyticsSelection selection = null);
}
