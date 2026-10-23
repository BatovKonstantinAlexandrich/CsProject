using WinFormsApp.AppViews.AppRootModule;

namespace WinFormsApp.AppComponents.AppControls
{

    /// <summary>
    /// Компонент корневого представления.
    /// Передаёт события фокусирования от верхнего представления к элементам.
    /// </summary>
    public class AppComponentRoot : ComponentBase<AppViewRoot, AppControlRoot>
    {
        public AppComponentRoot() : this(new(), new()) { }

        public AppComponentRoot([NotNullNotEmpty] AppViewRoot ViewModel, [NotNullNotEmpty] AppControlRoot Control) : base(ViewModel, Control)
        {
        }



        public Dictionary<string, List<string>> ValidateComponentTree()
        {
            Dictionary<string, List<string>> results = new();
            if (GetAllComponents().Any(comp => comp.Control != comp.ViewModel.control))
            {
                results["/"].Add("Ссылки на элементы управления отличаются у компонента и представления");
            }
            return results;
        }


        public override void Bind()
        {
            /*AppViewRoot view = GetViewModel();
            var ctrl = GetControl();
            view.FocusedViews.OnChanged += (sender, evt) =>
            {
                switch (evt.Target.Action)
                {
                    case "added":
                        {
                            var viewItem = (ViewItem)evt.Target.Item;
                            if (viewItem is AppViewBase)
                            {
                                ((AppViewBase)viewItem).Focused = true;
                            }
                            break;
                        }
                    case "removed":
                        {
                            var viewItem = (ViewItem)evt.Target.Item;
                            if (viewItem is AppViewBase)
                            {
                                ((AppViewBase)viewItem).Focused = false;
                            }
                            break;
                        }
                }
            };*/

        }
    }
}
