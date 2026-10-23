using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata
{
    public class ProcedureMetadata : BaseEntity
    {
        public int TableCatalogId { get; set; }
        public CatalogMetadata TableCatalog { get; set; }


        public int SchemaId { get; set; }
        public SchemaMetadata Schema { get; set; }

        /// <summary>
        /// Текстовая надпись
        /// </summary>        
        public string ProcedureLabel { get; set; }

        /// <summary>
        /// Квалификатор процедуры идентичен имени базы данных
        /// </summary>        
        [NotNullNotEmpty()]
        public string ProcedureQualifier { get; set; }

        /// <summary>
        /// Схема данных
        /// </summary>
        [NotNullNotEmpty()]
        public string ProcedureOwner { get; set; }

        /// <summary>
        /// Имя процедуры
        /// </summary>    
        [NotNullNotEmpty()]
        public string ProcedureName { get; set; }

        /// <summary>
        /// Полное наименование процедуры
        /// </summary>
        public string FullName { get => $"[{ProcedureQualifier}].[{ProcedureOwner}].[{ProcedureName}]"; }


        /// <summary>
        /// Сведения о параметрах
        /// </summary>
        [NotMapped]
        public IDictionary<string, ParameterMetadata> ParametersMetadata { get; set; } = new Dictionary<string, ParameterMetadata>();
        public List<ParameterMetadata> Parameters { get; set; }

        public string Sql { get; set; }

    }
}
