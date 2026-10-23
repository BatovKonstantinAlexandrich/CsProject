using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibViews.AppViews.TabsModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.TabsModule
{

    [Label("Панель вкладок")]
    public class TabsView : AppViewBase
    {
        [Label("Вкладки")]
        [InputCollectionDictionary()]
        public ChangingDictionary<string, AppViewBase> Tabs { get; set; }

        public TabsView()
        {
            Width = 300;
            Tabs = new();
            ViewDesigner = new TabsViewDesigner(this);
        }

        public void AddTab(string key, AppViewBase view)
        {
            view.Parent = this;
            Tabs.Add(key, view);
        }

        public void RemoveTab(string key, AppViewBase view)
        {
            Tabs.Remove(key);
        }

    }
}
