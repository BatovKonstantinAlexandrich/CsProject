using Console_CommonModel;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

using Microsoft.AspNetCore.Components.Routing;

using System.Text.Json.Serialization;

namespace WebApi_CommonModel.WebApiModule
{

    [Label("Программа запуска приложения WebApi")]
    public class WebApiProgram: MyMvcApplicationRouter
    {
        
        private static void ConfigureLogging(ILoggingBuilder builder) => builder.AddConsole();
        private ILogger<WebApiProgram> logger = LoggerFactory.Create(ConfigureLogging).CreateLogger<WebApiProgram>();

        private List<string> ConfigureServicesCommands = new List<string>() {};
        private bool active = true;
        private string[] args = new string[0];

        

        


        public void StartProcess(string[] started)
        {
            while (active)
            {
                logger = LoggerFactory.Create(ConfigureLogging).CreateLogger<WebApiProgram>();
                logger.LogInformation("StartProcess()");

                CancellationTokenSource _cts = new();
                              
                var builder = WebApplication.CreateBuilder(args);
                ConfigureServices(builder.Services, builder.Configuration);

                var app = builder.Build();                                            
                app.Use((http, next) => {
                    logger.LogDebug("http => ");                    
                    return this.InvokeAsync(http, next);                    
                });
                app.RunAsync(_cts.Token).Wait();
            }
        }

        private void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            
            foreach (var command in ConfigureServicesCommands)
            {
                try
                {
                    logger.LogDebug($"{nameof(ConfigureServices)} => {command}");
                    var componentType = command.ToType();
                    if (!componentType.IsImplements(typeof(IMiddleware)))
                        throw new Exception($"Тип {command} не реализует {nameof(IMiddleware)}");
                    var middlewareComponent = componentType.New<IMiddleware>();
                    
                    logger.LogDebug($"{nameof(ConfigureServices)} => {command} => Completed");
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                    logger.LogDebug($"{nameof(ConfigureServices)} => {command} => Error => {ex.Message}");
                }
            }
        }

        public static void Main(string[] args)
        {

            CommonProgram.Run<WebApiProgram>(args);
            //var program = new ActionsHubProgram();
            //program.StartProcess(args);
            var program = new WebApiProgram();
            program.StartProcess(args);
        }



         

    }


}
