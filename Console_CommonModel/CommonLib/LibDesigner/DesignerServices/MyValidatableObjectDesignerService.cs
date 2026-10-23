using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{

    [Label("Валидация")]
    public class MyValidatableObjectDesignerService : DesignerBase, IControlService<MyValidatableObject>
    {
        public MyValidatableObject Control { get; set; }


        public MyValidatableObjectDesignerService()
        {
        }


        public MyValidatableObjectDesignerService(MyValidatableObject view)
        {
            SetControl(Control = view);
        }


        public void SetControl(MyValidatableObject view)
        {
            Control = view;
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