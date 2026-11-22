using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;


namespace WebApi_CommonModel.BackgroungHosting
{

    [Label("Управление фоновыми процессами")]    
    public class BackgroundServicePool(
        ILogger<BackgroundServicePool> logger,
        BackgroundServiceContext context
    ) : BackgroundService, IDisposable
    {
        
        private void OnBackgroundServicesChanged(object? sender, ListChangedEvent e)
        {
            switch (e.Target.Action)
            {
                case "added":   StartBackgroundService((Type)e.Target.Item); break;
                case "removed": StopBackgroundService((Type)e.Target.Item); break;
            }
        }

        private void StartBackgroundService(Type item)
        {
            try
            {
                logger.LogInformation($"StartBackgroundService({item.GetTypeName()})");
                var backgroundService = (IWorkerService)AppProviderService.GetInstance().GetService(item);
                context.BackgroundServices[item] = backgroundService;
                backgroundService.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void StopBackgroundService(Type item)
        {
            try
            {
                logger.LogInformation($"StopBackgroundService({item.GetTypeName()})");

                context.BackgroundServices[item].Stop();
                context.BackgroundServices.TryRemove(item, out var p);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Started");
                context.BackgroundServiceTypes.OnChanged += OnBackgroundServicesChanged;
                while (!stoppingToken.IsCancellationRequested)
                {
                    //logger.LogInformation("Next");
                    Thread.Sleep(500);
                }
            }
            
            catch (TaskCanceledException ex)
            {
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
            finally
            {
                context.BackgroundServiceTypes.Clear();
                logger.LogInformation("Completed");
                context.BackgroundServiceTypes.OnChanged -= OnBackgroundServicesChanged;
            }

        }
    }
}
