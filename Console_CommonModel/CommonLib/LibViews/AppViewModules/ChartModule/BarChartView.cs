using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[Label("Горизонтальная диаграмма")]
public class BarChartView : AppViewBase
{
    [Label("Заголовок")]
    [InputOrder(1)]
    public  string Title { get => Get<string>(nameof(Title)); set => Set(nameof(Title), value); }

    [InputOrder(4)]
    [Label("Ряды")]
    [InputStructureCollection(type: "BarNamedSeriesArray")]
    public  ChangingList<NamedSeriesArray> Series { get; set; } = new();

    [InputOrder(2)]
    [Label("Ось Y")]
    public  string Yaxis { get => Get<string>(nameof(Yaxis)); set => Set(nameof(Yaxis), value); }


    [InputOrder(3)]
    [Label("Ось X")]
    public  string Xaxis { get => Get<string>(nameof(Xaxis)); set => Set(nameof(Xaxis), value); }

    public BarChartView()
    {
        ViewDesigner = CreateChartViewDesignerService(this);
    }

    private DesignerBase CreateChartViewDesignerService(BarChartView barChartView)
    {
        throw new NotImplementedException();
    }
}
