using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{
    public interface IDbMigBuilder : IdbModel
    {

        void CreateDatabase();
        int CreateDatabase(string name);
        string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null);
        string CreateInsertStatement(string schema, string table, Dictionary<string, object> values);
        string CreateSelectStatement(string schema, string table);
        string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values);
        string CreateCreateStatement(string schema, string table);

        string CreateTable(TableInfo metadata);
        TableInfo CreateTableMetaData(Type metadata);
        DbMigCommand[] DropAndCreate();
        void TestProcedures();
        void UpdateDatabase();
        IDictionary<string, TableMetadata> UpdateTablesMetadata();



    }
}
