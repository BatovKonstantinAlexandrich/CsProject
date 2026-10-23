namespace WinFormsApp.SharedViewControls
{
    public partial class TableViewControl : UserControl
    {        
        public TableViewControl()
        {
            InitializeComponent();
        }

        public void SetData<T>(ITableView<T> view)
        {
            this.dataGridView1.SetDataRows<T>(view.DataRows);
        }

    }
}
