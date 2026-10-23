using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

public static class Console_LocationModuleConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_LocationModule), typeof(LocationDbContext));
}
