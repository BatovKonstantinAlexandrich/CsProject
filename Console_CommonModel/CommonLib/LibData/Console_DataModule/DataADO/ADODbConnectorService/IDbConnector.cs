using System.Data.Common;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService
{
    public interface IDbConnector<TDBConnection> where TDBConnection : DbConnection
    {
        public TDBConnection CreateAndOpenConnection();
        public TDBConnection GetConnection();
        public bool CanConnect();
    }
}


