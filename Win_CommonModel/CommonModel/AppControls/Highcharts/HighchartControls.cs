using WinFormsDesignerApp.ViewModels.HighCharts;

namespace WinFormsApp.AppComponents.AppControls.Highcharts;

/// <summary>Фабрика типизированных контролов Highcharts (по одному на каждую модель).</summary>
public static class HighchartControls
{
    public static HighchartControl Highchart3dColumnInteractive() => For<Highchart3dColumnInteractive>();
    public static HighchartControl Highchart3dColumnNullValues() => For<Highchart3dColumnNullValues>();
    public static HighchartControl Highchart3dColumnStackingGrouping() => For<Highchart3dColumnStackingGrouping>();
    public static HighchartControl Highchart3dScatterDraggable() => For<Highchart3dScatterDraggable>();
    public static HighchartControl HighchartAccessibleLine() => For<HighchartAccessibleLine>();
    public static HighchartControl HighchartAccessiblePie() => For<HighchartAccessiblePie>();
    public static HighchartControl HighchartAnnotations() => For<HighchartAnnotations>();
    public static HighchartControl HighchartAreaInverted() => For<HighchartAreaInverted>();
    public static HighchartControl HighchartAreaNegative() => For<HighchartAreaNegative>();
    public static HighchartControl HighchartAreaspline() => For<HighchartAreaspline>();
    public static HighchartControl HighchartAreaStacked() => For<HighchartAreaStacked>();
    public static HighchartControl HighchartAreaStackedPercent() => For<HighchartAreaStackedPercent>();
    public static HighchartControl HighchartBarBasic() => For<HighchartBarBasic>();
    public static HighchartControl HighchartBarNegativeStack() => For<HighchartBarNegativeStack>();
    public static HighchartControl HighchartBarStacked() => For<HighchartBarStacked>();
    public static HighchartControl HighchartChartUpdate() => For<HighchartChartUpdate>();
    public static HighchartControl HighchartColumnBasic() => For<HighchartColumnBasic>();
    public static HighchartControl HighchartColumnNegative() => For<HighchartColumnNegative>();
    public static HighchartControl HighchartColumnStackedAndGrouped() => For<HighchartColumnStackedAndGrouped>();
    public static HighchartControl HighchartColumnStackedPercent() => For<HighchartColumnStackedPercent>();
    public static HighchartControl HighchartCombo() => For<HighchartCombo>();
    public static HighchartControl HighchartComboDualAxes() => For<HighchartComboDualAxes>();
    public static HighchartControl HighchartComboMultiAxes() => For<HighchartComboMultiAxes>();
    public static HighchartControl HighchartDynamicClickToAdd() => For<HighchartDynamicClickToAdd>();
    public static HighchartControl HighchartErrorBar() => For<HighchartErrorBar>();
    public static HighchartControl HighchartLineBasic() => For<HighchartLineBasic>();
    public static HighchartControl HighchartLineLabels() => For<HighchartLineLabels>();
    public static HighchartControl HighchartLineLogAxis() => For<HighchartLineLogAxis>();
    public static HighchartControl HighchartPieDonut() => For<HighchartPieDonut>();
    public static HighchartControl HighchartPieDrilldown() => For<HighchartPieDrilldown>();
    public static HighchartControl HighchartPolarRadialBar() => For<HighchartPolarRadialBar>();
    public static HighchartControl HighchartPolarSpider() => For<HighchartPolarSpider>();

    public static HighchartControl For<T>() where T : Highchart, new()
    {
        var ctrl = new HighchartControl();
        ctrl.SetOptions(new T());
        return ctrl;
    }
}

