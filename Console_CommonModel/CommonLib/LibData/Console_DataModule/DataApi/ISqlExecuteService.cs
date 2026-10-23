using System.Data;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface ISqlExecuteService
    {
        public DataTable ExecuteQuery(string ConnectionString, string SQL);
        public int PrepareQuery(string ConnectionString, string SQL);
    }
}
