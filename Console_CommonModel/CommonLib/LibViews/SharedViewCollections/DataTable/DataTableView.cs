using System.Linq;

using DataTableModel = System.Data.DataTable;
using DataRowModel = System.Data.DataRow;
using DataColumnModel = System.Data.DataColumn;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable
{
    /// <summary>
    /// Табличное представление: источник — <see cref="DataTable"/>, отображение — <see cref="DataTableControl"/>.
    /// </summary>
    [Label("Таблица DataTable")]
    [Description("Принимает System.Data.DataTable и отображает его в DataGridView с синхронизацией свойств.")]
    public class DataTableView : AppViewBase
    {
        [Label("Данные")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DataTableModel Data
        {
            get => Get<DataTableModel>(nameof(Data));
            set => Set(nameof(Data), value);
        }

        [Label("Видимые колонки")]
        [Description("Пустой список — показывать все колонки таблицы.")]
        public ChangingList<string> VisibleColumns { get; set; } = new();

        [Label("Строки (кэш для совместимости)")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ChangingList<object> DataRows { get; set; } = new();

        [Label("Выделенные строки")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ChangingList<object> DataRowsSelected { get; set; } = new();

        [Label("Только чтение")]
        public bool ReadOnly
        {
            get => Get<bool>(nameof(ReadOnly));
            set => Set(nameof(ReadOnly), value);
        }

        [Label("Разрешить редактирование ячеек")]
        public bool AllowEdit
        {
            get => Get<bool>(nameof(AllowEdit));
            set => Set(nameof(AllowEdit), value);
        }

        [Label("Автоширина колонок")]
        public bool AutoSizeColumns
        {
            get => Get<bool>(nameof(AutoSizeColumns));
            set => Set(nameof(AutoSizeColumns), value);
        }

        public DataTableView(DataTableModel data) : this()
        {
            if (data is not null)
            {
                SetData(data);
            }
        }

        public DataTableView()
        {
            ReadOnly = true;
            AllowEdit = false;
            AutoSizeColumns = true;
            Dock = "Fill";
            MinWidth = 200;
            MinHeight = 120;
            ViewDesigner = new DataTableViewDesigner(this);
            Data = new DataTableModel();
            AddPropertyChangeListener(nameof(Data), OnDataPropertyChanged);
            VisibleColumns.OnChanged += (_, _) => OnVisibleColumnsChanged();
        }

        /// <summary>Установить таблицу и обновить список колонок.</summary>
        public void SetData(DataTableModel? data)
        {
            Data = data;
        }

        public void SetVisibleColumns(IEnumerable<string> columns)
        {
            VisibleColumns.Clear();
            VisibleColumns.AddRange(columns);
        }

        private void OnDataPropertyChanged(PropertyChangedMessage message)
        {
            SyncVisibleColumnsFromData();
            SyncDataRowsFromData();
        }

        private void OnVisibleColumnsChanged()
        {
            // обновление грида выполняется в DataTableComponent
        }

        internal void SyncVisibleColumnsFromData()
        {
            if (Data is null)
                return;

            var names = new List<string>();
            foreach (DataColumnModel col in Data.Columns)
                names.Add(col.ColumnName);
            if (VisibleColumns.Count == 0)
            {
                VisibleColumns.AddRange(names);
                return;
            }

            var toRemove = VisibleColumns.Where(c => !names.Contains(c)).ToList();
            foreach (var col in toRemove)
                VisibleColumns.Remove(col);

            foreach (var name in names)
            {
                if (!VisibleColumns.Contains(name))
                    VisibleColumns.Add(name);
            }
        }

        internal void SyncDataRowsFromData()
        {
            DataRows.Clear();
            if (Data is null)
                return;

            foreach (DataRowModel row in Data.Rows)
            {
                if (row.RowState == System.Data.DataRowState.Deleted)
                    continue;
                DataRows.Add(row);
            }
        }

        internal void SyncSelectionFromRowIndexes(IEnumerable<int> rowIndexes)
        {
            DataRowsSelected.Clear();
            if (Data is null)
                return;

            foreach (var index in rowIndexes)
            {
                if (index >= 0 && index < Data.Rows.Count)
                {
                    var row = Data.Rows[index];
                    if (row.RowState != System.Data.DataRowState.Deleted)
                        DataRowsSelected.Add(row);
                }
            }
        }
    }
}
