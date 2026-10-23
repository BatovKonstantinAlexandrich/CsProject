
using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

using Microsoft.EntityFrameworkCore;

using NSwag;

[Label("База активации служебных программ")]

//Data Source=DESKTOP-IHJM9RD;Initial Catalog=DeliveryDbContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False
public class DbContextService: ServiceDbContextBase<ServiceResource>,
    IServiceBuilder<ServiceResource>,
    IBaseSignup<ServiceContext, ServiceSertificate, ServiceInfo>,
    IBaseSignin<ServiceContext, ServiceSertificate, ServiceInfo>
{
    private readonly string connectionString = $@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={typeof(DbContextService).GetTypeName()};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    private SigninService signinService;
    private SignupService signupService;

    //сервисы сборки и выполнения
    private static ServiceBuilder<DbContextService, ServiceResource> serviceBuilder;

    public virtual DbSet<ServiceGroup> ServiceGroups { get; set; }
    public virtual DbSet<ServiceGroupMessage> ServiceGroupMessages { get; set; }
    public virtual DbSet<ServiceGroupBindings> ServiceGroups_ServiceContext { get; set; }
    public virtual DbSet<ServiceMessage> ServiceMessages { get; set; }
    public virtual DbSet<ServiceInfo> ServiceInfos { get; set; }
    public virtual DbSet<ServiceSertificate> ServiceSertificates { get; set; }
    public virtual DbSet<ServiceContext> ServiceContexts { get; set; }
    public virtual DbSet<ServiceSettings> ServiceSettings { get; set; }
    

    public DbContextService(){
            if (serviceBuilder is null)
                serviceBuilder = new ServiceBuilder<DbContextService, ServiceResource>(this);
    }

    public DbContextService(string connectionString)
    {
        this.connectionString = connectionString;
        lock (serviceBuilder)
        {
            if (serviceBuilder is null)
                serviceBuilder = new ServiceBuilder<DbContextService, ServiceResource>(this);
        }
    }

    public DbContextService(DbContextOptions<DbContextBase> options) : base(options){
        lock (serviceBuilder)
        {
            if (serviceBuilder is null)
                serviceBuilder = new ServiceBuilder<DbContextService, ServiceResource>(this);
        }
    }
    


    public void InitializeServices()
    {
        var initializer = new DbContextServiceInitiliazer();
        initializer.Init(this);
    }

    public bool Compare(ServiceSertificate stored, ServiceSertificate input)
    {
        return ((IBaseSignup<ServiceContext, ServiceSertificate, ServiceInfo>)signupService).Compare(stored, input);
    }

    public ServiceContext GetBy(ServiceSertificate item)
    {
        return ((IBaseSignup<ServiceContext, ServiceSertificate, ServiceInfo>)signupService).GetBy(item);
    }

    public bool HasWith(ServiceSertificate item)
    {
        return ((IBaseSignup<ServiceContext, ServiceSertificate, ServiceInfo>)signupService).HasWith(item);
    }

    public MethodResult<ServiceContext> Signout(ServiceSertificate item)
    {
        return ((IBaseSignup<ServiceContext, ServiceSertificate, ServiceInfo>)signupService).Signout(item);
    }

    public MethodResult<ServiceContext> Signup(ServiceSertificate item, ServiceInfo info)
    {
        return ((IBaseSignup<ServiceContext, ServiceSertificate, ServiceInfo>)signupService).Signup(item, info);
    }

    public T GetFromSession<T>(string key)
    {
        throw new NotImplementedException();
    }

    public void PutIntoSession(string key, object item)
    {
        throw new NotImplementedException();
    }
 

    public MethodResult<ServiceContext> Signin(ServiceSertificate item)
    {        
        throw new NotImplementedException();
    }

    public bool Signout(string key)
    {
        throw new NotImplementedException();
    }

    public bool Validate(string key)
    {
        throw new NotImplementedException();
    }

    public void SetDbContext(DbContextAuth<ServiceContext, ServiceSertificate, ServiceInfo> db)
    {
        throw new NotImplementedException();
    }

    public void AddOpenApi(string url)
    {
        ((IServiceBuilder<ServiceResource>)serviceBuilder).AddOpenApi(url);
    }

    public void AddWsdl(string url)
    {
        ((IServiceBuilder<ServiceResource>)serviceBuilder).AddWsdl(url);
    }

    public Func<Dictionary<string, object>, MethodResult<object>> CreateCaller(Type serviceType, string action)
    {
        return ((IServiceBuilder<ServiceResource>)serviceBuilder).CreateCaller(serviceType, action);
    }

    public OpenApiDocument CreateOpenApiForUrl(string url)
    {
        return ((IServiceBuilder<ServiceResource>)serviceBuilder).CreateOpenApiForUrl(url);
    }

    public void CreateServiceModel(ServiceResource model)
    {
        ((IServiceBuilder<ServiceResource>)serviceBuilder).CreateServiceModel(model);
    }

    public Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>> CreateValidator(Type serviceType, string action)
    {
        return ((IServiceBuilder<ServiceResource>)serviceBuilder).CreateValidator(serviceType, action);
    }

    public MethodResult<object> ExecuteEndpoint(string key, Dictionary<string, object> args)
    {
        return ((IServiceBuilder<ServiceResource>)serviceBuilder).ExecuteEndpoint(key, args);
    }

    public List<string> GetAllRouteOptions()
    {
        return ((IServiceBuilder<ServiceResource>)serviceBuilder).GetAllRouteOptions();
    }

    public List<string> GetRouteOptions(string path = "/")
    {
        return ((IServiceBuilder<ServiceResource>)serviceBuilder).GetRouteOptions(path);
    }

    public void Init()
    {
        ((IServiceBuilder<ServiceResource>)serviceBuilder).Init();
    }
} 

