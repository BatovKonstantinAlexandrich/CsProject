using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.Console_ProgramModule;

public static class Console_ProgramModuleConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_ProgramModule), typeof(ProgramFile));
}
