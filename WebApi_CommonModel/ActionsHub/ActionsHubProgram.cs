using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using WebApi_CommonModel.WebApiModule;

namespace WebApi_CommonModel
{
    public class ActionsHubProgram
    {
        public void StartProcess(string[] started)
        {
            CancellationTokenSource _cts = new();
            var builder = WebApplication.CreateSlimBuilder(started);
            builder.Services.AddSignalR();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<ConnectionsContext>();
            builder.Services.AddSingleton<AsyncContext>();
            builder.Services.AddSingleton<ActionsContext>();
            var app = builder.Build();
            app.MapHub<ActionsHub>("/hubs/ActionsHub");
            app.RunAsync(_cts.Token).Wait();
            
        }
    }
}
