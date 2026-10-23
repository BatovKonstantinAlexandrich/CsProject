using Microsoft.Extensions.Configuration;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
[ViewUsageExample("var view = new TableView { Text = \"Таблица данных\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<table-view Text=\"Таблица данных\" Dock=\"Top\" />")]
[Label("Таблица данных")]
[Description("Представление таблицы данных для отображения коллекций объектов в табличном формате с настраиваемыми колонками, сортировкой и фильтрацией")]
public class TableView : AppViewBase 
{
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [Label("type")]
    public Type type { get; set; }
    [Label("Title")]
    public string Title { get => Get<string>("Title"); set => Set<string>("Title", value); }

    [Label("Data Rows Selected")]
    public ChangingList<object> DataRowsSelected { get; set; } = new();


    public List<string> Columns
    {
        get => Get<List<string>>("Columns"); 
        set
        {
            Set<IList<string>>("Columns", value);
        }
    }
    [Label("Visible Columns")]
    public ChangingList<string> VisibleColumns { get => Get<ChangingList<string>>("VisibleColumns"); set { Set<ChangingList<string>>("VisibleColumns", value); if(OnVisibleColumnsChanged is not null) OnVisibleColumnsChanged(VisibleColumns, DataRows);  } }

    [Label("Data Rows")]
    public ChangingList<object> DataRows
    {
        get => _dataRows;
        set
        {
            if (_dataRows is not null)
                _dataRows.OnChanged -= OnDataRowsChanged;
            _dataRows = value ?? new ChangingList<object>();
            _dataRows.OnChanged += OnDataRowsChanged;
        }
    }

    private ChangingList<object> _dataRows;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [Label("Cells")]
    public List<List<object>> Cells { get; set; } = new List<List<object>>();
    
    [Label("Is Primitive")]
    [InputBool]
    public bool IsPrimitive { get; set; } = false;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [Label("On Visible Columns Changed")]
    public Action<ChangingList<string>, ChangingList<object>> OnVisibleColumnsChanged { get; set; }
    [Label("On Selected")]
    public Action<object, object> OnSelected { get; internal set; }
    [Label("Current Row")]
    public object CurrentRow { get; set; }

    public TableView( ) : base()
    {
        this.Init();
        this.VisibleColumns = new ChangingList<string>();
        this.MinWidth = 360;
        this.MinHeight = 360;
        ClassList.Add("front-pane");
        type = this.GetType();
        this.Title = "Таблица";
        this.Columns = new List<string>();
        this._dataRows = new ChangingList<object>();
        this.DataRows.OnChanged += OnDataRowsChanged;
        this.Cells = new();
        this.ViewDesigner = new TableViewDesigner(this);
        this.OnVisibleColumnsChanged  += (s, e) => {
            //Save(this.VisibleColumns.ToJson());
        };
        
        this.AddPropertyChangeListener(nameof(VisibleColumns), msg => {
            this.OnVisibleColumnsChanged(VisibleColumns, DataRows);
        });
        this.VisibleColumns.OnChanged += (s, e) => {
            this.OnVisibleColumnsChanged(VisibleColumns, DataRows);
        };
    }


    public void SetDataRows(IEnumerable<object> data)
    {
        var pdata = data.FirstOrDefault();
        if(pdata is not null)
        {
            this.VisibleColumns.Clear();
            this.VisibleColumns.AddRange(pdata.GetType().GetOwnInputSimplePropertyNames());            
        }
        this.SetDataRows(this.VisibleColumns, data);
    }


    public void SetDataRows(IEnumerable<string> columns, IEnumerable<object> data)
    {
        this.VisibleColumns.Clear();
        this.VisibleColumns.AddRange(columns);
        this.DataRows.Clear();
        this.DataRows.AddRange(data);
    }


    private void OnDataRowsChanged(object sender, IChangingListEvent e)
    {
        
    }

 
    public void SetVisibleColumns(IEnumerable<string> columns)
    {
        this.VisibleColumns.Clear();
        this.VisibleColumns.AddRange(columns);
    }

    public void SetDataRows(List<string> columns, IEnumerable<object> values)
    {
        this.Info(new { columns= columns, values = values });
        this.DataRows.Clear();
        this.DataRows.AddRange(values.ToList());
        this.VisibleColumns.Clear();
        this.VisibleColumns.AddRange(columns);
    }

    public void UpdateDatarows()
    {
        this.SetDataRows(this.VisibleColumns.ToList(), this.DataRows.ToList());
    }


}