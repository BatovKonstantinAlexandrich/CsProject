using WebApi_CommonModel.BackgroungHosting;

namespace WebApi_CommonModel.BackgroundWorkers
{
    public class Worker2: IWorker
    {
        public Worker2()
        {
        }

        public async Task RunWorkerAsync(CancellationToken stoppingToken)
        {
            this.Info(DateTime.Now);
        }
    }
}
