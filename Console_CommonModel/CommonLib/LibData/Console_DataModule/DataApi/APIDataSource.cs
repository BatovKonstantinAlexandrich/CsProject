using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Text;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface APIdataSource
    {
        DatabaseMetadata GetDatabaseMetadata();
        JArray GetJsonResult(string sql);
        JObject GetSingleJObject(string command);

        bool canConnect();
        bool canReadAndWrite();
        bool canCreateAlterTables();

        string GetConenctionString();
        object SingleSelect(string sql);
        object MultiSelect(string sql);





        object Prepare(string sql);

        IEnumerable<string> GetTables();
        void InsertBlob(string sql, string v, byte[] data);
    }
}
