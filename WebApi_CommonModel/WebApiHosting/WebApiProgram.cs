using Console_CommonModel;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

using WebApi_CommonModel.BackgroundWorkers;
using WebApi_CommonModel.BackgroungHosting;


namespace WebApi_CommonModel.WebApiModule
{


    [Label("Программа запуска приложения WebApi")]
    public class WebApiProgram: IMiddleware, IBackgroundServiceContext
    {        
        private ILogger<WebApiProgram> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<WebApiProgram>();

        private bool active { get; set; } = true;
        private CancellationTokenSource _cts;
        private string[] started;
        private BackgroundServiceContext backgroundContext;

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

                    var app = builder.Build();
                    
                    app.Use((http, next) => {
                        logger.LogDebug("http => ");
                        return this.InvokeAsync(http, next);
                    });
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
            this.backgroundContext = BackgroungHostingStartup.ConfigureServices(services);
        }



        [NotInput]
        public static void Main(string[] args)
        {
            CommonProgram.Run<WebApiProgram>(args);
            //var program = new ActionsHubProgram();
            //program.StartProcess(args);
            
            var program = new WebApiProgram(args);

            Task.Run(() => {

                Thread.Sleep( 1);
                string[] iargs = new string[0];
                program.RunConsoleControl<WebApiProgram>(ref iargs);
                
            });


            program.StartProcess();                                    
        }


        [NotInput]
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
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
