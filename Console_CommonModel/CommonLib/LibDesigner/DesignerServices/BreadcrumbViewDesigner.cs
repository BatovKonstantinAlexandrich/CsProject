using Console_CommonModel.CommonLib.LibViews.AppViewModules.BreadcrumbModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{
    public class BreadcrumbViewDesigner : DesignerBase
    {
        public BreadcrumbViewDesigner(AppViewBase view) : base(view)
        {
        }

        protected override void Activating(ViewNode view)
        {
            if (view is BreadcrumbView bv)
            {
                bv.ContextMenu.AddContextMethods("Модель", new() {
                    { "добавить элемент", ()=>{
                        bv.Items.Add(new(){
                            Label = "Новый элемент"
                        });
                    } }
                });
            }
        }
    }
}
