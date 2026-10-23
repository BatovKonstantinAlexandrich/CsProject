using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API
{
    public interface IApi : IDbMigBuilder
    {
        HashSet<IProcedureExecuter> Procedures { get; set; }
        HashSet<IEntityFasade> Services { get; set; }

        IDbMetadata GetModel();


        ITableManager GetFasade(string schema, string table);
        IEntityFasade<T> GetFasade<T>() where T : BaseEntity;

        void Init();
        void InitServices();

    }
}