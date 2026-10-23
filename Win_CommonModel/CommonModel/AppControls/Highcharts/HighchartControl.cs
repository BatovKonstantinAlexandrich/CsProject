using ScottPlot.Drawing.Colormaps;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.ChartModule;

namespace WinFormsApp.AppComponents.AppControls.Highcharts;

/// <summary>
/// WinForms-контрол диаграммы: модели Highcharts отображаются через ScottPlot (без WebView2 и CDN).
/// </summary>
public class HighchartControl : ControlBase
{
    private readonly System.Windows.Forms.Control _plot;

    private Highchart _options;
    private Highchart _pendingOptions;

    public HighchartControl()
    {
        Dock = System.Windows.Forms.DockStyle.Fill;
        MinimumSize = new System.Drawing.Size(320, 240);
        _plot = ChartViewHelper.CreateChart();
        ChartViewHelper.SuppressScottPlotDefaultContextMenu(_plot);
        Controls.Add(_plot);
        HandleCreated += (_, _) => FlushPendingRender();
    }

    /// <summary>Привязывает к области графика только меню представления (после <see cref="ComponentsFactory.BindContextMenu"/>).</summary>
    public void ApplyViewContextMenu()
    {
        ChartViewHelper.SuppressScottPlotDefaultContextMenu(_plot);
        _plot.ContextMenuStrip = ContextMenuStrip;
    }

    public Highchart Options
    {
        get => _options;
        set => SetOptions(value);
    }

    public void SetOptions(Highchart options)
    {
        if (options is null)
            return;

        _options = options;
        QueueRender(options);
    }

    /// <summary>Перерисовывает диаграмму по текущей модели (после in-place обновления данных).</summary>
    public void RefreshOptions()
    {
        if (_options is not null)
            QueueRender(_options);
    }

    private void QueueRender(Highchart options)
    {
        if (options is null || _plot.IsDisposed)
            return;

        if (!IsHandleCreated)
        {
            _pendingOptions = options;
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(new Action(() =>
            {
                if (_options is not null && !_plot.IsDisposed)
                    ChartViewHelper.RenderHighchart(_plot, _options);
            }));
            return;
        }

        ChartViewHelper.RenderHighchart(_plot, options);
    }

    private void FlushPendingRender()
    {
        if (_pendingOptions is null)
            return;
        var pending = _pendingOptions;
        _pendingOptions = null;
        if (_options is null)
            _options = pending;
        QueueRender(_options);
    }
}
