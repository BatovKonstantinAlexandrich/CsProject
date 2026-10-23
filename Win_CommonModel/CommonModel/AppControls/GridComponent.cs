public class GridComponent : ComponentBase<GridView, GridControl>
{
    public GridComponent([NotNullNotEmpty] GridView ViewModel, [NotNullNotEmpty] GridControl Control) : base(ViewModel, Control)
    {
    }

    public override void Bind()
    {
        var view = this.GetViewModel();
        var ctrl = this.GetControl();

        foreach(AppViewBase viewControl in view.Controls)
        {
            ctrl.Add(viewControl.ToControl());
        }
        view.Controls.OnAdded((i, p) => { ctrl.Add(((AppViewBase)p).ToControl()); });
        view.Controls.OnRemoved((i, p) => { ctrl.Remove(((AppViewBase)p).ToControl()); });
    }
}