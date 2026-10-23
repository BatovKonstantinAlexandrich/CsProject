using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule
{


    /// <summary>
    /// Модель представления поисковой строки
    /// </summary>
    [Label("Поисковая строка")]
    public class SearchView : AppViewBase, ISearchLine, ISearchView
    {

        [Label("Поисковый запрос")]
        public string SearchQuery { get; set; } = "";

        [Label("Значения автоподстановки")]
        public ChangingList<string> SearchOptions { get; } = new();

        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("onsearch")]
        public Action<string> onsearch { get; set; } = (p) => { };

        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("oninput")]
        public Action<string> oninput { get; set; } = (p) => { };
        [Label("Show Search Options")]
        public bool ShowSearchOptions { get; set; }

        public SearchView()
        {
            AutoSize = false;
            ShowSearchOptions = true;
            MinWidth = 240;
            MinHeight = 60;
            //Width = MinWidth;
            //Height = MinHeight;
            this.Height = this.Width = 100;
            this.WidthUnit = this.HeightUnit = "%";
            SearchQuery = "";
            oninput = (p) =>
            {
                this.Info(p);
                this.Info(SearchOptions.ToJsonOnScreen());
            };

        }
    }
}
