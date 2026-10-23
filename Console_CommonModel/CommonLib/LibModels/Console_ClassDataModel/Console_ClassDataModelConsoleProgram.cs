using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

public static class Console_ClassDataModelConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(Console_ClassDataModel), typeof(ClassModel));
}
