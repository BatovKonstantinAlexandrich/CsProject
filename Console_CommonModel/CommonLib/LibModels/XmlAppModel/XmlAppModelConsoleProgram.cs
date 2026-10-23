using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.XmlAppModel;

public static class XmlAppModelConsoleProgram
{
    public static void Run() => LibModelsConsoleEngine.RunModule(nameof(XmlAppModel), typeof(XmlAppDbContext));
}
