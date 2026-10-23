using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class TableComponent : ComponentBase<TableView, TableControl>
    {
        public TableComponent() : base(new(), new()) { }

        public TableComponent([NotNullNotEmpty] TableView ViewModel, [NotNullNotEmpty] TableControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var tableView = GetViewModel();
            var tableControl = GetControl();
            tableControl.SetModel(tableView.DataRows);
            tableControl.SetColumns(tableView.VisibleColumns);
            tableView.AddPropertyChangeListener(nameof(tableView.Editable), (evt) =>
            {
                tableControl.dataGridView.Enabled = (bool)evt.After;
            });
        }
    }
}
