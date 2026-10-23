using Console_CommonModel.CommonLib.LibViews.Others;

namespace Console_CommonModel.CommonLib.LibViews
{
    public interface IToolsViewFactory
    {

        ToolsItemLabel CreateToolItemLabel(string text, Action onclick = null);
        ToolsItemButtonView CreateToolItemButton(string text, string icon = null, Action todo = null);
        ToolsItemPrograssView CreateToolItemProgressBar();
        ToolSItemSeparator CreateToolItemSeparator();
        ToolsItemButtonView CreateToolItemSplitButton(string text, Action onclick = null);
        ToolsItemTextboxView CreateToolItemTextBox(string text, Action onclick = null);
        ToolsView CreateToolItemToolsPane(List<object> services);
        IEnumerable<ToolsItemButtonView> CreateToolsPaneItems(List<object> services);
    }
}
