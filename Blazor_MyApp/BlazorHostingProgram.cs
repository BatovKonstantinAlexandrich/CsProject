using Blazor_CommonModel;
using Blazor_CommonModel.App;
using Blazor_CommonModel.Application;

using Blazor_MyApp.Components;
using Blazor_MyApp.Services;
using Blazor_MyApp.Services.ContextServices;

using ClassModelLib.ServicesModule;

using Console_CommonModel;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;


using pickpoint_delivery_service;

using System.Reflection;

using WebApi_CommonModel.BackgroungHosting;
using WebApi_CommonModel.MiddlewareComponents;
using WebApi_CommonModel.WebApiModule;

namespace Blazor_MyApp
{
    public class BlazorHostingProgram: WebApiHostingProgram
    {

        private ILogger<BlazorHostingProgram> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BlazorHostingProgram>();

        public BlazorHostingProgram(ref string[] started) : base(ref started)
        {
        }

        public static void Main(string[] args)
        {

            CommonProgram.Run<BlazorHostingProgram>(new string[] { "--no-force" });
            new MyValidatableObjectTest().Debug(Assembly.GetExecutingAssembly().GetClassTypes().Where(t => t.Namespace is not null && t.Namespace.Contains("Pages")).Select(t => t.GetTypeName()));

            ServiceFactory.Get().AddTypesAsync(typeof(BlazorHostingProgram).Assembly, true);


            var program = new BlazorHostingProgram(ref args);
            program.StartBlazorHost(args);
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

        private void StartBlazorHost(string[] args)
        {
            while (active)
            {
                try
                {
                    logger.LogInformation("StartProcess()");
                    this._cts = new();

                    AppProviderService.GetInstance().AddServiceImplementation<IClientAPI, BlazorClientAPI>();
                    AppProviderService.GetInstance().AddServiceImplementation<IComponentsFactory<IControlBase>, WebComponentsFactory>();
                    var ns = new NavMenuService(typeof(BlazorHostingProgram).Assembly);

                    this.Debug(ns.GetPageLinks("/").Select(p => p.Label));
                    this.Debug(ns.GetPageLinks("/actions").Select(p => p.Label));

                    //while(true)
                    //{
                    //    Console.Clear();
                    //    Console.Write("uri");
                    //    var uri = Console.ReadLine();
                    //    this.Debug(ns.GetPageLinks(uri).Select(p => p.Href));
                    //    InputConsole.ConfirmContinue();


                    //}

                    var builder = WebApplication.CreateBuilder(args);

                    var configuration = builder.Configuration;
                    //ConfigureServices(builder.Services, configuration);
                    //DeliveryDbContext.ConfigureDeliveryServices(builder.Services, builder.Configuration);
                    //builder.Services.AddSingleton<ApplicationEventsService>();
                    //builder.Services.AddSingleton<IpFilterMiddleware>(sp => this.ipFiltering);

                    //builder.Services.AddSingleton<ServiceMethodsMiddleware<DeliveryDbContext>>(sp => _serviceMethodsMiddleware.SetServiceProvider(sp.Get<IServiceScopeFactory>()));
                    //ModuleUser.ConfigureServices<DeliveryDbContext, CookieManager>(builder.Configuration, builder.Services);
                    ////ModuleService.ConfigureServices(builder.Configuration, builder.Services);

                    this.Info(new ViewServicesUnit(AppProviderService.GetInstance()).DoTest(false,true).ToDocument());
                    // Add services to the container.
                    builder.Services.AddRazorComponents()
                        .AddInteractiveServerComponents();
                    builder.Services.AddHttpContextAccessor();

                    builder.Services.AddNavMenu(GetType().Assembly);
                    // Program.cs
                    builder.Services.Configure<ForwardedHeadersOptions>(options =>
                    {
                        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                        // Если используется известный прокси, можно очистить KnownNetworks/Proxies для безопасности
                    });
  
                    //ModuleUser.ConfigureServices<DeliveryDbContext>(builder.Configuration, builder.Services);
                    //ModuleService.ConfigureServices<DeliveryDbContext>(builder.Configuration, builder.Services);
                    //DeliveryDbContext.ConfigureDeliveryServices(builder.Services, builder.Configuration);
                    //AppProviderService.GetInstance().Get<AuthorizationCollection<UserContext>>();
                    //ModuleUser.ConfigureServices<DeliveryDbContext, ClientLocalStorageTokenProvider>(builder.Configuration, builder.Services);
                    //builder.Services.AddScoped<ILocalStorage, LocalStorage>();

                    //ModuleService.ConfigureServices<DeliveryDbContext>(builder.Configuration, builder.Services);
                    ////builder.Services.AddHostedService<GrpcBackgroundMessaging>();
                    ////builder.Services.AddHostedService<DoCheckBackground>();
                    builder.Services.AddSingleton<AuthorizationOptions>();
                    builder.Services.AddSingleton<ConnectionsCollection>();

                    builder.Services.AddSingleton<ApplicationEventsService>();

                    builder.Services.AddHttpClient();
                    builder.Services.AddScoped<LayoutService>();
                    builder.Services.AddScoped<CookieManager>();
                    builder.Services.AddTransient<CircuitHandler, ConnectionsHandler>();
                    builder.Services.AddSingleton<ServiceMethodsMiddleware<DeliveryDbContext>>();

                    var app = builder.Build();
                    app.UseExceptionHandler("/Error");
                    app.UseHsts();

                    app.UseHttpsRedirection();

                    app.UseStaticFiles();
                    app.UseAntiforgery();

                    app.MapRazorComponents<App>()
                        .AddInteractiveServerRenderMode();

                    UseServiceMethods<DeliveryDbContext>(app, new List<IActionBuilder>());


                    //Task.Run(() => {
                    //    this.mainMenu = new() {
                    //        { "Настройка фильтрации ПО IP",     UseIpFiltering(app) },
                    //        { "Настройка http-методов действия",UseServiceMethods<DeliveryDbContext>(app,new List<IActionBuilder>()) },
                    //        { "Настройки авторизации",          UserAuthorization<DeliveryDbContext>()  },
                    //        { "Настройки службы доставки",      ServiceAuthorization<DeliveryDbContext>()  },
                    //        { "Настройки фоновых процессов",    ()=>{
                    //            this.RunConsoleControl<BlazorHostingProgram>(ref started);
                    //            ToMainMenu();
                    //        }  },
                    //        { "Завершить программу",                      ()=>{ this.active = false; this.StopProcess(); } },
                    //        { "Выполнить программу",                      ()=>{

                    //        } },
                    //    };
                    //    ToMainMenu();
                    //});

                    app.RunAsync(_cts.Token).Wait();
                }

                catch (TaskCanceledException ex)
                {
                    break;
                }
                catch (Exception ex)
                {
                     
                    this.Error(ex);
                    InputConsole.ConfirmContinue("Продолжить выполнение?");
                }
            }

        }
    }
}
