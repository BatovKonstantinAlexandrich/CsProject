using Grpc.Core;
using Grpc.Net.Client;

using Grpc_ProgramsService;

using System.Collections.Concurrent;

namespace Console_CsDeveloper
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Thread.Sleep(2500);
            var url = "https://localhost:7198";

            using var channel = GrpcChannel.ForAddress(url);
            var client = new GatewayService.GatewayServiceClient(channel);
            var reply = client.Start();


            ConcurrentQueue<ActionResponse> responses = new();

            var readTask = Task.Run(async () =>
            {
                while (true)
                {
                    while (responses.TryDequeue(out var p))
                    {
                        await reply.RequestStream.WriteAsync(p);
                    }
                    Thread.Sleep(100);
                }

                await reply.RequestStream.CompleteAsync();
            });



            Func<ActionRequest, ActionResponse> handler = request => {
                return new()
                {

                };
            };
            Predicate<string> Authenticated = token => true;
            Action<object> Log = message => {
            };

            // Чтение: цикл завершится, когда сервер закроет поток
            await foreach (var message in reply.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{message.AccessToken} [{message.ActionName}] {message.MessageObject}");
                if (Authenticated(message.AccessToken))
                {
                    Log(message);
                    var response = handler(message);
                    responses.Enqueue(response);
                    Log($"[{response}");
                }
            }
        }
    }
}
