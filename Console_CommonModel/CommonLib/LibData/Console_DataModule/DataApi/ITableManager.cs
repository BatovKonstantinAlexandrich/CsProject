using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

using Newtonsoft.Json.Linq;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface ITableManager
    {
        int Count();
        int Create();
        int Create(Dictionary<string, object> values);
        int Delete(long id);
        List<string> GetKeywords(string entity, string keywordsQuery);
        TableInfo GetMetadata();
        JArray Join(long id, string table);
        JArray Search(string entity, string searchedQuery);
        JArray Select(int id);
        JArray Select(long id);
        JArray SelectAll();
        int SelectMaxId();
        JArray SelectPage(long page, long size);
        int Update(Dictionary<string, object> values);
    }
}