using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;

using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService
{
    /// <summary>
    ///  
    /// </summary>
    [Label("Сервис установки соединения с MySQL")]
    public class MySqlDbConnector : MySqlConnectionString,
        IDbConnector<MySqlConnection>,
        IDisposable
    {

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private MySqlConnection _Connection;


        public MySqlDbConnector() : base()
        {
            this.Info("Create");
        }
        public MySqlDbConnector(string server, string database) : base(server, database)
        {

        }
        public MySqlConnection GetConnection()
        {
            //if (_Connection == null)
            //{
            _Connection = CreateAndOpenConnection();
            //}
            return _Connection;
        }



        public void Dispose()
        {
            if (_Connection != null)
            {
                _Connection.Close();
            }
        }



        public MySqlConnection CreateAndOpenConnection()
        {
            var connection = new MySqlConnection(base.ToString());

            connection.StateChange += OnStateChanged;
            connection.Open();
            return connection;
        }

        private void OnStateChanged(object sender, StateChangeEventArgs evt)
        {
            this.Info($"{evt.OriginalState}=>{evt.CurrentState}");
        }

        public bool CanConnect()
        {
            return false;
        }
    }
}

