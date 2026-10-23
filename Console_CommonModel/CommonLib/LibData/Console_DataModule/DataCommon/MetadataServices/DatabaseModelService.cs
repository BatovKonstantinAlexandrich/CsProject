using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;

using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.MetadataServices
{
    public class DatabaseModelService
    {
        public string TakeModel(string name, SqlServerWebApi bds)
        {
            int version = 0;
            var tables = bds.GetTablesMetadata();
            var keys = bds.GetKeysMetadata();
            var procedures = bds.GetProceduresMetadata();
            var schemas = bds.GetSchemasNames().ToList();
            var parameters = bds.GetParametersMetadata();
            using (var db = new ModelsDbContext())
            {
                version = db.Catalogs.Where(c => c.Name == name).Count() + 1;
                var catalog = new CatalogMetadata()
                {
                    Name = name,
                    Description = bds.GetType().GetDescription(),
                    Version = version.ToString()
                };
                db.Add(catalog);
                db.SaveChanges();

                var schemaMetadatas = schemas.ToList().Select(schemaName => new SchemaMetadata()
                {
                    Name = schemaName
                });
                db.Schemas.AddRange(schemaMetadatas);
                db.SaveChanges();



                foreach (var t in procedures.Values)
                {
                    t.TableCatalogId = catalog.Id;
                    //t.SchemaId = schemaMetadatas.FirstOrDefault(s => s.Name.ToLower() == t.ToLower()).Id;
                }
                this.Info(tables.Values.ToJsonOnScreen());

                foreach (TableMetadata t in tables.Values)
                {
                    this.Info(t.Validate());
                    t.SchemaId = db.Schemas.FirstOrDefault(s => s.Name.ToLower() == t.TableSchema.ToLower()).Id;
                    db.Tables.Add(t);
                    db.SaveChanges();
                }

                db.SaveChanges();
                foreach (var proc in procedures.Values)
                {
                    proc.TableCatalogId = catalog.Id;
                    proc.SchemaId = db.Schemas.FirstOrDefault(s => s.Name.ToLower() == proc.ProcedureOwner.ToLower()).Id;
                    db.Procedures.Add(new()
                    {
                        Name = proc.ProcedureName,
                        Code = proc.Sql
                    });
                    db.SaveChanges();
                }

                List<ParameterMetadata> p = new();
                foreach (KeyMetadata key in keys)
                {
                    db.Keys.Add(key);
                    db.SaveChanges();
                }
                foreach (ParameterMetadata par in parameters.Values)
                {

                    var proc = db.Procedures.FirstOrDefault(p => p.Id == par.ProcedureId);
                    par.ProcedureId = proc.Id;

                    db.ProcedureMetadata.Add(par);
                    db.SaveChanges();
                }

                db.SaveChanges();
            }
            return version.ToString();
        }


        /// <summary>
        /// Сохранение структуры данных
        /// </summary>
        /// <param name="datasource"></param>
        public void SaveModel(BusinessDatasource datasource)
        {
            var bds = new BusinessDatasourceService();
            bds.Setup(datasource);

            using (var db = new DatabaseModelDbContext())
            {
                var tables = bds.GetTablesMetadata();
                var procedures = bds.GetProceduresMetadata();
                var schemas = bds.GetSchemasNames();
                var parameters = bds.GetProceduresParametersMetadata();

                var version = $"{schemas.Count()}.{tables.Count()}.{procedures.Count()}.{parameters.Count()}";
                var hash = new
                {
                    tables,
                    procedures,
                    schemas,
                    parameters
                }.ToJson().GetHashSha256();

                var catalog = db.Catalogs.FirstOrDefault(c => c.Name == datasource.Name && c.Version == version);
                if (catalog is not null)
                {
                    if (catalog.Hash != hash)
                    {
                        this.Warn("Не поддерживается обновление пока ещё");
                    }
                }
                else
                {
                    db.Add(catalog = new CatalogMetadata()
                    {
                        Name = datasource.Name,
                        Description = datasource.Description,
                        Label = datasource.Label,
                        Version = version,
                        Hash = hash
                    });
                    var schemaMetadatas = schemas.ToList().Select(t => new SchemaMetadata()
                    {
                        Name = t,
                    });
                    db.Schemas.AddRange(schemaMetadatas);
                    db.SaveChanges();

                    tables.Values.ToList().ForEach(t =>
                    {
                        var ss = schemaMetadatas.FirstOrDefault(s => s.Name == t.TableSchema);
                        if (ss is null)
                        {
                            throw new Exception("Схема не найдена " + t.TableSchema);
                        }
                        t.SchemaId = ss.Id;
                    });
                    db.Tables.AddRange(tables.Values);
                    db.SaveChanges();

                }





                //db.Procedures.AddRange(procedures.Values);
                //db.SaveChanges();
                //List<ParameterMetadata> p = new();
                //foreach(var list in parameters.Values)
                //{
                //    p.AddRange(list.Values);
                //}
                //db.Parameters.AddRange(p);
                //db.SaveChanges();
            }

        }

        public void PutModel(BusinessDatasource datasource)
        {

        }
    }
}
