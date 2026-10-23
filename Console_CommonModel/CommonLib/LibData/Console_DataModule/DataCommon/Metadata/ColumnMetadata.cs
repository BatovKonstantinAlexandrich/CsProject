using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata
{
    public class ColumnMetadata : BaseEntity
    {
        public override int Id { get; set; }

        public int TableMetadataId { get; set; }
        public TableMetadata TableMetadata { get; set; }

        [NotNullNotEmpty]
        public string TableCatalog { get; set; }

        [NotNullNotEmpty]
        public string TableSchema { get; set; }

        [NotNullNotEmpty]
        public string TableName { get; set; }

        [NotNullNotEmpty]
        public string ColumnName { get; set; }

        [InputNumber]
        [InputPositive]
        [NotNullNotEmpty]
        public int OrdinalPosition { get; set; }
        public bool IsComputed { get; set; } = false;
        public bool IsNullable { get; set; }
        public string DataType { get; set; }

        [Label("Параметр сортировки")]
        public string CollationName { get; set; }
        public bool IsAutoIncrement { get; set; } = false;
        public string CharacterSetName { get; set; }

        public ColumnMetadata() { }
        public ColumnMetadata(ColumnMetaData columnMetaData)
        {
            DataType = columnMetaData.type;
            ColumnName = columnMetaData.name;
        }







    }
}
