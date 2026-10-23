using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Microsoft.EntityFrameworkCore;





public class AppDbContextView<TDeliveryDbContext>: AppViewRoot
    where TDeliveryDbContext: DeliveryDbContext
{
        
    public AppDbContextView()
    {                
    }

    private TDeliveryDbContext GetDbContext()
        => this.GetService<TDeliveryDbContext>();

    public void BindBusinessFunction<TBusinessResource, TEntity>(AppViewBase view)
        where TEntity: BaseEntity
    {
        using(var db = GetDbContext())
        {
            var businessFunctioins = db.GetBusinessFunctionsForEntity<TEntity>();


            var userContext = db.GetSigninUser().Verify();
            foreach (var entity in userContext.BusinessResources.ToList())
            {
            }


            db.GetBusinessDialogs("Holder").First().Value(data => { });
            db.InitServices();

            var dview = new AppViewBase();
            var resources = DeliveryDbContext.UserOperators.Keys.ToList();
            var list = view.CreateSelectList("Ресурсы", resources, selected =>
            {
                if (DeliveryDbContext.UserOperators.ContainsKey(selected))
                {
                    var operations = db.GetBusinessDialogs(selected);
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