
using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.HighchartsModule;
using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.DesignerServices;

namespace WinFormsApp.AppComponents.AppControls.Highcharts;

public class HighchartComponent : ComponentBase<HighchartView, HighchartControl>
{
    public HighchartComponent(HighchartView viewModel, HighchartControl control) : base(viewModel, control)
    {
    }

    public override List<DesignerBase> GetViewServices()
    {
        var results = base.GetViewServices();
        results.Add(new HighchartViewDesignerService(GetViewModel()));
        return results;
    }

    public override void Bind()
    {
        var view = GetViewModel();
        var ctrl = GetControl();

        void Refresh()
        {
            if (view.Model is not null)
                ctrl.SetOptions(view.Model);
        }

        view.AddPropertyChangeListener(nameof(HighchartView.ChartType2), _ =>
        {
            view.EnsureModel();
            Refresh();
        });

        Refresh();
    }
}
