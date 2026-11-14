using System.Collections.Concurrent;

namespace Console_CommonModel
{
    public class ActionsQueue: ConcurrentQueue<ActionRequest>
    {              



        public ConcurrentQueue<ActionResponse> CreateConnection( Func<ActionRequest, ActionResponse> handler, CancellationToken cancellationToken)
        {
            ConcurrentQueue<ActionResponse> output = new();
            Task.Run(() => { 
                while(!cancellationToken.IsCancellationRequested)
                {
                    if(this.TryDequeue(out var p))
                    {
                        var response = handler(p);
                        output.Enqueue(response);
                    }
                    Task.Delay(100).Wait();
                }
            }, cancellationToken);
            return output;
        }
    }
}
