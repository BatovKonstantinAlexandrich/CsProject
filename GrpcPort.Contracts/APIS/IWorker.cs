namespace WebApi_CommonModel.BackgroungHosting
{
    public interface IWorker
    {
        public Task RunWorkerAsync(CancellationToken stoppingToken);
    }
}
