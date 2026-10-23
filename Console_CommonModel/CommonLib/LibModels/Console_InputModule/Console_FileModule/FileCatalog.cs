using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule
{
    /// <summary>
    /// Модель файлового каталога
    /// </summary>
    [Label("Файловый каталог")]

    public class FileCatalog : HierTable<FileCatalog>
    {

        public FileCatalog()
        {
            Files = new ChangingList<FileResource>();
            Name = "D:\\";
        }

        public FileCatalog(string name)
        {
            Files = new ChangingList<FileResource>();
            Name = name;
        }



        public virtual ChangingList<FileResource> Files { get; set; } = new();
    }
}
