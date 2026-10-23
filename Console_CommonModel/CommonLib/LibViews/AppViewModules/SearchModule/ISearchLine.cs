using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule
{
    public interface ISearchLine
    {
        ChangingList<string> SearchOptions { get; set; }
        string SearchQuery { get; set; }
        bool ShowSearchOptions { get; set; }
        public Action<string> oninput { get; set; }
        public Action<string> onsearch { get; set; }

    }
}