using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.TabsModule
{
    [Label("Мастер настройки панели вкладок")]
    public class TabsViewDesigner : DesignerBase
    {
        public TabsViewDesigner() : base() { }

        public TabsViewDesigner(AppViewBase view) : base(view)
        {
        }

        protected override void Activating(ViewNode view)
        {
            var remove = ((AppViewBase)view).AddContextMethods("Мастер", new() {
                { "Настроить вкладки", ()=>{

                } }
            });
            rollback.Add(() =>
            {
                remove();
            });
        }
    }
}