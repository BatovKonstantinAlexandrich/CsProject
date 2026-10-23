using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable
{
    [Label("Мастер таблицы DataTable")]
    public class DataTableViewDesigner : DesignerBase
    {
        public DataTableViewDesigner() : base() { }

        public DataTableViewDesigner(AppViewBase view) : base(view)
        {
        }

        protected override void Activating(ViewNode view)
        {
        }
    }
}
