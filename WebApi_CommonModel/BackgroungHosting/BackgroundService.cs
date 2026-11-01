namespace WebApi_CommonModel.BackgroungHosting
{

    [Label("Класс для динамической регистрации служб ")]
    public class BackgroundService<TWorker>(TWorker worker) : IWorkerService
        where TWorker : IWorker
    {
        private CancellationTokenSource _cts = new();

        private ILogger<BackgroundService<TWorker>> logger = LoggerFactory.Create(o => o.AddConsole()).CreateLogger<BackgroundService<TWorker>>();


        public async Task Start()
        {
            var stoppingToken = _cts.Token;

            logger.LogInformation("Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await worker.RunWorkerAsync(stoppingToken);
                    await Task.Delay(100);
                    logger.LogInformation("Next");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex);
                }
            }
            logger.LogInformation("Completed");
        }

        public async Task Stop()
        {
            _cts.Cancel();
        }
    }
}
