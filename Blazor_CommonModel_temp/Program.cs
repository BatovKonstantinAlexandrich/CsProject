using Blazor_CommonModel.Components;

using BlazorErp.CommonModel;

using Console_CommonModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Microsoft.AspNetCore.Hosting;

using MudBlazor.Services;

using System.ComponentModel;
using System.Text;

namespace Blazor_CommonModel
{
    public class Program: BackgroundService
    {
        public static void Main(string[] args)
        {
            AppProviderService.GetInstance().AddServiceImplementation<IClientAPI, BlazorClientAPI>();
            AppProviderService.GetInstance().AddServiceImplementation<IComponentsFactory<IControlBase>, WebComponentsFactory>();

            CommonProgram.Run<Program>(new string[] { /*"-i",*/ "--no-force" });

            StartupApplication.OnStart();


            //var builder = Host.CreateDefaultBuilder(args);
            //builder.ConfigureServices(ConfigureBackground);
            //builder.ConfigureServices(ctx => {
            //    //ctx.AddHostedService<>();
            //});
            //builder.ConfigureWebHostDefaults(webBuilder => {

            //    webBuilder.UseStartup<ProgramStartup>();
            //});
            //StartupApplication.ConfigurationBackgroundServices.ToList().ForEach(next => builder.ConfigureServices(next));

            //var host = builder.Build();
            //using (var scope = host.Services.CreateScope())
            //{
            //    TestContainer(scope);
            //};
            //host.Run();

            
            var builder = WebApplication.CreateBuilder(args);

            var startup = new ProgramStartup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            builder.Services.AddMudServices();
            builder.Services.AddMudBlazorDialog();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }

        private static void TestContainer(IServiceScope scope)
        {
        }

        private static void ConfigureBackground(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.AddHostedService<Program>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                this.Info(DateTime.Now);
                await Task.Delay(1000);
            }
        }
    }


}
