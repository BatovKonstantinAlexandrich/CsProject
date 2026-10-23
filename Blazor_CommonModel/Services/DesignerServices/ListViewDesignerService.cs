using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.AppViews.ListModule;

namespace WinFormsApp.DesignerServices;

/// <summary>Сервис дизайнера для <see cref="ListView"/> (контекстное меню задаётся в <see cref="ListViewDesigner"/>).</summary>
[Label("Настройка списка")]
internal class ListViewDesignerService : DesignerBase, IControlService<ListView>
{
    public ListView Control { get; set; }

    public ListViewDesignerService()
    {
    }
    public ListViewDesignerService(ListView item)
    {
        SetControl(item);
    }
    public void SetControl(ListView item)
    {
        Control = item;
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
