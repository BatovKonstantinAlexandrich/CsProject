using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_ProgramModule;

[Label("ProgramModule: ProgramFile связан с FileResource")]
[Description("ProgramFile хранит ссылку на FileResource для исполняемого или конфигурационного файла программы.")]
public class ProgramModuleProgramFileResourceLinkTest : TestingElement
{
    public ProgramModuleProgramFileResourceLinkTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("ProgramFile + FileResource (app.exe)");
        Assert(_ =>
        {
            var file = new ProgramFile
            {
                Name = "Main",
                FileResourceId = 1,
                FileResource = new FileResource { Name = "app.exe" }
            };
            LogMessage($"FileResourceId={file.FileResourceId}, FileResource.Name={file.FileResource.Name}");
            return file.FileResourceId == 1 && file.FileResource.Name == "app.exe";
        },
        "ProgramFile хранит ссылку на FileResource",
        "ProgramFile не связан с FileResource");
    }
}
