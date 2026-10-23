


[ViewUsageExample("var view = new CoreChartView { Text = \"Круговая диаграмма\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<core-chart-view Text=\"Круговая диаграмма\" Dock=\"Top\" />")]
[Label("Круговая диаграмма")]
[Description("Круговая диаграмма (Core) по сумме значений рядов")]
[Icon("pie_chart")]
public class CoreChartView : AppViewBase
{
    [Label("Заголовок")]
    [InputOrder(1)]
    public string Title { get => Get<string>(nameof(Title)); set => Set<string>(nameof(Title), value); }

    [InputOrder(4)]
    [Label("Ряды")]
    [InputStructureCollection(type: "NamedSeriesArray")]
    public ChangingList<NamedSeriesArray> Series { get; set; } = new();
    
    public CoreChartView()
    {
        ViewDesigner = CreateChartViewDesignerService(this);
    }

    private DesignerBase CreateChartViewDesignerService(CoreChartView coreChartView)
    {
        throw new NotImplementedException();
    }
}
