using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{

    /// <summary>
    /// Свойства маршрутизации
    /// </summary>
    [Label("Маршруты в приложении")]
    public class AppRoute : BaseEntity
    {
        [Label("Маршрут")]
        [NotNullNotEmpty]
        [InputUrl]
        [UniqValue]
        public string Uri { get; set; }

        [Label("Страница")]
        [InputDictionary($"{nameof(AppPage)},{nameof(AppPage.Name)}")]
        public int? AppPageId { get; set; }

        [NotInput]
        public AppPage AppPage { get; set; }
    }

}
