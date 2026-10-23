using Microsoft.EntityFrameworkCore;
using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices;

[Label("Мастер диаграмм")]
public class ChartViewDesignerService : DesignerBase, IControlService<ChartSeriesViewBase>
{

    public ChartSeriesViewBase Control { get; set; }


    public ChartViewDesignerService()
    {
    }

    public ChartViewDesignerService(ChartSeriesViewBase view)
    {
        SetControl(view);
    }


    public void SetControl(ChartSeriesViewBase item)
    {
        Control = item;
        item.AddPropertyChangeListener(nameof(item.DesignMode), evt =>
        {
            var state = (bool)evt.After;
            if (state)
                Activating(item);
            else
                Deactivate(item);
        });
        if (item.DesignMode)
            Activating(item);
    }

    protected override void Activating(ViewNode view)
    {
        var viewBase = (ChartView)view;
        rollback.ToList().ForEach(viewBase.RunOnUiThread);
        rollback.Clear();
        var ctxMenu = new ContextView() 
        {
            Label = "Мастер диаграмм",
            OnClick = (_, _) =>
            {
                ShowChartMasterDialog(viewBase);
            }
        };
        viewBase.ContextMenu.Items.Add(ctxMenu);
        rollback.Add(() => viewBase.ContextMenu.Items.Remove(ctxMenu));
    }

    [Label("Мастер настройки диаграмм")]
    public void ShowChartMasterDialog(ChartView viewBase)
    {
        var masterView = new AppViewBase();
        masterView.LayoutFill(
            masterView.CreateEntityListEditor<BusinessResource, DeliveryDbContext>(
                "Источники данных", businessResource =>
                {
                    var checkedList =
                        masterView.CreateEntityCheckList<BusinessIndicator, DeliveryDbContext>(
                            "Показатели эффективности", indicators =>
                            {
                                var bas = new BusinessAnaliticsService();
                                var data = bas.GetBusinessData(businessResource, indicators);
                                Dictionary<string, List<float>> series = new();
                                var indicatorsIds = data.Select(d => d.BusinessIndicatorID).Distinct();
                                foreach (var id in indicatorsIds)
                                {
                                    var line = data.Where(d => d.BusinessIndicatorID == id).OrderBy(d => d.BeginDate);
                                    series[bas.GetIndicatorById(id).Name] = line.Select(d => (float)d.IndValue).ToList();
                                }
                                viewBase.SetSeries(series);
                                masterView.ToControl().GetAppForm().Close();
                            });
                    masterView.LayoutFill(checkedList);
                }));
        masterView.ShowDialog();
    }
}
