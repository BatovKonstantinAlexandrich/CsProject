using ActionModels;

using System;

namespace Console_CommonModel.CommonLib.LibModels.Console_DllModule
{
    /// <summary>Сохранённая скомпилированная сборка (байты DLL).</summary>
    public class DllAssemblyRecord : NamedObject
    {


        public string DbContextTypeName { get; set; }

        /// <summary>Имена ClassModel, вошедших в сборку (через запятую).</summary>
        public string IncludedClassNames { get; set; }

        /// <summary>Id классов в ClientClassDbContext для пересборки (через запятую).</summary>
        public string IncludedClassIds { get; set; }

        /// <summary>Полное имя интерфейса расширения из загруженной DLL (например MyApp.IOrderExtras).</summary>
        public string ExtensionInterfaceFullName { get; set; }

        /// <summary>Поле делегирования в генерируемом partial (по умолчанию _extensionService).</summary>
        public string ExtensionServiceFieldName { get; set; }

        /// <summary>Кэш последнего сгенерированного glue-кода (необязательно).</summary>
        public string ExtensionGlueSource { get; set; }

        public byte[] DllBytes { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}
