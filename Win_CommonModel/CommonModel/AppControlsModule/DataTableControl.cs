using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.SharedViewCollections.DataTable;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControlsModule
{


    public class DataTableControl : ControlBase
    {
        private readonly DataGridView dataGridView;

        public Action<List<int>> OnRowsSelected = (ids) => { };

        public DataTableControl()
        {
            this.LayoutFill(dataGridView = new DataGridView());
            dataGridView.ScrollBars = ScrollBars.Vertical;
            this.BindMouseEvents(dataGridView);
            this.dataGridView.SelectionChanged += (s, e) => {
                List<int> indexes = new();
                foreach(DataGridViewRow row in this.dataGridView.SelectedRows)
                {
                    indexes.Add(row.Index);
                }
                this.OnRowsSelected(indexes);

            };
            this.AutoSize = true;
        }

        public void BindTableView(DataTableView view)
        {
            SetHeaders(view);
            SetDataRows(view);
            view.VisibleColumns.OnChanged += (s, e) =>
            {
                try
                {
                    this.Info(view.VisibleColumns);
                    dataGridView.SetDataRows(view.VisibleColumns, view.DataRows);
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            };
            view.DataRows.OnChanged += (s, e) =>
            {
                try
                {
                    this.Info(view.VisibleColumns);
                    dataGridView.SetDataRows(view.VisibleColumns, view.DataRows);
                }                
                catch(Exception ex)
                {
                    this.Error(ex);
                }
            };
        }

        public void ApplyDataTable(DataTableView view)
        {
            if (view is null)
                return;
            SetHeaders(view);
            SetDataRows(view);
        }

        public void ApplyReadOnly(bool readOnly, bool allowEdit)
        {
            dataGridView.ReadOnly = readOnly || !allowEdit;
            dataGridView.AllowUserToAddRows = !dataGridView.ReadOnly;
            dataGridView.AllowUserToDeleteRows = !dataGridView.ReadOnly;
        }

        public void ApplyAutoSizeColumns(bool autoSize)
        {
            dataGridView.AutoSizeColumnsMode = autoSize
                ? DataGridViewAutoSizeColumnsMode.Fill
                : DataGridViewAutoSizeColumnsMode.None;
        }

        public void ApplyVisibleColumns(DataTableView view)
        {
            if (view is null)
                return;
            dataGridView.SetHeaders(view.VisibleColumns);
            dataGridView.SetDataRows(view.DataRows);
        }

        public void RefreshBinding() => dataGridView.Refresh();

        public void BindTableView(TableView view)
        {
            if (view.DataRows is null)
                view.DataRows = new ChangingList<object>();
            if (view.VisibleColumns is null)
                view.VisibleColumns = new ChangingList<string>();
            ApplyTableView(view);
        }
        public void Bind(DataTableView view)
        {
            SetHeaders(view.VisibleColumns);
            SetDataRows(view.DataRows);
            view.VisibleColumns.OnChanged += (s, e) =>
            {
                
                dataGridView.SetDataRows(view.DataRows);
                dataGridView.SetHeaders(view.VisibleColumns);
            };
            view.DataRows.OnChanged += (s, e) =>
            {
                
                dataGridView.SetDataRows(view.DataRows);
                dataGridView.SetHeaders(view.VisibleColumns);
            };
        }

        public void Bind(ViewItem p)
        {
            dynamic view = p;
            var listp = new List<object>();
            foreach (var pp in view.DataRows)
            {
                listp.Add(pp);
            }
            dataGridView.SetHeaders((ChangingList<string>)view.VisibleColumns);
            dataGridView.SetDataRows(listp);
            ((ChangingList<string>)view.VisibleColumns).OnChanged += (s, e) =>
            {
                dataGridView.SetHeaders((ChangingList<string>)view.VisibleColumns);
                var list = new List<object>();
                foreach (var pp in view.DataRows)
                {
                    list.Add(pp);
                }
                dataGridView.SetDataRows(list);
            };
            ((IChangingListEvents)view.VisibleColumns).OnChanged += (s, e) =>
            {
                var list = new List<object>();
                foreach (var pp in view.DataRows)
                {
                    list.Add(pp);
                }
                dataGridView.SetHeaders((ChangingList<string>)view.VisibleColumns);
                dataGridView.SetDataRows(list);
            };
        }

        private void SetDataRows(DataTableView view)
        {
            dataGridView.SetDataRows(view.DataRows);
        }

        private void SetHeaders(DataTableView view)
        {
            dataGridView.SetHeaders(view.VisibleColumns);
        }

        public IEnumerable<string> SetHeaders<TModel>()
        {
            return dataGridView.SetHeaders<TModel>(typeof(TModel).GetOwnParameterNames());
        }
        public IEnumerable<string> SetHeaders(IEnumerable<string> columns)
        {
            return dataGridView.SetHeaders(columns);
        }
        public void SetDataRows(IEnumerable<object> data)
        {
            dataGridView.SetDataRows(Array.Empty<string>(), data ?? Array.Empty<object>());
        }

        public void SetDataRows(IEnumerable<string> columns, IEnumerable<object> data)
        {
            dataGridView.SetDataRows(columns ?? Array.Empty<string>(), data ?? Array.Empty<object>());
        }
        public void SetDataRows<T>(IEnumerable<T> data) where T : class
        {
            var names = typeof(T).GetOwnPropertyNames();
            dataGridView.SetHeaders<T>(names);
            var properties = typeof(T).GetOwnPropertyNames();
            dataGridView.SetDataRows(data);
        }

        /// <summary>
        /// Снимает выделение с строк грида (и через SelectionChanged обновляет привязку к TableView.DataRowsSelected).
        /// </summary>
        public void ClearRowSelection()
        {
            if (dataGridView.IsDisposed)
                return;
            dataGridView.ClearSelection();
        }

        public void ApplyTableView(TableView view)
        {
            if (view is null)
                return;

            var columns = view.VisibleColumns ?? new ChangingList<string>();
            var rows = view.DataRows ?? new ChangingList<object>();
            dataGridView.SetDataRows(columns, rows);
        }
    }
}
