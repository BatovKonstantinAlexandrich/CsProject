using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

using System;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{
    public interface IDDLFactory
    {
        string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable);
        string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null);
        string CreateForeignKeys(TableInfo metadata);
        string CreateSchema(string schema);
        string CreateSelectStatement(string schema, string table);
        string CreateTable(TableInfo metadata);
        string CreateTable(Type metadata);
        TableInfo CreateTableMetaData(Type typeofEntity);
        string DropColumnFromTable(string fullTableName, string propertyName);
        string DropTable(TableInfo metadata);

    }
}