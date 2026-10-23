using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.SplitModule
{
    [Label("Мастер панели с разделителем")]
    public class SplitViewDesigner : DesignerBase
    {
        public SplitViewDesigner() : base() { }

        public SplitViewDesigner(AppViewBase view) : base(view)
        {
        }

        protected override void Activating(ViewNode view)
        {
        }
    }
}
