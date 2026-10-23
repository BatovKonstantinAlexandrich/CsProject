using Newtonsoft.Json.Linq;
using System.Data;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices
{
    /// <summary>
    /// Управление источником вне зависимости от СУБД.
    /// </summary>
    public class BusinessDatasourceService :
        ISqlExecutor, IDbMetadata, IDbMigBuilder, IWebApi
    {
        protected BusinessDatasource datasource;
        private ISqlExecutor executor;
        private IDbMetadata metadata;
        private IDbMigBuilder migbuilder;
        private IWebApi api;

        public BusinessDatasourceService()
        {
        }

        public void Setup(BusinessDatasource datasource)
        {
            this.datasource = datasource;
            executor = CreateProcedureExecuter();
            metadata = CreateDbMetadata();
            migbuilder = CreateDbMigBuilder();
            api = CreateWebApi();
        }

        public IWebApi CreateWebApi()
        {
            if (api is null)
            {
                lock (this)
                {
                    switch (datasource.ProviderDatabase)
                    {
                        case "MSSQL": var exec1 = new SqlServerWebApi(); api = exec1; exec1.SetConnectionString(datasource.ConnectionString); return exec1; break;
                        case "MySQL": var exec2 = new MySqlWebApi(); api = exec2; exec2.SetConnectionString(datasource.ConnectionString); return exec2; break;
                        case "Postgres": var exec3 = new PostgresWebApi(); api = exec3; exec3.SetConnectionString(datasource.ConnectionString); return exec3; break;
                        default: throw new NotImplementedException($"Не реализована поддержка ProviderDatabase={datasource.ProviderDatabase}");
                    }
                }
            }
            return api;
        }

        private IDbMigBuilder CreateDbMigBuilder()
        {
            switch (datasource.ProviderDatabase)
            {
                case "MSSQL": var exec1 = new SqlServerMigBuilder(); exec1.SetConnectionString(datasource.ConnectionString); return exec1; break;
                case "MySQL": var exec2 = new MySqlMigBuilder(); exec2.SetConnectionString(datasource.ConnectionString); return exec2; break;
                case "Postgres": var exec3 = new PostgresMigBuilder(); exec3.SetConnectionString(datasource.ConnectionString); return exec3; break;
                default: throw new NotImplementedException($"Не реализована поддержка ProviderDatabase={datasource.ProviderDatabase}");
            }
        }

        private IDbMetadata CreateDbMetadata()
        {

            switch (datasource.ProviderDatabase)
            {
                case "MSSQL": var exec1 = new SqlServerDbMetadata(); exec1.SetConnectionString(datasource.ConnectionString); return exec1; break;
                case "MySQL": var exec2 = new MySqlDbMetadata(); exec2.SetConnectionString(datasource.ConnectionString); return exec2; break;
                case "Postgres": var exec3 = new PostgresDbMetadata(); exec3.SetConnectionString(datasource.ConnectionString); return exec3; break;
                default: throw new NotImplementedException($"Не реализована поддержка ProviderDatabase={datasource.ProviderDatabase}");
            }
        }

        private ISqlExecutor CreateProcedureExecuter()
        {
            switch (datasource.ProviderDatabase)
            {
                case "MSSQL": var exec1 = new SqlServerExecutor(); exec1.SetConnectionString(datasource.ConnectionString); return exec1; break;
                case "MySQL": var exec2 = new MySqlExecutor(); exec2.SetConnectionString(datasource.ConnectionString); return exec2; break;
                case "Postgres": var exec3 = new PostgresExecutor(); exec3.SetConnectionString(datasource.ConnectionString); return exec3; break;
                default: throw new NotImplementedException($"Не реализована поддержка ProviderDatabase={datasource.ProviderDatabase}");
            }
        }

        public bool can(string url, Dictionary<string, string> queryParams, Dictionary<string, string> headers) => api.can(url, queryParams, headers);
        public Dictionary<string, string> GetNavigation(string url) => api.GetNavigation(url);
        public Dictionary<string, IFilter> GetQueryParams<TEntity>(string url) => api.GetQueryParams<TEntity>(url);
        public Task<Tuple<int, object>> Request(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null) => 
            api.Request(url, args, headers, body);
        public bool match(string url, Dictionary<string, string> queryParams) => api.match(url, queryParams);


        public List<BusinessDataset> GetDatasets(BusinessDatasource ds)
        {
            Setup(ds);
            var service = this;
            List<BusinessDataset> datasets = new();
            foreach (var kv in service.GetTablesMetadata())
            {
                datasets.Add(new BusinessDataset()
                {
                    Name = kv.Value.Fullname,
                    Label = kv.Value.TableLabel != "Надпись" ? kv.Value.TableLabel : kv.Value.TableName,
                    Description = service.CreateSelectStatement(kv.Value.TableSchema, kv.Value.TableName),
                    Expression = service.CreateSelectStatement(kv.Value.TableSchema, kv.Value.TableName),
                    ExpressionType = "TABLE",
                    DatasourceID = ds.Id
                });
            }
            foreach (var pr in service.GetProceduresMetadata().Values)
            {
                datasets.Add(new BusinessDataset()
                {
                    Name = pr.ProcedureName,
                    Label = pr.ProcedureLabel != "Надпись" ? pr.ProcedureLabel : pr.ProcedureName,
                    Description = pr.FullName,
                    Expression = $"EXEC {pr.FullName}",
                    ExpressionType = "PROCEDURE",
                    DatasourceID = ds.Id
                });
            }
            return datasets;
        }
        public JArray GetJsonResult(string sql)
        {
            return executor.GetJsonResult(sql);
        }

        public JObject GetSingleJObject(string command)
        {
            return executor.GetSingleJObject(command);
        }

        public DataTable ExecuteQuery(string command)
        {
            return executor.ExecuteQuery(command);
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
        {
            return executor.ExecuteQuery<TEntity>(command);
        }

        public IEnumerable<dynamic> ExecuteQuery(string command, Type entity)
        {
            return executor.ExecuteQuery(command, entity);
        }

        public int TryPrepareQuery(string command)
        {
            return executor.TryPrepareQuery(command);
        }

        public int PrepareQuery(string command)
        {
            return executor.PrepareQuery(command);
        }

        public DataTable ExecuteProcedure(string name, IDictionary<string, object> input)
        {
            return executor.ExecuteProcedure(name, input);
        }

        public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class
        {
            return executor.ExecuteProcedure<TResult>(name, input);
        }

        public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
        {
            return executor.ExecuteProcedure(command, input, output);
        }

        public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
        {
            return executor.ExecuteProcedure(command, input, output);
        }

        public IEnumerable<string> GetTableNames()
        {
            return metadata.GetTableNames();
        }

        public IEnumerable<string> GetSchemasNames()
        {
            return metadata.GetSchemasNames();
        }

        public IDictionary<string, TableMetadata> GetTablesMetadata()
        {
            return metadata.GetTablesMetadata();
        }

        public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName)
        {
            var r = metadata.GetColumnsMetadata(TableSchema, TableName);
            foreach (var v in r.Values)
            {
                v.EnsureIsValide();
            }
            return r;
        }

        public IEnumerable<KeyMetadata> GetKeysMetadata()
        {
            var r = metadata.GetKeysMetadata();
            foreach (var v in r)
            {
                v.EnsureIsValide();
            }
            return r;
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata()
        {
            
            var r = CreateDbMetadata().GetProceduresMetadata();
            foreach (var v in r)
            {
                v.EnsureIsValide();
            }
            return r;
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema)
        {

            var r = metadata.GetProceduresMetadata(Schema);
            foreach (var v in r)
            {
                v.EnsureIsValide();
            }
            return r;
        }

        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName)
        {

            var r = metadata.GetProcedureMetadata(SchemaName, ProcedureName);

            r.EnsureIsValide();

            return r;

        }

        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName)
        {
            var r = metadata.GetParametersMetadata(SchemaName, ProcedureName);
            foreach (var v in r)
            {
                v.EnsureIsValide();
            }
            return r;
        }

        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
        {
            var r = metadata.GetProceduresParametersMetadata();
            foreach (var v in r)
            {
                v.EnsureIsValide();
            }
            return r;
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            return metadata.GetDatabaseNames();
        }




        public void UpdateDatabase()
        {
            migbuilder.UpdateDatabase();
        }

        public IDbCommand CreateCommand<TIDbCommand>()
        {
            return executor.CreateCommand<TIDbCommand>();
        }

        public IDbCommand CreateCommand()
        {
            return executor.CreateCommand();
        }

        public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
        {
            return executor.GetResultSetDictionary(dataTable);
        }

        public void CreateDatabase()
        {
            migbuilder.CreateDatabase();
        }

        public int CreateDatabase(string name)
        {
            return migbuilder.CreateDatabase(name);
        }

        public string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null)
        {
            return migbuilder.CreateForeignkey(relativeTable, table, column, onDeleteCascade, onUpdateCascade);
        }

        public string CreateSelectStatement(string schema, string table)
        {
            return migbuilder.CreateSelectStatement(schema, table);
        }
        public string CreateInsertStatement(string schema, string table, Dictionary<string, object> values)
        {
            return migbuilder.CreateInsertStatement(schema, table, values);
        }

        public string CreateTable(TableInfo metadata)
        {
            return migbuilder.CreateTable(metadata);
        }

        public TableInfo CreateTableMetaData(Type metadata)
        {
            return migbuilder.CreateTableMetaData(metadata);
        }

        public DbMigCommand[] DropAndCreate()
        {
            return migbuilder.DropAndCreate();
        }

        public void TestProcedures()
        {
            migbuilder.TestProcedures();
        }

        public IDictionary<string, TableMetadata> UpdateTablesMetadata()
        {
            return migbuilder.UpdateTablesMetadata();
        }

        public ChangingList<Type> EntityTypes { get => migbuilder?.EntityTypes; }

        public Type[] GetEntityClasses()
        {
            return migbuilder.GetEntityClasses();
        }

        public void AddEntityType(Type entity)
        {
            migbuilder.AddEntityType(entity);
        }

        public string GetCurrentVersion()
        {
            return migbuilder.GetCurrentVersion();
        }

        public string GetTargetVersion()
        {
            return migbuilder.GetTargetVersion();
        }

        public IEntityFasade<TConvertableType> GetFasade<TConvertableType>(Type ptype) where TConvertableType : BaseEntity
        {
            return api.GetFasade<TConvertableType>(ptype);
        }

        public HashSet<IProcedureExecuter> Procedures { get => api?.Procedures; }
        public HashSet<IEntityFasade> Services { get => api?.Services; }

        public ITableManager GetFasade(string schema, string table)
        {
            return api.GetFasade(schema, table);
        }

        public IEntityFasade<T> GetFasade<T>() where T : BaseEntity
        {
            return api.GetFasade<T>();
        }

        public IDbMetadata GetModel()
        {
            return api.GetModel();
        }

        public void Init()
        {
            api.Init();
        }

        public void InitServices()
        {
            api.InitServices();
        }

        public string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values)
        {
            return migbuilder.CreateUpdateStatement(schema, table, values);
        }

        public string CreateCreateStatement(string schema, string table)
        {
            return migbuilder.CreateCreateStatement(schema, table);
        }
    }
}
