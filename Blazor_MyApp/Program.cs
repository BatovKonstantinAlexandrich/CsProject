using Blazor_CommonModel;
using Blazor_CommonModel.Application;

using Blazor_MyApp.Components;
using Blazor_MyApp.Services;
using Blazor_MyApp.Services.ContextServices;

using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.JSInterop;

namespace Blazor_MyApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHostedService<GrpcBackgroundMessaging>();
            builder.Services.AddHostedService<DoCheckBackground>();
            builder.Services.AddSingleton<AuthorizationOptions>();
            builder.Services.AddSingleton<ConnectionsCollection>();
            builder.Services.AddSingleton<ApplicationEventsService>();
            
            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddScoped<CookieManager>();
            builder.Services.AddTransient<CircuitHandler, ConnectionsHandler>();

            var app = builder.Build();
            app.UseExceptionHandler("/Error");
            app.UseHsts();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
