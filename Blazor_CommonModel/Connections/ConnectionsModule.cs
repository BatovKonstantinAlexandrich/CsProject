using Blazor_CommonModel.Application;

using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor_CommonModel.Connections
{
    public class ConnectionsModule: ConnectionTracker, IHostingStartup
    {
        public ConnectionsModule(AuthorizationOptions options, ApplicationEventsService applicationEventsService) : base(options, applicationEventsService)
        {
        }

        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(OnConfigureServices);
        }

        public void OnConfigureServices(IServiceCollection services)
        {
            services.AddScoped<CircuitHandler>(sp => new TrackingCircuitHandler(sp.Get<IHttpContextAccessor>(), sp) { _tracker = this });
            services.AddSingleton<ConnectionTracker>();
            services.AddHttpContextAccessor();

        }
    }
}
