using WinFormsDesignerApp;

namespace WinFormsApp.AppComponents.AppControls;

public class CoreChartComponent : ChartDiagramComponentBase<CoreChartView, CoreChartControl>
{
    public CoreChartComponent() : this(new(), new()) { }

    public CoreChartComponent(CoreChartView model, CoreChartControl view) : base(model, view)
    {
    }

    protected override Control CreateDiagram(CoreChartView view, ComponentsFactory factory)
        => factory.CreateCoreChart(view);
}
