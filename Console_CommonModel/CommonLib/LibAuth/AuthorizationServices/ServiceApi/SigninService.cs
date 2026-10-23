using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

public sealed class SigninService : BaseSignin<ServiceContext, ServiceSertificate, ServiceInfo>
{        
    public SigninService(ITokenProvider tokenProvider) : base(tokenProvider)
    {
        
    }

    private ServiceContext GetBy(ServiceSertificate item)    
        => _model.Set<ServiceContext>().Include(ctx => ctx.ServiceInfo).FirstOrDefault(ctx => Equals(ctx.ServiceSertificate.PublicKey, item.PublicKey));

    public override MethodResult<ServiceContext> Signin(ServiceSertificate item)
    {
        var service = GetBy(item);
        if( service == null)
            MethodResult<ServiceContext>.OnError(new ArgumentException("Пользователь не зарегистрирован","item"));
        if(Compare(service.ServiceSertificate, item)==false)
            MethodResult<ServiceContext>.OnError(new ArgumentException("Авторизация не выполнена","item"));
        if(_context.Has(service.SecretKey)==false)
        {
            service.SecretKey = _context.Put(service);
        }     
        _model.SaveChanges();
        return MethodResult<ServiceContext>.OnResult(service);

    }
    private bool Compare(ServiceSertificate stored, ServiceSertificate input)
        => Equals(stored.PrivateKey,input.PrivateKey) && Equals(stored.PublicKey,input.PublicKey);

    public override bool Signout(string key)    
        => _context.Remove(key)!=null;
    

    public override bool Validate(string key)
        => _context.Has(key) ;

    public object IsSignin()
    {
        throw new NotImplementedException();
    }

    public object SigninByLoginAndPassword(string username, string password)
    {
        return new
        {
            username = username,
            password = password
        };
    }

    public override T GetFromSession<T>(string key)
    {
        var session = _context.GetSession(this._tokenProvider.Get());
        if (session.ContainsKey(key) == false)
        {
            session[key] = typeof(T).New();
        }
        return (T)session[key];
    }

    public override void PutIntoSession(string key, object item)
    {
        throw new NotImplementedException();
    }

    public override ServiceContext Verify()
    {
        throw new NotImplementedException();
    }
}
public class SigninServiceTest : TestingElement
{
    public override void OnTest()
    {
        var service = provider.Get<SigninService>();       
    }
}
