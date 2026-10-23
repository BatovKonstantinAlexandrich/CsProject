using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

public class ListViewDesigner : DesignerBase
{
    public ListViewDesigner() : base() { }

    public ListViewDesigner(AppViewBase view) : base(view)
    {
    }

    protected override void Activating(ViewNode view)
    {
        if(view is ListView list)
        {
            rollback.Add(list.AddContextMethods("связать с данными", new Dictionary<string, Action>() { 

            }));
        }
    }
}