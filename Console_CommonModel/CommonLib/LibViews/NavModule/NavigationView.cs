using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.Dictionary;
using Console_CommonModel.CommonLib.Others;

namespace Console_CommonModel.CommonLib.LibViews.NavModule
{

    /// <summary>
    /// Представление модели навигации
    /// </summary>
    [Label("Представление меню навигации")]
    public class NavigationView : ListView, INavigationManager<AppViewBase>
    {
        public event EventHandler<ChangingDictionaryEvent<string, Task<AppViewBase>>> PageChanged;
        private NavigationView viewNavigation;
        private readonly AppViewBase viewPage;

        public NavigationView()

        {

            Options.Clear();
            PageChanged = (sender, evt) => { };
            //viewNavigation = new NavigationView(viewPage = new AppViewBase());
            // viewNavigation.AddChangedHandler(OnPagesChanged);
            OnSelected += p => { OnNavigationStarted($"{p}"); };

            //this.pages.OnChanged += OnPageChanged;
        }




        public void OnPagesChanged(object sender, ChangingDictionaryEvent<string, Task<AppViewBase>> evt)
        {
            switch (evt.Target.Action)
            {
                case "added": break;
                case "removed": break;
                case "updated": break;
            }
            PageChanged(sender, evt);
        }


        private void OnNavigationStarted(string state)
        {
            Navigate(state);
        }

        /*private void OnPageChanged(object sender, DictionaryChangesEvent<string, Task<ViewBase>> evt)
        {                            
            this.PageChanged(this,evt);
        }*/

        public void RemovePage(string state)
        {
            viewNavigation.RemovePage(state);
        }

        public void AddPage(string state, AppViewBase panel)
        {
            viewNavigation.AddPage(state, panel);
        }

        public void AddPage(string state, Task<AppViewBase> panel)
        {
            viewNavigation.AddPage(state, panel);
        }

        public IEnumerable<AppViewBase> GetNavComponents()
        {
            return viewNavigation.GetNavComponents();
        }

        public List<string> GetNavOptions()
        {
            return viewNavigation.GetNavOptions();
        }

        public AppViewBase Navigate(string state)
        {
            return viewNavigation.Navigate(state);
        }

        public void Refresh()
        {
            viewNavigation.Refresh();
        }
    }
}
