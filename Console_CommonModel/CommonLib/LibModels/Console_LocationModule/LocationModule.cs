using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{
    public class LocationModule
    {
        public static void AddLocationService(IServiceCollection services)
        {
            services.AddDbContext<LocationDbContext>(options =>
            options.UseSqlServer($@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={typeof(LocationDbContext).GetTypeName()};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));
            services.AddTransient<LocationMenuBuilder>();
            services.AddTransient<PageService>();
            services.AddTransient<RouteService>();
            services.AddTransient<LocationService>();

        }
    }
}
