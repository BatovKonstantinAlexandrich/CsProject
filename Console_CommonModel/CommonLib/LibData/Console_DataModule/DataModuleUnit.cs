using System;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.InputFormsAPI;


namespace Console_CommonModel.CommonLib.LibData.Console_DataModule
{
    [Label("Тестировыание уровня данных")]
    public class DataModuleUnit : TestingUnit
    {
        public DataModuleUnit() : base(AppProviderService.GetInstance())
        {
            Append(new DataAdoUnit(provider));
            Append(new SqlServerInputFormsApiTest(provider));
        }


        public DataModuleUnit(IServiceProvider sp)
        {

            Append(new ChangingDataListTest(sp));
            Append(new DataAdoUnit(sp));
            Append(new SqlServerInputFormsApiTest(sp));
        }


    }
}
