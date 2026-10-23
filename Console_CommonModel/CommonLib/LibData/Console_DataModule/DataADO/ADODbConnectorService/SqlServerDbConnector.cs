using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;

using Microsoft.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService
{

    /// <summary>
    /// Сервис выполнения sql-запросов 
    /// </summary>
    [Label("Соединения с SqlServer")]
    public class SqlServerDbConnector : SqlServerConnectionString,
        IDbConnector<SqlConnection>, IDisposable
    {
        private SqlConnection _Connection;


        public SqlServerDbConnector() : base()
        {

        }

        public SqlServerDbConnector(string server, string database) : base(server, database)
        {
        }

        public SqlServerDbConnector(string server, string database, bool trustedConnection, string userId, string password) : base(server, database, trustedConnection, userId, password)
        {
        }

        public SqlConnection GetConnection()
        {
            return CreateAndOpenConnection(); ;
        }


        public new void Dispose()
        {
            if (_Connection != null)
            {
                _Connection.Close();
                _Connection = null;
            }
        }


        public SqlConnection CreateAndOpenConnection()
        {
            //this.Info($"CreateAndOpenConnection() => {base.ToString()}");
            var connectionString = base.ToString();
            var connection = new SqlConnection(connectionString);
            connection.StateChange += OnStateChanged;
            connection.Open();
            return connection;
        }

        public bool CanConnect()
        {
            try
            {
                var tables = new List<string>();
                var ConnectionString = ToString();
                using (SqlConnection con = new SqlConnection("Connection Timeout=1;" + ConnectionString))
                {

                    SqlCommand command = new SqlCommand("SELECT * FROM sys.databases", con);
                    con.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        tables.Add(tableName);
                    }
                    reader.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Info("Ошибка при попытки установить соединение");
                this.Error(ex);
                return false;
            }
        }


        private void OnStateChanged(object sender, StateChangeEventArgs evt)
        {
            //this.Info($"{evt.OriginalState}=>{evt.CurrentState}");
        }


    }
}
