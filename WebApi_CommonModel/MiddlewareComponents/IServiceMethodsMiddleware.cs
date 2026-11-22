using Console_CommonModel.CommonLib.LibData.Console_DataModule;

namespace WebApi_CommonModel.MiddlewareComponents
{
    public interface IServiceMethodsMiddleware
    {
        public Action AddDbContext<TService>() where TService : DbContextBase;
        public Action RemoveDbContext<TService>() where TService : DbContextBase;
        public ActionsModel AddServiceMethod<TService>(string methodName) where TService : class;
        public ActionsModel RemoveServiceMethod<TService>(string methodName) where TService : class;
    }
}
