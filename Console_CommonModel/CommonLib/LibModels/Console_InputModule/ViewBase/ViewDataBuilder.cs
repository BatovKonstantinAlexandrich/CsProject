using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;

using Mysqlx.Expr;

using Newtonsoft.Json.Linq;

using pickpoint_delivery_service;

using System.Data;



using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOServices;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;

/// <summary>
/// Реализует функцию связывания преждставления с источником данных и сквозными событиями приложения.
/// </summary>
[Label("Конструктор интерфейса")]
public class ViewDataBuilder : ViewContextMenuBuilder, ISqlExecutor, IDbMetadata, IDbMigBuilder, IWebApi
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    private bool _isDatasetLoading;

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    private int? _lastLoadedDatasetId;

    public string GetLabel() => this.GetType().GetLabel();

    [Label("Корневой тег компонента представления")]
    public string Tag { get; set; }


    [Label("Источник")]
    [InputDbContextDictionary<DeliveryDbContext>($"{nameof(BusinessDatasource)},Name")]
    public virtual int? DatasourceId { get => Get<int?>("DatasourceId"); set => Set<int?>("DatasourceId", value); }


    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]
    [Label("Источник данных")]
    public virtual BusinessDatasource Datasource { get => Get<BusinessDatasource>("Datasource"); set => Set<BusinessDatasource>("Datasource", value); }


    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]
    [Label("Набор данных")]
    public BusinessDataset Dataset { get; set; }

    [Label("Набор данных")]
    [InputDictionary("BusinessDataset,Name")]
    public int? DatasetId { get => Get<int?>("DatasetId"); set=>Set<int?>("DatasetId", value); }

    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public DataTable ResultSet { get => Get<DataTable>("ResultSet"); set=>Set<DataTable>("ResultSet", value); }

    /// <summary>
    /// Связывание с типом модели элемента представления
    /// Для списка выбора, у которого элемент Option имеет свойство Value = { {"Value","Name"} };
    /// </summary>
    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public ChangingDictionary<string, string> ResultSetBindings = new ChangingDictionary<string, string>();


    /// <summary>
    /// При загрузки представления, EventsBindings генерируют слушатели событий операциями 
    /// компиляции выражений.
    /// </summary>
    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Dictionary<string, ChangingList<string>> EventsBindings = new Dictionary<string, ChangingList<string>>();

    /// <summary>
    /// Параметры однонаправленного внизходящегно связывания
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    public Dictionary<string, string> InterpolationBindings = new Dictionary<string, string>();


    /// <summary>
    /// Параметры восходящего канала передачи событий
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]

    public Dictionary<string, string> OutputBindings = new Dictionary<string, string>();

    /// <summary>
    /// Параметры связывания 
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    public ChangingDictionary<string, string> Bindings = new ChangingDictionary<string, string>();

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    private BusinessDatasourceService DatasourceService { get; set; } = new BusinessDatasourceService();
   

    public ViewDataBuilder() : base()
    {
        this.DatasourceId = null;
        this.DatasetId = null;
        this.Datasource = null;
        this.ContextMenuBuilder = new(this.ContextMenu);
        this.ResultSet = null;
        this.ResultSetBindings.OnChanged += (p, e) => {
            this.OnResultSetChanged(new()
            {
                After = this.ResultSetBindings
            });
        };
        this.OnLoad += (s, e) => {
            this.CompileEventsBindings();
        };
    }


    public IEnumerable<BusinessDataset> GetAvailableBusinessDatasets()
    {
        if (this.Datasource is null)
            return new List<BusinessDataset>();
        var dbs = new BusinessDatasourceService();
        dbs.Setup(this.Datasource);

        using (var db = new DeliveryDbContext())
        {
            var datasets = dbs.GetDatasets(this.Datasource);
            var newDatasets = datasets.Where(ds => db.BusinessDatasets.Any(bd => bd.Expression != ds.Expression)).ToList();
            foreach (var dataset in newDatasets)
                dataset.DatasourceID = (int)this.DatasourceId;
            db.BusinessDatasets.AddRange(newDatasets);
            db.SaveChanges();

            var dsid = (int)this.DatasourceId;

            Datasource.Datasets = datasets;
            return Datasource.Datasets;
        }
    }



    [Label("Метод компиляции выражений из EventsBindings в операторы действий")]
    public void CompileEventsBindings()
    {
        //this.Info("CompileEventsBindings()");

        foreach(var kv in EventsBindings)
        {
            this.Info(kv.Key);
            var expression = kv.Value;
            var paction = this.GetProperty(kv.Key);
            if(paction is Action<object,EventArgs> a1)
            {
                
                foreach(var exp in expression)
                {
                    var actionName = "Add" + kv.Key;
                    Action<object, EventArgs> todo = (s, e) => {
                        try
                        {
                            this.Compile(exp);
                        }
                        catch (Exception ex)
                        {
                            this.Error($"Исключение при выполнении выражения по событию: {kv.Key} => {kv.Value} => {ex.Message}");
                        }
                    };
                    this.Invoke(actionName, new object[] { todo });
                }
            }
        }
    }


    public IEnumerable<TModel> GetResultset<TModel>()
        where TModel: class
    {
        return ResultSet is null ? null : 
            new DataTableService().GetResultSet<TModel>(ResultSet);
    }

    public IEnumerable<HierTable<TModel>> GetHierTableResultset<TModel>()
        where TModel : BaseEntity
    {
        return ResultSet is null ? null :
            new DataTableService().GetResultSet<HierTable<TModel>>(ResultSet);
    }



    /// <summary>
    /// Создание представления иерархии из Resultset
    /// </summary>
    public TreeControlView GetTreeControlViewResult<TModel>(Action<HierTable<TModel>> onclicked)
        where TModel : BaseEntity
    {
        IEnumerable<HierTable<TModel>> results = this.GetHierTableResultset<TModel>();
        var factory = new TreeFactory();        
        var tree = factory.CreateTree(results, item => new() { 
            Title  = item.Name            
            }, 
            p => results.Where(r => r.ParentId == p.Id).ToList(),
            onclicked);
        return tree;
    }


    [OnChange("ResultSet")]
    public virtual void OnResultSetChanged(PropertyChangedMessage message)
    {
        //this.Info("Результирующий набор изменился");
        if (message.After is not null && message.After is DataTable table)
        {
            //this.Info($"Результирующий набор изменился: rows={table.Rows.Count}, cols={table.Columns.Count}");
            foreach (DataRow row in table.Rows)
            {                
                var record = new Dictionary<string, object>();
                foreach (DataColumn column in ResultSet.Columns)
                {
                    record[column.ColumnName] = row[column.ColumnName];
                }
                foreach (var kv in ResultSetBindings)
                {
                    var value = Expressions.Compile(kv.Value, record);
                    TextDataSetter.SetValue(this, kv.Key, value);
                }                
            }            
        }
            
    }


    //[OnChange("Datasource")]
    public virtual void OnDatasourceChanged(PropertyChangedMessage message)
    {
      
        if (this.Datasource is not null)
        {            
            this.DatasourceService.Setup(this.Datasource);
        }
       
    }





    [OnChange("DatasourceId")]
    public virtual void OnDatasourceIdChanged(PropertyChangedMessage message)
    {
        
        int? did = message.After is null? null: $"{message.After}".ToInt();
        if (did is not null)
        {
            using (var db = new DeliveryDbContext())
            {
                var founded = db.BusinessDatasources.FirstOrDefault(ds => ds.Id == did);
                if (founded is not null)
                {
                    this.Datasource = founded;
                }
            }
        }                
    }


    //[OnChange("DatasetId")]
    public virtual void OnDatasetChanged(PropertyChangedMessage message)
    {
        try
        {
            if (message.After is not null)
            {
                var datasetId = message.After.ToString().ToInt();
                if (_isDatasetLoading)
                    return;
                if (_lastLoadedDatasetId == datasetId)
                    return;

                var datasetFasade = typeof(BusinessDataset).GetEntityFasade<DeliveryDbContext, BusinessDataset>();

                if (message.After is null || String.IsNullOrWhiteSpace(message.After.ToString()))
                {
                    return;
                }

                _isDatasetLoading = true;
                var dataset = datasetFasade.Get(datasetId);
                var datasource = typeof(BusinessDatasource).GetEntityFasade<BusinessDatasource>().Get(dataset.BusinessDatasourceId);
                var odbc = new SqlServerADOServiceConnectionString(datasource.ConnectionString);
                this.ResultSet = odbc.ExecuteQuery(dataset.Expression);
                _lastLoadedDatasetId = datasetId;
            }
        }
        catch (Exception ex)
        {
            this.Error($"Ошибка при получении данных из источника. {ex.Message}");
        }
        finally
        {
            _isDatasetLoading = false;
        }
    }



    public List<string> GetDatasourceNames()
        => typeof(BusinessDatasource).GetEntityFasade<BusinessDatasource>().GetAll().Select(ds => ds.Name).ToList();

    public List<string> GetDatasetNames()
        => typeof(BusinessDatasource).GetEntityFasade<BusinessDataset>().GetAll().Select(ds => ds.Name).ToList();


    /// <summary>
    /// Выполнение выражения из бизнес набора данных по DatasetId
    /// и преобразование его в заданный тип.
    /// </summary>    
    public TEntity GetSingleResult<TEntity>() where TEntity: class
    {
        try
        {
            if (this.DatasetId is not null)
                this.Dataset = (BusinessDataset)typeof(BusinessDataset).GetEntityFasade<BusinessDataset>().Get((int)this.DatasetId);
            var exp = this.Dataset.GetProperty("Expression");
            TEntity result = null;
            if (exp is not null)
            {
                var datasource = (BusinessDatasource)typeof(BusinessDatasource).GetEntityFasade<BusinessDatasource>().Get(((BusinessDataset)this.Dataset).BusinessDatasourceId);

                var api = new SqlServerWebApi();
                api.Configure(datasource.ConnectionString);
                var results = api.ExecuteQuery<TEntity>(exp.ToString());
                result = results.FirstOrDefault();
            }
            if (result is null)
                throw new Exception($"Набор данных задан неверно {DatasetId}={exp}");
            return result;
        }
        catch(Exception ex)
        {
            this.Error($"Исключение при получении и преобразовании данных из BusinessDataset {this.DatasetId} => {ex.Message}");
            throw;
        }
    }


    /// <summary>
    /// Выполнение выражения из бизнес набора данных по DatasetId
    /// и преобразование его в коллекцию заданного типа.
    /// </summary>    
    public ChangingList<TEntity> GetMultiEntityFromDataset<TEntity>() where TEntity : class
    {
        try
        {
            if (this.DatasetId is not null)
                this.Dataset = (BusinessDataset)typeof(BusinessDataset).GetEntityFasade<BusinessDataset>().Get((int)this.DatasetId);
            var exp = this.Dataset.GetProperty("Expression");
            ChangingList<TEntity> result = null;
            if (exp is not null)
            {
                var datasource = (BusinessDatasource)typeof(BusinessDatasource).GetEntityFasade<BusinessDatasource>().Get(((BusinessDataset)this.Dataset).BusinessDatasourceId);
                
                var api = new SqlServerWebApi();
                api.Configure(datasource.ConnectionString);
                var results = api.ExecuteQuery<TEntity>(exp.ToString());
                result = new ChangingList<TEntity>(results);
            }
            if (result is null)
                throw new Exception($"Набор данных задан неверно {DatasetId}={exp}");
            return result;
        }
        catch (Exception ex)
        {
            this.Error($"Исключение при получении и преобразовании данных из BusinessDataset {this.DatasetId} => {ex.Message}");
            throw;
        }
    }
     
    public ComponentParams ToComponentParams()
    {
        var result = new ComponentParams();
        result.Tag = this.Tag = this.GetTypeName().ToKebabStyle("-");
        result.Attrs = new();
        foreach(var kv in this.DataState)
        {            
            result.Attrs[kv.Key] = $"{kv.Value}";
        }
        return result;
    }

    public object Compile()
    {
        foreach (var p in Bindings)
        {
            string expression = p.Value;
            if (expression.IndexOf("{{") != -1)
            {
                string interpolated = Interpolate(expression);
                Setter.SetValue(this, p.Key, interpolated);
            }
            else
            {
                object value = GetBindedValue(expression, this);
                Setter.SetValue(this, p.Key, value);
            }
        }
        return this;
    }

    public object Compile(Dictionary<string, string> bindings)
    {
        Bindings = new(bindings);
        return Compile();
    }
    public object Compile(string value, JToken jvalue)
    {
        foreach (var p in Bindings)
        {
            string expression = p.Value;
            if (expression.IndexOf("{{") != -1)
            {
                string interpolated = Interpolate(expression);
                Setter.SetValue(this, p.Key, interpolated);
            }
            else
            {
                object v = GetBindedValue(expression, this);
                Setter.SetValue(this, p.Key, v);
            }



        }
        return this;
    }

    private object GetBindedValue(string value, object data)
    {
        if (ReflectionService.GetPropertyNames(data.GetType()).Contains(value))
        {
            return data.GetType().GetProperty(value).GetValue(data);
        }
        else
        {
            return Expressions.Compile(value, data);
        }

    }

    object this[string path]
    {
        get
        {
            return this;
        }
        set
        {

        }
    }


    public string Interpolate(string expression)
    {
        return Expressions.Interpolate(expression, this);
    }

    public object Interpolate()
    {

        foreach (var p in InterpolationBindings)
        {

            object value = GetBindedValue(p.Value, this);
            Setter.SetValue(this, p.Key.ToCapitalStyle(), value);

        }
        return this;
    }

    

    public void SelectListView(string title, IEnumerable<string> options, Action<string> onclick)
    {
        this.Controls.Clear();

        var list = new global::ListSelectionView();
        foreach(var option in options)
        {
            list.ListItems.Add(new ListItemView()
            {
                Title = option
            });
        }

        this.Controls.Add(list);
    }

    public virtual bool can(string url, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
    {
        return ((IWebApi)DatasourceService).can(url, queryParams, headers);
    }

    public virtual bool match(string url, Dictionary<string, string> queryParams)
    {
        return ((IWebApi)DatasourceService).match(url, queryParams);
    }

    public virtual Dictionary<string, string> GetNavigation(string url)
    {
        return ((IWebApi)DatasourceService).GetNavigation(url);
    }

    public virtual Dictionary<string, IFilter> GetQueryParams<TEntity>(string url)
    {
        return ((IWebApi)DatasourceService).GetQueryParams<TEntity>(url);
    }

    public virtual IEntityFasade<TConvertableType> GetFasade<TConvertableType>(Type ptype) where TConvertableType : BaseEntity
    {
        return ((IWebApi)DatasourceService).GetFasade<TConvertableType>(ptype);
    }

    public virtual  Task<Tuple<int, object>> Request(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null)
    {
        return ((IWebApi)DatasourceService).Request(url, args, headers, body);
    }

    public HashSet<IProcedureExecuter> Procedures { get => ((IWebApi)DatasourceService).Procedures;}
    public HashSet<IEntityFasade> Services { get => ((IWebApi)DatasourceService).Services;  }

    public ITableManager GetFasade(string schema, string table)
    {
        return ((IWebApi)DatasourceService).GetFasade(schema, table);
    }

    public IEntityFasade<T> GetFasade<T>() where T : BaseEntity
    {
        return ((IWebApi)DatasourceService).GetFasade<T>();
    }

    public IDbMetadata GetModel()
    {
        return ((IWebApi)DatasourceService).GetModel();
    }

    public void InitServices()
    {
        ((IWebApi)DatasourceService).InitServices();
    }

    public void CreateDatabase()
    {
        ((IDbMigBuilder)DatasourceService).CreateDatabase();
    }

    public int CreateDatabase(string name)
    {
        return ((IDbMigBuilder)DatasourceService).CreateDatabase(name);
    }

    public string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null)
    {
        return ((IDbMigBuilder)DatasourceService).CreateForeignkey(relativeTable, table, column, onDeleteCascade, onUpdateCascade);
    }

    public string CreateInsertStatement(string schema, string table, Dictionary<string, object> values)
    {
        return ((IDbMigBuilder)DatasourceService).CreateInsertStatement(schema, table, values);
    }

    public string CreateSelectStatement(string schema, string table)
    {
        return ((IDbMigBuilder)DatasourceService).CreateSelectStatement(schema, table);
    }

    public string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values)
    {
        return ((IDbMigBuilder)DatasourceService).CreateUpdateStatement(schema, table, values);
    }

    public string CreateCreateStatement(string schema, string table)
    {
        return ((IDbMigBuilder)DatasourceService).CreateCreateStatement(schema, table);
    }

    public string CreateTable(TableInfo metadata)
    {
        return ((IDbMigBuilder)DatasourceService).CreateTable(metadata);
    }

    public TableInfo CreateTableMetaData(Type metadata)
    {
        return ((IDbMigBuilder)DatasourceService).CreateTableMetaData(metadata);
    }

    public DbMigCommand[] DropAndCreate()
    {
        return ((IDbMigBuilder)DatasourceService).DropAndCreate();
    }

    public void TestProcedures()
    {
        ((IDbMigBuilder)DatasourceService).TestProcedures();
    }

    public void UpdateDatabase()
    {
        ((IDbMigBuilder)DatasourceService).UpdateDatabase();
    }

    public IDictionary<string, TableMetadata> UpdateTablesMetadata()
    {
        return ((IDbMigBuilder)DatasourceService).UpdateTablesMetadata();
    }

    public ChangingList<Type> EntityTypes { get => ((IdbModel)DatasourceService).EntityTypes; }

    public Type[] GetEntityClasses()
    {
        return ((IdbModel)DatasourceService).GetEntityClasses();
    }

    public void AddEntityType(Type entity)
    {
        ((IdbModel)DatasourceService).AddEntityType(entity);
    }

    public string GetCurrentVersion()
    {
        return ((IdbModel)DatasourceService).GetCurrentVersion();
    }

    public string GetTargetVersion()
    {
        return ((IdbModel)DatasourceService).GetTargetVersion();
    }

    public IDbCommand CreateCommand<TIDbCommand>()
    {
        return ((ISqlExecutor)DatasourceService).CreateCommand<TIDbCommand>();
    }

    public IDbCommand CreateCommand()
    {
        return ((ISqlExecutor)DatasourceService).CreateCommand();
    }

    public JArray GetJsonResult(string sql)
    {
        return ((ISqlExecutor)DatasourceService).GetJsonResult(sql);
    }

    public JObject GetSingleJObject(string command)
    {
        return ((ISqlExecutor)DatasourceService).GetSingleJObject(command);
    }

    public DataTable ExecuteQuery(string command)
    {
        return ((ISqlExecutor)DatasourceService).ExecuteQuery(command);
    }

    public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
    {
        return ((ISqlExecutor)DatasourceService).ExecuteQuery<TEntity>(command);
    }

    public IEnumerable<dynamic> ExecuteQuery(string command, Type entity)
    {
        return ((ISqlExecutor)DatasourceService).ExecuteQuery(command, entity);
    }

    public int TryPrepareQuery(string command)
    {
        return ((ISqlExecutor)DatasourceService).TryPrepareQuery(command);
    }

    public int PrepareQuery(string command)
    {
        return ((ISqlExecutor)DatasourceService).PrepareQuery(command);
    }

    public DataTable ExecuteProcedure(string name, IDictionary<string, object> input)
    {
        return ((ISqlExecutor)DatasourceService).ExecuteProcedure(name, input);
    }

    public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class
    {
        return ((ISqlExecutor)DatasourceService).ExecuteProcedure<TResult>(name, input);
    }

    public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
    {
        return ((ISqlExecutor)DatasourceService).GetResultSetDictionary(dataTable);
    }

    public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
    {
        return ((ISqlExecutor)DatasourceService).ExecuteProcedure(command, input, output);
    }

    public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
    {
        return ((ISqlExecutor)DatasourceService).ExecuteProcedure(command, input, output);
    }

    public IEnumerable<string> GetTableNames()
    {
        return ((IDbMetadata)DatasourceService).GetTableNames();
    }

    public IEnumerable<string> GetDatabaseNames()
    {
        return ((IDbMetadata)DatasourceService).GetDatabaseNames();
    }

    public IEnumerable<string> GetSchemasNames()
    {
        return ((IDbMetadata)DatasourceService).GetSchemasNames();
    }

    public IDictionary<string, TableMetadata> GetTablesMetadata()
    {
        return ((IDbMetadata)DatasourceService).GetTablesMetadata();
    }

    public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName)
    {
        return ((IDbMetadata)DatasourceService).GetColumnsMetadata(TableSchema, TableName);
    }

    public IEnumerable<KeyMetadata> GetKeysMetadata()
    {
        return ((IDbMetadata)DatasourceService).GetKeysMetadata();
    }

    public IDictionary<string, ProcedureMetadata> GetProceduresMetadata()
    {
        return ((IDbMetadata)DatasourceService).GetProceduresMetadata();
    }

    public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema)
    {
        return ((IDbMetadata)DatasourceService).GetProceduresMetadata(Schema);
    }

    public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName)
    {
        return ((IDbMetadata)DatasourceService).GetProcedureMetadata(SchemaName, ProcedureName);
    }

    public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
    {
        return ((IDbMetadata)DatasourceService).GetProceduresParametersMetadata();
    }

    public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName)
    {
        return ((IDbMetadata)DatasourceService).GetParametersMetadata(SchemaName, ProcedureName);
    }
}

