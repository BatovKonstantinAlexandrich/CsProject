using Blazor_DesignModel.Components;

using Console_CommonModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using MudBlazor.Services;

namespace Blazor_DesignModel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceFactory.Get().AddTypesAsync(typeof(Program).Assembly, true);

            AppProviderService.GetInstance().AddServiceImplementation<IComponentsFactory<IControlBase>, WebComponentsFactory>();

            CommonProgram.Run<Program>(new string[] { "--interactive", "--no-force" });


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddMudServices();

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
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
