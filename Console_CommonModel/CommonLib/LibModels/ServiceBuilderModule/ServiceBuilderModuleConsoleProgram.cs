using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

public static class ServiceBuilderModuleConsoleProgram
{
    public static void Run() =>
        LibModelsConsoleEngine.RunModule(
            nameof(ServiceBuilderModule),
            typeof(ServiceBuilderProgram),
            dedicatedProgram: () => ServiceBuilderProgram.Run(Array.Empty<string>(), loadRegisteredServices: false));
}
