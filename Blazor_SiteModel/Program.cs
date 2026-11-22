using Blazor_SiteModel.Components;
using Blazor_SiteModel.Services;

using pickpoint_delivery_service;

namespace Blazor_SiteModel;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        ModuleUser.ConfigureServices<DeliveryDbContext>(builder.Configuration, builder.Services);

        builder.Services.AddScoped<ILocalStorage, LocalStorage>(); 
        builder.Services.AddSingleton<ICatalogService, CatalogService>();
        builder.Services.AddSingleton<ICatalogService, CatalogService>();
        builder.Services.AddSingleton<IStationService, StationService>();
        builder.Services.AddSingleton<IPromotionService, PromotionService>();
        builder.Services.AddSingleton<IContentService, ContentService>();
        builder.Services.AddSingleton<IMenuService, MenuService>();
        builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
        builder.Services.AddSingleton<IScheduleService, ScheduleService>();
        builder.Services.AddScoped<CityState>();
        builder.Services.AddScoped<UserSession>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
