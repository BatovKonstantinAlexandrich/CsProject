using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SqlServerDMLFactory: IDMLFactory
{
    private const string PrimaryKeyName = "Id";

    public string CreateInsertStatement(string schema, string table, Dictionary<string, object> values)
    {
        string keys = "";
        string valuesstr = "";

        foreach (var p in values)
        {
            keys += p.Key + ",";
            valuesstr += toSqlValueStr(InferSqlType(p.Value), p.Value) + ",";
        }

        keys = keys.Substring(0, keys.Length - 1);
        valuesstr = valuesstr.Substring(0, valuesstr.Length - 1);
        return "insert into " + $"[{schema}].[{table}]" + "(" + keys + ") values (" + valuesstr + ")";
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
            setup.Append(p.Key).Append('=').Append(toSqlValueStr(InferSqlType(p.Value), p.Value));
        }

        if (setup.Length == 0)
            throw new ArgumentException("Нет колонок для обновления", nameof(values));

        var pkValue = toSqlValueStr(InferSqlType(values[pkName]), values[pkName]);
        return $"UPDATE [{schema}].[{table}] SET {setup} WHERE {pkName} = {pkValue}";
    }

    private static string InferSqlType(object val)
    {
        if (val == null)
            return "nvarchar";

        return val switch
        {
            DateTime => "datetime",
            int or long or short => "int",
            float or double or decimal => "float",
            bool => "smallint",
            _ => "nvarchar"
        };
    }

    /// <summary>
    /// Преобразование данных к формату SQL
    /// </summary>
    /// <param name="name"> имя атрибута сущности </param>
    /// <param name="val"> значение </param>
    /// <returns></returns>
    private string toSqlValueStr(string type, object val)
    {



        if (val == null || val.ToString() == "NULL")
        {
            return "NULL";
        }
        string value = "";
        switch (type)
        {
            case "date":
                DateTime date = (DateTime)val;
                value =
                    "DateFromParts(" + date.Year + "," + date.Month + "," + date.Day + ")";
                break;
            case "datetime":
                DateTime datetime = (DateTime)val;
                value =
                    "DateTimeFromParts(" + datetime.Year + "," + datetime.Month + "," + datetime.Day +
                    $",{datetime.Hour},{datetime.Minute},{datetime.Second},0)";
                break;
            case "datetime2":
                DateTime datetime2 = (DateTime)val;
                value =
                    "DateTimeFromParts(" + datetime2.Year + "," + datetime2.Month + "," + datetime2.Day +
                    $",{datetime2.Hour},{datetime2.Minute},{datetime2.Second},0)";
                break;
            /*case "date":
                DateTime date = (DateTime)val;
                value = "'" + date.Year + "-" + date.Month + "-" + date.Day + "'";
                break;
            case "datetime":
                DateTime datetime = (DateTime)val;
                value = 
                    "'" + datetime.Year + "-" + datetime.Month + "-" + datetime.Day + 
                    $" {datetime.Hour}:{datetime.Minute}:{datetime.Second}'";
                break;*/
            case "ntext":
            case "text":
            case "nvarchar":
            case "varchar":
                value = "'" + val.ToString().Replace("'", "\"") + "'";
                break;
            case "smallint":
            case "int":
            case "float":
                value = "" + val.ToString().Replace(",", ".") + "";
                break;
            default:
                throw new Exception("Тип данных " + type + " пока не поддерживается");
        }

        return value;
    }
}
 