using Console_CommonModel;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

using Microsoft.Extensions.Logging;

using System.Threading.RateLimiting;

using WebApi_CommonModel.BackgroundWorkers;
using WebApi_CommonModel.BackgroungHosting;
using WebApi_CommonModel.MiddlewareComponents;


namespace WebApi_CommonModel.WebApiModule
{

    [Label("Программа запуска приложения WebApi")]
    public class WebApiProgram:  IBackgroundServiceContext
    {        
        private ILogger<WebApiProgram> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<WebApiProgram>();

        [HelpMessage("Отключается когда не нужен повторный рестарт")]
        private bool active { get; set; } = true;

        private CancellationTokenSource _cts;
        private string[] started;

        private BackgroundServiceContext backgroundContext;
        private MyMvcApplicationRouter applicationRouter = new();
        private Predicate<HttpContext> filters = p => true;

        public WebApiProgram(string[] started)
        {
            this.started = started;
        }

        [Label("Остановить")]
        public void StopProcess( )
        {
            if(_cts is not null)
            {
                _cts.Cancel();
            }
        }

        [Label("Запустить")]
        public void StartProcess()
        {          
            while (active)
            {
                try
                {
                    logger.LogInformation("StartProcess()");
                    this._cts = new();
                    var builder = WebApplication.CreateBuilder(started);

                    ConfigureServices(builder.Services, builder.Configuration);
                    builder.Services.AddSingleton<IpFilterMiddleware>();

                    var app = builder.Build();
                    app.Use((http, next) => {
                        logger.LogInformation(http.Connection.RemoteIpAddress.ToString());
                        return next.Invoke(http);
                    });
                    app.UseMiddleware<IpFilterMiddleware>();

                    

                    //app.UseMiddleware<IpFilterMiddleware>();
                    //app.Use((HttpContext http, RequestDelegate next) =>
                    //{
                    //    if (!http.Response.HasStarted)
                    //    {
                    //        var odbc = new OdbcMiddleware();
                    //        return odbc.OnHttpRequest(http);
                    //    }
                    //    return Task.CompletedTask;
                    //});
                    app.Use(async (http, next) => {
                        if (!http.Response.HasStarted)
                        {

                            await applicationRouter.InvokeAsync(http, next);
                        }
                    }); 
                    app.UseRouting();
                    app.MapControllerRoute(
                      name: "default",
                      pattern: "{controller=Mvc}/{action=Index}/{id?}");
                    //app.MapHub<ActionsHub>($"/hubs/{nameof(ActionsHub)}");                    
                    app.RunAsync(_cts.Token).Wait();
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            }            
        }

        [NotInput]
        private void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            //this.backgroundContext = BackgroungHostingStartup.ConfigureServices(services);
            services.AddSignalR();
            services.AddControllersWithViews();
        }



        [NotInput]
        public static void Main(string[] args)
        {    
            CommonProgram.Run<WebApiProgram>(args);                         

            //var program = new ActionsHubProgram();
            //program.StartProcess(args);

            var program = new WebApiProgram(args);

            Task.Run(() => {

                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), typeof(WebApiProgram)+".json");
                 
                string[] pargs = (System.IO.File.Exists(path))?
                    (System.IO.File.ReadAllLines(path)) : new string[0];
                if(pargs.Length >0)
                {
                    
                }
                program.RunConsoleControl<WebApiProgram>(ref pargs);
                
            });
            program.StartProcess();                                    
        }

 

        public void TestAdd()
        {            
            this.AddBackgroundServiceTypes<BackgroundService<Worker1>>();
        }

        public void TestRemove()
        {
            this.RemoveBackgroundServiceTypes<BackgroundService<Worker1>>();
        }

        public void AddBackgroundServiceTypes<TBackgroundServiceType>() where TBackgroundServiceType : IWorkerService
        {
            lock(backgroundContext)
            {
                if (!backgroundContext.BackgroundServiceTypes.Contains(typeof(TBackgroundServiceType)))
                {
                    ((IBackgroundServiceContext)backgroundContext).AddBackgroundServiceTypes<TBackgroundServiceType>();
                }
            }
        }

        public void RemoveBackgroundServiceTypes<TBackgroundServiceType>() where TBackgroundServiceType : IWorkerService
        {
            lock (backgroundContext)
            {
                if (backgroundContext.BackgroundServiceTypes.Contains(typeof(TBackgroundServiceType)))
                {
                    ((IBackgroundServiceContext)backgroundContext).RemoveBackgroundServiceTypes<TBackgroundServiceType>();
                }
            }            
        }
    }


}
