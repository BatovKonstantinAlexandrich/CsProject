using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;


namespace WinFormsApp
{

    [Label("Проектирование структуры данных")]
    public class DataModelView : AppViewBase
    {
        public DataModelView()
        {
            using (var dbContext = new DeliveryDbContext())
            {
                dbContext.GetBusinessDialogs("Holder").First().Value(data => { });
                dbContext.InitServices();
                var view = new AppViewBase();
                var resources = DeliveryDbContext.UserOperators.Keys.ToList();
                var list = view.CreateSelectList("Ресурсы", resources, selected =>
                {
                    if (DeliveryDbContext.UserOperators.ContainsKey(selected))
                    {
                        var operations = dbContext.GetBusinessDialogs(selected);
                        view.LayoutFill(view.CreateSelectList("Операции", operations.Keys.ToList(), selected2 =>
                        {
                            operations[selected2](datatable =>
                            {
                                view.LayoutFill(view.CreateDataTable(datatable));
                            });
                        }));
                    }
                });
                view.Left = list;
                this.LayoutFill(view);
            }
        }
    }
}