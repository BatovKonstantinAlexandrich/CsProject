using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.Controls
{
    [Icon("search")]
    [Label("Поисковый запрос")]
    public class SearchRequestModel : BaseEntity
    {
        [Icon("search")]
        [Label("Поисковое выражение")]
        [NotNullNotEmpty("Необходимо задать полисковое выражение")]
        public string query { get; set; }


        [Label("Номер страницы")]
        [NotNullNotEmpty("Необходимо указать номер страницы")]
        public string page { get; set; }


        [Label("Размер страницы")]
        [NotNullNotEmpty("Необходимо указать размер страницы")]
        public string size { get; set; }

        public static object FromDictionary(Dictionary<string, object> dictionary)
        {
            return ReflectionService.CopyValuesFromDictionary(new SearchRequestModel(), dictionary);
        }


    }
}
