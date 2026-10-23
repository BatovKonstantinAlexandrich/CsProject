

using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices
{

    [Label("Параметры визуализации")]
    public class StyledItemDesignerService : DesignerBase, IControlService<StyledItem>
    {
        public StyledItemDesignerService() { }
        public StyledItemDesignerService(StyledItem view)
        {
            this.Control = view;
        }

        public StyledItem Control { get; set; }

        public void SetControl(StyledItem item)
        {
            this.Control = item;
        }

        [Label("Установить значения контейнера")]
        public void SetParentStyle()
        {
            if(this.Control.Parent is not null)
            {
                var propertyNames = typeof(StyledItem).GetOwnPropertyNames();
                foreach (var property in propertyNames)
                {
                    var parentValue = this.Control.Parent.GetValue(property);
                    this.Info($"{property}={parentValue}");
                    this.Control.SetValue(property, parentValue);
                }
            }
        }


        [Label("Установить базовые значения")]
        public void SetDefaultStyle()
        {
            if (this.Control.Parent is not null)
            {
                var item = new StyledItem();
                foreach (var property in typeof(StyledItem).GetOwnPropertyNames())
                {
                    this.Control.SetValue(property, item.GetValue(property));
                }
            }
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