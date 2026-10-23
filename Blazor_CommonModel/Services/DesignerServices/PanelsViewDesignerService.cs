

using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices
{
    [Label("Панели инструментов")]
    internal class PanelsViewDesignerService : DesignerBase, IControlService<PanelsView>
    {
        public PanelsViewDesignerService() { }
        public PanelsViewDesignerService(PanelsView view)
        {
            this.Control = view;
        }

        public PanelsView Control { get; set; }

        public void SetControl(PanelsView item)
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