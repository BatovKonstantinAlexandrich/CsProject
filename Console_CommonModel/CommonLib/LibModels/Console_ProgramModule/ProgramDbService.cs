using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Microsoft.Extensions.Hosting;

using System.Collections.Concurrent;

namespace Console_CommonModel.CommonLib.LibModels.Console_ProgramModule
{


    [Label("Сведения о выполняемой программе")]
    public class ProgramExecutingInfo : NamedObject
    {
        public string Arguments { get; set; }
        public DateTime Started { get; set; }
        public Task Todo { get; set; }

        public override string ToString()
        {
            return $"{Started} Выполнение программы {Name} => {Todo.Status}";
        }
    }






    [Label("Управление выполнением программ")]
    public class ProgramDbService : BackgroundService, ICommandExecuter
    {
        public Action<string> OnStarted = (text) => { };
        public Action<string> OnCompleted = (text) => { };
        public Action<string, string> OnError = (text, error) => { };

        /// <summary>
        /// Очередь для линейного выполнения
        /// </summary>
        private ConcurrentQueue<(string, Action)> todos = new();
        private ConcurrentDictionary<string, ProgramExecutingInfo> active { get; set; } = new();

        public ProgramDbService()
        {
            OnCompleted += name =>
            {
                lock (active)
                {
                    if (active.TryRemove(name, out var p))
                    {
                        this.Info($"Завершилось выполнение: '{name}' => " + "успешно");
                    }
                }
            };
            OnError += (name, message) =>
            {
                lock (active)
                {
                    if (active.TryRemove(name, out var p))
                    {
                        this.Info(p.ToString());
                    }
                }
            };
        }




        /// <summary>
        /// В очередь на выполнение
        /// </summary>
        public void PutAction(string title, Action todo)
        {
            todos.Enqueue((title, todo));
        }





        /// <summary>
        /// Виртуальный метод, выполняющий операцию требует override
        /// </summary>    

        public Task DoActionAsync(string title, Action todo, CancellationToken token, Action cancel)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    OnStarted(title);
                    todo();
                    OnCompleted(title);
                }
                catch (Exception ex)
                {
                    OnError(title, ex.Message);
                }
            }, token);

            return task;
        }


        public ChangingList<string> GetActivities()
        {
            lock (active)
            {
                var card = new CardView();
                card.SelectList(
                    "Активные процесса",
                    active.Values.ToList(),
                    selected =>
                    {

                    }
                );
                active.Values.Select(value => value.ToString());
            }
            throw new NotImplementedException();
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                (string, Action) todo;
                if (todos.TryDequeue(out todo))
                {
                    try
                    {
                        var cts = new CancellationTokenSource();
                        DoActionAsync(todo.Item1, todo.Item2, cts.Token, () =>
                        {
                            cts.Cancel();
                        });
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            await Task.CompletedTask;
        }

        public void OnCommand(string command)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ProgramDbContextInitiallizer : DbContextInitiliazer<ProgramDbContext>
    {
        public override Dictionary<string, int> Init(ProgramDbContext context)
        {
            Dictionary<string, int> results = new Dictionary<string, int>();
            return results;
        }
    }

}