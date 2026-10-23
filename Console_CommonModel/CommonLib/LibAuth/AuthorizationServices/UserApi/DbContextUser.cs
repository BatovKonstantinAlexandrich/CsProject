using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Collections.Generic;
using System.Data;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;


[Label("База пользователей")]
public class DbContextUser : DbContextAuth<UserContext, UserAccount, UserPerson>    
{
    protected string connectionString;

    public BaseSignin<UserContext, UserAccount, UserPerson> signin;
    public IUserBusinessService business;

    public DbContextUser() { }
    public DbContextUser(string connectionString)
    {
        this.connectionString = connectionString;
    }
    public DbContextUser(DbContextOptions<DbContextBase> options) : base(options) {
    }

    public void InitServices()
    {
        signin = this.GetService<BaseSignin<UserContext, UserAccount, UserPerson>>();
        signin.SetDbContext(this);
    }
  

 
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //uniq constraint
        builder.Entity<BusinessResource>()
               .HasIndex(u => u.Name)
               .IsUnique();

        //uniq constraint
        builder.Entity<BusinessResource>()
               .HasIndex(u => u.Code)
               .IsUnique();

        //uniq constraint
        builder.Entity<UserAccount>()
               .HasIndex(u => u.Email)
               .IsUnique();


        //uniq constraint
        builder.Entity<UserGroup>()
               .HasIndex(u => u.Name)
               .IsUnique();

        //uniq constraint
        builder.Entity<UserGroupBinding>()
               .HasIndex(u => new { u.UserId, u.GroupId })
               .IsUnique();

        //uniq constraint
        builder.Entity<UserPerson>()
               .HasIndex(u => new { u.FirstName, u.SurName, u.LastName, u.Birthday })
               .IsUnique();
    }

    /// <summary>
    /// Обработка операция над
    /// </summary>
    /// <param name="state"></param>
    /// <param name="primary"></param>
    /// <param name="data"></param>
    [OnDataChanged<UsersResource>()]
    public void OnUsersResourceChanged(EntityState state, Dictionary<string, object> primary, Dictionary<string, object> data)
    {
        var userRole = new UsersResource();
        foreach (var item in data)
            userRole.SetValue(item.Key, item.Value);

        var schema = nameof(BusinessResource);
        var businessResource = this.BusinessResources.Find(userRole.RoleId);
        if (businessResource is null)
            throw new Exception($"Не найден BusinessResources c Id="+userRole.RoleId);
        var table = businessResource.Code;

        var api = new SqlServerWebApi();
        api.Configure(this.Database.GetConnectionString());
        switch (state)
        {
            case EntityState.Added:
                {
                                           
                    var tables = api.GetTablesMetadata();
                    //tables.Values.Select(tm => $"[{tm.TableSchema}].[{tm.TableName}]").ToJsonOnScreen().WriteToConsole();
                    //tables.Keys.ToJsonOnScreen().WriteToConsole();
                    //tables.Values.Select(v => new { shcema = v.TableSchema, name = v.TableName, search_schema = schema, search_table = table }).ToJsonOnScreen().WriteToConsole();

                    this.Info(tables.Select(t => new { t.Value.TableSchema, t.Value.TableName }).ToList());
                    this.Info($"[{schema}].["+table.ToLower()+"]");

                    var tableMetadata = tables.Where(meta => meta.Value.TableSchema.ToLower() == schema.ToLower() && meta.Value.TableName.ToLower() == table.ToLower()).Select(kv=>kv.Value).FirstOrDefault();                    
                    if(tableMetadata is null)
                    {
                        tables = api.InitTablesMetadata();
                        tableMetadata = tables.Where(meta => meta.Value.TableSchema.ToLower() == schema.ToLower() && meta.Value.TableName.ToLower() == table.ToLower()).Select(kv => kv.Value).FirstOrDefault();
                        if (tableMetadata is null)
                            throw new NullReferenceException($"Не удалось получить ссылку на метаданные таблицы: [{schema}].[{table}]");
                    }

                    var columns = new List<string>() { "id", "user_id", "userid" };
                    var indicatorColumns = tableMetadata.ColumnsMetadata.Values.Where(col => !columns.Contains(col.ColumnName.ToLower())).Select(col => col.ColumnName).ToList();
                    
                    var values = new Dictionary<string, object>()
                    {
                        { "UserId", userRole.UserId }
                    };
                    foreach (var key in tableMetadata.ColumnsMetadata.Keys)
                    {
                        if (key.ToLower() != "userid" && key.ToLower() != "id")
                        {
                            values[key] = 0;
                        }
                    }

                    string headerString = "", valuesString = "";
                    foreach (var p in values)
                    {
                        headerString += p.Key + ",";
                        var type = tableMetadata.ColumnsMetadata[p.Key].DataType;
                        valuesString += toSqlValueStr(type, values[p.Key]) + ",";
                    };
                    headerString = headerString.Substring(0, headerString.Length - 1);
                    valuesString = valuesString.Substring(0, valuesString.Length - 1);
                    string insertStatement = "insert into " + $"[{schema}].[{table}]" + "(" + headerString + ") values (" + valuesString + ")";
                    api.PrepareQuery(insertStatement);                    
                }
                break;
            case EntityState.Deleted:
                {
                    var tables = api.GetTablesMetadata();

                    var tableMetaDataKv = tables.FirstOrDefault(meta => meta.Value.TableSchema.ToLower() == schema.ToLower() && meta.Value.TableName.ToLower() == table.ToLower());
                    var tableMetadata = tableMetaDataKv.Value;
                    if (tableMetadata is null)
                    {
                        throw new NullReferenceException($"Не удалось получить ссылку на метаданные таблицы: [{schema}].[{table}]");
                    }
                    string deleteStatement = "delete from " + $"[{schema}].[{table}]" + " where UserId=" + userRole.UserId;
                    api.PrepareQuery(deleteStatement);
                }
                break;
        }
    }

    public string toSqlValueStr(string type, object val)
    {

        if (val == null || val.ToString() == "NULL")
        {
            return "NULL";
        }
        string value = "";
        switch (type)
        {
            case "date":
                DateTime date = (DateTime)val;
                value =
                    "DateFromParts(" + date.Year + "," + date.Month + "," + date.Day + ")";
                break;
            case "datetime":
                DateTime datetime = (DateTime)val;
                value =
                    "DateTimeFromParts(" + datetime.Year + "," + datetime.Month + "," + datetime.Day +
                    $",{datetime.Hour},{datetime.Minute},{datetime.Second},0)";
                break;
            case "datetime2":
                DateTime datetime2 = (DateTime)val;
                value =
                    "DateTimeFromParts(" + datetime2.Year + "," + datetime2.Month + "," + datetime2.Day +
                    $",{datetime2.Hour},{datetime2.Minute},{datetime2.Second},0)";
                break;
            /*case "date":
                DateTime date = (DateTime)val;
                value = "'" + date.Year + "-" + date.Month + "-" + date.Day + "'";
                break;
            case "datetime":
                DateTime datetime = (DateTime)val;
                value = 
                    "'" + datetime.Year + "-" + datetime.Month + "-" + datetime.Day + 
                    $" {datetime.Hour}:{datetime.Minute}:{datetime.Second}'";
                break;*/
            case "ntext":
            case "text":
            case "nvarchar":
            case "varchar":
                value = "'" + val.ToString().Replace("'", "\"") + "'";
                break;
            case "smallint":
            case "int":
            case "float":
                value = "" + val.ToString().Replace(",", ".") + "";
                break;
            default:
                throw new Exception("Тип данных " + type + " пока не поддерживается");
        }

        return value;
    }

    
    


    [OnDataChanged<BusinessResource>()]
    public void OnBusinessResourceChanged(EntityState state, Dictionary<string, object> primary, Dictionary<string, object> data)
    {
        var role = new BusinessResource();
        foreach (var item in data)
        {
            role.SetValue(item.Key, item.Value);
        }

        var schema = nameof(BusinessResource);
        var name = role.Code;

        
        //this.Info(api.ToString());
        switch (state)
        {
            case EntityState.Added:
                {
                    OnBusinessResourceAdded(schema, name);
                }
                break;
            case EntityState.Deleted:
                {
                    var api = new SqlServerWebApi();
                    var s = this.Database.GetConnectionString();
                    api.Configure(s);

                    api.PrepareQuery($"DROP TABLE IF EXISTS [{schema}].[{name}]");
                }
                break;
        }

    }


    [Label("Обработка события добавления BusinessResourse")]
    public void OnBusinessResourceAdded(string schema, string name)
    {
        try
        {
            this.Debug($"OnBusinessResourceAdded({schema},{name})");
            var api = this.GetSqlServerWebApi();
            api.CreateSchema(name);

            var builder = new TableMetadataBuilder();
            builder.AddPrimaryKey();
            //builder.AddIndicators("login");
            builder.AddReferenceOneToOne("dbo", "UserContexts", "UserId");

            TableInfo model = builder.Build();
            model.schema = schema;
            model.name = name;

            var bg = new SqlServerDDLFactory();
            var dml = bg.CreateTable(model);
            if (!api.GetSchemasNames().Any(name => name.ToLower() == name.ToLower()))
            {
                api.PrepareQuery(bg.CreateSchema(name));
            }
            if (!api.GetSchemasNames().Any(name => name.ToLower() == schema.ToLower()))
            {
                api.PrepareQuery(bg.CreateSchema(schema));
            }
            var tableNames = api.GetTableNames();
            this.Info(tableNames);
            if (!tableNames.Contains(model.name))
            {
                api.PrepareQuery(dml);
                api.InitTablesMetadata(model.schema, model.name);
                foreach (var kv in model.fk)
                {
                    string schemaName = kv.Value.IndexOf(".") != -1 ? kv.Value.Substring(0, kv.Value.IndexOf(".")).Replace("[", "").Replace("]", "").Replace(".", "") :
                        "dbo";
                    string relativeTable = kv.Value.IndexOf(".") != -1 ?
                        kv.Value.Substring(kv.Value.IndexOf(".") + 1).Replace("[", "").Replace("]", "")
                        : kv.Value.Replace("[", "").Replace("]", "");
                    var onDeleteCascade = false;
                    var onUpdateCascade = false;
                    string createKeyStatement =
                        $"ALTER TABLE {$"[{schema}].[{name}]"} ADD CONSTRAINT FK_{name + "_" + relativeTable} FOREIGN KEY" +
                            $" ({kv.Key}) REFERENCES {$"[{schemaName}].[{relativeTable}]"}(Id)" + " ON DELETE " + (onDeleteCascade == true ? "CASCADE" : "NO ACTION ") + " ON UPDATE " + (onUpdateCascade == true ? "CASCADE" : "NO ACTION ");
                    api.PrepareQuery(createKeyStatement);

                    if (model.columns[kv.Key].unique)
                    {
                        var uniqStatement = $"ALTER TABLE {$"[{schema}].[{name}]"} ADD CONSTRAINT UQ_{name}_{kv.Key} UNIQUE ({kv.Key})";
                        api.PrepareQuery(uniqStatement);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            this.Error($"{nameof(OnBusinessResourceAdded)}({name}) => {ex.Message}");
            throw;
        }
    }

    public List<BusinessFunction> GetBusinessFunctionsForEntity<TEntity>() where TEntity : class
    {
        return business.GetBusinessFunctionsForEntity<TEntity>();
    }

    public List<BusinessFunction> DoBusinessFunctionsForEntity<TEntity>(BusinessFunction function) where TEntity : class
    {
        return business.DoBusinessFunctionsForEntity<TEntity>(function);
    }

    public List<ContextView> CreateBusinessContextView<TDbContext, TEntity>(BusinessFunction function)
        where TDbContext : DeliveryDbContext
        where TEntity : class
    {
        return business.CreateBusinessContextView<TDbContext, TEntity>(function);
    }

    public Action TodoBusinessFunction<TDbContext, TEntity>(TDbContext db, BusinessFunction function)
        where TDbContext : DeliveryDbContext
        where TEntity : class
    {
        return business.TodoBusinessFunction<TDbContext, TEntity>(db, function);
    }

    public List<BusinessFunction> GetBusinessFunctions()
    {
        return business.GetBusinessFunctions();
    }

    public List<MessageProtocol> GetMessageProtocols()
    {
        return business.GetMessageProtocols();
    }

    public List<BusinessResource> GetBusinessResources()
    {
        return business.GetBusinessResources();
    }
}
