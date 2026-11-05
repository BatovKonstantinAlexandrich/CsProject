
using System.Collections.Concurrent;

using WebApi_CommonModel.BackgroungHosting;

namespace WebApi_CommonModel.BackgroundWorkers
{

    public abstract class BackgroundQueueWorker<TMessage>: IWorker
    {
        private ConcurrentQueue<TMessage> messages = new();


        public async Task RunWorkerAsync(CancellationToken stoppingToken)
        {
            while(messages.TryDequeue(out TMessage message))
            {
                await HandleMessage(message);
            }
        }

        protected abstract Task HandleMessage(TMessage message);
    }
}
