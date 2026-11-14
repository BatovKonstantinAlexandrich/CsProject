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
            var users = serviceProvider.GetServices<AuthorizationCollection<UserContext>>();
            this.Debug("users count = "+users.Count());
            foreach (var active in users)
            {
                active.DoCheck();
            }
            this.Debug("users count = " + users.Count());
            foreach (var active in serviceProvider.GetServices<AuthorizationCollection<ServiceContext>>())
            {
                active.DoCheck();
            } 
        }
    }
}