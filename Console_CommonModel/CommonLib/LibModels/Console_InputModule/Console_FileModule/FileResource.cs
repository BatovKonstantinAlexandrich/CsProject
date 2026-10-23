using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule
{
    [Label("Файловый ресурс")]

    public class FileResource : HierTable<FileCatalog>
    {
        [NotNullNotEmpty("Необходимо указать наименование ресурса")]
        [Label("Наименование")]
        public override string Name { get; set; }

        [Label("Корневой каталог")]
        [DisplayName("Корневой каталог")]
        [NotInput("")]
        [NotNullNotEmpty]
        public override int? ParentId { get; set; }

        [NotNull]
        [Label("Тип файла")]
        [InputSelect("text/html,text/cs")]
        [NotNullNotEmpty("Необходимо ввести задать тип ресурса (MimeType)")]
        public string Mime { get; set; }


        [Label("Файл")]
        [InputFile("")]
        public byte[] Data { get; set; }

        [Label("Дата загрузки")]
        [InputDateTime()]
        [NotInput("")]
        [NotNullNotEmpty("Необходимо указать время создания ресурса")]
        public DateTime Created { get; set; } = DateTime.Now;

    }
}
