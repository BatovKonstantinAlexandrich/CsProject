using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.Others
{

    [Label("Панель инструментов")]
    public class ToolsView : AppViewBase
    {
        public IToolsViewFactory toolsFactory = new ToolsViewFactory();

        public ChangingList<ToolsViewItemBase> tools = new();

        public ToolsView()
        {
            MinHeight = 25;
        }

        public ToolsItemLabel CreateToolItemLabel(string text, Action onclick = null)
        {
            var p = toolsFactory.CreateToolItemLabel(text, onclick);
            tools.Add(p);
            return p;
        }

        public ToolsItemButtonView CreateToolItemButton(string text, string icon, Action todo = null)
        {
            var p = toolsFactory.CreateToolItemButton(text, icon, todo);
            tools.Add(p);
            return p;
        }

        public ToolsItemPrograssView CreateToolItemProgressBar()
        {
            var p = toolsFactory.CreateToolItemProgressBar();
            tools.Add(p);
            return p;
        }

        public ToolSItemSeparator CreateToolItemSeparator()
        {
            var p = toolsFactory.CreateToolItemSeparator();
            tools.Add(p);
            return p;
        }

        public ToolsItemButtonView CreateToolItemSplitButton(string text, Action onclick = null)
        {
            var p = toolsFactory.CreateToolItemSplitButton(text, onclick);
            tools.Add(p);
            return p;
        }

        public ToolsItemTextboxView CreateToolItemTextBox(string text, Action onclick = null)
        {
            var p = toolsFactory.CreateToolItemTextBox(text, onclick);

            return p;
        }

        public ToolsView CreateToolItemToolsPane(List<object> services)
        {
            var p = toolsFactory.CreateToolItemToolsPane(services);

            return p;
        }

        public IEnumerable<ToolsItemButtonView> CreateToolsPaneItems(List<object> services)
        {
            var p = toolsFactory.CreateToolsPaneItems(services);
            tools.AddRange(p);
            return p;
        }
    }
}
