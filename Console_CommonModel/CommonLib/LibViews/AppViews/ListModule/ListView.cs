using System.Data;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibDesigner.DesignerServices;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[ViewUsageExample("var view = new ListView { Text = \"Представление списка\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<list-view Text=\"Представление списка\" Dock=\"Top\" />")]
[Label("Представление списка")]
public class ListView : ControlView, ILayoutViewNode, IListView
{
    private readonly ScrollView scrollView;


    [Label("Надпись")]
    [NotNullNotEmpty()]
    public override string Text { get => Get<string>("Text"); set => Set<string>("Text", value); }
    public string Icon { get => Get<string>("Icon"); set => Set<string>("Icon", value); }
    [InputBool]
    [Label("Включена/Выключена")]
    public override bool Enabled { get => Get<bool>("Enabled"); set => Set<bool>("Enabled", value); }


    [NotInput]
    [Label("Выбранный элемент")]
    public Option SelectedOption { get; set; }

    [Label("Свойства")]
    [InputStructureCollection<Option>()]
    public ChangingList<Option> Options { get; set; } = new() { };

    [Label("Действие при выборе")]
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<Option> OnSelected { get; set; }


    public ListView() : base()
    {
        this.Icon = "home";
        this.LayoutFill(this.scrollView = new ScrollView());
        this.Searchable = false;
        this.Options = new();
        this.OnSelected = (option) =>
        {
            this.SelectedOption = option;
        };
        this.ViewServices.Add(new ListViewDesignerService());
        this.ViewDesigner = new ListViewDesigner(this);
        this.MinHeight = 320;
        this.MinWidth = 320;
    }


    public override void Add(ViewNode item)
    {
        scrollView.Add(item);
        this.MinHeight = scrollView.Controls.Count() * 30;
    }

    public override void Remove(ViewNode item)
    {
        scrollView.Remove(item);
        this.MinHeight = scrollView.Controls.Count() * 30;
    }


    /// <summary>
    /// 
    /// </summary>    
    public override void OnResultSetChanged(PropertyChangedMessage message)
    {
        //this.DatasetId
        this.Info("Результирующий набор изменился " + message.After);
        ResultSetBindings = new()
        {
            { "Value", "Id" }
        };
        DataTable datatable = (DataTable)message.After;
        Options.Clear();
        foreach (DataRow row in datatable.Rows)
        {
            var option = new Option();
            var record = new Dictionary<string, object>();
            foreach (DataColumn column in datatable.Columns)
            {
                record[column.ColumnName] = row[column.ColumnName];
            }
            foreach (var kv in ResultSetBindings)
            {
                var value = Expressions.Compile(kv.Value, record);
                TextDataSetter.SetValue(option, kv.Key, value);
            }
            Options.Add(option);
        }



    }
}
