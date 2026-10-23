using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[Label("Изображение")]
public class FocusModel: AppViewBase
{
    
    [Label("Now In Focus")]
    public ChangingDictionary<int, ViewItem> NowInFocus { get; set; } = new ChangingDictionary<int, ViewItem>();
    public FocusModel() { 
    }

    public void Add(ViewNode item)
    {
        NowInFocus[item.GetHashCode()] = (ViewItem)item;
    }
    public void Remove(ViewNode item)
    { 
        if(NowInFocus.ContainsKey(item.GetHashCode()))
        {
            NowInFocus.Remove(item.GetHashCode(), out var p);
        }
        
    }
}