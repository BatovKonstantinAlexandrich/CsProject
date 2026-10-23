using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryDataServices;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule;

public static class Console_DeliveryModuleConsoleProgram
{
    public static void Run() =>
        LibModelsConsoleEngine.RunModule(
            nameof(Console_DeliveryModule),
            typeof(DeliveryDbContext),
            dedicatedProgram: DeliveryProgram.Run);
}
