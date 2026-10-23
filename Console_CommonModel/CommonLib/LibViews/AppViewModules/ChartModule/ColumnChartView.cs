
[ViewUsageExample("var view = new ColumnChartView { Text = \"Столбчатая диаграмма\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<column-chart-view Text=\"Столбчатая диаграмма\" Dock=\"Top\" />")]
[Label("Столбчатая диаграмма")]
[Description("Вертикальные столбцы по рядам данных")]
[Icon("bar_chart")]
public class ColumnChartView : AppViewBase
{
    [Label("Заголовок")]
    [InputOrder(1)]
    public string Title { get => Get<string>(nameof(Title)); set => Set<string>(nameof(Title), value); }

    [InputOrder(4)]
    [Label("Ряды")]
    [InputStructureCollection(type: "NamedSeriesArray")]
    public ChangingList<NamedSeriesArray> Series { get; set; } = new();
   
    public ColumnChartView()
    {
        ViewDesigner = CreateChartViewDesignerService(this);
    }

    private DesignerBase CreateChartViewDesignerService(ColumnChartView columnChartView)
    {
        throw new NotImplementedException();
    }
}
