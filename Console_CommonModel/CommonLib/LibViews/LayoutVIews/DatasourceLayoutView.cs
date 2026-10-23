using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using System.Data;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.MetadataServices;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;
using Console_CommonModel.CommonLib.LibPages;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;

namespace Console_CommonModel.CommonLib.LibViews.LayoutVIews
{
    [Label("Представление интегрированное с источником данных")]
    public class DatasourceLayoutView : AppViewBase
    {
        protected DatasourceLayoutService layoutViewService;

        public DatasourceLayoutView()
        {
            layoutViewService = new DatasourceLayoutService();
            layoutViewService.SetControl(this);
        }

        public int? GetDatasourceId() => layoutViewService.Control.DatasourceId;
        public BusinessDatasource GetDatasource() => layoutViewService.Control.Datasource;
        public int? GetDatasetId() => layoutViewService.Control.DatasetId;
        public BusinessDataset GetDataset() => layoutViewService.Control.Dataset;
        public string GetExpression() => layoutViewService.Control.Dataset?.Expression;
        public DataTable GetAdoDataTable() => layoutViewService.Control.ResultSet;
        public DataTableView GetTableView() => new DataTableView(layoutViewService.Control.ResultSet);
        public ListView SelectDataset()
        {
            var listView = CreateSelectList(
                "Наборы данных",
                GetAvailableBusinessDatasets().ToList(),
                dataset => DatasetId = dataset.Id
            );
            return listView;
        }


    }







    [Label("Представление интегрированное с источником данных")]
    public class DbContextLayoutView<TDbContextBase> : DatasourceLayoutView
        where TDbContextBase : DbContextBase
    {
        public TDbContextBase GetDbContext()
            => typeof(TDbContextBase).New<TDbContextBase>();

        public DbContextLayoutView() : base()
        {

            //DbContextInitiliazer<ModelsDbContext>.CreateDatabase();
            ValidateRegistration();
            using (var delivery = new DeliveryDbContext())
            using (var db = GetService<TDbContextBase>())
            {
                var ds = db.GetBusinessDatasource();
                var p = delivery.BusinessDatasources.FirstOrDefault(ds => ds.ConnectionString == ds.ConnectionString);
                layoutViewService.Control.DatasourceId = p.Id;
            }
        }

        public void RefreshDatabase()
        {
            using (var db = GetService<TDbContextBase>())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        public void ValidateRegistration()
        {
            this.InfoTime(nameof(ValidateRegistration), () =>
            {
                try
                {
                    using (var delivery = new DeliveryDbContext())
                    using (var db = GetService<TDbContextBase>())
                    {
                        var ds = db.GetBusinessDatasource();
                        if (!delivery.BusinessDatasources.Any(ds => ds.ConnectionString == ds.ConnectionString))
                        {
                            delivery.BusinessDatasources.Add(ds);
                            delivery.SaveChanges();
                        }
                        var dss = new BusinessDatasourceService();
                        dss.Setup(ds);


                        var dmss = new DatabaseModelService();
                        dmss.SaveModel(ds);
                    }
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при выполнении {nameof(ValidateRegistration)} => {ex.Message}");
                    
                }
            });
        }



        /// <param name="context">параметры которые не нужно вводить</param>                
        public ProcedureExecuteView CreateExecuteDialog(Dictionary<string, object> context, string SchemaName, string ProcedureName, Action<DataTable> onsuccess)
        {
            using (var db = GetService<TDbContextBase>())
            {
                var metadata = db.GetProcedureMetadata(SchemaName, ProcedureName);
                var execute = new ProcedureExecuteView();
                execute.SetModel(new DeliveryDbContext(), metadata, dataTable =>
                {
                    onsuccess(dataTable);
                    execute.ToControl().GetAppForm().Close();
                }, context);
                return execute;
            }


        }


    }
}
