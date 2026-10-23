using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

using System.Collections.Concurrent;
using System.Data;

using Console_CommonModel.CommonLib.LibPages;

using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;



[Label("Функциональная модель")]
[Description("Формирует концептульную схему обмена сообщениями между бизнес ресурсами")]
public class BusinessDbContext : DbContextUser, 
    IBusinessFunctionsService,
    IUserBusinessService
    
{

    /// <summary>
    /// Операции полученные динамическим путём
    /// ключом могут быть:
    ///   полное название таблицы начиная со схему [dbo].[users]
    /// </summary>
    public static ConcurrentDictionary<string, ConcurrentDictionary<string, Func<Dictionary<string, object>, object>>> UserOperators { get; set; } = new();


    public override DbSet<BusinessResource> BusinessResources { get; set; }

    [Label("Информационные атрибуты")]
    public DbSet<MessageAttribute> MessageAttributes { get; set; }
    
    [Label("Свойства протокола")]
    public DbSet<MessageProperty> MessageProperties { get; set; }

    [Label("Протоколы обмена сообщениями")]
    public DbSet<MessageProtocol> MessageProtocols { get; set; }





    public BusinessDbContext()
    {
    }

    public BusinessDbContext(DbContextOptions<DbContextBase> options) : base(options)
    {
    }

    public BusinessDbContext(string database)
    {
        this._database = database;        
    }

    public static void CreateDatabase()
    {
        using (var db = new BusinessDbContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }

    public SigninUser<DeliveryDbContext> GetSigninUser()
    {
        var signin = AppProviderService.GetInstance().Get<SigninUser<DeliveryDbContext>>();
        signin.SetDbContext(this);
        return signin;
    }




    

    public List<BusinessFunction> DoBusinessFunctionsForEntity<TEntity>(BusinessFunction function) where TEntity : class
    {
        return business.DoBusinessFunctionsForEntity<TEntity>(function);
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




    /// <summary>
    /// Операции полученные динамическим путём
    /// ключом могут быть:
    ///   полное название таблицы начиная со схему [dbo].[users]
    /// </summary>  
    public void InitServices()
    {
        foreach(var role in this.BusinessResources.ToList())
        {
            var todos = new ConcurrentDictionary<string, Func<Dictionary<string, object>, object>>();
            var procedures = this.GetProceduresMetadata(nameof(BusinessResource));
            foreach (var kv in procedures)
            {
                var execute = new ProcedureExecuteView();
                execute.SetModel(this, kv.Value, dataTable => {
                
                });

                todos[kv.Value.ProcedureLabel] = (args) =>
                {
                    var proc = kv.Value;
                    try
                    {
                        return this.ExecuteProcedure(proc.FullName, args);
                    }
                    catch(Exception ex)
                    {
                        this.Error(ex);
                        throw;
                    }
                };
            }
            UserOperators[role.Code] = todos;
        }
    }


 

    /// <summary>
    /// Бизнес-функции
    /// </summary>
    /// <param name="businessResourceName">имя роли</param>    
    public ConcurrentDictionary<string, Action<Action<DataTable>>> GetBusinessDialogs(string businessResourceName)
    {        
        var todos = new ConcurrentDictionary<string, Action<Action<DataTable>>>();
        var procedures = this.GetProceduresMetadata(businessResourceName);
        foreach (var kv in procedures)
        {               
            todos[kv.Value.ProcedureLabel] = (onsuccess) =>
            {
                ExecuteDialog(onsuccess, kv.Value);
            };
        }
        return todos;        
    }



    private static void ExecuteDialog(Action<DataTable> onsuccess, ProcedureMetadata metadata)
    {
        ExecuteDialog(metadata,onsuccess);
    }
    private static void ExecuteDialog(ProcedureMetadata metadata, Action<DataTable> onsuccess)
    {
        var execute = new ProcedureExecuteView();
        execute.SetModel(new DeliveryDbContext(), metadata, dataTable =>
        {
            onsuccess(dataTable);
            execute.ToControl().GetAppForm().Close();
        });
        execute.ShowDialog();
    }

    public virtual string GetConnectionString()
    {
        var builder = new SqlConnectionStringBuilder();
        builder.DataSource = @"DESKTOP-IHJM9RD";
        var databaseName = typeof(DeliveryDbContext).GetTypeName();
        //this.Info(databaseName);
        builder.InitialCatalog = databaseName;
        builder.TrustServerCertificate = true;
        builder.IntegratedSecurity = true;
        return builder.ToString();
    }


    /// <summary>
    /// Получение ссылки на MessageAttribute
    /// </summary>
    /// <typeparam name="TInputAttribute"></typeparam>
    /// <returns></returns>
    public MessageAttribute GetMessageAttribute<TInputAttribute>()
        where TInputAttribute : BaseInputAttribute
    {
        var messageAttribute = this.MessageAttributes.FirstOrDefault(a => a.Name == typeof(TInputAttribute).GetTypeName());
        if (messageAttribute is null)
        {
            messageAttribute = ToMessageAttribute(typeof(TInputAttribute));
            this.MessageAttributes.Add(messageAttribute);
            this.SaveChanges();
        }
        return messageAttribute;
    }



    public MessageAttribute GetMessageAttribute(Type propertyType)
    {
        var inputType = GetInputType(propertyType);
        var messageAttribute = this.MessageAttributes.FirstOrDefault(a => a.Name == inputType.GetType().GetTypeName());
        if (messageAttribute is null)
        {
            messageAttribute = ToMessageAttribute(inputType.GetType());
            this.MessageAttributes.Add(messageAttribute);
            this.SaveChanges();
        }
        return messageAttribute;
    }




    /// <summary>
    /// 
    /// </summary>
    public BaseInputAttribute GetInputType(Type propertyType)
    {
        if (propertyType.IsCollectionType())
        {
            var elementType = propertyType.GetElementType();
            return elementType.IsPrimitiveForType() ?
                nameof(InputPrimitiveCollectionAttribute).New<BaseInputAttribute>() :
                nameof(InputStructureCollectionAttribute).New<BaseInputAttribute>();
        }
        else
        {
            if (!propertyType.IsPrimitiveForType())
            {
                return nameof(InputObjectAttribute).New<BaseInputAttribute>();
            }
            else
            {
                switch (propertyType.GetTypeName())
                {
                    case "System.Byte[]": return nameof(InputFileAttribute).New<BaseInputAttribute>();
                    case "Byte[]": return nameof(InputFileAttribute).New<BaseInputAttribute>();
                    case "String": return nameof(InputTextAttribute).New<BaseInputAttribute>();
                    case "Boolean": return nameof(InputBoolAttribute).New<BaseInputAttribute>();
                    case "System.String": return nameof(InputTextAttribute).New<BaseInputAttribute>();
                    case "string": return nameof(InputTextAttribute).New<BaseInputAttribute>();
                    case "int": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "long": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "float": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "Nullable<System.Boolean>": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "Double": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "Nullable<System.Double>": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "Int16": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "Nullable<Int16>": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "Int32": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "Nullable<System.Int32>": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "UInt16": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "UInt32": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "UInt64": return nameof(InputNumberAttribute).New<BaseInputAttribute>();
                    case "DateTime": return nameof(InputDateAttribute).New<BaseInputAttribute>();
                    case "Nullable<System.DateTime>": return nameof(InputDateAttribute).New<BaseInputAttribute>();
                    case "text": return nameof(InputTextAttribute).New<BaseInputAttribute>();
                    default: return nameof(InputTextAttribute).New<BaseInputAttribute>();
                }
            }
        }

    }

    private MessageAttribute ToMessageAttribute(Type attribute)
    {
        string atrName = (attribute.GetType().Name.StartsWith("Input") ? attribute.GetType().Name.Substring("Input".Length) : attribute.GetType().Name).Replace("Attribute", "");

        BaseInputAttribute attr = attribute.New<BaseInputAttribute>();
        MessageAttribute res = new MessageAttribute()
        {
            Label = attribute.GetLabel(),
            SqlType = attr.GetSqlServerDataType(),
            CsType = attr.GetCSTypeName(),
            SqlServerDataType = attr.GetSqlServerDataType(),
            MySqlDataType = attr.GetMySQLDataType(),
            PostgreDataType = attr.GetPostgreDataType(),
            OracleDataType = attr.GetOracleDataType(),
            Description = attribute.GetDescription(),
            Icon = attribute.GetIcon(),
            Name = attribute.GetTypeName().Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", ""),
            Code = attribute.GetTypeName(),
            InputType = (attribute.Name.StartsWith("Input") ? attribute.Name.Substring("Input".Length) : attribute.Name).Replace("Attribute", "")
        };
        return res;
    }





  
    protected string _database = typeof(BusinessDbContext).GetTypeName();

    public void Init()    
        => this.GetInitializer<BusinessDbContext>().Init(this);        
    
    public IEnumerable<TModel> Get<TModel>() where TModel : class    
        => this.Set<TModel>().ToList();
    

    internal static void Configure(DbContextOptionsBuilder optionsBuilder)
    {
       
        if (optionsBuilder.IsConfigured == false)
        {
            string connectionString = $@"Data Source=DESKTOP-IHJM9RD;" +
                $"Initial Catalog={typeof(BusinessDbContext).GetTypeName()};" +
                "Integrated Security=True;" +
                "Connect Timeout=30;" +
                "Encrypt=False;" +
                "" +
                "Application Intent=ReadWrite;" +
                "Multi Subnet Failover=False";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    

    

    public IEnumerable<INavigation> GetNavFor(Type type)
    {
        return null;
    }

    public DbSet<BusinessProcess> BusinessProcesses { get; set; }    
    public DbSet<BusinessFunction> BusinessFunctions { get; set; }    
    public DbSet<BusinessDatasource> BusinessDatasources { get ; set ; }
    public DbSet<BusinessLogic> BusinessLogics { get ; set; }    
    public DbSet<BusinessReport> BusinessReports { get ; set ; }
    public DbSet<BusinessIndicator> BusinessIndicators { get ; set ; }
    public DbSet<BusinessDataset> BusinessDatasets { get ; set ; }
    public DbSet<BusinessGranularities> Granularities { get ; set ; }
    public DbSet<BusinessData> BusinessData { get; set; }



    /// <summary>
    /// 
    /// </summary>
    public BusinessFunction CreateBusinessFunctionForBusinessResource<TInput, TOutput>(string businessResourceName, string businessFunctionName)
        where TInput : BaseEntity
        where TOutput : BaseEntity
    {
        
        return ((IBusinessFunctionsService)new BusinessFunctionsService(this)).CreateBusinessFunctionForBusinessResource<TInput, TOutput>(businessResourceName, businessFunctionName);
    }

}
