

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.Others
{
    [Label("Элемент панели инструментов")]
    public abstract class ToolsViewItemBase : AppViewBase
    {

        [Label("Text")]
        public string Text { get; set; }

        [Label("Tool Type")]
        public ToolsViewItemType ToolType { get; set; }

        protected ToolsViewItemBase()
        {
        }
    }


}
