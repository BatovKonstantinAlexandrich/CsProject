
using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.DesignerServices;

namespace WinFormsApp.AppComponents.AppControls;

/// <summary>
/// Связывает <see cref="LineChartView.Xaxis"/> и <see cref="LineChartView.Yaxis"/> с осями
/// <see cref="LiveCharts.WinForms.CartesianChart"/> через пересоздание диаграммы в <see cref="ChartDiagramComponentBase{TView, TControl}.Bind"/>.
/// </summary>
public class LineChartComponent : ChartDiagramComponentBase<LineChartView, LineChartControl>
{
    public LineChartComponent() : this(new(), new()) { }

    public LineChartComponent(LineChartView model, LineChartControl view) : base(model, view)
    {
    }

    public override List<DesignerBase> GetViewServices()
    {
        var list = base.GetViewServices();
        //list.Add(new ChartViewDesignerService(GetViewModel()));
        return list;
    }

    protected override Control CreateDiagram(LineChartView view, ComponentsFactory factory)
        => factory.CreateLineChart(view);
}
