using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Newtonsoft.Json.Linq;

using System.Collections.Concurrent;
using System.Text.Json.Serialization;
[ViewUsageExample("var view = new ListObjectsView { Text = \"Набор данных\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<list-objects-view Text=\"Набор данных\" Dock=\"Top\" />")]
[Label("Набор данных")]
public class ListObjectsView: AppViewBase
{

    
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    [Label("Type")]
    public Type Type { get; set; }

    
    [NotInput]
    [Label("Items")]
    public List<object> Items { get; set; } = new();

    
    [NotInput]
    [Label("Dataset")]
    public JArray Dataset { get; set; }
    
[Label("Заголовок")]
    public string Title { get => Get<string>("Title"); set => Set<string>("Title", value); }

    
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    [Label("Actions")]
    public ConcurrentDictionary<string, Action<ListView>> Actions { get; set; } = new ConcurrentDictionary<string, Action<ListView>>();

    
    [NotInput]
    [Label("Bindings")]
    public Dictionary<string, string> Bindings { get; set; }

    public ListObjectsView() : base()
    {

        //Dataset = new OdbcDataSource().CleverExecute("select * from Persons").DataSet;
        Selectable = true;       
        Title = "List"; 
        Bindings = new Dictionary<string, string>() {
            { "Title", "Tel" }
        };
        
    }     
}