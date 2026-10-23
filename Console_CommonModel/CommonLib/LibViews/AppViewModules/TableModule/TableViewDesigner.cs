using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

public class TableViewDesigner : DesignerBase
{
    public TableViewDesigner() : base()
    {
    }

    public TableViewDesigner(AppViewBase view) : base(view)
    {
    }

    protected override void Activating(ViewNode view)
    {        
        TableView tableView = (TableView)view;
        
    }


    [Label("Мастер связывания данных")]
    public void CreateDbContextBindingMaster()
    {
        //var view = new NavDataView<DeliveryDbContext, BusinessDatasource, BusinessDataset>();
        //var submit = view.ToSubmit((s, p) => {
        //    if (s)
        //    {
        //        this.view.DatasetId = view.Selected.Id;
        //    }
        //});
        //submit.ShowDialog("Мастер связывания данных");
    }
}