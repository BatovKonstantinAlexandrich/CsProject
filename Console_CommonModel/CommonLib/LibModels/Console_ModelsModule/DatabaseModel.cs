using System.ComponentModel.DataAnnotations;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    [Label("Модель базы данных")]
    public class DatabaseModel : IntegratedObject
    {
        [NotNullNotEmpty]
        public int DatabaseConnectionId { get; set; }
        public DatabaseConnectionString DatabaseConnection { get; set; }
        /*public int? SqlServerConnectionStringId { get; set; }
        public SqlServerConnectionString SqlServerConnectionString { get; set; }
        public int? MySqlConnectionStringId { get; set; }
        public MySqlConnectionString MySqlConnectionString { get; set; }
        public int? PostgresConnectionStringId { get; set; }
        public PostgresConnectionString PostgresConnectionString { get; set; }*/


    }
}
