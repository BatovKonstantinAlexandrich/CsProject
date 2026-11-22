using Microsoft.Extensions.Hosting;

using RootConsumer.ActionsClient;

using System.Runtime.InteropServices;

namespace Blazor_CommonModel
{
    public class ActionsBackgroundClient: BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            string[] args = new string[0];
            var program = new ActionsProgram("https://localhost:7110/hubs/ActionsHub");
            //ClassModelLibProgram.program = program;
          
            Console.WriteLine("Запускаю программу...");
            CancellationTokenSource cts = null;
            program.OnConnected += () => {
                Console.WriteLine("Запуск консоли управления");
                cts = new();
                program.Load();
            };
            program.OnDisconnected += (ex) => {
                cts.Cancel();
            };
            program.StartAsync().Wait();
            Thread.Sleep(Timeout.Infinite);
            
              
        }
    }
}