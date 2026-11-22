using Blazor_CommonModel.App;
using Blazor_CommonModel.Services.DataModel;

using Blazor_RoyaltyModel.Components;

using Blazor_SiteModel.Application.Authorization;
using Blazor_SiteModel.Application.Royalty;
using Blazor_SiteModel.Services;

using Console_CommonModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_RoyaltyData.RoyaltyData;
using Console_RoyaltyData.RoyaltyServices;

using Microsoft.EntityFrameworkCore;

namespace Blazor_SiteModel;

public class Program
{
    public static void Main(string[] args)
    {

        ServiceFactory.Get().AddTypesAsync(typeof(Program).Assembly, true);
        ServiceFactory.Get().AddTypesAsync(typeof(MudBlazor.Colors).Assembly, true);  

        AppProviderService.GetInstance().AddServiceImplementation<IClientAPI, BlazorClientAPI>();
        AppProviderService.GetInstance().AddServiceImplementation<IComponentsFactory<IControlBase>, WebComponentsFactory>();

        try
        {
            CommonProgram.Run<Program>(new string[] { "--no-force" });
          
            var unit = new AppUnit();
            unit.DoTest(false, false).PrintDocument();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.AddKeycloak();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        builder.Services.AddDbContext<RoyaltyDbContext>(options => {
            RoyaltyDbContext.OnConfiguring<RoyaltyDbContext>(options);            
        });
        //builder.Services.AddTransient<ICatalogService, CatalogService>();
        builder.Services.AddTransient<ICatalogService, CatalogService>();
        builder.Services.AddTransient<IStationService, StationService>();
        builder.Services.AddTransient<IPromotionService, PromotionService>();
        builder.Services.AddTransient<IContentService, ContentService>();
        builder.Services.AddTransient<IMenuService, MenuService>();
        builder.Services.AddTransient<IAppointmentService, AppointmentService>();
        builder.Services.AddTransient<IScheduleService, ScheduleService>();
        builder.Services.AddScoped<CityState>();
        builder.Services.AddScoped<UserSession>();
        builder.Services.AddScoped<ApplicationAuthorizationService>();
        builder.Services.AddScoped<RoyaltyFullAccessService>();
        builder.Services.AddScoped<RoyaltyWorkspaceService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.MapKeycloakEndpoints();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
