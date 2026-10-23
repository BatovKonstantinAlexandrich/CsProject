using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.Data;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata
{
    [Label("Свойства параметра процедуры")]
    public class ParameterMetadata : BaseEntity
    {
        public int ProcedureId { get; set; }
        public ProcedureMetadata Procedure { get; set; }


        [Label("База данных")]
        public string SpecificCatalog { get; set; }

        [Label("Схема")]
        public string SpecificSchema { get; set; }

        /// <summary>
        /// Имя процедуры
        /// </summary>
        [Label("Наименование")]
        public string SpecificName { get; set; }

        [Label("Кодировка")]
        public string CharacterSetName { get; set; }

        [Label("Наименование")]
        public string ParameterName { get; set; }

        [Label("Тип данных")]
        public string DataType { get; set; }

        [InputSelectEnum(nameof(ParameterDirection))]
        public string ParameterMode { get; set; }
        public int OrdinalPosition { get; set; }
        public bool HasDefaultValue { get; internal set; }
        public string DefaultValue { get; internal set; }
    }
}