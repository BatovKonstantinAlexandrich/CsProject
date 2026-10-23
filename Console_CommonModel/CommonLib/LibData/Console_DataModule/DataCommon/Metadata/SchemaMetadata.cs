using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata
{
    public class SchemaMetadata : DescriptableObject
    {
        public int DatabaseModelId { get; set; }
        public DatabaseModel DatabaseModel { get; set; }

        public List<ProcedureMetadata> Procedures { get; set; }
        public List<TableMetadata> Tables { get; set; }
    }
}
