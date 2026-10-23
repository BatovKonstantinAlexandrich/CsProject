using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Регистрация первичныхданных
/// </summary>
[Label("Инициализация веб-сервисов и служебных программ")]
public class DbContextServiceInitiliazer : DbContextInitiliazer<DbContextService>
{
    public override Dictionary<string, int> Init(DbContextService context)
    {
        //context.Database.EnsureDeleted();
        //context.Database.EnsureCreated();
        var signup = AppProviderService.GetInstance().Get<SignupService>();
        var signupResult = signup.Signup(new ServiceInfo()
        {
            Url = "https;//pay.euroauto.ru",
            Name = "Payment",
            Dll = System.IO.File.ReadAllBytes("ActionClientLib.dll"),
            Version = "1.0.0",
        });
        var serviceContext = signupResult.Result;
        var sertificate = serviceContext.ServiceSertificate;
        var signin = AppProviderService.GetInstance().Get<SigninService>();
        var signinResult = signin.Signin(sertificate);

        context.ValidateOnChanges = false;
        var result = new Dictionary<string, int>();
        //this.Info(context.Database.GetConnectionString());
        var roles = new string[] { "Transport", "Holder", "Customer" };
        try
        {
            result[nameof(ServiceResource)] = InitServiceResource(context, roles);
            result[nameof(ServiceGroup)] = InitServiceGroups(context, roles);
            result[nameof(ServiceContext)] = InitServiceContexts(context);
            result[nameof(ServiceGroupMessage)] = InitServiceGroupMessages(context, roles);
            result[nameof(ServiceMessage)] = InitServiceMessages(context);
        }
        catch (Exception ex)
        {
            this.Error($"Исключение при инициализации первичных данных: \n{ex.ToTextDocument()}");
            throw;
        }

        return result;

        return result;
    }

    private int InitServiceMessages(DbContextService context)
    {
        throw new NotImplementedException();
    }

    private int InitServiceContexts(DbContextService context)
    {
        throw new NotImplementedException();
    }

    private int InitServiceGroupMessages(DbContextService context, string[] roles)
    {
        throw new NotImplementedException();
    }

    private int InitServiceGroups(DbContextService context, string[] roles)
    {
        throw new NotImplementedException();
    }

    private int InitServiceResource(DbContextService context, string[] roles)
    {
        foreach (var role in roles) {
            var resource = context.CheckRegistration<ServiceResource>(role);
            resource.EnsureIsValide();
            context.CreateServiceModel(resource);
        }
        return 0;
    }
}

