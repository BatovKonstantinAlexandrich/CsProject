using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule
{
    internal class MenuViewDesigner : DesignerBase
    {
        public MenuViewDesigner() : base() { }

        public MenuViewDesigner(AppViewBase view) : base(view)
        {
        }

        protected override void Activating(ViewNode view)
        {
            if (view is MenuView mv)
            {

            }
        }
    }
}