using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;

public static class Console_ServiceModuleConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_ServiceModule), typeof(ServiceModuleTest));
}
