using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Применяет загруженные показатели к конкретному типу диаграммы Highcharts.</summary>
public interface IHighchartChartDataBinder
{
    bool CanBind(string chartTypeName);
    void Apply(Highchart chart, HighchartBusinessDataResult data);
}
