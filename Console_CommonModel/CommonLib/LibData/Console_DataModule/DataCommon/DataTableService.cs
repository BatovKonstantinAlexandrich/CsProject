



using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public interface IDataTableService
{
    public IEnumerable<string> GetColumnsNames(DataTable dataTable);
    public IDictionary<string, string> GetColumnsCaptions(DataTable dataTable);
    public IDictionary<string, Type> GetDataTypes(DataTable dataTable);
    public IEnumerable<IDictionary<string, string>> GetTextData(DataTable dataTable);
    public IEnumerable<string> GetTextColumn(DataTable dataTable, string columnName);
    public IEnumerable<IDictionary<string, object>> GetRowsData(DataTable dataTable);
    public IEnumerable<TRecord> GetResultSet<TRecord>(DataTable dataTable) where TRecord : class;
    public IEnumerable<TRecord> GetResultSet<TRecord>(DataTable dataTable, Func<Dictionary<string,object>,TRecord> convert) where TRecord : class;
    public JArray GetJArray(DataTable dataTable);
    public IEnumerable<dynamic> GetResultSet(DataTable dataTable, Type entity);
    public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable);
}
public class DataTableService: MyValidatableObject,IDataTableService
{ 
    public IEnumerable<string> GetColumnsNames(DataTable dataTable)
    {
        List<string> columnNames = new List<string>();
        foreach (DataColumn column in dataTable.Columns)
        {
            columnNames.Add(column.ColumnName);
        }
        return columnNames.ToArray();
    }

        
    public IDictionary<string, string> GetColumnsCaptions(DataTable dataTable)
    {
        IDictionary<string, string> result = new Dictionary<string, string>();
        foreach (DataColumn column in dataTable.Columns)
        {
            result[column.ColumnName] = column.Caption;
        }
        return result;
    }

    public IDictionary<string, Type> GetDataTypes(DataTable dataTable)
    {
        IDictionary<string, Type> result = new Dictionary<string, Type>();
        foreach (DataColumn column in dataTable.Columns)
        {
            result[column.ColumnName] = column.DataType;
        }
        return result;
    }


    public IEnumerable<IDictionary<string, string>> GetTextData(DataTable dataTable)
    {
        var result = new List<IDictionary<string, string>>();
        foreach (DataRow row in dataTable.Rows)
        {
            IDictionary<string, string> data = new Dictionary<string, string>();
            foreach (DataColumn column in dataTable.Columns)
            {
                object value = row[column.ColumnName];
                data[column.ColumnName] = value==null? "NULL": value.ToString();
            }
            result.Add(data);
        }
        return result.ToArray();
    }

    public IEnumerable<IDictionary<string, object>> GetRowsData(DataTable dataTable)
    {
        var result = new List<IDictionary<string, object>>();
        foreach (DataRow row in dataTable.Rows)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            foreach (DataColumn column in dataTable.Columns)
            {
                object value = row[column.ColumnName];
                data[column.ColumnName] = value;
            }
            result.Add(data);
        }
        return result.ToArray();
    }

    public IEnumerable<string> GetTextColumn(DataTable dataTable, string columnName)
    {
        List<string> result = new List<string>();
        foreach (DataRow row in dataTable.Rows)
        {
            object value = row[columnName];
            string text = value == null ? "NULL" : value.ToString();
            result.Add(text);
        }
        return result.ToArray();
    }


     

    public IEnumerable<TRecord> GetResultSet<TRecord>(DataTable dataTable) where TRecord: class
    {
        var result = new List<TRecord>();
        Type type = typeof(TRecord);
        if (type.Equals(typeof(Dictionary<string, object>)))
        {
            
            foreach (DataRow row in dataTable.Rows)
            {
                dynamic item = typeof(TRecord).New();
                for (int i=0; i<dataTable.Columns.Count; i++)
                {
                    var column = dataTable.Columns[i];
                    item[column.ColumnName] = row[column.ColumnName];
                }
                result.Add(item);
            }
        }
        else
        {           
            var properties = type.GetOwnPropertyNames();            
            var columns = GetColumnsNames(dataTable);
            
            //this.Info(columns.ToJsonOnScreen());
            foreach (DataRow row in dataTable.Rows)
            {
                object next = type.New();
                foreach (var name in properties)
                {
                    if(name == "ProductCost")
                    {
                        int x = 0;

                    }
                    if (name == "TableSchema")
                    {
                        int x = 0;
                    }
                    string columnName = name.ToTSQLStyle();

                    string key = columns.FirstOrDefault(p => p.ToLower()==columnName.ToLower() || p.ToSnakeStyle().ToLower() == columnName.ToSnakeStyle().ToLower());
                    key = columns.FirstOrDefault(p => p.ToLower() == columnName.ToLower() || p.ToSnakeStyle().ToLower() == columnName.ToSnakeStyle().ToLower());
                    if (columns.Contains(key) == false)
                        continue;
                    try
                    {
                        if (columns.Contains(key) == false)
                            throw new Exception($"Не определена колонка {key} в результирующем наборе [{columns.ToJson().Replace("{", "").Replace("}", "")}]");
                        object value = row[key];
                        if (value != null)
                        {
                            Setter.SetValue(next, name, value.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error("Ошибка при разборе свойства " + name, ex);
                        this.Error(ex);
                    }
                }
                //if (next is MyValidatableObject)
                //    ((MyValidatableObject)next).EnsureIsValide();
                result.Add((TRecord)next);
            }
        }
        return result.ToArray();
    }

    public JArray GetJArray(DataTable dataTable)
    {
        Dictionary<string, object> resultSet = new Dictionary<string, object>();
        List<Dictionary<string, object>> listRow = new List<Dictionary<string, object>>();
        foreach (DataRow row in dataTable.Rows)
        {
            Dictionary<string, object> rowSet = new Dictionary<string, object>();
            foreach (DataColumn column in dataTable.Columns)
            {
                rowSet[column.Caption] = row[column.Caption];
            }
            listRow.Add(rowSet);
        }
        resultSet["rows"] = listRow;

        JObject jrs = JObject.FromObject(resultSet);
        return (JArray)jrs["rows"];
    }

    public IEnumerable<dynamic> GetResultSet(DataTable dataTable, Type entity)
    {
        Type type = entity;
        var properties = type.GetProperties().Select(p => p.Name).ToList();
        var result = new List<dynamic>();
        var columns = GetColumnsNames(dataTable);
        foreach (DataRow row in dataTable.Rows)
        {
            object next = type.New();
            foreach (var name in properties)
            {
                string columnName = name;//.ToTSQLStyle();
                string key = columns.Contains(columnName) ? columnName : columnName.ToCapitalStyle();
                if (columns.Contains(key) == false)
                    continue;
                try
                {
                    object value = row[columnName];
                    if (value != null)
                    {
                            
                        Setter.SetValue(next, name, value.ToString());
                    }
                }
                catch (Exception ex)
                {
                    this.Error("Ошибка при разборе свойства " + name);
                    this.Error(ex);
                }

            }



            if (next is MyValidatableObject)
                ((MyValidatableObject)next).EnsureIsValide();
            result.Add((dynamic)next);




        }
        return result.ToArray();
    }

    public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
    {
        var results = new List<Dictionary<string, object>>();
        foreach (DataRow row in dataTable.Rows)
        {
            Dictionary<string, object> record = new();
            try
            {
                
                foreach (DataColumn column in dataTable.Columns)
                {
                    record[column.ColumnName] = row[column.ColumnName];
                }
                
            }
            catch(Exception ex)
            {
                this.Warn(ex);
            }
            results.Add(record);
        }
        return results;
    }

    public IEnumerable<TRecord> GetResultSet<TRecord>(DataTable dataTable, Func<Dictionary<string, object>, TRecord> convert) where TRecord : class
    {
        var result = new List<TRecord>();
        Type type = typeof(TRecord);   
        foreach (DataRow row in dataTable.Rows)
        {
            Dictionary<string, object> item = new();
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var column = dataTable.Columns[i];
                item[column.ColumnName] = row[column.ColumnName];
            }
            var record = convert(item);
            result.Add(record);
        }                 
        return result.ToArray();
    }
}
