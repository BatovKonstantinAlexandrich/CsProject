namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class TableComponent1 : ComponentBase<TableView, DataTableControl>
    {
        public TableComponent1([NotNullNotEmpty] TableView ViewModel, [NotNullNotEmpty] DataTableControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
        }
    }
}
