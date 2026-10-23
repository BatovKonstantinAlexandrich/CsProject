namespace WinFormsDesignerApp;

/// <summary>
/// Панель универсальной диаграммы с выбором типа.
/// </summary>
public partial class ChartControl : WinFormsApp.AppComponents.AppControls.ChartDiagramControlBase
{
    public ChartControl()
    {
        InitializeComponent();
        ContextMenuStrip = new();
    }
}
