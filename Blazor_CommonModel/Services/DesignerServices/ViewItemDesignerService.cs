

using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices
{

    [Label("Базовые параметры")]
    internal class ViewItemDesignerService : DesignerBase, IControlService<ViewItem>
    {
        public ViewItemDesignerService() { }
        public ViewItemDesignerService(ViewItem viewItem )
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
                var ctx = this.CreateDesignerContextView();
                if (ctx.Items.Count() == 0)
                    return;
                vb.ContextMenu.Items.Add(ctx);
                this.rollback.Add(() => { vb.ContextMenu.Items.Remove(ctx); });
            }
        }
    }
}