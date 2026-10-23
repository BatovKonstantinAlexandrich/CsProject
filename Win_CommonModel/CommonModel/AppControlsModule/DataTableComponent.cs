using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.DesignerServices;
using WinFormsApp.SharedViewCollections.DataTable;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class TableComponent2 : ComponentBase<TableView, DataTableControl>
    {
        private ChangingList<object>? _wiredRows;
        private ChangingList<string>? _wiredColumns;

        public TableComponent2([NotNullNotEmpty] TableView ViewModel, [NotNullNotEmpty] DataTableControl Control) : base(ViewModel, Control)
        {
        }

        public override List<DesignerBase> GetViewServices()
        {
            var list = base.GetViewServices();
            list.Add(new TableViewDesigner(GetViewModel()));
            list.Add(new DataTableViewDesigner(GetViewModel()));
            return list;
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            WireTableView(view);

            ctrl.OnRowsSelected = ids =>
            {
                view.DataRowsSelected.Clear();
                if (view.DataRows is null)
                    return;
                foreach (var id in ids)
                {
                    if (id < 0 || id >= view.DataRows.Count)
                        continue;
                    view.DataRowsSelected.Add(view.DataRows[id]);
                }
            };

            view.AddPropertyChangeListener(nameof(TableView.DataRows), _ => WireTableView(view));
            view.AddPropertyChangeListener(nameof(TableView.VisibleColumns), _ => WireTableView(view));
        }

        private void WireTableView(TableView view)
        {
            UnwireTableView();
            if (view.DataRows is null)
                view.DataRows = new ChangingList<object>();
            if (view.VisibleColumns is null)
                view.VisibleColumns = new ChangingList<string>();

            _wiredRows = view.DataRows;
            _wiredColumns = view.VisibleColumns;
            _wiredRows.OnChanged += OnTableDataChanged;
            _wiredColumns.OnChanged += OnTableDataChanged;
            GetControl().ApplyTableView(view);
        }

        private void UnwireTableView()
        {
            if (_wiredRows is not null)
                _wiredRows.OnChanged -= OnTableDataChanged;
            if (_wiredColumns is not null)
                _wiredColumns.OnChanged -= OnTableDataChanged;
            _wiredRows = null;
            _wiredColumns = null;
        }

        private void OnTableDataChanged(object? sender, IChangingListEvent e) =>
            GetControl().ApplyTableView(GetViewModel());
    }





    public class DataTableComponent : ComponentBase<DataTableView, DataTableControl>
    {
        private System.Data.DataTable? _wiredTable;

        public DataTableComponent([NotNullNotEmpty] DataTableView viewModel, [NotNullNotEmpty] DataTableControl control)
            : base(viewModel, control)
        {
        }
        public override List<DesignerBase> GetViewServices()
        {
            var list = base.GetViewServices();
            list.Add(new TableViewDesigner(GetViewModel()));
            list.Add(new DataTableViewDesigner(GetViewModel()));
            return list;
        }
        public override void Bind()
        {
            /*var view = GetViewModel();
            var ctrl = GetControl();

            void ApplyAll()
            {
                ctrl.ApplyDataTable(view);
                WireTableEvents(view.Data);
            }

            ApplyAll();

            view.AddPropertyChangeListener(nameof(DataTableView.Data), _ => RefreshFromTable());
            view.AddPropertyChangeListener(nameof(DataTableView.ReadOnly), _ =>
                ctrl.ApplyReadOnly(view.ReadOnly, view.AllowEdit));
            view.AddPropertyChangeListener(nameof(DataTableView.AllowEdit), _ =>
                ctrl.ApplyReadOnly(view.ReadOnly, view.AllowEdit));
            view.AddPropertyChangeListener(nameof(DataTableView.AutoSizeColumns), _ =>
                ctrl.ApplyAutoSizeColumns(view.AutoSizeColumns));

            view.VisibleColumns.OnChanged += (_, _) => ctrl.ApplyVisibleColumns(view);

            ctrl.OnRowsSelected = indexes => view.SyncSelectionFromRowIndexes(indexes);

            var factory = new ComponentsFactory();
            factory.Bind(view, ctrl);
            factory.BindEvents(view, ctrl);
            factory.BindControlEvents(view, ctrl);
            factory.BindContextMenu(view, ctrl);*/
        }

        private void WireTableEvents(System.Data.DataTable? table)
        {
            UnwireTableEvents();
            _wiredTable = table;
            if (table is null)
                return;

            table.ColumnChanged += OnTableChanged;
            table.RowChanged += OnTableChanged;
            table.RowDeleted += OnTableChanged;
            table.TableCleared += OnTableChanged;
            table.Columns.CollectionChanged += OnTableCollectionChanged;
        }

        private void UnwireTableEvents()
        {
            if (_wiredTable is null)
                return;

            _wiredTable.ColumnChanged -= OnTableChanged;
            _wiredTable.RowChanged -= OnTableChanged;
            _wiredTable.RowDeleted -= OnTableChanged;
            _wiredTable.TableCleared -= OnTableChanged;
            _wiredTable.Columns.CollectionChanged -= OnTableCollectionChanged;
            _wiredTable = null;
        }

        private void OnTableChanged(object? sender, EventArgs e) => RefreshFromTable();

        private void OnTableCollectionChanged(object? sender, System.ComponentModel.CollectionChangeEventArgs e) =>
            RefreshFromTable();

        private void RefreshFromTable()
        {
            var view = GetViewModel();
            var ctrl = GetControl();
            view.RunOnUiThread(() =>
            {
                view.SyncVisibleColumnsFromData();
                view.SyncDataRowsFromData();
                ctrl.RefreshBinding();
                ctrl.ApplyVisibleColumns(view);
            });
        }
    }
}
