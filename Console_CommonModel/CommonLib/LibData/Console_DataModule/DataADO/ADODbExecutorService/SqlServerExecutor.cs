using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService;

using Dapper;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon;

using Microsoft.Data.SqlClient;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService
{

    [Label("Выполнение операций с источником SqlServer")]
    public class SqlServerExecutor : SqlServerDbConnector, ISqlExecutor

    {
        protected readonly IDataTableService DataTableService = new DataTableService();

        public SqlServerExecutor() : base()
        {

        }

        public SqlServerExecutor(string server, string database) : base(server, database)
        {
        }

        public SqlServerExecutor(string server, string database, bool trustedConnection, string userId, string password) : base(server, database, trustedConnection, userId, password)
        {
        }
        public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable) => DataTableService.GetResultSetDictionary(dataTable);
        public IEnumerable<TRecord> GetResultSet<TRecord>(DataTable dataTable) where TRecord : class => DataTableService.GetResultSet<TRecord>(dataTable);

        public int ExecuteProcedure([InputEngText][NotNullNotEmpty()] string name, IDictionary<string, string> input, IDictionary<string, string> output)
        {
            try
            {
                MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input, output });


                this.Info($"ExecuteProcedure({name})");
                SqlCommand command = new SqlCommand(name, GetConnection());
                command.CommandType = CommandType.StoredProcedure;


                foreach (var kv in input)
                {
                    command.Parameters.Add(new SqlParameter(kv.Key, kv.Value));
                }
                var list = new List<Dictionary<string, object>>();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var item = new Dictionary<string, object>();
                    foreach (var property in reader.GetColumnSchema().Select(p => p.ColumnName))
                    {
                        item[property] = reader[property];
                    }
                    list.Add(item);
                }
                return 1;
            }
            catch (Exception ex)
            {
                this.Error(ex.ToDocument());
                throw new Exception($"Не удалось выполнить процедуру {name}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
            }
        }

        public DataTable ExecuteProcedure([InputEngText][NotNullNotEmpty()] string name, IDictionary<string, object> input)
        {
            /*try
            {
                MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input });


                this.Info($"ExecuteProcedure({name})");
      

                DataTable table = new DataTable();
                using (var con = GetConnection())
                using (var cmd = new SqlCommand(name, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var kv in input)
                    {
                        if(kv.Value is null)
                        {
                            var p = new SqlParameter();
                            p.ParameterName = kv.Key;
                            p.Value = kv.Value;
                            cmd.Parameters.Add(p);
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter(kv.Key, kv.Value));
                        }
                        
                    }
                    con.Open();

                    da.Fill(table);
                }
                 
                return table;
            }
            catch (Exception ex)
            {
                this.Error(ex.ToDocument());
                throw new Exception($"Не удалось выполнить процедуру {name}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
            }*/

            try
            {
                MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input });


                this.Info($"ExecuteProcedure({name})");
                SqlCommand command = new SqlCommand(name, GetConnection());
                command.CommandType = CommandType.StoredProcedure;
                foreach (var kv in input)
                {
                    command.Parameters.Add(new SqlParameter(kv.Key, kv.Value));
                }

                var data = new DataTable();

                var reader = command.ExecuteReader();
                var da = new SqlDataAdapter(command);
                da.Fill(data);

                return data;
            }
            catch (Exception ex)
            {
                this.Error(ex.ToDocument());
                throw new Exception($"Не удалось выполнить процедуру {name}\nс параметрами\n{input.ToJsonOnScreen()}", ex);
            }
        }



        public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText][NotNullNotEmpty()] string name, IDictionary<string, string> input) where TResult : class
        {
            try
            {
                var result = new List<TResult>();
                MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { name, input });


                this.Info($"ExecuteProcedure({name})");
                SqlCommand command = new SqlCommand(name, GetConnection());
                command.CommandType = CommandType.StoredProcedure;
                foreach (var kv in input)
                {
                    command.Parameters.Add(new SqlParameter(kv.Key, kv.Value));
                }
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var item = typeof(TResult).New<TResult>();
                    foreach (var property in typeof(TResult).GetInputProperties())
                    {
                        item.SetValue(property, reader[property]);
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

        public JArray GetJsonResult(string sql)
        {
            this.Info($"GetJsonResult({sql})");
            DataTable ResultDataTable = ExecuteQuery(sql);
            JArray JResult = DataTableService.GetJArray(ResultDataTable);
            return JResult;
        }

        public JObject GetSingleJObject(string sql)
        {
            this.Info($"GetSingleJObject({sql})");
            DataTable ResultDataTable = ExecuteQuery(sql);
            JArray JResult = DataTableService.GetJArray(ResultDataTable);
            JToken token = JResult.FirstOrDefault();
            return token != null ? (JObject)token : null;
        }


        /// <summary>
        /// Выполнение запроса с 1 результирующим набором
        /// </summary>
        public DataTable ExecuteQuery(string tsql)
        {
            //this.Info($"ExecuteQuery: {tsql}");
            using (var con = GetConnection())
            {
                DataTable dataTable = new DataTable();
                //this.Info($"{tsql}=>{dataTable.Rows.Count}");
                SqlCommand command = new SqlCommand(tsql, GetConnection());
                command.CommandType = CommandType.Text;
                var reader = command.ExecuteReader();
                dataTable.Load(reader);

                /*using (SqlDataAdapter adapter = new SqlDataAdapter(tsql, con))
                {
                    adapter.Fill(dataTable);
                }*/
                return dataTable;
            }

        }

        /// <summary>
        /// Выполнение простой команды (без результирующего набора)
        /// </summary>
        public int PrepareQuery(string tsql)
        {
            //Clear();
            this.Info($"PrepareQuery: \n{tsql}");
            try
            {
                using (var con = GetConnection())
                {
                    SqlCommand command = new SqlCommand(tsql, con);
                    int result = command.ExecuteNonQuery();
                    return result;
                }
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка при выполнении запроса: {tsql}\n\n{ex.Message}\n\n{ex.StackTrace}");
                throw new Exception($"Ошибка при выполнении запроса: {tsql}\n\n{ex.Message}\n\n{ex.StackTrace}");
            }
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
        {
            //this.Info(this.ToString());
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



        public void SetConnectionString(string connectionString)
        {
            var dictionary = new Dictionary<string, string>(connectionString.Split(";").Select(s => { var arr = s.Split("="); return new KeyValuePair<string, string>(arr[0], arr[1]); }).ToList());
            this.SetTextValues(dictionary);
        }

        public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString()
        {
            return ToString();
        }

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


    }
}
