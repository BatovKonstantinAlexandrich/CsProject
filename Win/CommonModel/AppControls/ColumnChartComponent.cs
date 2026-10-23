using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.DesignerServices;

using WinFormsDesignerApp;

namespace WinFormsApp.AppComponents.AppControls;

public class ColumnChartComponent : ChartDiagramComponentBase<ColumnChartView, ColumnChartControl>
{
    public ColumnChartComponent() : this(new(), new()) { }

    public ColumnChartComponent(ColumnChartView model, ColumnChartControl view) : base(model, view)
    {
    }

    public override List<DesignerBase> GetViewServices()
    {
        var list = base.GetViewServices();
        list.Add(new ChartViewDesignerService(GetViewModel()));
        return list;
    }


    protected override Control CreateDiagram(ColumnChartView view, ComponentsFactory factory)
        => factory.CreateColumnChart(view);
}
