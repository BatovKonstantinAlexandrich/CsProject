

namespace WebApi_CommonModel.WebApiModule
{

    /// <summary>
    /// Программа запуска приложения WebApi
    /// </summary>
    public class WebApiProgram
    {
        ILogger<WebApiProgram> logger = LoggerFactory.Create(ConfigureLogging).CreateLogger<WebApiProgram>();

        private bool active = true;
        private string[] args = new string[0];

        private List<string> ConfigureServicesCommands = new List<string>();
        private static void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddConsole();
        }

        public void StartProcess(string[] started)
        {

            while (active)
            {
                logger = LoggerFactory.Create(ConfigureLogging).CreateLogger<WebApiProgram>();
                logger.LogInformation("StartProcess()");

                CancellationTokenSource _cts = new();
                var api = new WebApiComponent();
                var builder = WebApplication.CreateSlimBuilder(args);
                ConfigureServices(builder.Services, builder.Configuration);

                var app = builder.Build();
                var handler = api.OnHttpRequestHandler();
                app.Use((http, next) => handler(http, next));
                app.RunAsync(_cts.Token).Wait();
            }
        }

        private void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            
        }

        public static void Main(string[] args)
        {
            var program = new ActionsHubProgram();
            program.StartProcess(args);
        }


    }


}
