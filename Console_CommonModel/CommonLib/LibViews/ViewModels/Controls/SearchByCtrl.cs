using System.Collections.Generic;

[Label("Поиск через контроллер")]
public class SearchByCtrl : BookView  
{
    
    [Label("Query")]
    public string Query { get => Get<string>("Query"); set => Set<string>("Query", value); }
    
    [Label("Options")]
    public List<string> Options { get; set; } = new List<string>();

    [Label("Asp Area")]
    
    public string AspArea { get; set; }
    [Label("Asp Controller")]
    
    public string AspController { get; set; }        
    [Label("Search Query")]
    
    public string SearchQuery { get; set; }    

    public string GetEndpointUrl()
    {
        if (string.IsNullOrEmpty(AspArea))
        {
            return "/" + AspController;
        }
        else
        {
            return "/"+ AspArea + "/" + AspController;
        }
    }


    public SearchByCtrl() : base()
    {
        Init();
        this.Query = "";
        this.Changed = false;
    }
}
