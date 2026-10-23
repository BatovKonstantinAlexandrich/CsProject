using WinFormsDesignerApp;

namespace WinFormsApp.AppComponents.AppControls;

public class ChartComponent : ChartDiagramComponentBase<ChartView, ChartControl>
{
    public ChartComponent() : this(new(), new()) { }

    public ChartComponent(ChartView model, ChartControl view) : base(model, view)
    {
    }

    protected override IEnumerable<string> GetAdditionalRefreshPropertyNames()
        => new[] { nameof(ChartView.Type) };

    protected override Control CreateDiagram(ChartView view, ComponentsFactory factory)
        => factory.CreateChart(view);
}
