using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MySqlX.XDevAPI;

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{

    public class SqlServerDDLFactory : IDDLFactory
    {

        public TableInfo CreateTableMetaData(Type typeofEntity)
        {
            TableInfo tmd = new TableInfo();
            tmd.schema = "dbo";
            tmd.name = typeofEntity.Name;
            tmd.columns = new Dictionary<string, ColumnMetaData>();
            foreach (var key in typeofEntity.GetProperties().Select(p => p.Name))
            {
                try
                {
                    var p = typeofEntity.GetProperty(key);
                    var propertyType = typeofEntity.GetProperty(p.Name).PropertyType;
                    var meta = new ColumnMetaData()
                    {
                        primary = p.Name == "Id",
                        incremental = p.Name == "Id",
                        nullable = false,
                        caption = typeofEntity.GetPropertyLabel(p.Name),
                        description = typeofEntity.GetPropertyDescription(p.Name),
                        name = p.Name,// p.Name.ToTSQLStyle(),
                        unique = typeofEntity.IsPropertyUniq(p.Name),
                        cstype = p.PropertyType.GetTypeName(),
                        input_type = typeofEntity.GetPropertyInputType(p.Name)
                    };


                    meta.EnsureIsValide();
                    meta.type =
                        p.Name == "Id" ? "int" :
                            GetDataTypeFor(propertyType);
                    tmd.columns[p.Name] = meta;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            tmd.EnsureIsValide();
            return tmd;
        }

        private bool IsNullable(Type typeofEntity, string name)
        {
            throw new NotImplementedException();
        }

        private string GetDataTypeFor(Type propertyType)
        {
            switch (propertyType.GetTypeName().Trim())
            {

                case "Int32":
                case "Int64":
                case "Int":
                    return "int";
                case "DateTime": return "date";
                case "String": return "nvarchar(max)";
                default: throw new Exception("Не поддерживаемый тип данных " + propertyType.GetTypeName());
            }
        }




        /// <summary>
        /// Создать внешний ключ
        /// </summary>
        /// <returns></returns>
        public string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null)
        {
            return $"ALTER TABLE {table} ADD CONSTRAINT {table + relativeTable} FOREIGN KEY {column} REFERENCES ({relativeTable})" + " ON DELETE " + (onDeleteCascade == true ? "CASCADE" : "NO ACTION ") + " ON UPDATE " + (onUpdateCascade == true ? "CASCADE" : "NO ACTION ");
        }


        /// <summary>
        /// Создать таблицу
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public string DropTable(TableInfo metadata)
        {
            return $"DROP TABLE IF EXISTS  [{metadata.schema}].[{metadata.name}]";
        }
        public string CreateTable(TableInfo metadata)
        {
            string sql = $"CREATE TABLE [{metadata.schema}].[{metadata.name}]\n";
            sql += "(";
            Action<KeyValuePair<string, ColumnMetaData>> AddColumnDefinition = (p) =>
            {
                sql += $"\n  {p.Key/*.ToTSQLStyle()*/} {p.Value.type} ";
                if (p.Key.ToLower() == metadata.pk.ToLower())
                {
                    if (p.Value.incremental)
                    {
                        sql += " Identity(1,1) ";
                    }
                    sql += p.Value.nullable ? " NULL " : "NOT NULL ";
                    sql += " PRIMARY KEY,";
                }
                else
                {
                    sql += p.Value.nullable ? " NULL " : "NOT NULL ";
                    sql += ",";
                }
            };
            foreach (var p in metadata.columns)
            {
                if (p.Key == "Id")
                    AddColumnDefinition(p);
            }
            foreach (var p in metadata.columns)
            {
                if (p.Key != "Id")
                    AddColumnDefinition(p);
            }
            if (metadata.columns.Count > 0)
            {
                sql = sql.Substring(0, sql.Length - 1);
            }
            sql += "\n)\n";
            return sql;
        }

        public string CreateForeignKeys(TableInfo metadata)
        {
            string sql = $"CREATE TABLE [{metadata.schema}].[{metadata.name}]\n";
            sql += "(";
            Action<KeyValuePair<string, ColumnMetaData>> AddColumnDefinition = (p) =>
            {
                sql += $"\n  {p.Key/*.ToTSQLStyle()*/} {p.Value.type} ";
                if (p.Key.ToLower() == metadata.pk.ToLower())
                {
                    if (p.Value.incremental)
                    {
                        sql += "Identity(1,1) ";
                    }
                    sql += p.Value.nullable ? " NULL " : "NOT NULL ";
                    sql += " PRIMARY KEY,";
                }
                else
                {
                    sql += p.Value.nullable ? " NULL " : "NOT NULL ";
                    sql += ",";
                }
            };
            foreach (var p in metadata.columns)
            {
                if (p.Key == "Id")
                    AddColumnDefinition(p);
            }
            foreach (var p in metadata.columns)
            {
                if (p.Key != "Id")
                    AddColumnDefinition(p);
            }
            if (metadata.columns.Count > 0)
            {
                sql = sql.Substring(0, sql.Length - 1);
            }
            sql += "\n)\n";
            return sql;
        }

        public string CreateTable(Type metadata)
        {
            var tableMetaData = CreateTableMetaData(metadata);
            return CreateTable(tableMetaData);
        }

        public string CreateSchema(string schema)
        {
            return $"CREATE SCHEMA {schema}";
        }

        public string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable)
        {
            return $"ALTER TABLE {fullTableName} ADD {propertyName} {type} {(!nullable ? "NOT NULL" : "")}";
        }

        public string DropColumnFromTable(string fullTableName, string propertyName)
        {
            return $"ALTER TABLE {fullTableName} DROP COLUMN {propertyName}";
        }

        public string CreateSelectStatement(string schema, string table)
        {
            return $"SELECT * FROM [{schema}].[{table}]";
        }
    }
}
