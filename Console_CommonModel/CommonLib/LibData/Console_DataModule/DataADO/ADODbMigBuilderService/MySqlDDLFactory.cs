using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{
    public class MySqlDDLFactory : IDDLFactory
    {
        public TableInfo CreateTableMetaData(Type typeofEntity)
        {
            TableInfo tmd = new TableInfo();
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
                        name = p.Name,
                        unique = typeofEntity.IsPropertyUniq(p.Name),
                        cstype = p.PropertyType.GetTypeName(),
                        input_type = typeofEntity.GetPropertyInputType(p.Name)
                    };

                    meta.EnsureIsValide();
                    meta.type =
                        p.Name == "Id" ? "INT" :
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

        private string GetDataTypeFor(Type propertyType)
        {
            switch (propertyType.GetTypeName().Trim())
            {
                case "Int32":
                case "Int":
                    return "INT";
                case "Int64":
                    return "BIGINT";
                case "DateTime":
                    return "DATETIME";
                case "String":
                    return "VARCHAR(255)";
                case "Boolean":
                case "Bool":
                    return "BOOLEAN";
                case "Decimal":
                    return "DECIMAL(18,2)";
                case "Double":
                    return "DOUBLE";
                case "Float":
                    return "FLOAT";
                default:
                    throw new Exception("Не поддерживаемый тип данных " + propertyType.GetTypeName());
            }
        }

        public string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null)
        {
            string constraintName = $"FK_{table}_{relativeTable}_{column}";
            string onDelete = onDeleteCascade == true ? "CASCADE" : "NO ACTION";
            string onUpdate = onUpdateCascade == true ? "CASCADE" : "NO ACTION";

            // Убираем обратные кавычки для упрощения, если они есть
            table = table.Replace("`", "").Replace("[", "").Replace("]", "");
            relativeTable = relativeTable.Replace("`", "").Replace("[", "").Replace("]", "");
            column = column.Replace("`", "").Replace("[", "").Replace("]", "");

            return $"ALTER TABLE `{table}` ADD CONSTRAINT `{constraintName}` FOREIGN KEY (`{column}`) REFERENCES `{relativeTable}`(`Id`) ON DELETE {onDelete} ON UPDATE {onUpdate}";
        }

        public string CreateTable(Type metadata)
        {
            var tableMetaData = CreateTableMetaData(metadata);
            return CreateTable(tableMetaData);
        }

        public string DropTable(TableInfo metadata)
        {
            if (metadata is null)
                throw new ArgumentNullException(nameof(metadata));

            return $"DROP TABLE IF EXISTS {FormatTableName(metadata.schema, metadata.name)}";
        }

        public string CreateSchema(string schema)
        {
            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentException("Имя схемы не задано", nameof(schema));

            return $"CREATE SCHEMA IF NOT EXISTS {QuoteIdentifier(NormalizeIdentifier(schema))}";
        }

        public string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable)
        {
            var column = QuoteIdentifier(NormalizeIdentifier(propertyName));
            var nullClause = nullable ? "NULL" : "NOT NULL";
            return $"ALTER TABLE {NormalizeTableReference(fullTableName)} ADD COLUMN {column} {type} {nullClause}";
        }

        public string DropColumnFromTable(string fullTableName, string propertyName)
        {
            var column = QuoteIdentifier(NormalizeIdentifier(propertyName));
            return $"ALTER TABLE {NormalizeTableReference(fullTableName)} DROP COLUMN {column}";
        }

        public string CreateForeignKeys(TableInfo metadata)
        {
            if (metadata?.fk is null || metadata.fk.Count == 0)
                return string.Empty;

            var builder = new StringBuilder();
            foreach (var entry in metadata.fk)
            {
                if (builder.Length > 0)
                    builder.AppendLine();

                builder.Append(CreateForeignkey(entry.Value, metadata.name, entry.Key));
            }

            return builder.ToString();
        }

        public string CreateTable(TableInfo metadata)
        {
            string tableName = FormatTableName(metadata.schema, metadata.name);
            string sql = $"CREATE TABLE {tableName}\n";
            sql += "(";

            Action<KeyValuePair<string, ColumnMetaData>> AddColumnDefinition = (p) =>
            {
                sql += $"\n  `{p.Key}` {p.Value.type} ";
                if (p.Key.ToLower() == metadata.pk.ToLower())
                {
                    if (p.Value.incremental)
                    {
                        sql += " AUTO_INCREMENT ";
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
            sql += "\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;\n";
            return sql;
        }

        private static string FormatTableName(string schema, string table) =>
            string.IsNullOrWhiteSpace(schema)
                ? QuoteIdentifier(NormalizeIdentifier(table))
                : $"{QuoteIdentifier(NormalizeIdentifier(schema))}.{QuoteIdentifier(NormalizeIdentifier(table))}";

        private static string NormalizeTableReference(string fullTableName)
        {
            if (string.IsNullOrWhiteSpace(fullTableName))
                throw new ArgumentException("Имя таблицы не задано", nameof(fullTableName));

            var normalized = fullTableName.Trim()
                .Replace("[", "")
                .Replace("]", "");

            if (normalized.Contains('.'))
            {
                var parts = normalized.Split('.', 2);
                return FormatTableName(
                    NormalizeIdentifier(parts[0].Trim('`')),
                    NormalizeIdentifier(parts[1].Trim('`')));
            }

            return QuoteIdentifier(NormalizeIdentifier(normalized.Trim('`')));
        }

        private static string QuoteIdentifier(string identifier) => $"`{identifier}`";

        private static string NormalizeIdentifier(string identifier) =>
            identifier?.Trim().Trim('"').Trim('`') ?? string.Empty;

        public string CreateSelectStatement(string schema, string table)
        {
            return $"SELECT * FROM {FormatTableName(schema, table)}";
        }
    }
}
