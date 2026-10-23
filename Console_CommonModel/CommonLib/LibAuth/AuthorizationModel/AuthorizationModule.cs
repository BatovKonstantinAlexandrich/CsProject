using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationServices.AuthorizationApi;

using Microsoft.Extensions.DependencyInjection;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel
{
    public class AuthorizationModule
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            //services.AddTransient<APIAuthorization, AuthorizationService<DeliveryDbContext>>();
            services.AddTransient<IUserMessagesService, UserMessagesService<DeliveryDbContext>>();
        }
    }
}
