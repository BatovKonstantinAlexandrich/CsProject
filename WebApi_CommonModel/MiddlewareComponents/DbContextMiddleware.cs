using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;

namespace WebApi_CommonModel.MiddlewareComponents
{
    public class DbContextMiddleware<TDbContextBase>(IServiceProvider provider)
        where TDbContextBase: DbContextBase
    {
        public void Use(WebApplication app)
        {
            app.MapGet($"/api/{typeof(TDbContextBase).GetTypeName()}/", () => {
                using(var scope = provider.CreateScope())
                {
                    using (var db = scope.ServiceProvider.Get<TDbContextBase>())
                    {
                        var list = db.GetEntitiesTypes().Select(t => t.GetLabel());
                        //TODO: вывод списка для выбора
                    }
                }
            });
        }

      
    }
}
