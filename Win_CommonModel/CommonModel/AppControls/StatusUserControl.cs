using MaterialSkin.Controls;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppViews.StatusModule
{
    public partial class StatusUserControl : UserControl
    {
        public StatusUserControl()
        {
            InitializeComponent();
            var side = new AppViewBase();
            side.TogglePanels();
            this.LayoutTop(this.progressBar1 = new MaterialProgressBar());
            this.LayoutTop(this.lblStatus = new Label());
            this.Height = 25;

            InitializeBackgroundWorker();

            progressWorker.RunWorkerAsync();
            actionWorker.RunWorkerAsync();
        }

        private BackgroundWorker actionWorker = new();
        private BackgroundWorker progressWorker = new();
        private MaterialProgressBar progressBar1;
        private Label lblStatus;
        private bool active = false;
        private ConcurrentQueue<(string, Action)> todos = new();
        private string actionName;
 

        private void InitializeBackgroundWorker()
        {

            actionWorker.WorkerReportsProgress = true;
            actionWorker.WorkerSupportsCancellation = true;
            actionWorker.DoWork += ActionBackgroundWorker_DoWork;

            progressWorker.WorkerReportsProgress = true;
            progressWorker.WorkerSupportsCancellation = true;

            progressWorker.DoWork += ProgressBackgroundWorker_DoWork;
            progressWorker.ProgressChanged += ProgressBackgroundWorker_ProgressChanged;
            progressWorker.RunWorkerCompleted += ProgressBackgroundWorker_RunWorkerCompleted;
        }

        private void ActionBackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            bool completed = false;
            (string, Action) todo;
            while (!completed)
            {
                if (todos.TryDequeue(out todo))
                {
                    try
                    {
                        DoAction(todo.Item1, todo.Item2);
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
            e.Result = "Задача завершена успешно!"; // Результат операции
        }

        /// <summary>
        /// В очередь на выполнение
        /// </summary>
        public void PutAction(string name, Action todo)
        {
            todos.Enqueue((name, todo));
        }


        public virtual void DoAction(string name, Action todo)
        {
            try
            {
                this.OnActionStarted(name);
                todo();
                this.OnActionCompleted(name);

            }
            catch (Exception ex)
            {
                this.OnActionError(name, ex);

            }
        }

        private void OnActionError(string name, Exception ex)
        {
            this.active = false;
            this.SetStatusText($"Ошибка: {name} => {ex.Message}");
            this.progressBar1.Value = 0;
            this.Invoke(() =>
            {
                this.progressBar1.Value = 0;
            });
        }

        private void OnActionCompleted(string name)
        {
            this.active = false;
            lblStatus.Text = $"Выполнено: {name}";
            this.Info($"Выполнено: {name}");
            this.progressBar1.Value = 100;
            this.Invoke(() =>
            {

                this.progressBar1.Value = 100;
            });
        }

        public void SetValue(int value)
        {
            this.progressBar1.Value = value;
        }

        private void OnActionStarted(string name)
        {
            this.actionName = name;

            this.Invoke(() => { this.active = true; lblStatus.Text = $"Выполнение: {name}"; });
            this.Info(lblStatus.Text);
        }


        // Запуск фоновой задачи
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!progressWorker.IsBusy)
            {
                progressBar1.Value = 0;
                lblStatus.Text = $"Выполнение: {actionName}";
                progressWorker.RunWorkerAsync(); // Запуск фонового потока
            }
        }

        // Отмена задачи
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (progressWorker.IsBusy)
            {
                lblStatus.Text = $"Отмена: {actionName}";
                progressWorker.CancelAsync(); // Запрос на отмену
            }
        }

        // Фоновая задача (выполняется в отдельном потоке)
        private void ProgressBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            BackgroundWorker worker = sender as BackgroundWorker;
            int i = 0;
            while (true)
            {
                if (active)
                {

                    i += 1;
                    if (i == 100) i = 1;
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    System.Threading.Thread.Sleep(5);
                    string title = lblStatus.Text = $"Выполнение: {actionName}"; ;
                    worker.ReportProgress(i, title);
                    this.Invoke(() =>
                    {
                        Text = title;
                    });
                }

            }

        }

        // Обновление UI (выполняется в UI-потоке)
        private void ProgressBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lblStatus.Text = e.UserState?.ToString() ?? $"Прогресс: {e.ProgressPercentage}%";
        }

        // Завершение задачи
        private void ProgressBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblStatus.Text = "Операция отменена";
                progressBar1.Value = 0;
            }
            else if (e.Error != null)
            {
                lblStatus.Text = $"Ошибка: {e.Error.Message}";
            }
            else
            {
                lblStatus.Text = e.Result?.ToString() ?? "Завершено";
                progressBar1.Value = 100;
            }
        }

        public void SetStatusText(string exception)
        {
            lblStatus.Text = exception;
        }
    }
}
