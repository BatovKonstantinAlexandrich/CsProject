
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using System.Data;
using System.Reflection;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;

using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices
{
    [Label("Управление метаданными Postgres")]


    public class PostgresDbMetadata : PostgresExecutor, IDbMetadata
    {
        private Dictionary<string, TableMetadata> TablesMetadata;
        private IDictionary<string, string> ForeignKeys { get; set; } = new Dictionary<string, string>();
        private ChangingDictionary<string, IDictionary<string, ProcedureMetadata>> ProceduresMetadata { get; set; } =
            new ChangingDictionary<string, IDictionary<string, ProcedureMetadata>>();
        private Dictionary<string, ProcedureMetadata> Procedures { get; set; } = new();
        public IEnumerable<KeyMetadata> KeyMetadata { get; private set; }
        protected readonly IDataTableService DataTableService = new DataTableService();

        public PostgresDbMetadata() : base()
        {
        }

        public PostgresDbMetadata(string dataSource, int port, string database, string userId, string password)
            : base(dataSource, port, database, userId, password)
        {
        }

        public IEnumerable<string> GetTableNames()
        {
            //this.Info($"GetTableNames()");
            lock (this)
            {
                if (TablesMetadata == null)
                {
                    TablesMetadata = InitTablesMetadata();
                }
            }
            return TablesMetadata.Keys.ToList();
        }

        public IDictionary<string, TableMetadata> GetTablesMetadata()
        {
            this.Info($"GetTablesMetadata()");
            lock (this)
            {
                if (TablesMetadata == null)
                {
                    TablesMetadata = InitTablesMetadata();
                }
            }
            return TablesMetadata;
        }

        public IDictionary<string, TableMetadata> UpdateTablesMetadata()
        {
            this.Info($"UpdateTablesMetadata()");
            lock (this)
            {
                TablesMetadata = InitTablesMetadata();
            }
            return TablesMetadata;
        }

        private Dictionary<string, TableMetadata> InitTablesMetadata()
        {
            using (var db = new ModelsDbContext())
            {
                this.Info($"InitTablesMetadata()");

                TablesMetadata = new Dictionary<string, TableMetadata>();
                var TablesInfoDT = ExecuteQuery(@"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'");
                IEnumerable<TableMetadata> TableMetadataArr = DataTableService.GetResultSet<TableMetadata>(TablesInfoDT);
                var columnsInfo = GetColumnsMetadata();
                foreach (var next in TableMetadataArr)
                {
                    next.TableType = nameof(BaseEntity);

                    if (next.TableName.ToSingleCount().IsType())
                    {
                        var entityType = next.TableName.ToSingleCount().ToType();
                        next.TableLabel = entityType.GetLabel();
                    }

                    next.ToJsonOnScreen().WriteToConsole();
                    TablesMetadata[next.TableName] = next;
                    if (columnsInfo.ContainsKey(next.TableName))
                    {
                        next.ColumnsMetadata = columnsInfo[next.TableName];
                    }
                }
                IEnumerable<KeyMetadata> KeysMetadata = GetKeysMetadata();
                foreach (KeyMetadata Key in KeysMetadata)
                {
                    ForeignKeys[Key.SourceTable + "." + Key.SourceColumn] = Key.TargetTable + "." + Key.TargetColumn;
                    if (Key.TargetTable != null && TablesMetadata.ContainsKey(Key.SourceTable))
                    {
                        TablesMetadata[Key.SourceTable].ForeignKeys[Key.SourceColumn] = Key.TargetTable;
                    }
                    else if (TablesMetadata.ContainsKey(Key.SourceTable))
                    {
                        TablesMetadata[Key.SourceTable].PrimaryKey = Key.SourceColumn;
                    }
                }
            }
            return TablesMetadata;
        }

        public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName)
        {
            //this.Info($"GetColumnsMetadata({TableSchema},{TableName})");
            IDictionary<string, ColumnMetadata> Result = new Dictionary<string, ColumnMetadata>();
            DataTable ColumnsDataTable = ExecuteQuery($@"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='{TableSchema}' AND TABLE_NAME='{TableName}'");
            IEnumerable<ColumnMetadata> ColumnsMetaData = DataTableService.GetResultSet<ColumnMetadata>(ColumnsDataTable);
            foreach (var Column in ColumnsMetaData)
            {
                Result[Column.ColumnName] = Column;
            }
            return Result;
        }

        private Dictionary<string, Dictionary<string, ColumnMetadata>> GetColumnsMetadata()
        {
            //this.Info($"GetColumnsMetadata()");
            Dictionary<string, Dictionary<string, ColumnMetadata>> result = new();
            DataTable ColumnsDataTable = ExecuteQuery($@"SELECT * FROM INFORMATION_SCHEMA.COLUMNS");
            IEnumerable<ColumnMetadata> ColumnsMetaData = DataTableService.GetResultSet<ColumnMetadata>(ColumnsDataTable);
            foreach (var Column in ColumnsMetaData)
            {
                if (!result.ContainsKey(Column.TableName))
                    result[Column.TableName] = new();
                result[Column.TableName][Column.ColumnName] = Column;
            }
            return result;
        }

        public IEnumerable<KeyMetadata> GetKeysMetadata()
        {
            //this.Info($"GetKeysMetadata()");
            lock (this)
            {
                if (KeyMetadata is null)
                {
                    KeyMetadata = InitKeysMetadata();
                }
            }
            return KeyMetadata;
        }

        private IEnumerable<KeyMetadata> InitKeysMetadata()
        {
            this.Info($"InitKeysMetadata()");
            DataTable KeysDataTable = ExecuteQuery(@"
                SELECT 
                    tc.TABLE_NAME AS SOURCE_TABLE,
                    kcu.COLUMN_NAME AS SOURCE_COLUMN,
                    ccu.TABLE_NAME AS TARGET_TABLE,
                    ccu.COLUMN_NAME AS TARGET_COLUMN
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu
                    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
                    AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA
                LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS ccu
                    ON ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                    AND ccu.TABLE_SCHEMA = tc.TABLE_SCHEMA
                WHERE tc.CONSTRAINT_TYPE IN ('FOREIGN KEY', 'PRIMARY KEY')
                ORDER BY tc.TABLE_NAME");
            return KeyMetadata = DataTableService.GetResultSet<KeyMetadata>(KeysDataTable);
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema = "public")
        {
            MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(Schema);
            this.Info($"GetProceduresMetadata({Schema})");
            lock (this)
            {
                if (Schema is not null && !GetSchemasNames().Select(s => s.ToLower()).Contains(Schema.ToLower()))
                {
                    //throw new ArgumentException(nameof(Schema), $"В базе данных не существует указанной схемы: {Schema}: {this.ToString()}");
                    var message = $"В базе данных не существует указанной схемы: {Schema}: {ToString()}";
                    this.Warn(message);
                    var ddlFactrory = new PostgresDDLFactory();
                    PrepareQuery(ddlFactrory.CreateSchema(Schema));

                }
                if (ProceduresMetadata is null || ProceduresMetadata.Count() == 0)
                {
                    foreach (var schema in GetSchemasNames())
                    {
                        if (string.IsNullOrWhiteSpace(schema))
                        {
                            continue;
                        }
                        ProceduresMetadata[schema.ToLower()] = InitProceduresMetadata(schema);
                        foreach (var items in ProceduresMetadata[schema.ToLower()].Values)
                        {
                            Procedures[items.FullName] = items;
                        }
                    }
                }
                if (Schema is null || Schema == "")
                {
                    return Procedures;
                }
                else
                {
                    if (!ProceduresMetadata.ContainsKey(Schema.ToLower()))
                    {
                        ProceduresMetadata[Schema.ToLower()] = InitProceduresMetadata(Schema.ToLower());
                    }
                    return ProceduresMetadata[Schema.ToLower()];
                }
            }
        }

        private IDictionary<string, ProcedureMetadata> InitProceduresMetadata(string Schema = null)
        {
            try
            {
                this.Info($"InitProceduresMetadata({Schema})");
                this.Info(ToString());
                IDictionary<string, ProcedureMetadata> ProceduresMetadataDictionary = new Dictionary<string, ProcedureMetadata>();

                // В PostgreSQL функции хранятся в INFORMATION_SCHEMA.ROUTINES
                string query = Schema is null
                    ? @"SELECT ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_CATALOG 
                       FROM INFORMATION_SCHEMA.ROUTINES 
                       WHERE ROUTINE_TYPE = 'FUNCTION'"
                    : $@"SELECT ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_CATALOG 
                        FROM INFORMATION_SCHEMA.ROUTINES 
                        WHERE ROUTINE_TYPE = 'FUNCTION' AND ROUTINE_SCHEMA = '{Schema}'";

                DataTable SPDataTable = ExecuteQuery(query);
                int proceduresCount = SPDataTable.Rows.Count;
                int n = 1;

                foreach (DataRow row in SPDataTable.Rows)
                {
                    //this.Info($"Процедура №{n++} (Всего {proceduresCount})");
                    string ProcedureName = row["ROUTINE_NAME"].ToString();
                    string ProcedureOwner = row["ROUTINE_SCHEMA"].ToString();
                    string ProcedureQualifier = row["ROUTINE_CATALOG"].ToString();

                    if (ProcedureOwner == "pg_catalog" || ProcedureOwner == "information_schema")
                        continue;

                    ProceduresMetadataDictionary[ProcedureName] = new ProcedureMetadata();
                    ProcedureMetadata ProcedureMetadata = GetProcedureMetadata(ProcedureQualifier, ProcedureOwner, ProcedureName);
                    ProcedureMetadata.ProcedureQualifier = ProcedureQualifier;

                    // Получаем SQL-код функции из PostgreSQL
                    try
                    {
                        var sqlQuery = $@"
                            SELECT pg_get_functiondef(oid) as sql
                            FROM pg_proc
                            WHERE proname = '{ProcedureName}'
                            AND pronamespace = (SELECT oid FROM pg_namespace WHERE nspname = '{ProcedureOwner}')
                            LIMIT 1";
                        var sqlDataTable = ExecuteQuery(sqlQuery);
                        if (sqlDataTable.Rows.Count > 0)
                        {
                            ProcedureMetadata.Sql = sqlDataTable.Rows[0]["sql"]?.ToString() ?? string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Warn($"Не удалось получить SQL для процедуры {ProcedureName}: {ex.Message}");
                    }

                    ProceduresMetadataDictionary[ProcedureName] = ProcedureMetadata;
                    ProcedureMetadata.ParametersMetadata = GetParametersMetadata(ProcedureOwner, ProcedureName);
                    this.Info(ProcedureMetadata.ToJsonOnScreen());
                    ProcedureMetadata.EnsureIsValide();
                }
                return ProceduresMetadataDictionary;
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось получить сведения о процедурах: {ex.Message}", ex);
            }
        }

        public ProcedureMetadata GetProcedureMetadata(string Database, string SchemaName, string ProcedureName)
        {
            this.Info($"GetProcedureMetadata({SchemaName},{ProcedureName})");

            ProcedureMetadata ProcedureMetadata = new ProcedureMetadata()
            {
                ProcedureName = ProcedureName,
                ProcedureOwner = SchemaName,
                ProcedureQualifier = Database
            };

            ProcedureMetadata.ParametersMetadata = GetParametersMetadata(SchemaName, ProcedureName);
            return ProcedureMetadata;
        }

        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName)
        {
            // Получаем имя базы данных из строки подключения или используем текущую
            var database = Database ?? "postgres";
            return GetProcedureMetadata(database, SchemaName, ProcedureName);
        }

        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName)
        {
            this.Info($"GetParametersMetadata({SchemaName},{ProcedureName})");

            IDictionary<string, ParameterMetadata> result = new Dictionary<string, ParameterMetadata>();
            var TablesInfoDT = ExecuteQuery($@"
                SELECT * FROM INFORMATION_SCHEMA.PARAMETERS 
                WHERE SPECIFIC_SCHEMA = '{SchemaName}' AND SPECIFIC_NAME = '{ProcedureName}'");
            IEnumerable<ParameterMetadata> ParameterMetadataArr = DataTableService.GetResultSet<ParameterMetadata>(TablesInfoDT);
            List<ParameterMetadata> ParameterMetadataList = new List<ParameterMetadata>(ParameterMetadataArr);
            ParameterMetadataList.Sort((pm1, pm2) => pm1.OrdinalPosition.CompareTo(pm2.OrdinalPosition));

            foreach (var next in ParameterMetadataList)
            {
                result[next.ParameterName] = next;
            }
            return result;
        }

        public IEnumerable<string> GetSchemasNames()
        {
            //this.Info($"GetSchemasNames()");
            var schemasResultSet = ExecuteQuery(@"SELECT schema_name FROM INFORMATION_SCHEMA.SCHEMATA WHERE schema_name NOT IN ('pg_catalog', 'information_schema', 'pg_toast')");
            IEnumerable<SchemaMetadata> schemas = DataTableService.GetResultSet(schemasResultSet, (data) => new SchemaMetadata()
            {
                Name = $"{data["schema_name"]}",
                Label = $"{data["schema_name"]}"
            });
            return schemas.Select(s => s.Name);
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata()
        {
            IDictionary<string, ProcedureMetadata> results = new Dictionary<string, ProcedureMetadata>();
            foreach (var schema in GetSchemasNames())
            {
                foreach (var kv in GetProceduresMetadata(schema))
                {
                    results[kv.Key] = kv.Value;
                }
            }
            return results;
        }

        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
        {
            IDictionary<string, IDictionary<string, ParameterMetadata>> result = new Dictionary<string, IDictionary<string, ParameterMetadata>>();
            foreach (var schema in GetSchemasNames())
            {
                foreach (var proc in GetProceduresMetadata(schema))
                {
                    var paramsDict = GetParametersMetadata(schema, proc.Value.ProcedureName);
                    result[$"{schema}.{proc.Key}"] = paramsDict;
                }
            }
            return result;
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            var results = ExecuteQuery("SELECT datname FROM pg_database WHERE datistemplate = false");
            return DataTableService.GetTextColumn(results, "datname");
        }

        public List<string> GetProcedureSql(string schema, string name)
        {
            try
            {
                var sqlQuery = $@"
                    SELECT pg_get_functiondef(oid) as sql
                    FROM pg_proc
                    WHERE proname = '{name}'
                    AND pronamespace = (SELECT oid FROM pg_namespace WHERE nspname = '{schema}')
                    LIMIT 1";
                var sqlDataTable = ExecuteQuery(sqlQuery);
                if (sqlDataTable.Rows.Count > 0)
                {
                    var sql = sqlDataTable.Rows[0]["sql"]?.ToString() ?? string.Empty;
                    return sql.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                this.Error($"Не удалось получить SQL для процедуры {schema}.{name}: {ex.Message}");
                return new List<string>();
            }
        }
    }
}