using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.Manager;

using Newtonsoft.Json.Linq;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface ITableManagerStatefull
    {
        long Count();
        long Create(Dictionary<string, object> values);
        long Delete(long id);
        OdbcTableManager Get(string table);
        object GetAssociations(string key);
        object GetCommands();
        object GetErrors();
        IDictionary<string, int> GetKeywords();
        TableInfo GetMetadata();
        object GetPopular();
        JArray Join(long id, string table);
        JArray Search(List<string> terms, string query);
        JToken Select(long id);
        JArray SelectAll();
        object SelectNotReferencesTo(string table, long record_id);
        JArray SelectPage(long page, long size);
        object SelectReferencesFrom(string table, long record_id);
        object SelectReferencesTo(string table, long record_id);
        long Update(Dictionary<string, object> values);
        object WhereColumnValueIn(string column, JArray values);
    }
}