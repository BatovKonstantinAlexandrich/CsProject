using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Модель представления постраничной навигации, 
/// при изменении текущей страницы или размера страницы
/// выполняется метод OnPageChanged.
/// </summary>
[ViewUsageExample("var view = new PaginationView();\nview.Dock = \"Top\";\nparent.Add(view);", "<pagination-view Dock=\"Top\" />")]
[Label("Панель постраничной навигации")]
public class PaginationView: AppViewBase
{
    [Label("Total Pages")]
    [InputNumber]
    public int TotalPages { get; set; } = 1;
    [Label("Current Page")]
    [InputNumber]
    public int CurrentPage { get; set; } = 1;
    [Label("Page Size")]
    [InputNumber]
    public int PageSize { get; set; } = 10;
        
    public int PrevPage()
    {
        return CurrentPage - 1;
    }

    public int NextPage()
    {
        return CurrentPage + 1;
    }

    public void UpdatePages(int TotalResults)
    {
        TotalPages = (TotalResults % PageSize == 0) ? TotalResults / PageSize : (int)Math.Floor((decimal)(TotalResults / PageSize));
    }

    public PaginationView() {
        TotalPages = 10;
        CurrentPage = 3;
        PageSize = 10;
        MinWidth = 320;
        MinHeight = 44;
        Width = MinWidth;
        Height = MinHeight;
    }


}
