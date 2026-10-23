
using Console_CommonModel.CommonLib.LibDesigner.DesignerServices;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;

[ViewUsageExample("var view = new HighchartLineBasicView();\nview.Dock = \"Fill\";\nparent.Add(view);", "<highchart-line-basic-view Dock=\"Fill\" />")]
[Label("Диаграмма Highcharts")]
[Description("Диаграмма по модели Highcharts (отрисовка ScottPlot)")]
[Icon("insert_chart")]
public abstract class HighchartView : ControlView
{
    [Label("Тип диаграммы")]
    [InputSelect("HighchartLineBasic,HighchartColumnBasic,HighchartPieDonut,HighchartBarBasic,HighchartAreaspline")]
    public virtual string ChartType2
    {
        get => Get<string>(nameof(ChartType2));
        set => Set(nameof(ChartType2), value);
    }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Модель данных")]
    public virtual Highchart Model { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Analytics Selection")]
    public HighchartAnalyticsSelection AnalyticsSelection { get => Get<HighchartAnalyticsSelection>(nameof(AnalyticsSelection)); set => Set(nameof(AnalyticsSelection), value); }


    [OnChange(nameof(AnalyticsSelection))]
    public virtual void OnAnaliticsSectionChanged()
    {

        if (AnalyticsSelection is null)
            return;

        var hasCriteria = AnalyticsSelection.HasResourceAndIndicators
            || AnalyticsSelection.HasDatasetFilter && AnalyticsSelection.BusinessResourceId is > 0;
        if (!hasCriteria)
            return;

        try
        {
            ApplyBusinessData(AnalyticsSelection);
        }
        catch (Exception ex)
        {
            this.Error($"Не удалось загрузить BusinessData: {ex.Message}");
        }
    }

    [OnChange(nameof(ChartType2))]
    public virtual void OnChartType2Changed()
    {
        EnsureModel();
        RefreshVisualization();
    }

    public HighchartView()
    {
        AnalyticsSelection = new();
        MinWidth = 400;
        MinHeight = 300;
        Width = MinWidth;
        Height = MinHeight;
        ChartType2 = this.GetTypeName();
        ViewDesigner = new HighchartViewDesignerService(this);
        EnsureModel();

    }

    /// <summary>Загружает показатели из BusinessData и обновляет модель диаграммы.</summary>
    public void ApplyBusinessData(HighchartAnalyticsSelection selection = null)
    {
        HighchartViewDataServiceRegistry.Get(this).ApplyFromAnalytics(this, selection);
    }

    /// <summary>
    /// Заполняет <see cref="Model"/> рядами из <see cref="BusinessData.IndValue"/>
    /// (тип диаграммы определяет биндер: линейный, столбчатый, круговой, scatter).
    /// </summary>
    public virtual void ApplyBusinessData(List<BusinessData> data)
    {
        if (data is null || data.Count == 0)
            return;

        var chartData = HighchartBusinessDataMapper.FromRows(data);
        if (chartData.Series.Count == 0)
            return;

        EnsureModel();
        var chartType = string.IsNullOrWhiteSpace(ChartType2) ? Model.GetType().Name : ChartType2;
        HighchartChartDataBinderResolver.Resolve(chartType).Apply(Model, chartData);

        Changed = true;
        RefreshVisualization();
    }

    /// <summary>Перерисовывает WinForms-контрол диаграммы после изменения <see cref="Model"/>.</summary>
    public void RefreshVisualization()
    {
        //if (Model is null || control is not HighchartControl chartControl)
        //    return;

        //RunOnUiThread(() =>
        //{
        //    if (Model is null || control is not HighchartControl hc)
        //        return;
        //    hc.SetOptions(Model);
        //});
    }
    public void EnsureModel()
    {
        //if (string.IsNullOrWhiteSpace(ChartType2))
        //    return;
        //if (Model is not null && string.Equals(Model.GetType().Name, ChartType2, StringComparison.Ordinal))
        //    return;
        //try
        //{
        //    Model = HighchartTypeRegistry.GetDefaultModel(ChartType2);
        //    Changed = true;
        //}
        //catch
        //{
        //    /* неверный тип — оставляем предыдущую модель */
        //}
    }

    public void SetModel(Highchart model)
    {
        if (model is null)
            return;
        Model = model;
        ChartType2 = model.GetType().Name;
        Changed = true;
        RefreshVisualization();
    }


}

/// <summary>Универсальное представление диаграммы с переключаемым <see cref="ChartType2"/>.</summary>
[Label("Диаграмма Highcharts (универсальная)")]
public class HighchartGenericView : HighchartView
{
}
