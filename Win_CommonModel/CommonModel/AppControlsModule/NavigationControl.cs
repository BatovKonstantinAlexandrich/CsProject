using MaterialSkin.Controls;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppNavModule;
using WinFormsApp.AppViews.ListModule;

namespace WinFormsApp.AppComponents.AppControlsModule
{

    /// <summary>
    /// Список навигации
    /// </summary>
    public partial class NavigationControl : ControlBase, INavigationManager
    {
        private MaterialListView materialListView;
        public ListViewControl list;
        private readonly ControlNavigation navigationManager;
        public Panel viewPanel;

        public NavigationControl()
        {
            navigationManager = new ControlNavigation(viewPanel = new Panel());
            this.LayoutFill(materialListView = new MaterialListView());
        }

        public Control GetContentControl()
        {
            return navigationManager.panel;
        }



        public void AddPage(string state, Control panel)
        {
            ((INavigationManager)navigationManager).AddPage(state, panel);
        }

        public void AddPage(string state, Task<Control> panel)
        {
            ((INavigationManager)navigationManager).AddPage(state, panel);
        }

        public IEnumerable<Control> GetNavComponents()
        {
            return ((INavigationManager)navigationManager).GetNavComponents();
        }

        public List<string> GetNavOptions()
        {
            return ((INavigationManager)navigationManager).GetNavOptions();
        }

        public Control Navigate(string state)
        {
            return ((INavigationManager)navigationManager).Navigate(state);
        }

        public void RemovePage(string state)
        {
            ((INavigationManager)navigationManager).RemovePage(state);
        }

        internal void SetList(ListViewControl list)
        {
            this.LayoutFill(this.list = list);
        }
    }
}
