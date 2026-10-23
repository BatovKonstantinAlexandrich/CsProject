namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOServices
{
    [Label("Строка соединения c SqlServer")]
    public class SqlServerADOServiceConnectionString : SqlServerADOService
    {
        private string connectionString;
        public SqlServerADOServiceConnectionString() { }
        public SqlServerADOServiceConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public override string ToString()
        {
            return connectionString;
        }
    }

}
