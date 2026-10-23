using ClassModelLib.ActionsClient;

using Grpc_WorkerService.WorkerModule;

namespace Grpc_WorkerService
{
    public class Worker<TRequest, TResponrt> : BackgroundService
        
    {
        private readonly ILogger<Worker<TRequest, TResponrt>> _logger;
        private readonly WorkerContext _context;
        private HashSet<Func<TRequest, TResponrt>> _handlers = new();
        private readonly IQueueConsumer _consumer;

        public Worker(ILogger<Worker<TRequest, TResponrt>> logger, WorkerContext context, IQueueConsumer<TRequest> consumer)
        {
            _logger = logger;
            _context = context;            
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Information))                    
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    _consumer.Take();
                    await Task.Delay(100, stoppingToken);
                }
            } 
            catch(Exception ex)
            {
                
            }
            
        }
    }
}
