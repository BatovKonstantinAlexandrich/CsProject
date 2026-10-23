using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{
    [Label("Панели инструментов")]
    public class PanelsViewDesignerService : DesignerBase, IControlService<PanelsView>
    {
        public PanelsViewDesignerService() { }
        public PanelsViewDesignerService(PanelsView view)
        {
            Control = view;
        }

        public PanelsView Control { get; set; }

        public void SetControl(PanelsView item)
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