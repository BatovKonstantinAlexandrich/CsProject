using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService
{
    public interface ISqlExecutor
    {

        public IDbCommand CreateCommand<TIDbCommand>();
        public IDbCommand CreateCommand();
        public JArray GetJsonResult(string sql);
        public JObject GetSingleJObject(string command);
        public DataTable ExecuteQuery(string command);
        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class;
        public IEnumerable<dynamic> ExecuteQuery(string command, Type entity);
        public int TryPrepareQuery(string command);
        public int PrepareQuery(string command);
        public DataTable ExecuteProcedure(string name, IDictionary<string, object> input);
        public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText][NotNullNotEmpty()] string name, IDictionary<string, string> input) where TResult : class;

        public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable);

        public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output);
        public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output);
    }
}
