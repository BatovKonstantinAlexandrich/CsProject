using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{


    /// <summary>
    /// Мета данные для выполнения хранимых процедур
    /// </summary>
    [Label("База информации об источниках")]
    public class ModelsDbContext : DbContextBase
    {


        public DbSet<DatabaseModel> Databases { get; set; }
        public DbSet<DatabaseConnectionString> DatabaseConnections { get; set; }




        [DataModelIntegration()]
        public DbSet<ParameterMetadata> ProcedureMetadata { get; set; }
        public DbSet<ProcedureMetadata> ParameterMetadata { get; set; }
        public DbSet<ProcedureModel> Procedures { get; set; }
        public DbSet<ParameterModel> Parameters { get; set; }
        public DbSet<ParameterAttributeModel> ParameterAttributes { get; set; }

        public DbSet<CatalogMetadata> Catalogs { get; set; }
        public DbSet<TableMetadata> Tables { get; set; }
        public DbSet<ColumnMetadata> Columns { get; set; }
        public DbSet<KeyMetadata> Keys { get; set; }
        public DbSet<SchemaMetadata> Schemas { get; set; }


        public ModelsDbContext()
        {
        }

        public SchemaMetadata GetSchema(int databaseId, string tableSchema)
        {
            var schema = Schemas.FirstOrDefault(s => s.DatabaseModelId == databaseId && s.Name == tableSchema);
            if (schema is not null)
                return schema;
            schema = new()
            {
                Label = tableSchema,
                Name = tableSchema,
                DatabaseModelId = databaseId
            };
            Schemas.Add(schema);
            SaveChanges();
            return schema;
        }
        public int GetSchemaId(int databaseId, string tableSchema)
        {
            return GetSchema(databaseId, tableSchema).Id;
        }
    }
}
