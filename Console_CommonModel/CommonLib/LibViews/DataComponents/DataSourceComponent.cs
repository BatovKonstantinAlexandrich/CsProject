using System.ComponentModel;
using System.Data;
using Newtonsoft.Json.Linq;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;

namespace Console_CommonModel.CommonLib.LibViews.DataComponents
{


    /// <summary>
    /// Компонент для работы с источниками данных
    /// </summary>
    [Label("Источник данных")]
    [Description("Компонент для подключения к базам данных (SQL Server, MySQL, PostgreSQL, Oracle) и выполнения SQL-запросов, получения метаданных и работы с хранимыми процедурами")]
    public class DataSourceComponent : Component, ISqlExecutor
    {
        public bool IsConnected { get; set; }
        public string ConnectionString { get; set; } = "Data Source=DESKTOP-IHJM9RD;Initial Catalog=DeliveryDbContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public DataSourceComponentDriver Driver { get; set; } = DataSourceComponentDriver.SqlServer;


        public enum DataSourceComponentDriver
        {
            MySQL, SqlServer, Oracle, Postgres
        }

        public List<string> GetProcedures()
        {
            return GetDbMetadata().GetProceduresMetadata().Keys.ToList();
        }

        public void CloseConnection()
        {

        }

        public JArray GetJsonResult(string sql)
        {
            return GetAdoDatasource().GetJsonResult(sql);
        }

        public ISqlExecutor GetAdoDatasource()
        {
            switch (Driver)
            {
                case DataSourceComponentDriver.SqlServer:
                    var sql = new SqlServerMigBuilder();
                    sql.SetConnectionString(ConnectionString);
                    return sql;
                case DataSourceComponentDriver.Postgres:
                    var pg = new PostgresDbMetadata();
                    pg.SetConnectionString(ConnectionString);
                    return pg;
                case DataSourceComponentDriver.MySQL:
                    var mysql = new MySqlDbMetadata();
                    mysql.SetConnectionString(ConnectionString);
                    return mysql;
                case DataSourceComponentDriver.Oracle:
                default:
                    throw new NotImplementedException();
            }
        }
        public IDbMetadata GetDbMetadata()
        {
            switch (Driver)
            {
                case DataSourceComponentDriver.SqlServer:
                    var sql = new SqlServerMigBuilder();
                    sql.SetConnectionString(ConnectionString);
                    return sql;
                case DataSourceComponentDriver.Postgres:
                    var pg = new PostgresDbMetadata();
                    pg.SetConnectionString(ConnectionString);
                    return pg;
                case DataSourceComponentDriver.MySQL:
                    var mysql = new MySqlDbMetadata();
                    mysql.SetConnectionString(ConnectionString);
                    return mysql;
                case DataSourceComponentDriver.Oracle:
                default:
                    throw new NotImplementedException();
            }
        }

        public JObject GetSingleJObject(string command)
        {
            return GetAdoDatasource().GetSingleJObject(command);
        }

        public DataTable ExecuteQuery(string command)
        {
            return GetAdoDatasource().ExecuteQuery(command);
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
        {
            return GetAdoDatasource().ExecuteQuery<TEntity>(command);
        }

        public IEnumerable<dynamic> ExecuteQuery(string command, Type entity)
        {
            return GetAdoDatasource().ExecuteQuery(command, entity);
        }

        public int TryPrepareQuery(string command)
        {
            return GetAdoDatasource().TryPrepareQuery(command);
        }

        public int PrepareQuery(string command)
        {
            return GetAdoDatasource().PrepareQuery(command);
        }

        public DataTable ExecuteProcedure(string name, IDictionary<string, object> input)
        {
            return GetAdoDatasource().ExecuteProcedure(name, input);
        }

        public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class
        {
            return GetAdoDatasource().ExecuteProcedure<TResult>(name, input);
        }

        public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
        {
            return GetAdoDatasource().ExecuteProcedure(command, input, output);
        }

        public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
        {
            return GetAdoDatasource().ExecuteProcedure(command, input, output);
        }

        public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
        {
            return GetAdoDatasource().GetResultSetDictionary(dataTable);
        }

        public string GetConnectionString()
        {
            return GetAdoDatasource().ToString();
        }

        public IDbCommand CreateCommand<TIDbCommand>()
        {
            return GetAdoDatasource().CreateCommand<TIDbCommand>();
        }

        public IDbCommand CreateCommand()
        {
            return GetAdoDatasource().CreateCommand();
        }
    }
}

