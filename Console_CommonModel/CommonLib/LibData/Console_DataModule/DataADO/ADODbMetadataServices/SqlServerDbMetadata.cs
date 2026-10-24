
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon;

using Google.Protobuf.WellKnownTypes;

using MySqlX.XDevAPI;

using pickpoint_delivery_service;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices
{
    /*
    public string DropAndCreateTable(string TableName) 
    { 
        return $@"
            DECLARE @COUNT INT
            SET @COUNT=(SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{TableName}')
            IF( @COUNT > 0 )
            BEGIN
                DROP TABLE {TableName}
            END
            CREATE TABLE History( 
                Id INT PRIMARY KEY Identity, 
                COMMAND nvarchar(512) NOT NULL,
                CREATED DateTime NOT NULL DEFAULT(GetDate())                 
            )
        ";
    }
     */
    [Label("Метаданные SqlServer")]
    public class SqlServerDbMetadata : SqlServerExecutor, IDbMetadata, IDbConnectionsService
    {
        protected static Dictionary<string, Dictionary<string, TableMetadata>> TablesMetadataTemp = new();
        protected Dictionary<string, TableMetadata> TablesMetadata;

        private IDbConnectionsService dbConenctionsService = new DbConnectionsService();
        /// <summary>
        /// Внешние ключи
        /// Ключ = [TableName].[ColumnName] 
        /// Значение = [TableName].[ColumnName] 
        /// </summary>
        private IDictionary<string, string> ForeignKeys { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Параметры хранимых процедур с доступом по схемам
        /// </summary>        
        private ChangingDictionary<string, IDictionary<string, ProcedureMetadata>> ProceduresMetadata { get; set; } =
            new ChangingDictionary<string, IDictionary<string, ProcedureMetadata>>();

        /// <summary>
        /// Все хранимые процедуры, наполняется по событиям изменения ProceduresMetadata
        /// </summary>
        private Dictionary<string, ProcedureMetadata> Procedures { get; set; } = new();

        [Label("Key Metadata")]
        public IEnumerable<KeyMetadata> KeyMetadata { get; private set; }

        public SqlServerDbMetadata() : base("DESKTOP-IHJM9RD", nameof(DeliveryDbContext))
        {
        }

        public SqlServerDbMetadata(string server, string database) : base(server, database)
        {
        }

        public SqlServerDbMetadata(string server, string database, bool trustedConnection, string userId, string password) : base(server, database, trustedConnection, userId, password)
        {
        }


        /// <summary>
        /// Получение наименований таблиц
        /// </summary>
        /// <returns>Наименования таблиц</returns>
        public IEnumerable<string> GetTableFullNames()
        {
            this.Info($"GetTableNames()");
            lock (this)
            {
                if (TablesMetadata == null)
                {

                    if(!TablesMetadataTemp.ContainsKey(this.ToString()) )
                    {
                        TablesMetadataTemp[this.ToString()] = TablesMetadata = InitTablesMetadata();
                    }
                    else
                    {
                        TablesMetadata = TablesMetadataTemp[this.ToString()];
                    }                    
                }
            }
            return TablesMetadata.Values.Select(val => $"[{val.TableCatalog}].[{val.TableSchema}].[{val.TableName}]").ToList();
        }
        public IEnumerable<string> GetTableNames()
        {
            this.Info($"GetTableNames()");
            lock (this)
            {
                if (TablesMetadata == null)
                {

                    if (!TablesMetadataTemp.ContainsKey(this.ToString()))
                    {
                        TablesMetadataTemp[this.ToString()] = TablesMetadata = InitTablesMetadata();
                    }
                    else
                    {
                        TablesMetadata = TablesMetadataTemp[this.ToString()];
                    }
                }
            }
            return TablesMetadata.Keys.ToList();
            //DataTable ResultSet = ExecuteQuery(@"SELECT * FROM INFORMATION_SCHEMA.TABLES");
            //return DataTableService.GetTextColumn(ResultSet, "TABLE_NAME");
        }
        public IEnumerable<string> ReGetTableNames()
        {
            this.Info($"GetTableNames()");
            lock (this)
            {
                if (TablesMetadata == null)
                {

                    if (!TablesMetadataTemp.ContainsKey(this.ToString()))
                    {
                        TablesMetadataTemp[this.ToString()] = TablesMetadata = InitTablesMetadata();
                    }
                    else
                    {
                        TablesMetadata = TablesMetadataTemp[this.ToString()];
                    }
                }
                var before = TablesMetadata.Keys.ToHashSet();
                TablesMetadata = InitTablesMetadata();
                var after = TablesMetadata.Keys.ToHashSet();

                var dropped = before.Except(after);
                var created = after.Except(before);

                this.Info(new
                {
                    dropped,
                    created
                });
            }
            return TablesMetadata.Keys.ToList();
            //DataTable ResultSet = ExecuteQuery(@"SELECT * FROM INFORMATION_SCHEMA.TABLES");
            //return DataTableService.GetTextColumn(ResultSet, "TABLE_NAME");
        }


        /// <summary>
        /// Получение сведений о таблицах базы данных
        /// </summary>
        /// <returns>сведения о таблицах базы данных</returns>
        public IDictionary<string, TableMetadata> GetTablesMetadata()
        {
            lock (this)
            {
                if (TablesMetadata == null)
                {

                    if (!TablesMetadataTemp.ContainsKey(this.ToString()))
                    {
                        TablesMetadataTemp[this.ToString()] = TablesMetadata = InitTablesMetadata();
                    }
                    else
                    {
                        TablesMetadata = TablesMetadataTemp[this.ToString()];
                    }
                }
            }
            return TablesMetadata;
        }
        public IDictionary<string, TableMetadata> UpdateTablesMetadata()
        {
            lock (this)
            {

                TablesMetadata = InitTablesMetadata();

            }
            return TablesMetadata;
        }
        public TableMetadata GetTableMetadata(string tableName)
        {
            lock (this)
            {
                if (TablesMetadata == null)
                {

                    if (!TablesMetadataTemp.ContainsKey(this.ToString()))
                    {
                        TablesMetadataTemp[this.ToString()] = TablesMetadata = InitTablesMetadata();
                    }
                    else
                    {
                        TablesMetadata = TablesMetadataTemp[this.ToString()];
                    }
                }
                if (!TablesMetadata.ContainsKey(tableName))
                    throw new ArgumentException(nameof(tableName), $"Не найдена таблица {tableName}");
            }
            return TablesMetadata[tableName];
        }



        /// <summary>
        /// Получение сведений о таблицах базы данных
        /// </summary>
        /// <returns>сведения о таблицах базы данных</returns>
        public Dictionary<string, TableMetadata> InitTablesMetadata()
        {
            using (var db = new ModelsDbContext())
            {

                var databaseId = db.GetDatabaseId(ToString());
                TablesMetadata = new Dictionary<string, TableMetadata>();
                var TablesInfoDT = ExecuteQuery(@"SELECT * FROM INFORMATION_SCHEMA.TABLES");
                IEnumerable<TableMetadata> TableMetadataArr = DataTableService.GetResultSet<TableMetadata>(TablesInfoDT);

                var columnsInfo = GetColumnsMetadata();
                foreach (var next in TableMetadataArr)
                {
                    next.Schema = db.GetSchema(databaseId, next.TableSchema);
                    next.SchemaId = next.Schema.Id;
                    next.TableCatalog = db.Databases.Find(databaseId).Name;

                    next.TableType = nameof(BaseEntity);
                    //this.Info(next.TableName);
                    if (next.TableName.ToSingleCount().IsType())
                    {
                        var entityType = next.TableName.ToSingleCount().ToType();
                        next.TableLabel = entityType.GetLabel();
                    }

                    //next.ToJsonOnScreen().WriteToConsole();
                    TablesMetadata[next.TableName] = next;
                    next.ColumnsMetadata = columnsInfo[next.TableName];//GetColumnsMetadata(next.TableSchema, next.TableName);                    
                    next.EnsureIsValide();
                }
                IEnumerable<KeyMetadata> KeysMetadata = GetKeysMetadata();
                foreach (KeyMetadata Key in KeysMetadata)
                {
                    ForeignKeys[Key.SourceTable + "." + Key.SourceColumn] = Key.TargetTable + "." + Key.TargetColumn;
                    if (Key.TargetTable != null)
                    {
                        TablesMetadata[Key.SourceTable].ForeignKeys[Key.SourceColumn] = Key.TargetTable;
                    }
                    else
                    {
                        TablesMetadata[Key.SourceTable].PrimaryKey = Key.SourceColumn;
                    }
                }
            }
            //TablesMetadata.Values.ToList().ForEach(t => this.Info(t.Validate()));
            return TablesMetadataTemp[this.ToString()] = TablesMetadata;
        }

        public TableMetadata InitTablesMetadata(string schema, string name)
        {
            using (var db = new ModelsDbContext())
            {

                var databaseId = db.GetDatabaseId(ToString());
                TablesMetadata = new Dictionary<string, TableMetadata>();
                var TablesInfoDT = ExecuteQuery($@"SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = '{schema}' and TABLE_NAME = '{name}'");
                IEnumerable<TableMetadata> TableMetadataArr = DataTableService.GetResultSet<TableMetadata>(TablesInfoDT);
                var next = TableMetadataArr.First();

                var columnsInfo = GetColumnsMetadata();
                
                    next.Schema = db.GetSchema(databaseId, next.TableSchema);
                    next.SchemaId = next.Schema.Id;
                    next.TableCatalog = db.Databases.Find(databaseId).Name;

                    next.TableType = nameof(BaseEntity);
                    //this.Info(next.TableName);
                    if (next.TableName.ToSingleCount().IsType())
                    {
                        var entityType = next.TableName.ToSingleCount().ToType();
                        next.TableLabel = entityType.GetLabel();
                    }

                    //next.ToJsonOnScreen().WriteToConsole();
                    TablesMetadata[next.TableName] = next;
                    next.ColumnsMetadata = columnsInfo[next.TableName];//GetColumnsMetadata(next.TableSchema, next.TableName);                    
                    next.EnsureIsValide();
                
                IEnumerable<KeyMetadata> KeysMetadata = GetKeysMetadata();
                foreach (KeyMetadata Key in KeysMetadata)
                {
                    ForeignKeys[Key.SourceTable + "." + Key.SourceColumn] = Key.TargetTable + "." + Key.TargetColumn;
                    if(!TablesMetadata.Keys.Select(k => k.ToLower()).Contains(Key.SourceTable.ToLower()))
                    {
                        this.TablesMetadata = InitTablesMetadata();
                    }
                    if (Key.TargetTable != null)
                    {
                        TablesMetadata[Key.SourceTable].ForeignKeys[Key.SourceColumn] = Key.TargetTable;
                    }
                    else
                    {
                        TablesMetadata[Key.SourceTable].PrimaryKey = Key.SourceColumn;
                    }
                }
                this.GetTablesMetadata()[next.TableName] = next;
                return next;

            }
            //TablesMetadata.Values.ToList().ForEach(t => this.Info(t.Validate()));
        }


        /// <summary>
        /// Получение сведений о структуре данных таблицы
        /// </summary>
        /// <param name="TableSchema">Имя схемы</param>
        /// <param name="TableName">Имя таблицы</param>
        /// <returns>Сведения о структуре данных таблицы</returns>
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
        public Dictionary<string, Dictionary<string, ColumnMetadata>> GetColumnsMetadata()
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

        /// <summary>
        /// Получение сведений о внешних ключах
        /// </summary>
        /// <returns>сведения о внешних ключах</returns>
        public IEnumerable<KeyMetadata> GetKeysMetadata()
        {
            //this.Info($"GetKeysMetadata( )");
            lock (this)
            {
                if (KeyMetadata is null)
                {
                    KeyMetadata = InitKeysMetadata();
                }
            }
            return KeyMetadata;
        }


        public IEnumerable<KeyMetadata> InitKeysMetadata()
        {
            //this.Info($"GetKeysMetadata()");
            DataTable KeysDataTable =
                ExecuteQuery("\n SELECT " +
                                " CCU.TABLE_NAME AS SOURCE_TABLE " +
                                ",CCU.COLUMN_NAME AS SOURCE_COLUMN " +
                                ",KCU.TABLE_NAME AS TARGET_TABLE " +
                                ",KCU.COLUMN_NAME AS TARGET_COLUMN " +
                            "FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU " +
                                "INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC " +
                                " ON CCU.CONSTRAINT_NAME = RC.CONSTRAINT_NAME " +
                                "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU " +
                                " ON KCU.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME " +
                            "ORDER BY CCU.TABLE_NAME\n");
            return KeyMetadata = DataTableService.GetResultSet<KeyMetadata>(KeysDataTable);
        }





        /// <summary>
        /// Получение сведений о хранимых процедурах
        /// </summary>
        /// <param name="Schema">Имя схемы</param>
        /// <returns>сведения о хранимых процедурах</returns>
        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata([NotNullNotEmpty] string Schema = "dbo")
        {
            try
            {
                MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(Schema);
                this.Info($"GetProceduresMetadata({Schema})");
                lock (this)
                {
                    var schemaNames = GetSchemasNames();
                    if (Schema is not null && !schemaNames.Select(s => s.ToLower()).Contains(Schema.ToLower()))
                    {                                                
                        return new Dictionary<string, ProcedureMetadata>();
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
                return ProceduresMetadata[Schema];
            }
            catch(Exception ex)
            {
                throw new Exception($"{nameof(GetProcedureMetadata)} => {ex.Message}");
            }
        }

        public class ProcedureSqlCode
        {
            [Label("Text")]

            public string Text { get; set; }
        }

        public IEnumerable<string> GetProceduresSqlCode(string fullname)
        {
            var lines = ExecuteProcedure<ProcedureSqlCode>("sp_helptext",
                new Dictionary<string, string>() {
                    { "@objname", fullname }
                }
            );
            return lines.Select(line => line.Text);
        }

        public class SqlCodeLine
        {
            [Label("Text")]

            public string Text { get; set; }
        }
        /// <summary>
        /// Получение сведений о хранимых процедурах
        /// </summary>
        /// <param name="Schema">Имя схемы</param>
        /// <returns>сведения о хранимых процедурах</returns>
        private IDictionary<string, ProcedureMetadata> InitProceduresMetadata(string Schema = null)
        {

            try
            {
                this.Info($"InitProceduresMetadata({Schema})");
                this.Info(ToString());
                IDictionary<string, ProcedureMetadata> ProceduresMetadataDictionary = new Dictionary<string, ProcedureMetadata>();
                DataTable SPDataTable =
                    Schema is null ? ExecuteQuery($@"EXEC sp_stored_procedures") :
                    ExecuteQuery($@"EXEC sp_stored_procedures @sp_owner='{Schema}'");
                int proceduresCount = SPDataTable.Rows.Count;
                int n = 1;

                foreach (DataRow row in SPDataTable.Rows)
                {
                    //this.Info($"Процедура №{n++} (Всего {proceduresCount})");
                    string ProcedureName = row["PROCEDURE_NAME"].ToString();
                    ProcedureName = ProcedureName.Substring(0, ProcedureName.LastIndexOf(";"));
                    string ProcedureOwner = row["PROCEDURE_OWNER"].ToString();
                    if (ProcedureOwner == "sys")
                        continue;
                    string ProcedureQualifier = row["PROCEDURE_QUALIFIER"].ToString();

                    ProceduresMetadataDictionary[ProcedureName] = new ProcedureMetadata();
                    ProcedureMetadata ProcedureMetadata = GetProcedureMetadata(Database, ProcedureOwner, ProcedureName);
                    ProcedureMetadata.ProcedureQualifier = ProcedureQualifier;
                    string fullname = $"[{ProcedureMetadata.ProcedureOwner}].[{ProcedureName}]";
                    var input = new Dictionary<string, string>() { { "@objname", fullname } };
                    var lines = ExecuteProcedure<SqlCodeLine>("sys.sp_helptext", input);
                    var sqlList = lines.Select(line => line.Text).ToList();
                    string sql = "";
                    foreach (var line in sqlList)
                    {
                        sql += line;
                    }
                    ProceduresMetadataDictionary[ProcedureName].Sql = sql;
                    var metadata = GetProcedureModel(ToString(), Database, ProcedureMetadata.ProcedureOwner, ProcedureMetadata.ProcedureName);
                    if (metadata is not null)
                    {
                        ProcedureMetadata.ProcedureLabel = metadata.Label;
                    }

                    ProcedureMetadata.ParametersMetadata = GetParametersMetadata(Database, ProcedureOwner, ProcedureName);
                    this.Info(ProcedureMetadata.ToJsonOnScreen());
                    ProcedureMetadata.EnsureIsValide();
                    ProceduresMetadataDictionary[ProcedureName] = ProcedureMetadata;
                    if (n++ == 10)
                        break;
                }
                return ProceduresMetadataDictionary;
            }
            catch (Exception ex)
            {

                throw new Exception($"Не удалось получить сведения о процедурах: {ex.Message}");
            }
        }


        /// <summary>
        /// Получение сведений о хранимой процедуре
        /// </summary>
        /// <param name="SchemaName">Имя схемы</param>
        /// <param name="ProcedureName">Имя процедуры</param>
        /// <returns>Сведения о хранимой процедуре</returns>
        public ProcedureMetadata GetProcedureMetadata(string Database, string SchemaName, string ProcedureName)
        {
            this.Info($"GetProcedureMetadata({SchemaName},{ProcedureName})");

            ProcedureMetadata ProcedureMetadata = new ProcedureMetadata()
            {
                ProcedureName = ProcedureName,
                ProcedureOwner = SchemaName,
                ProcedureQualifier = this.Database
            };

            var metadata = GetProcedureModel(ToString(), this.Database, ProcedureMetadata.ProcedureOwner, ProcedureMetadata.ProcedureName);
            if (metadata is not null)
            {
                ProcedureMetadata.ProcedureLabel = metadata.Label;
            }
            ProcedureMetadata.ParametersMetadata = GetParametersMetadata(SchemaName, ProcedureName);
            return ProcedureMetadata;
        }




        public class ParameterDefaultValueInfo
        {
            [Label("name")]

            public string name { get; set; }
            [Label("has default value")]
            [InputBool]
            public bool has_default_value { get; set; }
            [Label("default value")]

            public string default_value { get; set; }
        }

        /// <summary>
        /// Получение сведений о параметрах хранимой процедуры
        /// </summary>
        /// <param name="SchemaName">Имя схемы</param>
        /// <param name="ProcedureName">Имя процедуры</param>
        /// <returns>Информация о параметрах</returns>
        /// 
        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
        {
            Dictionary<string, IDictionary<string, ParameterMetadata>> res = new();
            foreach (var schema in GetSchemasNames())
            {
                foreach (var procedure in GetProceduresMetadata(schema).Keys.ToList())
                {
                    res[procedure] = GetParametersMetadata(schema, procedure);
                }
            }
            return res;
        }
        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName)
        {
            this.Info($"GetParameterMetadata({SchemaName},{ProcedureName})");

            using (var db = new ModelsDbContext())
            {
                IDictionary<string, ParameterMetadata> result = new Dictionary<string, ParameterMetadata>();
                var TablesInfoDT = ExecuteQuery($@"SELECT * FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_SCHEMA='{SchemaName}' AND SPECIFIC_NAME='{ProcedureName}'");
                IEnumerable<ParameterMetadata> ParameterMetadataArr = DataTableService.GetResultSet<ParameterMetadata>(TablesInfoDT);
                List<ParameterMetadata> ParameterMetadataList = new List<ParameterMetadata>(ParameterMetadataArr);
                ParameterMetadataList.Sort((pm1, pm2) => pm2.OrdinalPosition - pm1.OrdinalPosition);
                var ParametersInfos = ExecuteQuery<ParameterDefaultValueInfo>($"SELECT name, has_default_value, default_value FROM sys.parameters pr where pr.object_id = (SELECT object_id FROM sys.procedures where name = '{ProcedureName}')");
                foreach (var next in ParameterMetadataList)
                {
                    var info = ParametersInfos.FirstOrDefault(p => p.name.ToUpper() == next.ParameterName.ToUpper());
                    result[next.ParameterName] = next;
                    next.HasDefaultValue = info.has_default_value;
                    next.DefaultValue = $"{info.default_value}";

                }
                return result;
            }

        }

        public IDictionary<string, ParameterMetadata> GetParametersMetadata()
        {
            this.Info($"GetParameterMetadata()");

            using (var db = new ModelsDbContext())
            {
                IDictionary<string, ParameterMetadata> result = new Dictionary<string, ParameterMetadata>();
                var TablesInfoDT = ExecuteQuery($@"SELECT * FROM INFORMATION_SCHEMA.PARAMETERS");
                IEnumerable<ParameterMetadata> ParameterMetadataArr = DataTableService.GetResultSet<ParameterMetadata>(TablesInfoDT);
                List<ParameterMetadata> ParameterMetadataList = new List<ParameterMetadata>(ParameterMetadataArr);
                ParameterMetadataList.Sort((pm1, pm2) => pm2.OrdinalPosition - pm1.OrdinalPosition);
                var ParametersInfos = ExecuteQuery<ParameterDefaultValueInfo>($"SELECT name, has_default_value, default_value FROM sys.parameters pr where pr.object_id = (SELECT object_id FROM sys.procedures)");
                foreach (var next in ParameterMetadataList)
                {
                    var info = ParametersInfos.FirstOrDefault(p => p.name.ToUpper() == next.ParameterName.ToUpper());
                    result[next.ParameterName] = next;
                    next.HasDefaultValue = info.has_default_value;
                    next.DefaultValue = $"{info.default_value}";

                }
                return result;
            }

        }

        public IDictionary<string, ParameterMetadata> GetParameters(string SchemaName, string ProcedureName)
        {
            return GetParametersMetadata(Database, SchemaName, ProcedureName);
        }
        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string DatabaseName, string SchemaName, string ProcedureName)
        {
            this.Info($"GetParameterMetadata({SchemaName},{ProcedureName})");
            string key = $"[{DatabaseName}].[{SchemaName}].[{ProcedureName}]";
            using (var db = new ModelsDbContext())
            {
                IDictionary<string, ParameterMetadata> result = new Dictionary<string, ParameterMetadata>();
                var TablesInfoDT = ExecuteQuery($@"SELECT * FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_SCHEMA='{SchemaName}' AND SPECIFIC_NAME='{ProcedureName}'");
                IEnumerable<ParameterMetadata> ParameterMetadataArr = DataTableService.GetResultSet<ParameterMetadata>(TablesInfoDT);
                List<ParameterMetadata> ParameterMetadataList = new List<ParameterMetadata>(ParameterMetadataArr);
                ParameterMetadataList.Sort((pm1, pm2) => pm2.OrdinalPosition - pm1.OrdinalPosition);
                IEnumerable<ParameterDefaultValueInfo> ParametersInfos = ExecuteQuery<ParameterDefaultValueInfo>($"SELECT name, has_default_value, default_value FROM sys.parameters pr where pr.object_id = (SELECT object_id FROM sys.procedures where name = '{ProcedureName}')");

                foreach (ParameterMetadata next in ParameterMetadataList)
                {
                    result[next.ParameterName] = next;
                    var dbc = new DbConnectionsService();
                    var fullname = $"[{Database}].[{SchemaName}].[{ProcedureName}]".Replace("[[", "[").Replace("]]", "]");
                    var procModel = dbc.GetProcedureModel(ToString(), Database, SchemaName, ProcedureName);
                    if (procModel is not null)
                    {
                        var paramsModels = db.Parameters.Where(p => p.ProcedureModelId == procModel.Id);
                        ParameterModel parModel = paramsModels.FirstOrDefault(p => p.Name.ToLower() == next.ParameterName.ToLower());
                        if (parModel is null)
                        {
                            db.Add(parModel = new ParameterModel()
                            {
                                ProcedureModelId = procModel.Id,
                                Code = next.ParameterName,
                                Name = next.ParameterName,
                                Label = next.ParameterName
                            });
                            db.SaveChanges();
                        }
                    }


                    var info = ParametersInfos.FirstOrDefault(p => p.name.ToUpper() == next.ParameterName.ToUpper());
                    next.HasDefaultValue = info.has_default_value;
                    next.DefaultValue = $"{info.default_value}";





                }
                return result;
            }

        }

        public IEnumerable<string> GetSchemasNames()
        {
            //this.Info($"GetSchemasNames()");
            var schemsResultSet = ExecuteQuery($@"SELECT * FROM sys.schemas");
            IEnumerable<SchemaMetadata> schemas = DataTableService.GetResultSet(schemsResultSet, (data) => new SchemaMetadata()
            {
                Name = $"{data["name"]}",
                Label = $"{data["name"]}"
            });
            return schemas.Select(s => s.Name);
        }

        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName)
        {
            return GetProcedureMetadata(Database, SchemaName, ProcedureName);
        }

        public DatabaseModel GetDatabase(string connectionString)
        {
            return dbConenctionsService.GetDatabase(connectionString);
        }

        public int GetDatabaseId(string connectionString)
        {
            return dbConenctionsService.GetDatabaseId(connectionString);
        }

        public DatabaseConnectionString GetDbConnection(string server)
        {
            return dbConenctionsService.GetDbConnection(server);
        }



        public ProcedureModel GetProcedureModel([NotNullNotEmpty] string connectionString, [NotNullNotEmpty] string database, [NotNullNotEmpty] string schema, [NotNullNotEmpty] string name)
        {
            return dbConenctionsService.GetProcedureModel(connectionString, database, schema, name);
        }

        public ProcedureModel GetProcedureModel(ProcedureMetadata meta)
        {
            return GetProcedureModel(meta.ProcedureOwner, meta.ProcedureName);
        }
        public ProcedureModel GetProcedureModel([NotNullNotEmpty] string schema, [NotNullNotEmpty] string name)
        {
            return dbConenctionsService.GetProcedureModel(ToString(), Database, schema, name);
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata()
        {
            try
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
            catch (Exception ex)
            {
                this.Error($"Исключение при получение сведений о процедурах GetProceduresMetadata() => {ex.Message}\n\n{ToString()}");
                throw new Exception($"Исключение при получение сведений о процедурах GetProceduresMetadata() => {ex.Message}\n\n{ToString()}");
            }
        }


        public IEnumerable<string> GetDatabaseNames()
        {
            var results = ExecuteQuery<Dictionary<string, object>>("select * from sys.databases");
            return results.Select(d => $"{d["name"]}").ToList();
        }


    }
}
