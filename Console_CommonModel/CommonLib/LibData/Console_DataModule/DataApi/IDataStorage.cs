namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface IdataStorage
    {

        public void RestoreDatabaseFrom(
                string ConnectionString,
                string FilePath,
                bool FullMode);
        public void BackupDatabaseToFile(
                string ConnectionString,
                string FilePath,
                bool FullMode);
    }
}