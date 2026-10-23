using System.Linq;

using WinFormsApp.AppViewModules.HighchartsModule;
using WinFormsApp.AppViewModules.HighchartsModule.Services;
using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices;

/// <summary>Мастер подключения BusinessData к диаграмме Highcharts.</summary>
[Label("Данные Highcharts")]
public class HighchartViewDesignerService : DesignerBase, IControlService<HighchartView>
{
    private readonly IHighchartViewDataService _viewData;

    public HighchartViewDesignerService()
    {
        _viewData = new HighchartViewDataService();
    }

    public HighchartViewDesignerService(HighchartView view)
        : this(view, new HighchartViewDataService())
    {
        SetControl(view);
    }

    public HighchartViewDesignerService(HighchartView view, IHighchartViewDataService viewData)
        : base(view)
    {
        _viewData = viewData ?? HighchartViewDataServiceRegistry.Get(view);
        SetControl(view);
    }

    public HighchartView Control { get; set; }

    public void SetControl(HighchartView item) => Control = item;

    protected override void Activating(ViewNode viewNode)
    {
        var viewBase = Control;
        if (viewBase is null)
            return;

        rollback.ToList().ForEach(viewBase.RunOnUiThread);
        rollback.Clear();

        var ctxMenu = new ContextView()
        {
            Label = "Данные BusinessAnalytics",
            OnClick = (_, _) => ShowDataWizard(),
        };
        viewBase.ContextMenu.Items.Add(ctxMenu);
        rollback.Add(() => viewBase.ContextMenu.Items.Remove(ctxMenu));
    }

    [Label("Мастер настройки")]
    public void ShowDataWizard()
    {
        HighchartView chartView = this.Control;
        var masterView = new AppViewBase();
        var selection = chartView.AnalyticsSelection ?? new HighchartAnalyticsSelection();

        masterView.LayoutFill(
            masterView.CreateEntityListEditor<BusinessDataset, DeliveryDbContext>(
                "Набор данных",
                dataset =>
                {
                    selection.BusinessDatasetId = dataset.Id;
                    masterView.LayoutFill(
                        masterView.CreateEntityListEditor<BusinessGranularities, DeliveryDbContext>(
                            "Периодичность",
                            granularity =>
                            {
                                selection.GranularityId = granularity.Id;
                                masterView.LayoutFill(
                                    masterView.CreateEntityListEditor<BusinessResource, DeliveryDbContext>(
                                        "Ресурс (объект мониторинга)",
                                        resource =>
                                        {
                                            selection.BusinessResourceId = resource.Id;
                                            masterView.LayoutFill(
                                                masterView.CreateEntityCheckList<BusinessIndicator, DeliveryDbContext>(
                                                    "Показатели (значения из BusinessData)",
                                                    indicators =>
                                                    {
                                                        
                                                        try
                                                        {
                                                            selection.BusinessIndicatorIds = indicators
                                                                .Select(i => i.Id)
                                                                .ToList();

                                                            var bas = new BusinessAnaliticsService();
                                                            var resource = bas.GetResourceById((int)selection.BusinessResourceId);
                                                            var dates = bas.GetBeginDates((int)selection.BusinessResourceId, (int)selection.GranularityId, selection.BusinessIndicatorIds);


                                                            if(dates.Count()==0)
                                                            {
                                                                masterView.ConfirmDialog("Внимание", "Нет данных по заданным условиям");
                                                            }
                                                            else
                                                            {
                                                                masterView.LayoutFill(masterView.CreateSelectList("Отчётные периоды",
                                                                dates.Select(dateTime => dateTime.ToString()).ToList(),
                                                                selectedDates => {

                                                                    var bas = new BusinessAnaliticsService();
                                                                    var indicators = selection.BusinessIndicatorIds.Select(i => bas.GetIndicatorById(i)).ToList();
                                                                    var resource = bas.GetResourceById((int)selection.BusinessResourceId);
                                                                    var data = bas.GetBusinessData(resource, indicators);
                                                                    var dataset = data.Where(data => selectedDates.Contains(data.BeginDate.ToString())).ToList();
                                                                    chartView.ApplyBusinessData(dataset);

                                                                    masterView.ToControl().GetAppForm()?.Close();
                                                        
                                                            }));
                                                            }                                                                                                                                                                             
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            ex.Message.WriteRedLine();
                                                        }                                                        
                                                    }));
                                        }));
                            }));
                }));

        masterView.ShowDialog();
    }
}
