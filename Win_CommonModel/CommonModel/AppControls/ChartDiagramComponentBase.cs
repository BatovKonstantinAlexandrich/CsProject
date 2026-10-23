using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.Others;

namespace WinFormsApp.AppComponents.AppControls;

public abstract class ChartDiagramComponentBase<TView, TControl> : ComponentBase<TView, TControl>
    where TView : LineChartView
    where TControl : ChartDiagramControlBase
{
    protected ChartDiagramComponentBase(TView viewModel, TControl control) : base(viewModel, control)
    {
    }

    protected abstract Control CreateDiagram(TView view, IComponentsFactory<IControlBase> factory);

    /// <summary>Дополнительные свойства представления, при изменении которых пересоздаётся диаграмма.</summary>
    protected virtual IEnumerable<string> GetAdditionalRefreshPropertyNames()
        => Array.Empty<string>();

    public override void Bind()
    {
        var ctrl = GetControl();
        var view = GetViewModel();

        var factory = AppProviderService.GetInstance().Get<IComponentsFactory<IControlBase>>();

        var refreshing = false;

        void Refresh()
        {
            if (refreshing)
                return;
            refreshing = true;
            try
            {
                var diagram = CreateDiagram(view, factory);
                if (diagram is null)
                    return;
                diagram.Width = view.Width;
                diagram.Height = view.Height;
                ctrl.SetDiagram(diagram);
            }
            finally
            {
                refreshing = false;
            }
        }

        view.Series.OnChanged += (_, _) => Refresh();

        view.AddPropertyChangeListener(nameof(view.Title), _ => Refresh());
        //view.AddPropertyChangeListener(nameof(view.Xaxis), _ => Refresh());
        //view.AddPropertyChangeListener(nameof(view.Yaxis), _ => Refresh());

        foreach (var propertyName in GetAdditionalRefreshPropertyNames())
            view.AddPropertyChangeListener(propertyName, _ => Refresh());

        view.AddPropertyChangeListener(nameof(view.Width), _ =>
        {
            if (ctrl.diagram is not null && ctrl.diagram.Width != view.Width)
                ctrl.diagram.Width = view.Width;
        });
        view.AddPropertyChangeListener(nameof(view.Height), _ =>
        {
            if (ctrl.diagram is not null && ctrl.diagram.Height != view.Height)
                ctrl.diagram.Height = view.Height;
        });

        ctrl.Resize += (_, _) =>
        {
            // До первого layout WinForms может отдавать 0×0 — не затираем размеры модели и диаграммы.
            if (ctrl.Width <= 0 || ctrl.Height <= 0)
                return;

            if (ctrl.diagram is not null)
            {
                if (ctrl.diagram.Width != ctrl.Width)
                    ctrl.diagram.Width = ctrl.Width;
                if (ctrl.diagram.Height != ctrl.Height)
                    ctrl.diagram.Height = ctrl.Height;
            }

            if (view.Width != ctrl.Width)
                view.Width = ctrl.Width;
            if (view.Height != ctrl.Height)
                view.Height = ctrl.Height;
        };

        view.AddPropertyChangeListener(nameof(AppViewBase.DesignMode), _ =>
            ctrl.ApplyDiagramDesignInteractivity());

        ctrl.Load += (_, _) => ctrl.ApplyDiagramDesignInteractivity();

        Refresh();
    }
}
