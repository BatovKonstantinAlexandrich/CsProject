using Blazor_CommonModel;
using Blazor_CommonModel.Application;

using Blazor_MyApp.Components;
using Blazor_MyApp.Services;
using Blazor_MyApp.Services.ContextServices;

using Console_CommonModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.JSInterop;


using pickpoint_delivery_service;

namespace Blazor_MyApp
{
    public class Program
    {
        public static void Main(string[] args) { 

            CommonProgram.Run<Program>(new string[] { "--no-force" });



            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddNavMenu();
            // Program.cs
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                // Если используется известный прокси, можно очистить KnownNetworks/Proxies для безопасности
            });


            DeliveryDbContext.ConfigureDeliveryServices(builder.Services, builder.Configuration);
            AppProviderService.GetInstance().Get<AuthorizationCollection<UserContext>>();
            ModuleUser.ConfigureServices(builder.Configuration, builder.Services);

            ModuleService.ConfigureServices<DeliveryDbContext>(builder.Configuration, builder.Services);
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
