using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{

    [Label("Параметры визуализации")]
    public class StyledItemDesignerService : DesignerBase, IControlService<StyledItem>
    {
        public StyledItemDesignerService() { }
        public StyledItemDesignerService(StyledItem view)
        {
            Control = view;
        }

        public StyledItem Control { get; set; }

        public void SetControl(StyledItem item)
        {
            Control = item;
        }

        [Label("Установить значения контейнера")]
        public void SetParentStyle()
        {
            if (Control.Parent is not null)
            {
                var propertyNames = typeof(StyledItem).GetOwnPropertyNames();
                foreach (var property in propertyNames)
                {
                    var parentValue = Control.Parent.GetValue(property);
                    this.Info($"{property}={parentValue}");
                    Control.SetValue(property, parentValue);
                }
            }
        }


        [Label("Установить базовые значения")]
        public void SetDefaultStyle()
        {
            if (Control.Parent is not null)
            {
                var item = new StyledItem();
                foreach (var property in typeof(StyledItem).GetOwnPropertyNames())
                {
                    Control.SetValue(property, item.GetValue(property));
                }
            }
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