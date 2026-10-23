using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{
    public class MySqlDMLFactory : IDMLFactory
    {
        private const string PrimaryKeyName = "Id";

        public string CreateInsertStatement(string schema, string table, Dictionary<string, object> values)
        {
            string keys = "";
            string valuesstr = "";

            foreach (var p in values)
            {
                keys += $"`{p.Key}`,";
                valuesstr += toSqlValueStr(InferSqlType(p.Value), p.Value) + ",";
            }

            if (keys.Length > 0)
                keys = keys.Substring(0, keys.Length - 1);
            if (valuesstr.Length > 0)
                valuesstr = valuesstr.Substring(0, valuesstr.Length - 1);

            string tableName = string.IsNullOrEmpty(schema) ? $"`{table}`" : $"`{schema}`.`{table}`";
            return $"INSERT INTO {tableName} ({keys}) VALUES ({valuesstr})";
        }

        public string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("Не заданы значения для обновления", nameof(values));

            var pkName = values.Keys.FirstOrDefault(k => string.Equals(k, PrimaryKeyName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException($"Первичный ключ '{PrimaryKeyName}' должен присутствовать в values", nameof(values));

            var setup = new StringBuilder();
            foreach (var p in values)
            {
                if (string.Equals(p.Key, pkName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (setup.Length > 0)
                    setup.Append(',');
                setup.Append('`').Append(p.Key).Append('`')
                    .Append('=')
                    .Append(toSqlValueStr(InferSqlType(p.Value), p.Value));
            }

            if (setup.Length == 0)
                throw new ArgumentException("Нет колонок для обновления", nameof(values));

            var tableName = string.IsNullOrEmpty(schema) ? $"`{table}`" : $"`{schema}`.`{table}`";
            var pkValue = toSqlValueStr(InferSqlType(values[pkName]), values[pkName]);
            return $"UPDATE {tableName} SET {setup} WHERE `{pkName}` = {pkValue}";
        }

        private static string InferSqlType(object val)
        {
            if (val == null)
                return "varchar";

            return val switch
            {
                DateTime => "datetime",
                int or long or short => "int",
                float or double or decimal => "decimal",
                bool => "bit",
                _ => "varchar"
            };
        }

        private string toSqlValueStr(string type, object val)
        {
            if (val == null || val.ToString() == "NULL")
            {
                return "NULL";
            }

            string value = "";
            switch (type.ToLower())
            {
                case "date":
                    DateTime date = (DateTime)val;
                    value = $"'{date:yyyy-MM-dd}'";
                    break;
                case "datetime":
                case "timestamp":
                    DateTime datetime = (DateTime)val;
                    value = $"'{datetime:yyyy-MM-dd HH:mm:ss}'";
                    break;
                case "text":
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "longtext":
                case "mediumtext":
                case "tinytext":
                    value = "'" + val.ToString().Replace("'", "''") + "'";
                    break;
                case "tinyint":
                case "smallint":
                case "int":
                case "integer":
                case "bigint":
                case "float":
                case "double":
                case "decimal":
                case "numeric":
                    value = val.ToString().Replace(",", ".");
                    break;
                case "bit":
                case "boolean":
                    value = (bool)val ? "1" : "0";
                    break;
                default:
                    throw new Exception($"Тип данных {type} пока не поддерживается");
            }

            return value;
        }
    }
}
