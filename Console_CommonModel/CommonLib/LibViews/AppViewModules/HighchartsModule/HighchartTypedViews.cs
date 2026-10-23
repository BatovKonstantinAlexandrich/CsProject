using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;

[Label("3D столбчатая (интерактивная)")]
public class Highchart3dColumnInteractiveView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public Highchart3dColumnInteractiveView()
    {
        ChartType2 = nameof(Highchart3dColumnInteractive);
        EnsureModel();
    }
}

[Label("3D столбчатая (null-значения)")]
public class Highchart3dColumnNullValuesView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public Highchart3dColumnNullValuesView()
    {
        ChartType2 = nameof(Highchart3dColumnNullValues);
        EnsureModel();
    }
}

[Label("3D столбчатая (стек и группы)")]
public class Highchart3dColumnStackingGroupingView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public Highchart3dColumnStackingGroupingView()
    {
        ChartType2 = nameof(Highchart3dColumnStackingGrouping);
        EnsureModel();
    }
}

[Label("3D рассеяние (перетаскивание)")]
public class Highchart3dScatterDraggableView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public Highchart3dScatterDraggableView()
    {
        ChartType2 = nameof(Highchart3dScatterDraggable);
        EnsureModel();
    }
}

[Label("Линейная (доступность)")]
public class HighchartAccessibleLineView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartAccessibleLineView()
    {
        ChartType2 = nameof(HighchartAccessibleLine);
        EnsureModel();
    }
}

[Label("Круговая (доступность)")]
public class HighchartAccessiblePieView : HighchartView
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Model")]
    public override Highchart Model { get; set; }
    public HighchartAccessiblePieView()
    {
        ChartType2 = nameof(HighchartAccessiblePie);
        EnsureModel();
    }
}

[Label("Диаграмма с аннотациями")]
public class HighchartAnnotationsView : HighchartView
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Model")]
    public override Highchart Model { get; set; }
    public HighchartAnnotationsView()
    {
        ChartType2 = nameof(HighchartAnnotations);
        EnsureModel();
    }
}

[Label("Областная (инвертированная)")]
public class HighchartAreaInvertedView : HighchartView
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Model")]
    public override Highchart Model { get; set; }
    public HighchartAreaInvertedView()
    {
        ChartType2 = nameof(HighchartAreaInverted);
        EnsureModel();
    }
}

[Label("Областная (отрицательные)")]
public class HighchartAreaNegativeView : HighchartView
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Model")]
    public override Highchart Model { get; set; }
    public HighchartAreaNegativeView()
    {
        ChartType2 = nameof(HighchartAreaNegative);
        EnsureModel();
    }
}

[Label("Областная (сглаженная)")]
public class HighchartAreasplineView : HighchartView
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Model")]
    public override Highchart Model { get; set; }
    public HighchartAreasplineView()
    {
        ChartType2 = nameof(HighchartAreaspline);
        EnsureModel();
    }
}

[Label("Областная (накопительная)")]
public class HighchartAreaStackedView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartAreaStackedView()
    {
        ChartType2 = nameof(HighchartAreaStacked);
        EnsureModel();
    }
}

[Label("Областная (100% накопительная)")]
public class HighchartAreaStackedPercentView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }

    public HighchartAreaStackedPercentView()
    {
        ChartType2 = nameof(HighchartAreaStackedPercent);
        EnsureModel();
    }
}

[Label("Линейчатая")]
public class HighchartBarBasicView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartBarBasicView()
    {
        ChartType2 = nameof(HighchartBarBasic);
        EnsureModel();
    }
}

[Label("Линейчатая (отрицательный стек)")]
public class HighchartBarNegativeStackView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartBarNegativeStackView()
    {
        ChartType2 = nameof(HighchartBarNegativeStack);
        EnsureModel();
    }
}

[Label("Линейчатая (накопительная)")]
public class HighchartBarStackedView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartBarStackedView()
    {
        ChartType2 = nameof(HighchartBarStacked);
        EnsureModel();
    }
}

[Label("Диаграмма с обновлением")]
public class HighchartChartUpdateView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartChartUpdateView()
    {
        ChartType2 = nameof(HighchartChartUpdate);
        EnsureModel();
    }
}

[Label("Столбчатая")]
public class HighchartColumnBasicView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartColumnBasicView()
    {
        ChartType2 = nameof(HighchartColumnBasic);
        EnsureModel();
    }
}

[Label("Столбчатая (отрицательные)")]
public class HighchartColumnNegativeView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartColumnNegativeView()
    {
        ChartType2 = nameof(HighchartColumnNegative);
        EnsureModel();
    }
}

[Label("Столбчатая (накопительная и группировка)")]
public class HighchartColumnStackedAndGroupedView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartColumnStackedAndGroupedView()
    {
        ChartType2 = nameof(HighchartColumnStackedAndGrouped);
        EnsureModel();
    }
}

[Label("Столбчатая (100% накопительная)")]
public class HighchartColumnStackedPercentView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartColumnStackedPercentView()
    {
        ChartType2 = nameof(HighchartColumnStackedPercent);
        EnsureModel();
    }
}

[Label("Комбинированная")]
public class HighchartComboView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartComboView()
    {
        ChartType2 = nameof(HighchartCombo);
        EnsureModel();
    }
}

[Label("Комбинированная (две оси)")]
public class HighchartComboDualAxesView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartComboDualAxesView()
    {
        ChartType2 = nameof(HighchartComboDualAxes);
        EnsureModel();
    }
}

[Label("Комбинированная (много осей)")]
public class HighchartComboMultiAxesView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartComboMultiAxesView()
    {
        ChartType2 = nameof(HighchartComboMultiAxes);
        EnsureModel();
    }
}

[Label("Динамическая (добавление кликом)")]
public class HighchartDynamicClickToAddView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartDynamicClickToAddView()
    {
        ChartType2 = nameof(HighchartDynamicClickToAdd);
        EnsureModel();
    }
}

[Label("Диаграмма погрешностей")]
public class HighchartErrorBarView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartErrorBarView()
    {
        ChartType2 = nameof(HighchartErrorBar);
        EnsureModel();
    }
}

[Label("Линейная")]
public class HighchartLineBasicView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartLineBasicView()
    {
        ChartType2 = nameof(HighchartLineBasic);
        EnsureModel();
    }
}

[Label("Линейная (подписи)")]
public class HighchartLineLabelsView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartLineLabelsView()
    {
        ChartType2 = nameof(HighchartLineLabels);
        EnsureModel();
    }

}

[Label("Линейная (логарифмическая ось)")]
public class HighchartLineLogAxisView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartLineLogAxisView()
    {
        ChartType2 = nameof(HighchartLineLogAxis);
        EnsureModel();
    }
}

[Label("Кольцевая")]
public class HighchartPieDonutView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartPieDonutView()
    {
        ChartType2 = nameof(HighchartPieDonut);
        EnsureModel();
    }
}

[Label("Круговая с детализацией")]
public class HighchartPieDrilldownView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartPieDrilldownView()
    {
        ChartType2 = nameof(HighchartPieDrilldown);
        EnsureModel();
    }
}

[Label("Полярная радиальная")]
public class HighchartPolarRadialBarView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartPolarRadialBarView()
    {
        ChartType2 = nameof(HighchartPolarRadialBar);
        EnsureModel();
    }
}

[Label("Полярная лепестковая")]
public class HighchartPolarSpiderView : HighchartView
{
    [Label("Модель данных")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Highchart Model { get; set; }
    public HighchartPolarSpiderView()
    {
        ChartType2 = nameof(HighchartPolarSpider);
        EnsureModel();
    }
}


