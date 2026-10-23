using Blazor_CommonModel.Components;

using Blazored.Modal;

using System.Configuration;

namespace Blazor_CommonModel
{
    public class ProgramStartup
    {
        private IConfiguration Configuration { get; }

        public ProgramStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            // blazor deps
            services.AddBlazorContextMenu();
            services.AddBlazoredModal();
            services.AddInputForm();
            services.AddBusinessPages();

            services.AddInputModal();

            //// fasade
            //services.AddScoped(typeof(IEntityRepository<BusinessPageModel>), (provider) => {
            //    return new EntityRepository<BusinessPageModel>(provider.GetService<InputFormDbContext>());
            //});

           

            // ui deps
            //services.AddRazorPages();
            //services.AddServerSideBlazor();
            //services.AddSingleton<WeatherForecastService>();

            //// auto registration
            //services.AddDescriptors(Configuration);
            IEnumerable<ServiceDescriptor> descriptors =
            StartupApplication.ApplyConfigurationServices(services, Configuration);
            services.AddScoped(typeof(IEnumerable<ServiceDescriptor>), (provide) => {
                return descriptors;
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");

            });
        }
    }


}
