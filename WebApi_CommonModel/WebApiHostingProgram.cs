using Blazor_CommonModel.Application;

using ClassModelLib.ActionsClient;
using Console_CommonModel;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;
using Console_CommonModel.CommonLib.Others;

using pickpoint_delivery_service;

using WebApi_CommonModel.BackgroundWorkers;
using WebApi_CommonModel.BackgroungHosting;
using WebApi_CommonModel.MiddlewareComponents;

namespace WebApi_CommonModel.WebApiModule
{
 


    [Label("Программа запуска приложения WebApi")]
    public class WebApiHostingProgram:  IBackgroundServiceContext
    {        
        public ILogger<WebApiHostingProgram> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<WebApiHostingProgram>();

        [HelpMessage("Отключается когда не нужен повторный рестарт")]
        public bool active { get; set; } = true;

        public CancellationTokenSource _cts;
        public string[] started;

        public ServiceMethodsMiddleware<DeliveryDbContext> _serviceMethodsMiddleware = new();
        public BackgroundServiceContext backgroundContext;
        public IpFilterMiddleware ipFiltering = new();

        public ChangingDictionary<string, Func<ActionRequest, ActionResponse>> controllers { get; set; }
        public Dictionary<string, Action> mainMenu { get; set; }

        public WebApiHostingProgram(ref string[] started)
        {
            this.started = started;            
        }



        [NotInput]
        public static void Main(string[] args)
        {
            if (System.IO.File.Exists("program.json"))
            {
                args = System.IO.File.ReadAllText("program.json").FromJson<List<string>>().ToArray();
            }
        

            //InputConsole.Info(args);
            //InputConsole.Interactive();
            //CommonProgram.Run<Program>(args=new string[0]);
            CommonProgram.Run<WebApiHostingProgram>(args);
            new ClassDbContext().Database.EnsureDeleted();
            new ClassDbContext().Database.EnsureCreated();

            var program1 = new object();
            var tservice = typeof(SigninUser<DeliveryDbContext>);
            

         
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

            var program = new WebApiHostingProgram(ref args);



            var middleware = new ServiceMethodsMiddleware<DeliveryDbContext>();
       

            middleware.AddServiceMethods<MyMvcDbContext>();

            program.StartProcess();
            if (InputConsole.UserInteractive)
            {
                program.RemoveLast(ref program.started);
                var json = program.started.ToJsonOnScreen();
                InputConsole.ConfirmExecute($"Сохранить программу: \n{json}", () =>
                {

                    System.IO.File.WriteAllText("program.json", json);
                }, ref args);
            }
        }

        public static void TesttClassDbContext()
        {
            var middleware = new ServiceMethodsMiddleware<DeliveryDbContext>();
            var methodNames = middleware.GetGenericMethodNames
            (
                nameof(ServiceMethodsMiddleware<DeliveryDbContext>.LoadWithBuilder)
            );
            
            middleware.AddDbContext<DeliveryDbContext>();
            middleware.Info(middleware.GetRoutes());
            new object().Info(methodNames);
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
                    builder.Services.AddSingleton<ApplicationEventsService>(  );
                    builder.Services.AddSingleton<IpFilterMiddleware>( sp => this.ipFiltering );
                    
                    builder.Services.AddSingleton<ServiceMethodsMiddleware<DeliveryDbContext>>( sp => _serviceMethodsMiddleware.SetServiceProvider(sp.Get<IServiceScopeFactory>()) );
                    ModuleUser.ConfigureServices<DeliveryDbContext, CookieManager>(builder.Configuration, builder.Services);
                    //ModuleService.ConfigureServices(builder.Configuration, builder.Services);
                    var app = builder.Build();
                    
                    var middleware = new ServiceMethodsMiddleware<DeliveryDbContext>(null);
                    var map = middleware.GetGenericMethodMap(nameof(middleware.AddDbContext));
                    middleware.Info(map.Keys);
                    map[map.Keys.First()].Invoke(middleware, new object[0]);

                    this.mainMenu = new() {
                        { "Настройка фильтрации ПО IP",     UseIpFiltering(app) },
                        { "Настройка http-методов действия",UseServiceMethods<DeliveryDbContext>(app,new List<IActionBuilder>()) },
                        { "Настройки авторизации",          UserAuthorization<DeliveryDbContext>()  },
                        { "Настройки службы доставки",      ServiceAuthorization<DeliveryDbContext>()  },
                        { "Настройки фоновых процессов",    ()=>{
                            this.RunConsoleControl<WebApiHostingProgram>(ref started);
                            ToMainMenu();
                        }  },
                        { "Завершить программу",                      ()=>{ this.active = false; this.StopProcess(); } },
                        { "Выполнить программу",                      ()=>{
                            
                        } },
                    };

                  

                    app.UseRouting();
                    //app.MapControllerRoute(
                    //  name: "default",
                    //  pattern: "{controller=Mvc}/{action=Index}/{id?}");
                    app.MapHub<ActionsHub>($"/hubs/{nameof(ActionsHub)}");
                    Task.Run(() => {
                        ToMainMenu();
                    });
                    app.RunAsync(_cts.Token);

                    
                   

                }
           
                catch (TaskCanceledException ex)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if(ex is CancelException)
                    {
                        continue;
                    }
                    this.Error(ex);
                    InputConsole.ConfirmContinue("Продолжить выполнение?");
                }
            }            
        }

        public void ToMainMenu()
        {
            
            InputConsole.SelectAction("Главное меню", this.mainMenu, ref started);
        }


        [Label("Настройка подключения сервисов")]
        public Action ServiceAuthorization<TServiceDbContext>()
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
        public Action UseServiceMethods<TServiceDbContext>(WebApplication app, List<IActionBuilder> builders)
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
        public Action UseIpFiltering(WebApplication app)
        {            
            app.UseMiddleware<IpFilterMiddleware>(  );
            return () => 
            {
                this.ipFiltering.RunConsoleControl<IpFilterMiddleware>(ref started);
                this.ToMainMenu();
            };

        }

        [NotInput]
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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
