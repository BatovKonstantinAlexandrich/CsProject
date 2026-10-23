using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.TreeViewServices
{
    /// <summary>
    /// Модель формы для создания TreeView
    /// </summary>
    [Label("Создание TreeView")]
    [Description("Форма для настройки и создания иерархического представления данных")]
    public class TreeViewCreateFormModel : ChangeSupport
    {

        [Label("Заголовок")]
        [NotNullNotEmpty("Необходимо указать заголовок")]
        [InputOrder(1)]
        public string Title { get => Get<string>(nameof(Title)); set => Set(nameof(Title), value); }


        [Label("Источник данных")]
        [NotNullNotEmpty("Необходимо выбрать источник данных")]
        [SelectDictionary($"{nameof(BusinessDatasource)},{nameof(BusinessDatasource.Name)}")]
        [InputOrder(2)]
        public int BusinessDatasourceId { get; set; }


        [Label("Источник данных")]
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public BusinessDatasource BusinessDatasource { get => Get<BusinessDatasource>(nameof(BusinessDatasource)); set => Set(nameof(BusinessDatasource), value); }


        [OnChange(nameof(BusinessDatasource))]
        public void OnBusinessDatasourceChanged(PropertyChangedMessage message)
        {
            try
            {
                var service = new BusinessDatasourceService();
                service.Setup(BusinessDatasource);
                var before = new List<string>(BusinessEntities);
                BusinessEntities.Clear();
                var tableNames = service.GetTableNames();
                BusinessEntities.AddRange(tableNames);
                OnChange(new()
                {
                    Property = nameof(BusinessEntities),
                    After = BusinessEntities,
                    Before = before
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение при OnBusinessDatasourceChanged() => {ex.Message}");
            }
        }


        [NotInput]
        public List<string> BusinessEntities { get; set; } = new();


        [Label("Свойство отображения")]
        [NotNullNotEmpty]
        [InputSelectExpression("BusinessEntities")]
        [UpdateWhenChanged()]
        public string TextExpression { get => Get<string>("TextExpression"); set => Set("TextExpression", value); }



        [Label("SQL запросы")]
        [InputOrder(4)]
        [NotInput]
        public ChangingList<SequentialQuery> Queries { get; set; } = new ChangingList<SequentialQuery>();

        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Connection { get => BusinessDatasource?.ConnectionString; }

        public TreeViewCreateFormModel()
        {
            Title = "Дерево данных";
            Queries = new ChangingList<SequentialQuery>();
            BusinessDatasource = new BusinessDatasource()
            {
                ConnectionString = $"Data Source=DESKTOP-IHJM9RD;Initial Catalog=DeliveryDbContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
            };
            TextExpression = "";
        }
    }
}



