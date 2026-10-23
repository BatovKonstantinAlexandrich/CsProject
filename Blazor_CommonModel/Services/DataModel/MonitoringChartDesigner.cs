
namespace Blazor_CommonModel.Services.DataModel
{

    [Label("Разработка диаграмм")]
    [Description("Сервис предоставляет интерфейс дял настройки представления в режиме разработки." +
        "Должна быть предоставлена возможность выбора ресурса, периодичности, даты отчёта.")]
    public class MonitoringChartDesigner: DesignerBase
    {

        [Label("Подключает инструменты для проектирования")]
        protected override void Activating(ViewNode view)
        {
            var monitoringView = view.Cast<MonitoringView>();
            using (var db = AppProviderService.GetInstance().Get<MonitoringDbContext>())
            {
                var resources = db.BusinessResources.Select(r => r.Name).ToList();
                var menu = new MenuView().CreateContextView(
                    "Бизнес-ресурс",
                    resources.ToDictionary<string, Action>(resource => () =>
                    {
                        monitoringView.BusinessResourceId = db.BusinessResources.FirstOrDefault(r => r.Name == resource).Id;
                        monitoringView.Update();
                    })
                );
                monitoringView.ContextMenu.Items.Add(menu);
                rollback.Add(() => {

                    this.Info("rollback");
                    monitoringView.ContextMenu.Items.Remove(menu);
                });
            }
        }
    }
    
}
