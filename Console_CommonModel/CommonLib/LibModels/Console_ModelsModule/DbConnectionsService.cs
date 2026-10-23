using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

using System.Reflection;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{

    /// <summary>
    /// Управление соединениями
    /// </summary>
    public class DbConnectionsService : IDbConnectionsService
    {

        public ProcedureModel GetProcedureModel([NotNullNotEmpty] string connectionString, [NotNullNotEmpty] string database, [NotNullNotEmpty] string schema, [NotNullNotEmpty] string name)
        {
            MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(new object[] { connectionString, database, schema, name });
            using (var db = new ModelsDbContext())
            {
                db.Database.EnsureCreated();

                var databaseModel = GetDatabase(connectionString);

                databaseModel.EnsureIsValide();
                var dbConnection = GetDbConnection(connectionString);

                dbConnection.EnsureIsValide();


                ProcedureModel model = db.Procedures.FirstOrDefault(p => databaseModel.Id == p.DatabaseModelId && p.Name.ToUpper() == name.ToUpper() && p.Schema.ToUpper() == schema.ToUpper());
                if (model is null)
                {
                    model = new ProcedureModel();
                    var api = new SqlServerWebApi();
                    api.Configure(connectionString);

                    model.DatabaseModelId = GetDatabase(connectionString).Id;
                    //var metadata = api.GetProcedureMetadata(database,schema,name);
                    model.Schema = schema;
                    model.Name = name;
                    model.Code = name;
                    db.Add(model);
                    db.SaveChanges();

                    foreach (var kv in api.GetParameters(schema, name))
                    {
                        var parModel = new ParameterModel()
                        {
                            Name = kv.Value.ParameterName,
                            Code = kv.Value.ParameterName,
                            Label = kv.Value.ParameterName,
                            ProcedureModelId = model.Id
                        };
                        db.Add(parModel);
                        parModel.EnsureIsValide();
                        db.SaveChanges();
                    }


                }
                return model;
            }
        }


        public int GetDatabaseId(string connectionString) => GetDatabase(connectionString).Id;
        public DatabaseModel GetDatabase(string connectionString)
        {
            var connection = GetDbConnection(connectionString);
            var api = new SqlServerWebApi();
            api.Configure(connectionString);
            using (var db = new ModelsDbContext())
            {
                db.Database.EnsureCreated();
                var record = db.Databases.FirstOrDefault(con =>
                    con.DatabaseConnectionId == connection.Id && con.Name.ToLower() == api.Database.ToLower()
                );
                if (record is null)
                {
                    record = new DatabaseModel()
                    {
                        DatabaseConnectionId = connection.Id,
                        Name = api.Database,
                        Code = api.Database
                    };
                    db.Add(record);
                    record.EnsureIsValide();
                    db.SaveChanges();
                }
                return record;
            }
        }

        public DatabaseConnectionString GetDbConnection([NotNullNotEmpty] string server)
        {
            //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(server);
            using (var db = new ModelsDbContext())
            {

                var record = db.DatabaseConnections.FirstOrDefault(con =>
                    con.ConnectionString != null && con.ConnectionString.ToLower() == server.ToLower()
                );
                if (record is null)
                {
                    record = new DatabaseConnectionString();
                    record.ConnectionString = server;
                    db.Add(record);
                    db.SaveChanges();
                }
                return record;
            }
        }


    }
}
