using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;

using System.Collections.Concurrent;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.DataComponents;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.StatusModule
{


    public interface IStatusControl : IStatusView, IControlBase
    {
        void SetStatusText(string text);
        void SetValue(int value);
    }


    [Label("Панель состояния")]
    public class StatusView : ColumnsView, IStatusView
    {

        [Label("Text")]
        public string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }
        [InputNumber]
        [Label("Value")]
        public int Value { get => Get<int>(nameof(Value)); set => Set(nameof(Value), value); }
        [InputBool]
        [Label("Active")]
        public bool Active { get => Get<bool>(nameof(Active)); set => Set(nameof(Active), value); }

        private readonly BlockingCollection<(string Name, Action Todo, IControlBase SyncRoot)> _actionQueue = new();

        //private System.Windows.Forms.Timer _progressTimer;
        private const int ProgressMax = 100;
        private const int ProgressStep = 4;

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public static ConcurrentQueue<(string, Action)> todos = new();

        public StatusView()
        {
            Text = "Ожидание";
            Active = false;
            Value = 0;
            Height = 30;
            HeightUnit = "px";
            MinHeight = 30;
            
            Task.Run(ProcessActionQueue);
        }

        public override IControlBase ToControl()
        {
            return control;
        }

        public override void PutError(string name, string error)
        {
            Text = $"{name} => {error}";
        }

        /// <summary>
        /// Выполняет делегат в потоке UI (синхронно для вызывающего фонового потока).
        /// Вызывайте из действия <see cref="PutAction"/> для обновления WinForms.
        /// </summary>
        public void InvokeUi(Action action)
        {
            if (action is null)
                return;
            try
            {
                var ui = ToControl().GetAppForm();
                if (ui is null || ui.IsDisposed)
                    return;
                if (ui.InvokeRequired)
                    ui.Invoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Действие выполняется в отдельном потоке; индикатор на UI крутится, пока оно не завершится.
        /// </summary>
        public override void PutAction(string name, Action todo, int timeout)
        {
            if (todo is null)
                return;
            IControlBase sync = null;
            try
            {
                var viewRoot = this.GetAppRootView();
                sync = viewRoot.control;
            }
            catch
            {
            }
            _actionQueue.Add((name, todo, sync));
            DoAsync();
        }

        private void ProcessActionQueue()
        {
            foreach (var job in _actionQueue.GetConsumingEnumerable())
            {
                try
                {
                    RunQueuedAction(job.Name, job.Todo,job.SyncRoot );
                }
                catch (Exception ex)
                {
                    this.Info($"PutAction queue: {ex.Message}");
                }
            }
        }

        private void RunQueuedAction(string Name, Action Todo, IControlBase SyncRoot)
        {
            var ui = SyncRoot;
            if (ui is null || ui.IsDisposed)
            {
                try
                {
                    ui = this.ToControl().GetAppForm();
                }
                catch
                {
                    try { Todo(); }
                    catch (Exception ex) { this.Info(ex.Message); }
                    return;
                }
            }

            if (ui is null || ui.IsDisposed)
            {
                try { Todo(); }
                catch (Exception ex) { this.Info(ex.Message); }
                return;
            }

            Exception error = null;
            ui.Invoke(new Action(() => StartProgressAnimation(Name)));

            var work = Task.Run(() =>
            {
                try
                {
                    Todo();
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            });
            work.Wait();

            if (ui.IsDisposed)
                return;

            ui.Invoke(new Action(() =>
            {
                StopProgressAnimation();
                if (error is not null)
                    OnActionError(Name, error);
                else
                    OnActionCompleted(Name);
            }));
        }

        private void StartProgressAnimation(string name)
        {
            OnActionStarted(name);
            
            Value = 0;
            Active = true;
             
        }

        private void ProgressTimerOnTick(object sender, EventArgs e)
        {
            if (!Active)
            {
                //_progressTimer?.Stop();
                return;
            }
            int v = Value + ProgressStep;
            if (v > ProgressMax)
                v = 0;
            Value = v;
        }

        private void StopProgressAnimation()
        {
            //_progressTimer?.Stop();
        }

        public virtual void DoAction(string name, Action todo)
        {
            try
            {
                OnActionStarted(name);
                todo();
                OnActionCompleted(name);
            }
            catch (Exception ex)
            {
                OnActionError(name, ex);
            }
        }

        public void OnActionError(string name, Exception ex)
        {
            Active = false;
            SetStatusText($"Ошибка: {name} => {ex.Message}");
            SetValue(0);
        }

        private void SetValue(int value)
            => Value = value;

        private void SetStatusText(string text)
            => Text = text;

        public void OnActionCompleted(string name)
        {
            Active = false;
            Text = $"Выполнено: {name}";
            //this.Info($"Выполнено: {name}");
            Value = 100;
        }

        public void OnActionStarted(string name)
        {
            Active = true;
            Text = $"Выполнение: {name}";
            this.Info(Text);
        }

        /// <summary>
        /// Оставлено для совместимости с <see cref="AppViewRoot"/>; очередь обрабатывается в конструкторе.
        /// </summary>
        public void StartAsync()
        {
            //this.Info("Ожидание следующей операции..");
            Task.Run(() => { 
                while(true)
                {
                    DoAsync();
                    Thread.Sleep(100);
                }
            });
        }

        public void DoAsync()
        {
            this.Info("Следующая итерация");
            if (_actionQueue.TryTake(out var p))
            {
                DoAction(p.Name, p.Todo);
            }
            
        }
    }
}
