using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;


namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata
{



    [Label("Структура таблицы")]
    public class TableMetadata : BaseEntity
    {
        public TableMetadata()
        {
        }

        public TableMetadata(TableInfo metadata)
        {
            TableName = metadata.name;
            TableSchema = metadata.schema;
            ColumnsMetadata = new Dictionary<string, ColumnMetadata>();
            foreach (var kv in metadata.columns)
            {
                ColumnsMetadata[kv.Key] = new ColumnMetadata(metadata.columns[kv.Key]);
            }
        }

        public string GetFullName()
            => $"[{TableCatalog}].[{TableSchema}].[{TableName}]".ToUpper();



        [NotInput]
        public string TableCatalog { get; set; }

        [InputDbContextDictionary<ModelsDbContext>($"{nameof(SchemaMetadata)},{nameof(SchemaMetadata.Name)}")]
        public int SchemaId { get; set; }
        public SchemaMetadata Schema { get; set; }

        [Display(Name = "Схема")]
        [NotInput]
        public string TableSchema { get; set; } = "dbo";

        [Label("Код")]
        [NotNullNotEmpty]
        public string TableName { get; set; }

        [Label("Название")]
        public string TableLabel { get; set; }



        [InputSelect($"{nameof(NamedObject)},{nameof(BaseEntity)},{nameof(ActiveObject)},{nameof(EventsTable)},{"HierTable"},{nameof(EventsTable)}")]
        public string TableType { get; set; } = "BaseEntity";

        [NotNullNotEmpty]
        [NotMapped]
        [NotInput]

        public IDictionary<string, ColumnMetadata> ColumnsMetadata;


        [Label("Первичный ключ")]
        [NotNullNotEmpty]
        [InputSelectExpression("ColumnNames")]
        public string PrimaryKey { get; set; } = "Id";

        [NotMapped]
        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public List<string> ColumnNames { get => this.GetOwnPropertyNames(); }

        /// <summary>
        /// Внешние ключи
        /// Ключ = [ColumnName] 
        /// Значение = [TableName]
        /// </summary>
        [NotMapped]
        [NotNullNotEmpty]
        [InputCollectionDictionary]
        public IDictionary<string, string> ForeignKeys { get; set; } = new ChangingDictionary<string, string>();

        [NotInput]
        public string Fullname { get => $"[{TableCatalog}].[{TableSchema}].[{TableName}]"; }
    }
}
