
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{
    public class DataAdoUnit : TestingUnit
    {
        public DataAdoUnit() : this(AppProviderService.GetInstance()) { }
        public DataAdoUnit(IServiceProvider sp)
        {
            Append(new EntityRelationsTest(sp));

            Append(new ADODbConnectorsTest(sp));
            Append(new ADODbExecutorTest(sp));
            Append(new ADODbMetadataTest(sp));
            Append(new ADODbMigBuilderTest(sp));
        }
    }
}
