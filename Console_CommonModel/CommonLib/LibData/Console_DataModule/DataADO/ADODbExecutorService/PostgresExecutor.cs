using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Npgsql;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService
{
    [Label("Выполнение операций в Postgres")]

    public class PostgresExecutor : PostgresDbConnector, ISqlExecutor
    {
        protected readonly IDataTableService DataTableService = new DataTableService();

        public PostgresExecutor() : base()
        {
        }

        public PostgresExecutor(string dataSource, int port, string database, string userId, string password)
            : base(dataSource, port, database, userId, password)
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
            //this.Info($"ExecuteQuery: {command}");
            using (var con = GetConnection())
            {
                DataTable dataTable = new DataTable();
                NpgsqlCommand commandObj = new NpgsqlCommand(command, con);

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
                    NpgsqlCommand commandObj = new NpgsqlCommand(command, con);
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

        public int ExecuteProcedure(string command, IDictionary<string, object> input, IDictionary<string, object> output)
        {
            try
            {
                MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { command, input, output });

                this.Info($"ExecuteProcedure({command})");
                NpgsqlCommand cmd = new NpgsqlCommand(command, GetConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var kv in input)
                {
                    var param = new NpgsqlParameter(kv.Key, kv.Value);
                    cmd.Parameters.Add(param);
                }

                // Добавляем выходные параметры
                foreach (var kv in output)
                {
                    var param = new NpgsqlParameter(kv.Key, NpgsqlTypes.NpgsqlDbType.Text);
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
            this.Info(ToString());
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
                NpgsqlCommand cmd = new NpgsqlCommand(command, GetConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < input.Count; i++)
                {
                    var param = new NpgsqlParameter($"param{i}", input[i] ?? DBNull.Value);
                    cmd.Parameters.Add(param);
                }

                // Добавляем выходные параметры
                foreach (var kv in output)
                {
                    var param = new NpgsqlParameter(kv.Key, NpgsqlTypes.NpgsqlDbType.Text);
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

        public DataTable ExecuteProcedure(string name, IDictionary<string, object> input)
        {
            try
            {
                MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input });

                this.Info($"ExecuteProcedure({name})");

                DataTable table = new DataTable();
                using (var con = GetConnection())
                using (var cmd = new NpgsqlCommand(name, con))
                using (var da = new NpgsqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var kv in input)
                    {
                        if (kv.Value is null)
                        {
                            var p = new NpgsqlParameter();
                            p.ParameterName = kv.Key;
                            p.Value = DBNull.Value;
                            cmd.Parameters.Add(p);
                        }
                        else
                        {
                            cmd.Parameters.Add(new NpgsqlParameter(kv.Key, kv.Value));
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
                return new KeyValuePair<string, string>(arr[0].Trim(), arr.Length > 1 ? arr[1].Trim() : string.Empty);
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
                NpgsqlCommand command = new NpgsqlCommand(name, GetConnection());
                command.CommandType = CommandType.StoredProcedure;

                foreach (var kv in input)
                {
                    command.Parameters.Add(new NpgsqlParameter(kv.Key, kv.Value));
                }

                // Добавляем выходные параметры
                foreach (var kv in output)
                {
                    var param = new NpgsqlParameter(kv.Key, NpgsqlTypes.NpgsqlDbType.Text);
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
            try
            {
                var result = new List<TResult>();
                MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input });

                this.Info($"ExecuteProcedure({name})");
                NpgsqlCommand command = new NpgsqlCommand(name, GetConnection());
                command.CommandType = CommandType.StoredProcedure;

                foreach (var kv in input)
                {
                    command.Parameters.Add(new NpgsqlParameter(kv.Key, kv.Value));
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

                return result;
            }
            catch (Exception ex)
            {
                this.Error(ex.ToDocument());
                throw new Exception($"Не удалось выполнить процедуру {name}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
            }
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

        public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
        {
            throw new NotImplementedException();
        }
    }
}