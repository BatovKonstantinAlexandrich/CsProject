using pickpoint_delivery_service;
using Console_CommonModel.CommonLib.LibDesigner.DesignerControls;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{


    [Label("Источник данных")]
    [Description("Контейнер представления с привязкой к источнику данных ADO")]
    [Icon("storage")]
    public class DatasourceView : AppViewBase
    {
        private readonly List<Action> _designModeRollback = new();
        [Label("Источник")]
        [InputDictionary($"{nameof(BusinessDatasource)},Name")]
        public override int? DatasourceId { get => Get<int?>("DatasourceId"); set => Set("DatasourceId", value); }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        [Label("Источник данных")]
        public override BusinessDatasource Datasource { get => Get<BusinessDatasource>("Datasource"); set => Set("Datasource", value); }

        public DatasourceView()
        {
            MinHeight = 320;
            MinWidth = 320;
            Width = MinWidth;
            Height = MinHeight;
            BorderWidth = 1;
            BackgroundColor = "#F8F9FB";
        }

        [OnChange(nameof(DatasourceId))]
        public void OnDatasourceIdChanged(PropertyChangedMessage message)
        {
            if (message.After is null)
            {
                Datasource = null;
                Changed = true;
                return;
            }

            var id = message.After.ToString().ToInt();
            if (id <= 0)
                return;

            try
            {
                Datasource = typeof(BusinessDatasource).GetEntityFasade<BusinessDatasource>().Get(id);
                Changed = true;
            }
            catch (Exception ex)
            {
                this.Error($"Не удалось загрузить источник данных: {ex.Message}");
            }
        }




        /// <summary>
        /// Службы дочерних представлений
        /// </summary>        
        public override IEnumerable<DesignerBase> GetLayoutDesignServices()
        {
            return new List<DesignerBase>()
            {
                new ViewItemDatasourceService(this)
            };
        }




        protected override void OnDesignModeChanged(PropertyChangedMessage message)
        {
            base.OnDesignModeChanged(message);

            var isDesignMode = message.After.ToString().ToBool();
            if (!isDesignMode)
            {
                _designModeRollback.ForEach(r => r());
                _designModeRollback.Clear();
                return;
            }

            try
            {
                using var db = new DeliveryDbContext();
                var datasourceSelectContextMenus = typeof(BusinessDatasource)
                    .GetEntityFasade<BusinessDatasource>()
                    .GetAll()
                    .Select(ds => new KeyValuePair<string, Action>(ds.Name, () => { DatasourceId = ds.Id; }))
                    .ToDictionary<string, Action>();

                _designModeRollback.Add(AddContextMethods("Источник данных", datasourceSelectContextMenus));
            }
            catch (Exception ex)
            {
                this.Error($"Не удалось загрузить список источников данных: {ex.Message}");
            }
        }
    }
}
