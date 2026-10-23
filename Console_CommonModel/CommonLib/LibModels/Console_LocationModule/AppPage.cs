using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using static Console_CommonModel.CommonLib.LibModels.Console_LocationModule.LocationDbContext;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{

    /// <summary>
    /// Содержимое страницы
    /// </summary>
    [Label("Содержимое страницы")]
    public class AppPage : DescriptableObject
    {
        [Label("Модель данных JSON")]

        [InputOrder(2)]
        public string PageModel { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        [InputOrder(3)]
        [Label("Компоненты")]
        [InputStructureCollection]
        public ChangingList<PageComponent> PageComponents { get; set; } = new ChangingList<PageComponent>();

        [NotMapped()]
        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        [NotInput]
        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new();

        [Label("Тип модель данных")]

        [NotNullNotEmpty()]
        public string ModelType { get; set; }

        [Label("Ссылка на модель")]
        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        [NotMapped]
        [NotInput]
        public object ModelInstance { get; set; }
    }

}
