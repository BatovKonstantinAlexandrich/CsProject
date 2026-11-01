
using WebApi_CommonModel.BackgroungHosting;

namespace WebApi_CommonModel.BackgroundWorkers
{
    public class Worker1: IWorker
    {
        public Worker1()
        {
        }

        public async Task RunWorkerAsync(CancellationToken stoppingToken)
        {
            this.Info(DateTime.Now);
        }
    }
}
