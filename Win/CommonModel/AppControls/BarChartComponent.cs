using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.DesignerServices;

using WinFormsDesignerApp;

namespace WinFormsApp.AppComponents.AppControls;

public class BarChartComponent : ChartDiagramComponentBase<BarChartView, BarChartControl>
{
    public BarChartComponent() : this(new(), new()) { }

    public BarChartComponent(BarChartView model, BarChartControl view) : base(model, view)
    {
    }

    public override List<DesignerBase> GetViewServices()
    {
        var list = base.GetViewServices();
        list.Add(new ChartViewDesignerService(GetViewModel()));
        return list;
    }


    protected override Control CreateDiagram(BarChartView view, ComponentsFactory factory)
        => factory.CreateBarChart(view);
}
