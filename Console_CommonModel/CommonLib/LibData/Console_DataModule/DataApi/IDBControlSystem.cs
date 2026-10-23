namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface IdBControlSystem
    {
        public IEnumerable<string> GetDatabases(string connectionString);
        public IEnumerable<string> GetDatabases();
    }
}
