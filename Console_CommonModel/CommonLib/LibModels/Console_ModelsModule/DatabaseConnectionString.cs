using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public class DatabaseConnectionString : BaseEntity
    {
        [NotNullNotEmpty]
        public string ConnectionString { get; set; }
    }
}