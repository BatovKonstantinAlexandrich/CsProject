[ViewUsageExample("var view = new LineChartView { Text = \"Линейная диаграмма\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<line-chart-view Text=\"Линейная диаграмма\" Dock=\"Top\" />")]
[Label("Линейная диаграмма")]
[Description("График с линиями по рядам данных")]
[Icon("show_chart")]
public class LineChartView : AppViewBase
{
    [Label("Заголовок")]
    [InputOrder(1)]
    public string Title { get => Get<string>(nameof(Title)); set => Set<string>(nameof(Title), value); }


    public ChangingList<string> XLabels { get; set; } = new();


    [InputOrder(4)]
    [Label("Ряды")]
    [InputStructureCollection(type: "NamedSeriesArray")]
    public ChangingList<NamedSeriesArray> Series { get; set; } = new();    
    
    public LineChartView()
    {
        //ViewDesigner = new ChartViewDesignerService(this);
        this.AddPropertyChangeListener(nameof(BackgroundColor), evt => {
            this.Info(evt.After);
        });
    }
}
