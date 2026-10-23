using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;


namespace Console_CommonModel.CommonLib.LibModels.Console_ProgramModule
{
    public class ProgramFile : NamedObject
    {
        [NotNullNotEmpty]
        public int FileResourceId { get; set; }

        [NotNullNotEmpty]
        public FileResource FileResource { get; set; }


    }
}