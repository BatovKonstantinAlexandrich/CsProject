using MaterialSurface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.StatusModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class StatusComponent : ComponentBase<StatusView, StatusControl>
    {
        public StatusComponent() : base(new(), new()) { }

        public StatusComponent([NotNullNotEmpty] StatusView ViewModel, [NotNullNotEmpty] StatusControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var control = GetControl();
            view.OnChange += (evt) =>
            {
                switch (evt.Property)
                {
                    case nameof(StatusView.Text):
                        var text = evt.After.ToString();
                        control.SetStatusText(text);
                        break;
                    case nameof(StatusView.Value):
                        var value = evt.After.ToString().ToInt();
                        control.SetValue(value);
                        break;

                }
            };
            bool completed = false;
            (string, Action) todo;
            Task.Run(() =>
            {
                while (!completed)
                {
                    if (StatusView.todos.TryDequeue(out todo))
                    {
                        try
                        {
                            control.PutAction(todo.Item1, todo.Item2);
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
    }
}
