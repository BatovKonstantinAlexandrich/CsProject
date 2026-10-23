namespace Console_CommonModel.CommonLib.LibViews.Others
{
    public class ToolsViewFactory : IToolsViewFactory
    {


        public ToolsItemLabel CreateToolItemLabel(string text, Action onclick = null)
        {
            throw new NotImplementedException();
        }

        public ToolsItemButtonView CreateToolItemButton(string text, string icon, Action todo = null)
        {
            var button = new ToolsItemButtonView() { Text = text };
            button.OnClick += (p, e) => todo();
            button.Icon = icon;
            button.OnItemClicked += todo;

            return button;
        }

        public ToolsItemPrograssView CreateToolItemProgressBar()
        {
            throw new NotImplementedException();
        }

        public ToolSItemSeparator CreateToolItemSeparator()
        {
            throw new NotImplementedException();
        }

        public ToolsItemButtonView CreateToolItemSplitButton(string text, Action onclick = null)
        {
            throw new NotImplementedException();
        }

        public ToolsItemTextboxView CreateToolItemTextBox(string text, Action onclick = null)
        {
            throw new NotImplementedException();
        }

        public ToolsView CreateToolItemToolsPane(List<object> services)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ToolsItemButtonView> CreateToolsPaneItems(List<object> services)
        {
            throw new NotImplementedException();
        }
    }
}
