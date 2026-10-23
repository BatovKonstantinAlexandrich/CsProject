using Blazored.Modal;
using RootForms.Data;

namespace RootForms
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            // blazor deps
            services.AddBlazorContextMenu();
            services.AddBlazoredModal();

            // fasade
            services.AddScoped(typeof(IEntityRepository<BusinessPageModel>),(provider)=> {                
                return new EntityRepository<BusinessPageModel>(provider.GetService<InputFormDbContext>());
            });

            // app ui
            services.AddTable();
            services.AddInputModal();
            services.AddBusinessPages();            
            services.AddInputForm();

            // ui deps
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            // auto registration
            services.AddDescriptors(Configuration);
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
