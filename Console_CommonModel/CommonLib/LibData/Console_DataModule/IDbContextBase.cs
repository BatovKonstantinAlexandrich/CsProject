using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using Newtonsoft.Json.Linq;

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule
{
    public interface IDbContextBase
    {


        string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable);
        void AddEntityType(Type entity);
        void AddOrUpdate(BaseEntity entity);
        int AddParameterAttributes(int parameterId, Dictionary<string, string> attributes);

        IEnumerable<TNamedObject> CheckRegistration<TNamedObject>(IEnumerable<string> names) where TNamedObject : NamedObject;
        TNamedObject CheckRegistration<TNamedObject>(string name) where TNamedObject : NamedObject;
        TNamedObject CheckRegistration<TNamedObject>(TNamedObject current) where TNamedObject : NamedObject;
        string CopyDataModel(string name);
        int Create(BaseEntity p);
        IDbCommand CreateCommand();
        IDbCommand CreateCommand<TIDbCommand>();
        string CreateCreateStatement(string schema, string table);
        void CreateDatabase();
        int CreateDatabase(string name);
        AppViewBase CreateDbSetView(string entityName);
        AppViewBase CreateDbSetView<TEntity>();
        EditView CreateExecuterForm(string schema, string name);
        string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null);
        string CreateForeignKeys(TableInfo metadata);
        string CreateInsertStatement(string schema, string table, Dictionary<string, object> values);
        string CreateSchema(string schema);
        ListView CreateSelectList<TEntity>(Action<TEntity> onselected) where TEntity : BaseEntity;
        string CreateSelectStatement(string schema, string table);
        string CreateTable(TableInfo metadata);
        string CreateTable(Type metadata);
        TableInfo CreateTableMetaData(Type metadata);
        TreeControlView CreateTreeView<TEntity>(Action<TEntity> onclicked) where TEntity : HierTable<TEntity>;
        TreeControlView CreateTreeView<TEntity>(IQueryable<TEntity> dbset, Action<TEntity> onclicked) where TEntity : HierTable<TEntity>;
        TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2, TEntity3>(Func<TEntity1, TreeViewNodeModel> c1, Func<TEntity2, TreeViewNodeModel> c2, Func<TEntity3, TreeViewNodeModel> c3)
            where TDbContext : DbContextBase
            where TEntity1 : BaseEntity
            where TEntity2 : BaseEntity
            where TEntity3 : BaseEntity;
        TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2>()
            where TDbContext : DbContextBase
            where TEntity1 : NamedObject
            where TEntity2 : NamedObject;
        TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2>(Func<TEntity1, TreeViewNodeModel> c1, Func<TEntity2, TreeViewNodeModel> c2)
            where TDbContext : DbContextBase
            where TEntity1 : BaseEntity
            where TEntity2 : BaseEntity;
        string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values);

        DbMigCommand[] DropAndCreate();
        string DropColumnFromTable(string fullTableName, string propertyName);
        string DropTable(TableInfo metadata);
        DataTable ExecuteProcedure(string name, IDictionary<string, object> input);
        int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output);
        int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output);
        IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class;
        DataTable ExecuteQuery(string command);
        IEnumerable<dynamic> ExecuteQuery(string command, Type entity);
        IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class;
        BusinessDatasource GetBusinessDatasource();
        IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName);
        string GetConnectionString();
        DatabaseModel GetDatabase(string connectionString);
        int GetDatabaseId(string connectionString);
        IEnumerable<string> GetDatabaseNames();
        IQueryable<TEntity> GetDataset<TEntity>(params Predicate<TEntity>[] filters) where TEntity : BaseEntity;
        DatabaseConnectionString GetDbConnection(string server);
        DbContextInitiliazer<TDbContext> GetDbContextInitializer<TDbContext>() where TDbContext : DbContextBase;
        Type[] GetEntityClasses();
        EfEntityFasade<TEntity> GetEntityFasade<TEntity>() where TEntity : BaseEntity;
        List<EntityRelations> GetEntityRelations<TEntity>();
        List<EntityRelations> GetEntityRelationsForType(Type typeofEntity);
        IDictionary<string, EntityRelations> GetEntityRelationsForType2(Type ptype);
        List<EntityRelations> GetEntityRelationsForTypeWithExpected(Type typeofEntity, List<string> expected);
        List<EntityRelations> GetEntityRelationsWithExpected<TEntity>(List<string> expected = null);
        JArray GetJsonResult(string sql);
        IEnumerable<KeyMetadata> GetKeysMetadata();
        ConcurrentDictionary<string, List<MethodInfo>> GetMethodsWithDataChangedAttributes();
        Dictionary<string, string> GetParameterAttributes(int parameterId);
        IDictionary<string, ParameterMetadata> GetParameters(string SchemaName, string ProcedureName);
        IDictionary<string, ParameterMetadata> GetParametersMetadata();
        IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName);
        Dictionary<string, InputFormField> GetProcedureFormFields(int procedureId);
        Dictionary<string, InputFormField> GetProcedureFormFields(string schema, string name);
        ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName);
        ProcedureModel GetProcedureModel([NotNullNotEmpty] string schema, [NotNullNotEmpty] string name);
        ProcedureModel GetProcedureModel([NotNullNotEmpty] string connectionString, [NotNullNotEmpty] string database, [NotNullNotEmpty] string schema, [NotNullNotEmpty] string name);
        IDictionary<string, ProcedureMetadata> GetProceduresMetadata();
        IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema = null);
        List<string> GetProcedureSql(string schema, string name);
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        dynamic GetRepositoryForType(Type entityType);
        IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable);
        IEnumerable<string> GetSchemasNames();
        JObject GetSingleJObject(string command);
        SqlServerWebApi GetSqlServerWebApi();
        SqlServerWebApi GetSqlServerWebApi(string connectionString);
        IEnumerable<string> GetTableNames();
        IDictionary<string, TableMetadata> GetTablesMetadata();
        Dictionary<string, string> ParseConnectionString(string connectionString);
        int PrepareQuery(string command);
        int SaveChanges();
        string SaveDataModel();
        IEnumerable<dynamic> Search(IEnumerable<string> keywords);
        dynamic SetEntity(Type entityType);
        void TestProcedures();
        AppViewBase ToView();
        int TryPrepareQuery(string command);
        void UpdateDatabase();
        IDictionary<string, TableMetadata> UpdateTablesMetadata();
        Dictionary<string, List<string>> ValidateOptional();
    }
}