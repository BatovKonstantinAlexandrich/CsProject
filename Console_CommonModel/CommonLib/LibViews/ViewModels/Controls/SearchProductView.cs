using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Newtonsoft.Json;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[ViewUsageExample("var view = new SearchProductView { Text = \"Поиск товара\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<search-product-view Text=\"Поиск товара\" Dock=\"Top\" />")]
[Label("Поиск товара")]
public class SearchProductView: AppViewBase
{
    [Label("Query")]
    
    public string Query { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("At Input")]
    public Func<SearchProductView, object> AtInput { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("At Search")]
    public Func<SearchProductView, object> AtSearch { get; set; }

    public SearchProductView()
    {
        Changed = false;    
    }

    public object OnInput(string query)
    {
        this.Query = query;
        if (AtInput != null)
        {
            return AtInput(this);
        }
        return new { };
    }

    public object OnSearch(string query )
    {
        this.Query = query;
        if (AtSearch != null)
        {
            return AtSearch(this);
        }
        return new { };
    }


}

#if false
// public class SearchView<TProduct> : ViewItem, ISearchLine where TProduct: BaseEntity { ... }
#endif