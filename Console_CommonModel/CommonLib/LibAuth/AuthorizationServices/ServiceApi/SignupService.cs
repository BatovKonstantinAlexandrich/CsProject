using System.Linq;
using Microsoft.EntityFrameworkCore;

public class SignupService : BaseSignup<DbContextService, ServiceContext, ServiceSertificate, ServiceInfo>
{
    private readonly DbContextService _model;

    public SignupService(DbContextService model)
    {
        this._model = model;    
    }

    public override bool Compare(ServiceSertificate stored, ServiceSertificate input)
        => stored.PrivateKey.Count() == input.PrivateKey.Count();//Equals(stored.PrivateKey,input.PrivateKey) && Equals(stored.PublicKey,input.PublicKey);

    public override ServiceContext GetBy(ServiceSertificate item)

        => _model.ServiceContexts.Include(sc => sc.ServiceSertificate).First(sc => sc.ServiceSertificate.PublicKey == item.PublicKey);

    public override bool HasWith(ServiceSertificate item)
    
        => _model.ServiceSertificates.Any(i => Compare(i, item));        
    
 
    public ServiceSertificate GenerateSertificate()
    {
        var res = new ServiceSertificate()
        {
            PrivateKey = AppProviderService.GetInstance().Get<AuthorizationServices>().GeneratePrivateKey().ToList(),
            PublicKey = AppProviderService.GetInstance().Get<AuthorizationServices>().GeneratePrivateKey().ToList()
        };
        
        return res;
    }
    public MethodResult<ServiceContext> Signup(ServiceInfo info)
        => Signup(GenerateSertificate(), info);
    public override MethodResult<ServiceContext> Signup(ServiceSertificate item, ServiceInfo info)
    {
        try
        {
            var context = new ServiceContext();
            _model.Add(context.ServiceInfo = info);
            _model.Add(context.ServiceSertificate = item);
            _model.Add(context.ServiceSettings = new ServiceSettings());
            _model.Add(context);
            _model.SaveChanges();
            return MethodResult<ServiceContext>.OnResult(context);
        }
        catch(DbUpdateException ex)
        {
            return MethodResult<ServiceContext>.OnError(ex);
        }
    }

    public override MethodResult<ServiceContext> Signout(ServiceSertificate item)
    {
        var context = this.GetBy(item);
        _model.Remove(context);
        _model.Remove(context.ServiceInfo);
        _model.Remove(context.ServiceSertificate);
        _model.Remove(context.ServiceSettings);
        _model.SaveChanges();
        return MethodResult<ServiceContext>.OnResult(context);
    }
}