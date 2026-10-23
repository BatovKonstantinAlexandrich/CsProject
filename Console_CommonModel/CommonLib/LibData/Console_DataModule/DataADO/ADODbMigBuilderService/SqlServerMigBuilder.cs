using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Microsoft.EntityFrameworkCore.Migrations;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{
    [Label("Служба миграции SqlServer")]
    public class SqlServerMigBuilder : SqlServerDbModel, IDbMigBuilder, IDMLFactory, IDDLFactory
    {
        protected IDDLFactory DDLFactory = new SqlServerDDLFactory();
        protected IDMLFactory DMLFactory = new SqlServerDMLFactory();

        public SqlServerMigBuilder() : base()
        {
        }

        public SqlServerMigBuilder(string server, string database) : base(server, database)
        {
        }

        public SqlServerMigBuilder(string server, string database, bool trustedConnection, string userId, string password) : base(server, database, trustedConnection, userId, password)
        {
        }

        private string CreateTable(Type EntityType)
            => DDLFactory.CreateTable(EntityType);


        public int CreateDatabase(string name)
        {
            try
            {
                return PrepareQuery($"CREATE DATABASE {name}");
            }
            catch (Exception ex)
            {
                this.Warn($"CreateDatabase({name})", ex);
                return 0;
            }
        }

        public bool HasDatabaseCreated()
        {
            var temp = this.Database;
            this.Database = "master";
            var result = GetDatabaseNames().Select(name => name.ToLower()).Contains(this.Database.ToLower());
            this.Database = temp;
            return result;
        }
        public bool EnsureDatabaseCreated()
        {
            var temp = this.Database;
            this.Database = "master";
            var result = GetDatabaseNames().Select(name => name.ToLower()).Contains(this.Database.ToLower());
            if(!result)
            {
                this.CreateDatabase(temp);
            }
            this.Database = temp;
            return result;
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

        public void CreateOnlyDatabase()
        {
            var tableNames = GetTableNames();
            this.Info(tableNames);
            foreach (var p in EntityTypes)
            {
                var sql = new DbMigCommand(CreateTable(p), DropTable(p), 1)
                {

                };
                this.Info(sql.Up);
                try
                {
                    if (!tableNames.Contains(CreateTableMetaData(p).name.ToMultiCount()))
                    {
                        PrepareQuery(sql.Up);
                    }


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
            //Clear();
            //таблицы
            var tables = GetTableNames();
            this.Info(tables.ToJsonOnScreen());
            this.Info(GetTableFullNames().ToJsonOnScreen());
            foreach (Type EntityType in EntityTypes)
            {
                var TableMetaData = DDLFactory.CreateTableMetaData(EntityType);
                TableMetaData.EnsureIsValide();
                string TableSchema = TableMetaData.schema;
                string TableName = TableMetaData.name;

                string DropTable = @"DROP TABLE  IF EXISTS " + TableName;
                var table = CreateTable(EntityType);
                this.Info(table);

                commands.Add(new DbMigCommand($"{"\n"}" +
                    table,
                    DropTable, commands.Count()));

                PrepareQuery(DropTable);
                PrepareQuery(table);
            }


            //внешние ключи
            foreach (Type EntityType in EntityTypes)
            {
                var keys = GetKeysMetadata();
                var exists = keys.Select(key => key.ToString().ToUpper()).ToList();
                foreach (var ForeignKey in GetForeignKeys(EntityType))
                {
                    var id = $"FK_{ForeignKey.SourceTable.Split(".")[1].ReplaceAll("[", "").ReplaceAll("]", "")}_{ForeignKey.SourceColumn}".ToUpper();
                    if (exists.Contains(id))
                    {
                        commands.Add(new DbMigCommand(
                            $@"ALTER TABLE {ForeignKey.SourceTable}
                                        ADD CONSTRAINT FK_{ForeignKey.SourceTable.Split(".")[1].ReplaceAll("[", "").ReplaceAll("]", "")}_{ForeignKey.SourceColumn} 
                                        FOREIGN KEY ({ForeignKey.SourceColumn.Replace("[", "").Replace("]", "")}) REFERENCES {ForeignKey.TargetTable.Split(".")[1]}({ForeignKey.TargetColumn});",
                            $@"ALTER TABLE {ForeignKey.SourceTable}
                                        DROP CONSTRAINT FK_{ForeignKey.SourceTable.Split(".")[1].Replace("[", "").Replace("]", "")}_{ForeignKey.SourceColumn} ", commands.Count()));
                    }
                    else
                    {
                        this.Info($"Внешний ключ уже существует: {id}");
                    }

                    //commands.Add($"ALTER TABLE {ForeignKey.SourceTable} "+
                    //    $@"ADD CONSTRAINT FK_{ForeignKey.SourceTable.ReplaceAll(".","_")}_{ForeignKey.TargetTable.ReplaceAll(".","_")} FOREIGN KEY ({ForeignKey.SourceColumn}) REFERENCE {ForeignKey.TargetTable}({ForeignKey.TargetColumn})");
                }
            }
            /*}
            catch (Exception ex)
            {
                var TypeName = MethodBase.GetCurrentMethod().DeclaringType.GetTypeName();
                var MethodName = MethodBase.GetCurrentMethod().Name;                                                
                this.Error($"Метод действия {TypeName}.{MethodName} не выполнен", ex);
            }*/

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
                string RefTableSchema = TableMetaData.schema;
                string RefTableName = TableMetaData.name;
                return $"[{RefTableSchema}].[{RefTableName}]";
            };
            var keys = entityType.GetProperties().Where(property => IsMapped(entityType, property) && IsPrimitive(property.PropertyType) == false)
                .Select(property => new KeyMetadata()
                {
                    SourceTable = $"[{TableSchema}]" + "." + $"[{TableName}]",
                    SourceColumn = property.Name/*.ToTSQLStyle()*/ + "_Id",
                    TargetTable = GetRefenceTableName(property),
                    TargetColumn = "Id"
                }).ToArray();
            this.Info(keys);
            return keys; ;
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




        public void TestProcedures()
        {
            var api = new SqlServerWebApi();
            foreach (var p in api.GetProceduresMetadata())
            {
                var input = new Dictionary<string, string>(p.Value.ParametersMetadata.Keys.Select(key => new KeyValuePair<string, string>(key, "1")));
                api.Info(input.ToJsonOnScreen());
                api.Info(api.Request($"/{p.Key}", input).Result);
            }
        }
        public void UpdateDatabase()
        {
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
                    Catch(() => PrepareQuery(message), (ex) => { this.Error(ex); });
                    this.Info(message.Split("/n"));
                };
                messages.AddRange(new List<string>(message.Split("/n")));
            }

        }

        private void Catch(Func<int> p1, Action<object> p2)
        {
            throw new NotImplementedException();
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

        public string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable)
        {
            return DDLFactory.AddColumnToTable(fullTableName, propertyName, type, nullable);
        }

        public string CreateForeignKeys(TableInfo metadata)
        {
            return DDLFactory.CreateForeignKeys(metadata);
        }

        public string CreateSchema(string schema)
        {
            return DDLFactory.CreateSchema(schema);
        }

        public string DropColumnFromTable(string fullTableName, string propertyName)
        {
            return DDLFactory.DropColumnFromTable(fullTableName, propertyName);
        }

        public string DropTable(TableInfo metadata)
        {
            return DDLFactory.DropTable(metadata);
        }
        public string DropTable(Type metadata)
        {

            return DDLFactory.DropTable(CreateTableMetaData(metadata));
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
