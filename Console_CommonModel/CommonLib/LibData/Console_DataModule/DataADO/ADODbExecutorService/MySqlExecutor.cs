using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService
{
    [Label("Сервис выполнения sql-запросов MySQL")]
    public class MySqlExecutor : MySqlDbConnector, ISqlExecutor
    {


        public MySqlExecutor(string server, string database) : base(server, database)
        {
        }

        public MySqlExecutor()
        {
        }

        public IDbCommand CreateCommand<TIDbCommand>()
        {
            throw new NotImplementedException();
        }
        public void SetConnectionString(string str) { }
        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public JArray GetJsonResult(string sql)
        {
            throw new NotImplementedException();
        }

        public JObject GetSingleJObject(string command)
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteQuery(string command)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> ExecuteQuery(string command, Type entity)
        {
            throw new NotImplementedException();
        }

        public int TryPrepareQuery(string command)
        {
            throw new NotImplementedException();
        }

        public int PrepareQuery(string command)
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteProcedure(string name, IDictionary<string, string> input)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
        {
            throw new NotImplementedException();
        }

        public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteProcedure(string name, IDictionary<string, object> input)
        {
            throw new NotImplementedException();
        }


        /*private IDataTableService DataTableService = new DataTableService();

public MySqlExecutor() : base()
{
}
public MySqlExecutor(string server, string database) : base(server, database)
{

}

public JArray GetJsonResult(string sql)
{
this.Info($"GetJsonResult({sql})");
DataTable ResultDataTable = ExecuteQuery(sql);
JArray JResult = DataTableService.GetJArray(ResultDataTable);
return JResult;
}

public JObject GetSingleJObject(string command)
{
this.Info($"GetSingleJObject({command})");
DataTable ResultDataTable = ExecuteQuery(command);
JArray JResult = DataTableService.GetJArray(ResultDataTable);
JToken token = JResult.FirstOrDefault();
return token != null ? (JObject)token : null;
}

public DataTable ExecuteQuery(string command)
{
this.Info($"ExecuteQuery: {command}");
using (var con = GetConnection())
{
DataTable dataTable = new DataTable();
MySqlCommand commandObj = new MySqlCommand(command, con);
commandObj.CommandType = CommandType.Text;
var reader = commandObj.ExecuteReader();
dataTable.Load(reader);
this.Info($"{command}=>{dataTable.Rows.Count}");
return dataTable;
}
}

public int PrepareQuery(string command)
{
this.Info($"PrepareQuery: \n{command}");
try
{
using (var con = GetConnection())
{
MySqlCommand commandObj = new MySqlCommand(command, con);
commandObj.CommandType = CommandType.Text;
int result = commandObj.ExecuteNonQuery();
return result;
}
}
catch (Exception ex)
{
this.Error(ex);
throw;
}
}

public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
{
try
{
MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { command, input, output });

this.Info($"ExecuteProcedure({command})");
MySqlCommand cmd = new MySqlCommand(command, GetConnection());
cmd.CommandType = CommandType.StoredProcedure;

foreach (var kv in input)
{
var param = new MySqlParameter(kv.Key, kv.Value);
cmd.Parameters.Add(param);
}

// Добавляем выходные параметры
foreach (var kv in output)
{
var param = new MySqlParameter(kv.Key, MySqlDbType.VarChar);
param.Direction = ParameterDirection.Output;
cmd.Parameters.Add(param);
}

int result = cmd.ExecuteNonQuery();

// Заполняем выходные параметры
foreach (var kv in output)
{
if (cmd.Parameters.Contains(kv.Key))
{
output[kv.Key] = cmd.Parameters[kv.Key].Value?.ToString() ?? string.Empty;
}
}

return result;
}
catch (Exception ex)
{
this.Error(ex.ToDocument());
throw new Exception($"Не удалось выполнить процедуру {command}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
}
}

public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
{
this.Info(this.ToString());
var dt = ExecuteQuery(command);
return DataTableService.GetResultSet<TEntity>(dt);
}

public int TryPrepareQuery(string command)
{
try
{
return PrepareQuery(command);
}
catch (Exception ex)
{
this.Error(ex);
return 0;
}
}

public IEnumerable<dynamic> ExecuteQuery(string command, Type entity)
{
var items = DataTableService.GetResultSet(ExecuteQuery(command), entity);
this.Info(items.ToJsonOnScreen());
return items;
}

public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
{
try
{
this.Info($"ExecuteProcedure({command})");
MySqlCommand cmd = new MySqlCommand(command, GetConnection());
cmd.CommandType = CommandType.StoredProcedure;

for (int i = 0; i < input.Count; i++)
{
var param = new MySqlParameter($"@param{i}", input[i] ?? DBNull.Value);
cmd.Parameters.Add(param);
}

// Добавляем выходные параметры
foreach (var kv in output)
{
var param = new MySqlParameter(kv.Key, MySqlDbType.VarChar);
param.Direction = ParameterDirection.Output;
cmd.Parameters.Add(param);
}

int result = cmd.ExecuteNonQuery();

// Заполняем выходные параметры
foreach (var kv in output)
{
if (cmd.Parameters.Contains(kv.Key))
{
output[kv.Key] = cmd.Parameters[kv.Key].Value?.ToString() ?? string.Empty;
}
}

return result;
}
catch (Exception ex)
{
this.Error(ex.ToDocument());
throw new Exception($"Не удалось выполнить процедуру {command}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
}
}

public DataTable ExecuteProcedure(string name, IDictionary<string, string> input)
{
try
{
MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input });

this.Info($"ExecuteProcedure({name})");

DataTable table = new DataTable();
using (var con = GetConnection())
using (var cmd = new MySqlCommand(name, con))
using (var da = new MySqlDataAdapter(cmd))
{
cmd.CommandType = CommandType.StoredProcedure;
foreach (var kv in input)
{
if (kv.Value is null)
{
var p = new MySqlParameter();
p.ParameterName = kv.Key;
p.Value = DBNull.Value;
cmd.Parameters.Add(p);
}
else
{
cmd.Parameters.Add(new MySqlParameter(kv.Key, kv.Value));
}
}
da.Fill(table);
}

return table;
}
catch (Exception ex)
{
this.Error(ex.ToDocument());
throw new Exception($"Не удалось выполнить процедуру {name}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
}
}

internal void SetConnectionString(string connectionString)
{
var dictionary = new Dictionary<string, string>(connectionString.Split(";").Select(s => 
{ 
var arr = s.Split("="); 
return new KeyValuePair<string, string>(arr[0], arr.Length > 1 ? arr[1] : string.Empty); 
}).ToList());
this.SetTextValues(dictionary);
}

public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input, IDictionary<string, string> output) where TResult : class
{
try
{
var result = new List<TResult>();
MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input, output });

this.Info($"ExecuteProcedure({name})");
MySqlCommand command = new MySqlCommand(name, GetConnection());
command.CommandType = CommandType.StoredProcedure;

foreach (var kv in input)
{
command.Parameters.Add(new MySqlParameter(kv.Key, kv.Value));
}

// Добавляем выходные параметры
foreach (var kv in output)
{
var param = new MySqlParameter(kv.Key, MySqlDbType.VarChar);
param.Direction = ParameterDirection.Output;
command.Parameters.Add(param);
}

var reader = command.ExecuteReader();
while (reader.Read())
{
var item = typeof(TResult).New<TResult>();
foreach (var property in typeof(TResult).GetInputProperties())
{
try
{
if (reader[property] != DBNull.Value)
{
item.SetValue(property, reader[property]);
}
}
catch (Exception ex)
{
this.Warn($"Ошибка при установке свойства {property}: {ex.Message}");
}
}
result.Add(item);
}
reader.Close();

// Заполняем выходные параметры
foreach (var kv in output)
{
if (command.Parameters.Contains(kv.Key))
{
output[kv.Key] = command.Parameters[kv.Key].Value?.ToString() ?? string.Empty;
}
}

return result;
}
catch (Exception ex)
{
this.Error(ex.ToDocument());
throw new Exception($"Не удалось выполнить процедуру {name}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
}
}

public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class
{
throw new NotImplementedException();
}

public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
{
throw new NotImplementedException();
}

public string GetConnectionString()
=> ToString();

public IDbCommand CreateCommand<TIDbCommand>()
{
throw new NotImplementedException();
}

public IDbCommand CreateCommand()
{
throw new NotImplementedException();
}

public IDbTransaction BeginTransaction()
{
throw new NotImplementedException();
}

public IDbTransaction BeginTransaction(IsolationLevel il)
{
throw new NotImplementedException();
}

public void ChangeDatabase(string databaseName)
{
throw new NotImplementedException();
}

public void Close()
{
throw new NotImplementedException();
}

public void Open()
{
throw new NotImplementedException();
}

public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

public int ConnectionTimeout => throw new NotImplementedException();

public ConnectionState State => throw new NotImplementedException();*/
    }
}