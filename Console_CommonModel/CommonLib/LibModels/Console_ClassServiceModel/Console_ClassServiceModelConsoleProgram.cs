using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;

public static class Console_ClassServiceModelConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_ClassServiceModel), typeof(ClassDbBuilder));
}
