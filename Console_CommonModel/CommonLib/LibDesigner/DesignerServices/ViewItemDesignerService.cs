using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{

    [Label("Базовые параметры")]
    public class ViewItemDesignerService : DesignerBase, IControlService<ViewItem>
    {
        public ViewItemDesignerService() { }
        public ViewItemDesignerService(ViewItem viewItem)
        {
        }

        public ViewItem Control { get; set; }

        public void SetControl(ViewItem item)
        {
        }

        protected override void Activating(ViewNode view)
        {
            if (view is AppViewBase vb)
            {
                var ctx = CreateDesignerContextView();
                if (ctx.Items.Count() == 0)
                    return;
                vb.ContextMenu.Items.Add(ctx);
                rollback.Add(() => { vb.ContextMenu.Items.Remove(ctx); });
            }
        }
    }
}