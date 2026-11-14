using Blazor_MyApp.Services.ContextServices;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blazor_CommonModel
{
    public class DoCheckBackground(IServiceProvider serviceProvider, ILogger<DoCheckBackground> logger) : BackgroundService
    {

        public bool EnableLogging { get; private set; } = false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {                
                this.DoCheck();
                await Task.Delay(100);
            }
        }

        private void DoCheck()
        {
            if(this.EnableLogging)
                logger.LogInformation("DoCheck()");
            serviceProvider.Get<ConnectionsCollection>().DoCheck();
            var connections = serviceProvider.Get<ConnectionsCollection>().GetAll();
            this.Debug("connections count = " + connections.Count());

            var users = serviceProvider.Get<AuthorizationCollection<UserContext>>().GetAll();
            this.Debug("users count = "+users.Count());
          
            //var services = serviceProvider.Get<AuthorizationCollection<ServiceContext>>().GetAll();
            //serviceProvider.Get<AuthorizationCollection<ServiceContext>>().DoCheck();
            //this.Debug("services count = " + services.Count());
            
        }
    }
}