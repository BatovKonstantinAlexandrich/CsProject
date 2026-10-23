using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule;

public static class Console_WebModuleConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_WebModule), typeof(OdbcMiddleware));
}
