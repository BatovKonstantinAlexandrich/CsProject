

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule
{
    public interface ISearchControl: IControlBase, ISearchView
    {

    }
    public interface ISearchView
    {
        Action<string> oninput { get; set; }
        Action<string> onsearch { get; set; }
        ChangingList<string> SearchOptions { get; }
        string SearchQuery { get; set; }
        bool ShowSearchOptions { get; set; }
    }
}