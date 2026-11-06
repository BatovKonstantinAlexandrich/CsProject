using Grpc_ProgramsService.Services;

namespace Grpc_ProgramsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddGrpc();

            var app = builder.Build();

            app.MapGrpcService<MessageGateway>();

            app.Run();
        }
    }
}