using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{

    [Label("Создать представление")]
    public class ViewFactoryDesignerService : DesignerBase, IControlService<ViewFactory>
    {
        public ViewFactoryDesignerService() { }
        public ViewFactoryDesignerService(ViewFactory view)
        {
            Control = view;
        }

        public ViewFactory Control { get; set; }


        public void SetControl(ViewFactory item)
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