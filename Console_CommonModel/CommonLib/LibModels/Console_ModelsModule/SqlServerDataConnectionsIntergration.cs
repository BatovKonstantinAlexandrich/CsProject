using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public class SqlServerDataConnectionsIntergration :
        DataIntegration<ModelsDbContext, SqlServerConnectionString, DatabaseConnectionString>
    {
        public SqlServerDataConnectionsIntergration()
        {
        }

        public SqlServerDataConnectionsIntergration(ModelsDbContext dbContext) : base(dbContext)
        {

        }

        protected override DatabaseConnectionString FindDataObject(SqlServerConnectionString integrated)
        {
            var connectionString = integrated.ToString();
            using (var db = new ModelsDbContext())
            {
                var connection = db.DatabaseConnections.FirstOrDefault(con => con.ConnectionString.ToUpper() == connectionString.ToUpper());
                return connection;
            }
        }

        protected override DatabaseConnectionString CreateDataObject(SqlServerConnectionString integrated)
        {
            return new DatabaseConnectionString()
            {
                ConnectionString = integrated.ToString()
            };
        }
    }
}
