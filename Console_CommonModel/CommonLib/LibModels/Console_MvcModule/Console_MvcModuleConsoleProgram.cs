using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule;

public static class Console_MvcModuleConsoleProgram
{
    public static void Run() =>
        LibModelsConsoleEngine.RunModule(
            nameof(Console_MvcModule),
            typeof(MyMvcProgram),
            dedicatedProgram: MyMvcProgram.Run);
}
