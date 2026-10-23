using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Label("Поиск по базе данных")]
public class SearchDatabase: AppViewBase
{
    [Label("Query")]
    
    public string Query { get; set; }
    [Label("Endpoint")]
    
    public string Endpoint { get; set; }
    public string GetEndpointUrl()
    {
        return Endpoint;
    }
}