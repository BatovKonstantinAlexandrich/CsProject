using ClassModelLib.ActionsClient;
using Console_CommonModel;
using Console_CommonModel.CommonLib.Others;

using pickpoint_delivery_service;

using WebApi_CommonModel.BackgroundWorkers;
using WebApi_CommonModel.BackgroungHosting;
using WebApi_CommonModel.MiddlewareComponents;

namespace WebApi_CommonModel.WebApiModule
{
 
    [Label("Программа запуска приложения WebApi")]
    public class Program:  IBackgroundServiceContext
    {        
        private ILogger<Program> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();

        [HelpMessage("Отключается когда не нужен повторный рестарт")]
        private bool active { get; set; } = true;

        private CancellationTokenSource _cts;
        private string[] started;

        private ServiceMethodsMiddleware<DeliveryDbContext> _serviceMethodsMiddleware = new();
        private BackgroundServiceContext backgroundContext;
        private IpFilterMiddleware ipFiltering = new();

        private ChangingDictionary<string, Func<ActionRequest, ActionResponse>> controllers { get; set; }
        public Dictionary<string, Action> mainMenu { get; private set; }

        public Program(ref string[] started)
        {
            this.started = started;            
        }

        [NotInput]
        public static void Main(string[] args)
        {
            if(System.IO.File.Exists("program.json"))
            {
                args = System.IO.File.ReadAllText("program.json").FromJson<List<string>>().ToArray();
            }
            //InputConsole.Info(args);
            //InputConsole.Interactive();
            //CommonProgram.Run<Program>(args=new string[0]);
            CommonProgram.Run<Program>(args);
            var middleware = new ServiceMethodsMiddleware<DeliveryDbContext>();
            var methodNames = middleware.GetGenericMethodNames
            (
                nameof(ServiceMethodsMiddleware<DeliveryDbContext>.LoadWithBuilder)
            );
            middleware.AddDbContext<DeliveryDbContext>();
            middleware.Info(middleware.GetRoutes());
            new object().Info(methodNames);

            //var middleware = new ServiceMethodsMiddleware(AppProviderService.GetInstance());
            //var result = middleware.IsRoute
            //(
            //    "http://sps.euroauto.ru/api/SigninUser/1/Signout",
            //    "http://sps.euroauto.ru/api/{{service}}/{{id}}/{{action}}"
            //);

            //Console.WriteLine(middleware.GetRoutes().ToJsonOnScreen());
            //Console.WriteLine(middleware.AddServiceMethods<SigninUser<DeliveryDbContext>>());
            //Console.WriteLine(middleware.GetRoutes().ToJsonOnScreen());

            //middleware.HandleHttpRequest(new() { 
            //    url = "/api/servises/SigninUser<DeliveryDbContext>/actions/IsSignin"
            //}).Wait();

            //Console.WriteLine(result);

            //var program = new ActionsHubProgram();
            //program.StartProcess(args);

            var program = new Program(ref args);

            /*Task.Run(() => {

                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), typeof(Program) + ".json");

                string[] pargs = (System.IO.File.Exists(path)) ?
                    (System.IO.File.ReadAllLines(path)) : new string[0];
                if (pargs.Length > 0)
                {

                }
                program.RunConsoleControl<Program>(ref pargs);

            });*/
            program.StartProcess();
            if(InputConsole.UserInteractive)
            {
                program.RemoveLast(ref program.started);
                var json = program.started.ToJsonOnScreen();
                InputConsole.ConfirmExecute($"Сохранить программу: \n{json}", () => {

                    System.IO.File.WriteAllText("program.json", json);
                }, ref args);
            }            
        }


        [Label("Остановить")]
        public void StopProcess( )
        {
            if(_cts is not null)
            {
                _cts.Cancel();
            }
        }


        [NotInput]
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

                    new ActionDbContext().Database.EnsureDeleted();
                    new ActionDbContext().Database.EnsureCreated();
                    new DeliveryDbContext().Database.EnsureDeleted();
                    new DeliveryDbContext().Database.EnsureCreated();

                    var configuration = builder.Configuration;
                    ConfigureServices(builder.Services, configuration);
                    DeliveryDbContext.ConfigureDeliveryServices(builder.Services, builder.Configuration);
                    builder.Services.AddSingleton<IpFilterMiddleware>( sp => this.ipFiltering );
                    
                    builder.Services.AddSingleton<ServiceMethodsMiddleware<DeliveryDbContext>>( sp => _serviceMethodsMiddleware.SetServiceProvider(sp.Get<IServiceScopeFactory>()) );
                    ModuleUser.ConfigureServices<DeliveryDbContext, CookieManager>(builder.Configuration, builder.Services);
                    //ModuleService.ConfigureServices(builder.Configuration, builder.Services);
                    var app = builder.Build();


                    this.mainMenu = new() {
                        { "Настройка фильтрации ПО IP",     UseIpFiltering(app) },
                        { "Настройка http-методов действия",UseServiceMethods<DeliveryDbContext>(app,new List<IActionBuilder>()) },
                        { "Настройки авторизации",          UserAuthorization<DeliveryDbContext>()  },
                        { "Настройки службы доставки",      ServiceAuthorization<DeliveryDbContext>()  },
                        { "Настройки фоновых процессов",    ()=>{
                            this.RunConsoleControl<Program>(ref started);
                        }  },
                        { "Завершить программу",                      ()=>{ this.active = false; this.StopProcess(); } },
                        { "Выполнить программу",                      ()=>{  } },
                    };
                    ToMainMenu();

                    app.UseRouting();
                    //app.MapControllerRoute(
                    //  name: "default",
                    //  pattern: "{controller=Mvc}/{action=Index}/{id?}");
                    app.MapHub<ActionsHub>($"/hubs/{nameof(ActionsHub)}");
                    app.RunAsync(_cts.Token).Wait();                    
                }
                catch (TaskCanceledException ex)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            }            
        }

        private void ToMainMenu()
        {
            InputConsole.SelectAction("Главное меню", this.mainMenu, ref started);
        }


        [Label("Настройка подключения сервисов")]
        private Action ServiceAuthorization<TServiceDbContext>()
            where TServiceDbContext : DbContextService
        {
            var initiallizer = new DbContextServiceInitiliazer();
            return () => {
                initiallizer.RunConsoleControl<DbContextServiceInitiliazer>(ref started);
                this.ToMainMenu();
            };
        }




        [Label("Настройка авторизации")]
        public Action UserAuthorization<TServiceDbContext>()
            where TServiceDbContext : DbContextUser
        {
            
            var initiallizer = new DbContextUserInitializer();
            return () => {
                initiallizer.RunConsoleControl<DbContextUserInitializer>( ref started );
                this.ToMainMenu();
            };
        }



        [Label("Интеграция динамических сборок в api")]
        private Action UseServiceMethods<TServiceDbContext>(WebApplication app, List<IActionBuilder> builders)
            where TServiceDbContext: ActionDbContext
        {            
            app.UseMiddleware<ServiceMethodsMiddleware<TServiceDbContext>>(  );
            return () => {
                this._serviceMethodsMiddleware.RunConsoleControl( ref started );
                this.ToMainMenu();
            };
        }




        /// <summary>
        /// Подключение компонента фильтрации по IP
        /// Возвращает ссылку на переход к конфигурации
        /// </summary>
        private Action UseIpFiltering(WebApplication app)
        {            
            app.UseMiddleware<IpFilterMiddleware>(  );
            return () => 
            {
                this.ipFiltering.RunConsoleControl<IpFilterMiddleware>(ref started);
                this.ToMainMenu();
            };

        }

        [NotInput]
        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            this.backgroundContext = BackgroungHostingStartup.ConfigureServices(services);

            services.AddSignalR();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            
            services.AddControllersWithViews();            
        }



        

 

        public void TestAdd()
        {            
             
            this.AddBackgroundServiceTypes<BackgroundService<Worker1>>();
        }

        public void TestRemove()
        {
            this.RemoveBackgroundServiceTypes<BackgroundService<Worker1>>();
        }


        [NotInput()]
        public void AddBackgroundServiceTypes<TBackgroundServiceType>() where TBackgroundServiceType : IWorkerService
        {
            try
            {
                lock (backgroundContext)
                {
                    if (!backgroundContext.BackgroundServiceTypes.Contains(typeof(TBackgroundServiceType)))
                    {
                        ((IBackgroundServiceContext)backgroundContext).AddBackgroundServiceTypes<TBackgroundServiceType>();
                    }
                }
            }
            catch(Exception ex)
            {
                this.Error(ex);
                throw;
            }
        }

        [NotInput()]
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
