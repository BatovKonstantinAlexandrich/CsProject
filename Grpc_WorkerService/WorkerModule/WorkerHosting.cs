
using ClassModelLib.ActionsClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc_WorkerService.WorkerModule
{
    internal class WorkerHosting
    {
        
        internal static void BuildHost<TActionClient>(HostApplicationBuilder builder, ConfigurationManager configuration)
            where TActionClient: class, IQueueConsumer
        {
            builder.Services.AddSingleton<WorkerContext>();
            builder.Services.AddSingleton<IQueueConsumer, TActionClient>();
            //builder.Services.AddHostedService<Worker<TActionClient>>();
        }
    }
}
