

using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices
{

    [Label("Создать представление")]
    public class ViewFactoryDesignerService : DesignerBase, IControlService<ViewFactory>
    {
        public ViewFactoryDesignerService() { }
        public ViewFactoryDesignerService(ViewFactory view)
        {
            this.Control = view;
        }

        public ViewFactory Control { get; set; }

        
        public void SetControl(ViewFactory item)
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