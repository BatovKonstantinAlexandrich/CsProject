using System.Collections.Concurrent;

namespace Console_CommonModel.CommonLib.Others
{
    /// <summary>
    /// Программа фонового выполнения операций
    /// </summary>
    public class ProgramBackgroundSingle : ProgramBackground
    {
        public override void DoAction(Action todo)
        {
            todo();
        }
    }
    public partial class ProgramBackground
    {
        private static ConcurrentQueue<Action> todos = new();

        public ProgramBackground()
        {
            StartAsync();
        }


        /// <summary>
        /// В очередь на выполнение
        /// </summary>
        public void PutAction(Action todo)
        {
            todos.Enqueue(todo);
        }


        /// <summary>
        /// Периодически просматриваем  очередь операций
        /// </summary>
        public void StartAsync()
        {
            this.Info("Ожидание следующей операции..");
            Task.Run(() =>
            {
                bool completed = false;
                Action todo = null;
                while (!completed)
                {
                    todos.TryDequeue(out todo);
                    if (todo is not null)
                    {
                        try
                        {
                            DoAction(todo);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                }
            });
        }


        /// <summary>
        /// Виртуальный метод, выполняющий операцию требует override
        /// </summary>
        public virtual void DoAction(Action todo)
        {
            todo();
        }
    }
}
