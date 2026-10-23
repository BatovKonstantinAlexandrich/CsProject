using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.Console_DbContextModule;

public static class Console_DbContextModuleConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_DbContextModule), typeof(DbContextAdapter));
}
