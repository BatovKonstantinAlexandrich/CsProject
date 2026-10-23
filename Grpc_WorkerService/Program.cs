using Grpc_WorkerService.WorkerModule;

namespace Grpc_WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            WorkerHosting.BuildHost<>(builder, builder.Configuration);

            var host = builder.Build();
            host.Run();
        }
    }
}