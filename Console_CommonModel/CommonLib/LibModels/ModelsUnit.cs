using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using Console_CommonModel.CommonLib.LibModels.Console_DbContextModule;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule;
using Console_CommonModel.CommonLib.LibModels.Console_DllModule;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;
using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule;
using Console_CommonModel.CommonLib.LibModels.Console_ProgramModule;
using Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule;
using Console_CommonModel.CommonLib.LibModels.Console_WebModule;
using Console_CommonModel.CommonLib.LibModels.Console_XmlModule;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibModels.XmlAppModel;

namespace Console_CommonModel.CommonLib.LibModels;

[Label("Модуль тестирования моделей данных")]
public class ModelsUnit : TestingUnit
{
    public ModelsUnit(IServiceProvider parent) : base(parent)
    {
        Append(new FileModuleTest(parent));
        Append(new ServiceModuleTest(parent));
        Append(new ClassDataModelModuleTest(parent));
        Append(new ClassServiceModelModuleTest(parent));
        Append(new DbContextModuleTest(parent));
        Append(new DeliveryModuleTest(parent));
        Append(new DllModuleTest(parent));
        Append(new InputModuleTest(parent));
        Append(new LocationModuleTest(parent));
        Append(new ModelsModuleTest(parent));
        Append(new MvcModuleTest(parent));
        Append(new ProgramModuleTest(parent));        
        Append(new ServiceBuilderModuleTest(parent));
        Append(new BusinessAnaliticsUnit(parent));
        Append(new WebModuleTest(parent));
        Append(new XmlModuleTest(parent));
        Append(new XmlAppModelModuleTest(parent));
    }

    public override TestingReport DoTest(bool interactive = false, bool force = false)
    {
        return base.DoTest(interactive, force);
    }
}
