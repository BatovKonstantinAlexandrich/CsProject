using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;

namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule;

public static class Console_StatisticsModuleConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_StatisticsModule), typeof(BusinessDbContext));
}
