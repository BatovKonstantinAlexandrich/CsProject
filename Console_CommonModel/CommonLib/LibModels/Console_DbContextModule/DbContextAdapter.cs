

using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.MetadataServices;


using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibModels.Console_DbContextModule
{
    public class DbContextAdapter : ISqlExecutor, IDbMetadata, IDbMigBuilder, IDDLFactory, IDMLFactory,
        IDbConnectionsService, IDataModelService, ITreeService, IComponent//, IDbInterface, IDbContextBase
    //IDbContextBase
    {
        private IDataModelService dataModelService = new DataModelService();
        private readonly DbContext dbContext;
        private readonly SqlServerWebApi webApi;
        public TreeService treeService = new();

        public DbContextAdapter(DbContext dbContext)
        {
            this.dbContext = dbContext;
            webApi = new SqlServerWebApi();
            webApi.Configure(this.dbContext.Database.GetConnectionString());
        }

        public string CreateInsertStatement(string schema, string table, Dictionary<string, object> values)
        {
            return ((IDMLFactory)webApi).CreateInsertStatement(schema, table, values);
        }

        public string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values)
        {
            return ((IDMLFactory)webApi).CreateUpdateStatement(schema, table, values);
        }

        public string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable)
        {
            return ((IDDLFactory)webApi).AddColumnToTable(fullTableName, propertyName, type, nullable);
        }

        public string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null)
        {
            return ((IDDLFactory)webApi).CreateForeignkey(relativeTable, table, column, onDeleteCascade, onUpdateCascade);
        }

        public string CreateForeignKeys(TableInfo metadata)
        {
            return ((IDDLFactory)webApi).CreateForeignKeys(metadata);
        }

        public string CreateSchema(string schema)
        {
            return ((IDDLFactory)webApi).CreateSchema(schema);
        }

        public string CreateSelectStatement(string schema, string table)
        {
            return ((IDDLFactory)webApi).CreateSelectStatement(schema, table);
        }

        public string CreateTable(TableInfo metadata)
        {
            return ((IDDLFactory)webApi).CreateTable(metadata);
        }

        public string CreateTable(Type metadata)
        {
            return ((IDDLFactory)webApi).CreateTable(metadata);
        }

        public TableInfo CreateTableMetaData(Type typeofEntity)
        {
            return ((IDDLFactory)webApi).CreateTableMetaData(typeofEntity);
        }

        public string DropColumnFromTable(string fullTableName, string propertyName)
        {
            return ((IDDLFactory)webApi).DropColumnFromTable(fullTableName, propertyName);
        }

        public string DropTable(TableInfo metadata)
        {
            return ((IDDLFactory)webApi).DropTable(metadata);
        }

        public void CreateDatabase()
        {
            ((IDbMigBuilder)webApi).CreateDatabase();
        }

        public int CreateDatabase(string name)
        {
            return ((IDbMigBuilder)webApi).CreateDatabase(name);
        }

        public string CreateCreateStatement(string schema, string table)
        {
            return ((IDbMigBuilder)webApi).CreateCreateStatement(schema, table);
        }

        public DbMigCommand[] DropAndCreate()
        {
            return ((IDbMigBuilder)webApi).DropAndCreate();
        }

        public void TestProcedures()
        {
            ((IDbMigBuilder)webApi).TestProcedures();
        }

        public void UpdateDatabase()
        {
            ((IDbMigBuilder)webApi).UpdateDatabase();
        }

        public IDictionary<string, TableMetadata> UpdateTablesMetadata()
        {
            return ((IDbMigBuilder)webApi).UpdateTablesMetadata();
        }

        public ChangingList<Type> EntityTypes { get => ((IdbModel)webApi).EntityTypes; }

        public Type[] GetEntityClasses()
        {
            return ((IdbModel)webApi).GetEntityClasses();
        }

        public void AddEntityType(Type entity)
        {
            ((IdbModel)webApi).AddEntityType(entity);
        }

        public string GetCurrentVersion()
        {
            return ((IdbModel)webApi).GetCurrentVersion();
        }

        public string GetTargetVersion()
        {
            return ((IdbModel)webApi).GetTargetVersion();
        }

        public IDbCommand CreateCommand<TIDbCommand>()
        {
            return ((ISqlExecutor)webApi).CreateCommand<TIDbCommand>();
        }

        public IDbCommand CreateCommand()
        {
            return ((ISqlExecutor)webApi).CreateCommand();
        }

        public JArray GetJsonResult(string sql)
        {
            return ((ISqlExecutor)webApi).GetJsonResult(sql);
        }

        public JObject GetSingleJObject(string command)
        {
            return ((ISqlExecutor)webApi).GetSingleJObject(command);
        }

        public DataTable ExecuteQuery(string command)
        {
            return ((ISqlExecutor)webApi).ExecuteQuery(command);
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
        {
            return ((ISqlExecutor)webApi).ExecuteQuery<TEntity>(command);
        }

        public IEnumerable<dynamic> ExecuteQuery(string command, Type entity)
        {
            return ((ISqlExecutor)webApi).ExecuteQuery(command, entity);
        }

        public int TryPrepareQuery(string command)
        {
            return ((ISqlExecutor)webApi).TryPrepareQuery(command);
        }

        public int PrepareQuery(string command)
        {
            return ((ISqlExecutor)webApi).PrepareQuery(command);
        }

        public DataTable ExecuteProcedure(string name, IDictionary<string, object> input)
        {
            return ((ISqlExecutor)webApi).ExecuteProcedure(name, input);
        }

        public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class
        {
            return ((ISqlExecutor)webApi).ExecuteProcedure<TResult>(name, input);
        }

        public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
        {
            return ((ISqlExecutor)webApi).GetResultSetDictionary(dataTable);
        }

        public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
        {
            return ((ISqlExecutor)webApi).ExecuteProcedure(command, input, output);
        }

        public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
        {
            return ((ISqlExecutor)webApi).ExecuteProcedure(command, input, output);
        }

        public IEnumerable<string> GetTableNames()
        {
            return ((IDbMetadata)webApi).GetTableNames();
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            return ((IDbMetadata)webApi).GetDatabaseNames();
        }

        public IEnumerable<string> GetSchemasNames()
        {
            return ((IDbMetadata)webApi).GetSchemasNames();
        }

        public IDictionary<string, TableMetadata> GetTablesMetadata()
        {
            return ((IDbMetadata)webApi).GetTablesMetadata();
        }

        public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName)
        {
            return ((IDbMetadata)webApi).GetColumnsMetadata(TableSchema, TableName);
        }

        public IEnumerable<KeyMetadata> GetKeysMetadata()
        {
            return ((IDbMetadata)webApi).GetKeysMetadata();
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata()
        {
            return ((IDbMetadata)webApi).GetProceduresMetadata();
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema)
        {
            return ((IDbMetadata)webApi).GetProceduresMetadata(Schema);
        }

        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName)
        {
            return ((IDbMetadata)webApi).GetProcedureMetadata(SchemaName, ProcedureName);
        }

        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
        {
            return ((IDbMetadata)webApi).GetProceduresParametersMetadata();
        }

        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName)
        {
            return ((IDbMetadata)webApi).GetParametersMetadata(SchemaName, ProcedureName);
        }

        public DatabaseModel GetDatabase(string connectionString)
        {
            return ((IDbConnectionsService)webApi).GetDatabase(connectionString);
        }

        public int GetDatabaseId(string connectionString)
        {
            return ((IDbConnectionsService)webApi).GetDatabaseId(connectionString);
        }

        public DatabaseConnectionString GetDbConnection(string server)
        {
            return ((IDbConnectionsService)webApi).GetDbConnection(server);
        }

        public ProcedureModel GetProcedureModel([NotNullNotEmpty] string connectionString, [NotNullNotEmpty] string database, [NotNullNotEmpty] string schema, [NotNullNotEmpty] string name)
        {
            return ((IDbConnectionsService)webApi).GetProcedureModel(connectionString, database, schema, name);
        }

        public int AddParameterAttributes(int parameterId, Dictionary<string, string> attributes)
        {
            return dataModelService.AddParameterAttributes(parameterId, attributes);
        }

        public Dictionary<string, string> GetParameterAttributes(int parameterId)
        {
            return dataModelService.GetParameterAttributes(parameterId);
        }

        public Dictionary<string, InputFormField> GetProcedureFormFields(int procedureId)
        {
            return dataModelService.GetProcedureFormFields(procedureId);
        }

        public ProcedureModel GetProcedureModel(string schema, string name)
        {
            return dataModelService.GetProcedureModel(schema, name);
        }

        public TreeControlView CreateTreeView<TEntity>(IQueryable<TEntity> dbset, Action<TEntity> onclicked) where TEntity : HierTable<TEntity>
        {
            return ((ITreeService)treeService).CreateTreeView(dbset, onclicked);
        }

        public ISite Site { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler Disposed;

        public void Dispose()
        {
            dbContext.Dispose();
        }

        //public void CreateExecuterForm(string schema, string name)
        //{
        //    var metadata = GetProcedureMetadata(schema, name);
        //    var formfields = GetProcedureFormFields(schema, name);
        //    var control = new ProcedureExecuteControl();
        //    control.SetModel(this, metadata, null);
        //}



        public virtual DbSet<TEntity> Set<TEntity>()
            where TEntity : class
            => dbContext.Set<TEntity>();

        /// <summary>
        /// Проверка на наличие зарегистрированной записи по уникальному имени
        /// </summary>
        public TNamedObject CheckRegistration<TNamedObject>(string name)
            where TNamedObject : NamedObject
        {
            var target = Set<TNamedObject>().FirstOrDefault(p => p.Name == name);
            if (target == null)
            {
                target = typeof(TNamedObject).New<TNamedObject>();
                target.Name = name;
                if (target is IntegratedObject io)
                {
                    io.Code = name;
                }
                //target.Code = name;
                target.EnsureIsValide();

                Set<TNamedObject>().Add(target);
                SaveChanges();
            }
            return target;
        }


        public IEnumerable<TNamedObject> CheckRegistration<TNamedObject>(IEnumerable<string> names)
                where TNamedObject : NamedObject
        {
            List<TNamedObject> results = new();
            foreach (var name in names)
            {
                results.Add(CheckRegistration<TNamedObject>(name));
            }
            return results;
        }


        /// <summary>
        /// Проверка на наличие зарегистрированной записи по уникальному имени
        /// </summary>
        public TNamedObject CheckRegistration<TNamedObject>(TNamedObject current)
            where TNamedObject : NamedObject
        {
            try
            {
                if (current is null)
                    return null;
                var target = Set<TNamedObject>().FirstOrDefault(p => p.Name == current.Name);
                if (target == null)
                {
                    if (current is MessageProtocol mp)
                    {
                        mp.ResetPropertiesMessageAttributeRef();
                    }
                    Set<TNamedObject>().Add(current);
                    SaveChanges();
                    return current;
                }
                return target;
            }
            catch (DbUpdateException ex)
            {
                this.Error($"Исключение при CheckRegistration => {current.ToJsonOnScreen()} => {ex.Message + "\n" + ex.InnerException.Message}");
                throw new Exception($"Исключение при CheckRegistration => {current.ToJsonOnScreen()} => {ex.Message + "\n" + ex.InnerException.Message}");
            }
        }


        public IRepository<T> GetRepository<T>()
            where T : BaseEntity
        {
            return typeof(Repository<,>).MakeGenericType(new Type[] {

                typeof(T),
                GetType()
            }).New<IRepository<T>>(new object[] { this });
        }



        public dynamic GetRepositoryForType(Type entityType)
        {
            var ptype = dbContext.GetType();
            this.Info($"Repository<{ptype.GetTypeName()},{entityType.GetTypeName()}>");
            return typeof(Repository<,>).MakeGenericType(

                entityType,
                ptype
            ).New(new object[] { dbContext });
        }



        public string SaveDataModel()
        {
            var dms = new DatabaseModelService();
            return dms.TakeModel(this.GetTypeName(), GetSqlServerWebApi());
        }
        public string CopyDataModel(string name)
        {
            var dms = new DatabaseModelService();
            return dms.TakeModel(name, GetSqlServerWebApi());
        }

        public AppViewBase CreateDbSetView(string entityName)
        {
            var viewType = typeof(DbSetView<,>).MakeGenericType(new Type[] { GetType(), entityName.ToType() });
            return viewType.New<AppViewBase>(this);
        }
        public AppViewBase CreateDbSetView<TEntity>()
        {
            var viewType = typeof(DbSetView<,>).MakeGenericType(new Type[] { GetType(), typeof(TEntity) });
            return viewType.New<AppViewBase>(this);
        }

        public int Create(BaseEntity p)
        {
            throw new NotImplementedException();
        }


        public ListView CreateSelectList<TEntity>(Action<TEntity> onselected) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public TreeControlView CreateTreeView<TEntity>(Action<TEntity> onclicked) where TEntity : HierTable<TEntity>
        {
            throw new NotImplementedException();
        }

        public TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2, TEntity3>(Func<TEntity1, TreeViewNodeModel> c1, Func<TEntity2, TreeViewNodeModel> c2, Func<TEntity3, TreeViewNodeModel> c3)
            where TDbContext : DbContextBase
            where TEntity1 : BaseEntity
            where TEntity2 : BaseEntity
            where TEntity3 : BaseEntity
        {
            throw new NotImplementedException();
        }

        public TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2>()
            where TDbContext : DbContextBase
            where TEntity1 : NamedObject
            where TEntity2 : NamedObject
        {
            throw new NotImplementedException();
        }

        public TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2>(Func<TEntity1, TreeViewNodeModel> c1, Func<TEntity2, TreeViewNodeModel> c2)
            where TDbContext : DbContextBase
            where TEntity1 : BaseEntity
            where TEntity2 : BaseEntity
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public BusinessDatasource GetBusinessDatasource()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetDataset<TEntity>(params Predicate<TEntity>[] filters) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public DbContextInitiliazer<TDbContext> GetDbContextInitializer<TDbContext>() where TDbContext : DbContextBase
        {
            throw new NotImplementedException();
        }

        public EfEntityFasade<TEntity> GetEntityFasade<TEntity>() where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }
        public List<EntityRelations> GetEntityRelations<TEntity>()
        {
            var results = new List<EntityRelations>();
            var entity = dbContext.Model.GetEntityTypes().FirstOrDefault(p => p.Name == typeof(TEntity).GetTypeName());
            if (entity is not null)
            {
                var navigations = entity.GetNavigations();

                foreach (var navigation in navigations)
                {
                    var relations = new EntityRelations()
                    {
                        EntityTypeName = entity.Name,
                        ForeignKey = $"{navigation.ForeignKey?.Properties.FirstOrDefault()?.Name}",
                        PropertyName = navigation.Name,
                        PropertyTypeName = navigation.ClrType.GetTypeName(),
                        RelationshipType = $"{(navigation.IsCollection ? "One-to-Many" : "One-to-One")}"
                    };

                    //определение отношений много-ко-многим
                    var attrs = typeof(TEntity).GetPropertyAttributes(navigation.PropertyInfo.Name);
                    var kvs = attrs.Where(a => a.Key == nameof(ManyToManyAttribute)).ToList();
                    var manyToManyAttribute = kvs is not null && kvs.Count() > 0 ?
                         kvs.First().Key.ToType().New<ManyToManyAttribute>(new object[] { kvs.First().Value }) : null;
                    if (manyToManyAttribute is not null)
                    {
                        relations.RelativeEntityType = manyToManyAttribute.PropertyName;
                        relations.RelationshipType = "Many-to-Many";
                    }



                    var propertyType = navigation.ClrType.GetTypeName().ToType();
                    if (navigation.IsCollection)
                    {
                        var referencxeEntityType = propertyType.GetGenericArguments()[0];
                        var isCollection = propertyType.IsCollectionType();
                    }
                    results.Add(relations);
                }
            }
            return results;
        }
        public List<EntityRelations> GetEntityRelationsForType(Type typeofEntity)
        {
            var res = this.InvokeGenericMethod(
                nameof(GetEntityRelations), new List<Type>() { typeofEntity }, new List<object>());
            return (List<EntityRelations>)res;

        }

        public List<EntityRelations> GetEntityRelationsForTypeWithExpected(Type typeofEntity, List<string> expected)
        {
            return (List<EntityRelations>)this.InvokeGenericMethod(nameof(GetEntityRelationsWithExpected), new List<Type>() { typeofEntity }, new List<object>() { expected });
        }

        public IDictionary<string, EntityRelations> GetEntityRelationsForType2(Type ptype)
        {
            throw new NotImplementedException();
        }


        public List<EntityRelations> GetEntityRelationsWithExpected<TEntity>(List<string> expected = null)
        {
            throw new NotImplementedException();
        }

        public ConcurrentDictionary<string, List<MethodInfo>> GetMethodsWithDataChangedAttributes()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, ParameterMetadata> GetParameters(string SchemaName, string ProcedureName)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, ParameterMetadata> GetParametersMetadata()
        {
            throw new NotImplementedException();
        }



        public List<string> GetProcedureSql(string schema, string name)
        {
            throw new NotImplementedException();
        }



        public SqlServerWebApi GetSqlServerWebApi()
        {
            return webApi;
        }

        public SqlServerWebApi GetSqlServerWebApi(string connectionString)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }



        public IEnumerable<dynamic> Search(IEnumerable<string> keywords)
        {
            List<Task> tasks = new();
            ConcurrentBag<dynamic> results = new();
            foreach (var entityType in dbContext.GetEntitiesTypes())
            {
                if (entityType.GetExtendedTypeNames().Any(n => n.Contains("PrivateData")))
                    continue;
                var repository = GetRepositoryForType(entityType);

                try
                {
                    this.Info(((object)repository).GetTypeName());
                    if ("Repository<UserSettings,DeliveryDbContext>" == ((object)repository).GetTypeName())
                    {
                        this.Info(123);
                    }
                    var searchResults = repository.SearchByKeywords(keywords);
                    tasks.Add(Task.Run(() =>
                    {
                        foreach (var p in results)
                        {
                            results.Add(p);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    this.Error($"{entityType.GetTypeName()} => " + ex);
                }
            }
            Task.WaitAll(tasks.ToArray());
            return results;
        }


        public dynamic SetEntity(Type entityType)
        {
            return GetType().GetMethod("Set").MakeGenericMethod(entityType).Invoke(this, new object[0]);
        }


         

        public Dictionary<string, List<string>> ValidateOptional()
        {
            return new();
        }

        public void AddOrUpdate(BaseEntity entity)
        {

            if (entity is NamedObject)
            {
                var nameValue = ((NamedObject)entity).Name;

                dynamic fasade = entity.GetType().GetNamedEntityFasadeForType();
                var current = fasade.FindByName(nameValue);


                if (current is null)
                {
                    dbContext.Add(entity);
                    SaveChanges();
                }
                else
                {
                    entity.Id = current.Id;
                    ((object)current).SetProperties(entity.ToTextDictionary());
                    //this.Info(((object)current));
                    dbContext.Update(current);
                    SaveChanges();
                }

            }
            else
            {
                if (entity.Id == 0)
                {

                    dbContext.Add(entity);
                    SaveChanges();
                }
                else
                {
                    var data = entity.GetType().GetEntityFasade<BaseEntity>().Get(entity.Id);
                    data.SetProperties(entity.ToTextDictionary());
                    dbContext.Update(data);
                    SaveChanges();
                }
            }

        }
    }
}
