using Blazor_CommonModel.Application;
using System.Collections.Concurrent;

namespace Console_CommonModel
{
    public class BackgroundCollection<TActiveObject> : AuthorizationCollection<TActiveObject>
         where TActiveObject : ActiveObject
    {
        public ConcurrentDictionary<string, CancellationTokenSource> cancelations = new ConcurrentDictionary<string, CancellationTokenSource>();
        public ConcurrentDictionary<string, ConcurrentQueue<ActionRequest>> input = new ConcurrentDictionary<string, ConcurrentQueue<ActionRequest>>();
        public ConcurrentDictionary<string, ConcurrentQueue<ActionResponse>> output = new ConcurrentDictionary<string, ConcurrentQueue<ActionResponse>>();

        public BackgroundCollection(
            AuthorizationOptions options, 
            ApplicationEventsService applicationEventsService)
            : base(options, applicationEventsService)
        {
        }

        public override string Put(TActiveObject item)
        {
            lock(this)
            {
                var token = base.Put(item);
                cancelations[token] = new();
                item.input = input[token] = new();
                item.output = output[token] = new();
                Task.Run(() => {
                    while (!cancelations[token].Token.IsCancellationRequested)
                    {
                        this.Info(token);
                        if (input[token].TryDequeue(out var result))
                        {
                            lock(item)
                            {
                                output[token].Enqueue(item.Invoke(result));
                            }
                        }
                        Thread.Sleep(250);
                    }
                }, cancelations[token].Token);
                Task.Run(() => {
                    while (!cancelations[token].Token.IsCancellationRequested)
                    {
                        this.Info(token);
                        if (output[token].TryDequeue(out var result))
                        {
                            lock (item)
                            {
                                var handler = item.Take(result.SerialKey);
                                if(result.ActionSuccess)
                                {
                                    handler.OnSuccess(result.ActionResult);
                                }
                                else
                                {
                                    handler.OnError(result.ErrorMessage);
                                }                                
                            }
                        }
                        Thread.Sleep(250);
                    }
                }, cancelations[token].Token);
                return token;
            }
        }

        public override TActiveObject Remove(string token)
        {
            lock(this)
            {
                TActiveObject p = base.ContainsKey(token)? 
                    base.Remove(token): null;
                cancelations[token].Cancel();
                cancelations.TryRemove(token, out var in1);
                input.TryRemove(token, out var in2);
                output.TryRemove(token, out var in3);
                return p;
            }
        }
    }
}
