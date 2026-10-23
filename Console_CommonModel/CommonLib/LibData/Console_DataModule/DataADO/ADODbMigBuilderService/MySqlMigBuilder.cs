using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{

    [Label("Сервис управления миграциями MySQL")]
    public class MySqlMigBuilder : MySqlDbModel, IDbMigBuilder, IDDLFactory, IDMLFactory
    {
        protected IDDLFactory DDLFactory = new MySqlDDLFactory();
        protected IDMLFactory DMLFactory = new MySqlDMLFactory();

        public HashSet<IEntityFasade> Services { get; set; } = new();
        public HashSet<IProcedureExecuter> Procedures { get; set; } = new();

        public MySqlMigBuilder() : base()
        {
        }

        public MySqlMigBuilder(string server, string database) : base(server, database)
        {

        }


        private string CreateTable(Type EntityType)
            => DDLFactory.CreateTable(EntityType);

        public int CreateDatabase(string name)
        {
            return PrepareQuery($"CREATE DATABASE IF NOT EXISTS `{name}`");
        }

        public void CreateDatabase()
        {
            foreach (var sql in DropAndCreate())
            {
                this.Info(sql.Up);
                try
                {
                    PrepareQuery(sql.Down);
                }
                catch (Exception ex)
                {
                    this.Error($"Ошибка при выполнении {sql.Down}: {ex.Message}");
                }
                try
                {
                    PrepareQuery(sql.Up);
                }
                catch (Exception ex)
                {
                    this.Error($"Ошибка при выполнении {sql.Up}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Команды создания структуры данных
        /// </summary>       
        public DbMigCommand[] DropAndCreate()
        {
            this.Info($"DropAndCreate()");
            List<DbMigCommand> commands = new List<DbMigCommand>();

            var tables = GetTableNames();
            this.Info(tables.ToJsonOnScreen());

            foreach (Type EntityType in EntityTypes)
            {
                var TableMetaData = DDLFactory.CreateTableMetaData(EntityType);
                string TableSchema = TableMetaData.schema;
                string TableName = TableMetaData.name;

                string DropTable = $"DROP TABLE IF EXISTS `{TableName}`";
                var table = CreateTable(EntityType);
                this.Info(table);

                commands.Add(new DbMigCommand($"{"\n"}" +
                    table,
                    DropTable, commands.Count()));

                //PrepareQuery(DropTable);
                //PrepareQuery(table);
            }

            //внешние ключи
            foreach (Type EntityType in EntityTypes)
            {
                var keys = GetKeysMetadata();
                var exists = keys.Select(key => key.ToString().ToUpper()).ToList();
                foreach (var ForeignKey in GetForeignKeys(EntityType))
                {
                    var sourceTable = ForeignKey.SourceTable.Replace("[", "").Replace("]", "").Replace("`", "");
                    var targetTable = ForeignKey.TargetTable.Replace("[", "").Replace("]", "").Replace("`", "");
                    var sourceColumn = ForeignKey.SourceColumn.Replace("[", "").Replace("]", "").Replace("`", "");
                    var targetColumn = ForeignKey.TargetColumn.Replace("[", "").Replace("]", "").Replace("`", "");

                    var id = $"FK_{sourceTable}_{sourceColumn}".ToUpper();
                    if (!exists.Contains(id))
                    {
                        commands.Add(new DbMigCommand(
                            $@"ALTER TABLE `{sourceTable}`
                                        ADD CONSTRAINT `FK_{sourceTable}_{sourceColumn}` 
                                        FOREIGN KEY (`{sourceColumn}`) REFERENCES `{targetTable}`(`{targetColumn}`);",
                            $@"ALTER TABLE `{sourceTable}`
                                        DROP FOREIGN KEY `FK_{sourceTable}_{sourceColumn}`", commands.Count()));
                    }
                    else
                    {
                        this.Info($"Внешний ключ уже существует: {id}");
                    }
                }
            }

            return commands.ToArray();
        }

        private IEnumerable<KeyMetadata> GetForeignKeys(Type entityType)
        {
            var TableMetaData = DDLFactory.CreateTableMetaData(entityType);
            string TableSchema = TableMetaData.schema;
            string TableName = TableMetaData.name;

            Func<PropertyInfo, string> GetRefenceTableName = (info) =>
            {
                var RefTableMetaData = DDLFactory.CreateTableMetaData(info.PropertyType);
                string RefTableSchema = RefTableMetaData.schema;
                string RefTableName = RefTableMetaData.name;
                return $"`{RefTableName}`";
            };

            var keys = entityType.GetProperties().Where(property => entityType.IsMapped(property.Name) && property.PropertyType.IsPrimitive() == false)
                .Select(property => new KeyMetadata()
                {
                    SourceTable = $"`{TableName}`",
                    SourceColumn = property.Name + "_Id",
                    TargetTable = GetRefenceTableName(property),
                    TargetColumn = "Id"
                }).ToArray();
            this.Info(keys);
            return keys;
        }

        private bool IsMapped(Type type, PropertyInfo property)
        {
            return
                Utils.ForProperty2(type, property.Name).ContainsKey(nameof(NotMappedAttribute)) == false &&
                Utils.ForProperty2(type, property.Name).ContainsKey("NotMapped") == false;
        }

        string[] PrimitiveTypeNames = new string[]
        {
            nameof(Byte),
            nameof(IEnumerable<Byte>),
            nameof(Boolean),
            nameof(Option),
            nameof(Int32),
            nameof(Int64),
            nameof(DateTime)
        };

        private bool IsPrimitive(Type propertyType)
        {
            return PrimitiveTypeNames.Contains(propertyType.Name);
        }

        public void UpdateDatabase()
        {
            if (!GetTableNames().Contains(nameof(DbMigCommand)))
                TryPrepareQuery(CreateTable(typeof(DbMigCommand)));

            var messages = new List<string>();
            var migration = DropAndCreate();

            foreach (var mig in migration)
            {
                //log
                string message = mig.Up
                        .ReplaceAll("  ", " ")
                        .ReplaceAll("  ", " ");
                Action todo = () =>
                {
                    //exec
                    Catch(() => PrepareQuery(message), (ex) => { this.Error(ex); });
                    this.Info(message.Split("/n"));
                };

                // Выполняем миграцию
                todo();

                messages.AddRange(new List<string>(message.Split("/n")));
            }
        }

        private void Catch(Func<int> p1, Action<object> p2)
        {
            try
            {
                p1();
            }
            catch (Exception ex)
            {
                p2(ex);
            }
        }

        public string CreateInsertStatement(string schema, string table, Dictionary<string, object> values)
        {
            return DMLFactory.CreateInsertStatement(schema, table, values);
        }

        public string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null)
        {
            return DDLFactory.CreateForeignkey(relativeTable, table, column, onDeleteCascade, onUpdateCascade);
        }

        string IDDLFactory.CreateTable(Type metadata)
        {
            return DDLFactory.CreateTable(metadata);
        }

        public TableInfo CreateTableMetaData(Type metadata)
        {
            return DDLFactory.CreateTableMetaData(metadata);
        }

        public string CreateTable(TableInfo metadata)
        {
            return DDLFactory.CreateTable(metadata);
        }

        public string DropTable(TableInfo metadata) => DDLFactory.DropTable(metadata);

        public string CreateSchema(string schema) => DDLFactory.CreateSchema(schema);

        public string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable)
            => DDLFactory.AddColumnToTable(fullTableName, propertyName, type, nullable);

        public string DropColumnFromTable(string fullTableName, string propertyName)
            => DDLFactory.DropColumnFromTable(fullTableName, propertyName);

        public string CreateForeignKeys(TableInfo metadata) => DDLFactory.CreateForeignKeys(metadata);

        public void TestProcedures()
        {
            // Для MySQL можно реализовать тестирование процедур/функций
            var procedures = GetProceduresMetadata();
            foreach (var proc in procedures)
            {
                //this.Info($"Процедура: {proc.Key}");
                // Здесь можно добавить логику тестирования процедур
            }
        }

        public IDictionary<string, TableMetadata> UpdateTablesMetadata()
        {
            // Делегируем вызов базовому классу MySqlDbMetadata
            return base.UpdateTablesMetadata();
        }

        public string CreateSelectStatement(string schema, string table)
        {
            return DDLFactory.CreateSelectStatement(schema, table);
        }

        public string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values)
        {
            return DMLFactory.CreateUpdateStatement(schema, table, values);
        }

        public string CreateCreateStatement(string schema, string table)
        {
            var entityType = EntityTypes.FirstOrDefault(t => string.Equals(t.Name, table, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException($"Тип сущности для таблицы {schema}.{table} не найден", nameof(table));
            return DDLFactory.CreateTable(entityType);
        }
    }
}
