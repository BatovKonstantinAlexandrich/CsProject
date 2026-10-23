using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public interface IDbConnectionsService
    {
        DatabaseModel GetDatabase(string connectionString);
        int GetDatabaseId(string connectionString);
        DatabaseConnectionString GetDbConnection(string server);
        public ProcedureModel GetProcedureModel([NotNullNotEmpty] string connectionString, [NotNullNotEmpty] string database, [NotNullNotEmpty] string schema, [NotNullNotEmpty] string name);
    }
}