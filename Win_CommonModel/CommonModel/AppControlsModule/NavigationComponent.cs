using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppNavModule;
using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.AppViews.ListModule;
using WinFormsApp.Console.Console_InputModule.ViewEvents.Dictionary;

using WinFormsApp.AppViews.ViewServices;

namespace WinFormsApp.AppComponents.AppControlsModule
{

    public class NavigationComponent : ComponentBase<NavigationView, NavigationControl>
    {
        private ComponentsFactory factory = new();
        public NavigationComponent() : this(new(), new()) { }
        public NavigationComponent([NotNullNotEmpty] NavigationView ViewModel, [NotNullNotEmpty] NavigationControl Control) : base(ViewModel, Control)
        {
            var list = new ListViewControl();
            var listComponent = new ListComponent(ViewModel, list);
            GetControl().SetList(list);
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var control = GetControl();
            view.PageChanged += OnPageChanged;
            GetViewModel().OnSelected += (text) =>
            {
                GetViewModel().Navigate($"{text}");
                GetControl().Navigate($"{text}");
            };
        }

        private void OnPageChanged(object sender, ChangingDictionaryEvent<string, Task<AppViewBase>> evt)
        {
            try
            {
                var control = GetControl();
                switch (evt.Target.Action)
                {
                    case "added":
                        {
                            var view = evt.Target.Value.Result;
                            control.AddPage(evt.Target.Key, new Task<Control>(() =>
                            {
                                return factory.CreateControl(view);
                            }));
                            GetViewModel().Options.Add(new Option()
                            {
                                Value = evt.Target.Key
                            });
                            break;
                        }
                    case "removed":
                        {
                            control.RemovePage(evt.Target.Key);
                            GetViewModel().Options.Remove(GetViewModel().Options.First(o => o.Value == evt.Target.Key));
                            break;
                        }
                    case "updated":
                        {
                            control.RemovePage(evt.Target.Key);
                            GetViewModel().Options.Remove(GetViewModel().Options.First(o => o.Value == evt.Target.Key));
                            var view = evt.Target.Value.Result;
                            control.AddPage(evt.Target.Key, new Task<Control>(() =>
                            {
                                return factory.CreateControl(view);
                            }));
                            GetViewModel().Options.Add(new Option()
                            {
                                Value = evt.Target.Key
                            });
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                this.Error($"Исключение при обработки события изменения навигации: {ex}");
            }
        }
    }

}
