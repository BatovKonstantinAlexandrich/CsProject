using Console_CommonModel.CommonLib.LibModels.ConsoleAgents;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using Console_CommonModel.CommonLib.LibModels.Console_DbContextModule;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule;
using Console_CommonModel.CommonLib.LibModels.Console_DllModule;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule;
using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule;
using Console_CommonModel.CommonLib.LibModels.Console_ProgramModule;
using Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule;
using Console_CommonModel.CommonLib.LibModels.Console_WebModule;
using Console_CommonModel.CommonLib.LibModels.Console_XmlModule;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibModels.XmlAppModel;
using Console_CommonModel.CommonLib.Others;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;

namespace Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

/// <summary>
/// Агент запуска консольных программ всех подкаталогов LibModels.
/// </summary>
public static class ProgAgent
{
    readonly record struct ModuleEntry(string Title, Action Run);

    static readonly ModuleEntry[] Modules =
    {
        new("Console_ClassDataModel", () => Console_ClassDataModelConsoleProgram.Run()),
        new("Console_ClassServiceModel", () => Console_ClassServiceModelConsoleProgram.Run()),
        new("Console_DbContextModule", () => Console_DbContextModuleConsoleProgram.Run()),
        new("Console_DeliveryModule", () => Console_DeliveryModuleConsoleProgram.Run()),
        new("Console_DllModule", () => Console_DllModuleConsoleProgram.Run()),
        new("Console_FileModule", () => Console_FileModuleConsoleProgram.Run()),
        new("Console_InputModule", () => Console_InputModuleConsoleProgram.Run()),
        new("Console_LocationModule", () => Console_LocationModuleConsoleProgram.Run()),
        new("Console_MvcModule", () => Console_MvcModuleConsoleProgram.Run()),
        new("Console_ProgramModule", () => Console_ProgramModuleConsoleProgram.Run()),
        new("Console_ServiceModule", () => Console_ServiceModuleConsoleProgram.Run()),
        new("Console_StatisticsModule", () => Console_StatisticsModuleConsoleProgram.Run()),
        new("Console_WebModule", () => Console_WebModuleConsoleProgram.Run()),
        new("Console_XmlModule", () => Console_XmlModuleConsoleProgram.Run()),
        new("ServiceBuilderModule", () => ServiceBuilderModuleConsoleProgram.Run()),
        new("XmlAppModel", () => XmlAppModelConsoleProgram.Run()),
    };

    public static void Run()
    {
        var args = new string[0];
        InputConsole.Write("ProgAgent — консольные интерфейсы LibModels");
        var exit = false;
        while (!exit)
        {
            var options = Modules.Select(m => m.Title).ToList();
            InputConsole.SelectListWithExit("Выберите модуль LibModels", options, choice =>
            {
                if (choice == "Выход")
                    exit = true;
                else
                    Modules.First(m => m.Title == choice).Run();
            }, ref args);
        }
    }
}
