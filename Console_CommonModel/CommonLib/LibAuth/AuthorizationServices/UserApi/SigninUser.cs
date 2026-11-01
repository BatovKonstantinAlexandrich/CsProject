using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;


public class FileTokenProvider<TDbContextUser> : ITokenProvider
    where TDbContextUser : ActiveObject
{

    public string KEY = "token";

    public FileTokenProvider()
    {
    }

    public string Get() => System.IO.File.Exists(KEY) ? System.IO.File.ReadAllText(KEY) : null;

    public async Task Set(string token)
    {
        System.IO.File.WriteAllText(KEY, token);
        await Task.CompletedTask;
    }

    public void SetDirectory(string name)
    {
        KEY = name;
    }
}

public class DeliveryUserSignin : SigninUser<DeliveryDbContext>
{
    public DeliveryUserSignin( ) : base( new(),new FileTokenProvider<UserContext>() )
    {
    }
}
public class SigninUser<TDbContextUser> : BaseSignin<UserContext, UserAccount, UserPerson>, ISigninUser<TDbContextUser> 
    where TDbContextUser : DeliveryDbContext


{
    private TDbContextUser _model;


    public AuthorizationUsers _users;
    public TDbContextUser GetDbContext() => _model;
    public SigninUser(TDbContextUser dbContextUser, ITokenProvider tokenProvider) : base(tokenProvider)
    {
        if (dbContextUser is null)
            dbContextUser = typeof(TDbContextUser).New<TDbContextUser>();
        this.SetDbContext(dbContextUser);
        this._users = AuthorizationUsers.Get();
     
    }
    public override UserContext Verify()
    {
        var token = this._tokenProvider.Get();
        return String.IsNullOrEmpty(token) ? null : this._users.Take(token);
    }

    public bool Signout()
    {
        throw new NotImplementedException();
    }

    public override bool Signout(string key)
    {
        throw new NotImplementedException();
    }
    public bool IsSignin()
    {
        string token = this._tokenProvider.Get();
        return String.IsNullOrWhiteSpace(token) ? false : Validate(token);
    }
    public MethodResult<UserContext> Signin([InputEngWord] string user, [InputPassword] string password)
    {
        return Signin(new() { Email = user, Password = password, Hash = UserAccount.GetHashSha256(password) });
    }

    public override MethodResult<UserContext> Signin(UserAccount account)
    {
        MethodResult<UserContext> result = null;
        
        try
        {

            account.EnsureIsValide();

            var user = GetBy(account);
            //_model.SaveChanges();

            if (user == null)
            {
                var ex = new Exception("Пользователь не зарегистрирован");
                result = MethodResult<UserContext>.OnError(ex);
                throw ex;
            }

            if (user != null && user.Account.Hash != UserAccount.GetHashSha256(account.Password))
            {
                var ex = new Exception("Пароль задан неверно ");
                result = MethodResult<UserContext>.OnError(ex);
                throw ex;
            }

            if ((((AuthorizationUsers)_users).Has(user.SecretKey)))
            {
                _users.Remove(user.SecretKey);
            }
            /*var key = ((AuthorizationUsers)_users).GetByEmail(item.Email);
            if (key != null)
                _users.Remove(key);*/

            _tokenProvider.Set(user.SecretKey = _users.Put(user));
            user.SecretKey = user.SecretKey;
            user.LoginCount += 1;
            this._model.SaveChanges();

            result = MethodResult<UserContext>.OnResult(user);

            this._model.UserLogins.Add(new(account)
            {
                IpAddress = "127.0.0.1"
            });
            this._model.SaveChanges();

            if (this._model is TDbContextUser bd)
            {
                foreach (var role in new UserBusinessService<TDbContextUser>(this).GetBusinessResources())
                {
                    var businessDialogs = bd.GetBusinessDialogs(role.Name);
                    foreach (var kv in businessDialogs)
                    {
                        var todo = kv.Value;
                        user.ActiveFunctions[kv.Key] = () =>
                        {
                            todo(dt =>
                            {

                            });
                        };
                    }

                }
            }
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public UserContext GetBy(UserAccount userAccount)
    {
        if (userAccount is null)
            throw new ArgumentException(nameof(userAccount));
        return GetBy(userAccount.Email);
    }

    public UserContext GetBy(string username)
    {
        if (username is null)
            username = "";
        //this.Info(_model.GetConnectionString());
        var account = _model.UserAccounts.FirstOrDefault(account => account.Email.ToUpper() == username.ToUpper());
        if (account is null)
        {
            throw new ArgumentException(nameof(username));
        }
        var user = _model.UserContexts.FirstOrDefault(ctx => ctx.AccountId == account.Id);
        if (user is null)
        {
            _model.UserContexts.Add(user = new() { AccountId = account.Id });
            _model.SaveChanges();
        }
        //user.GetReference<UserSettings>();
        //user.GetReference<UserAccount>();
        user.Account = _model.UserAccounts.Find(user.AccountId);
        user.Person = _model.UserPersons.Find(user.PersonId);

        //user.GetReference<UserWallet>();        

        //user.GetReferenceCollectionManyToMany<UserGroups, UserGroup>();
        //user.GetReferenceCollectionManyToMany<UsersResource, BusinessResource>();

        //var user = _model.UserContexts.Include(ctx => ctx.Wallet).Include(ctx => ctx.Settings).Include(ctx => ctx.Person).Include(ctx => ctx.Account).Include(ctx => ctx.BusinessFunctions).FirstOrDefault(ctx => ctx.Account.Email.ToUpper() == username.ToUpper());
        if (user is not null)
        {
            user.BusinessResources = _model.BusinessResources.Where(r => user.UsersResources.Select(bd => bd.RoleId).Contains(r.Id)).ToList();
            if (user.UsersResources is null)
                user.UsersResources = new();
        }

        return user;
    }

    public override bool Validate(string key)
    {
        var result = key != null && _users.GetMemory().ContainsKey(key);
        return result;
    }

    public override void PutIntoSession(string key, object item)
    {
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            //_httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = item;
            }

        }
    }
    public override T GetFromSession<T>(string key)
    {

        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            //_httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = typeof(T).New();
            }
            return (T)session[key];
        }


    }
    public T GetFromSession<T>(string key, T defaultValue)
    {
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            ///_httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = defaultValue;
            }
            return (T)session[key];
        }


    }

    public void SetDbContext(TDbContextUser db)
    {
        this._model = db;
    }

 
}

/*
public sealed class SigninUser<DeliveryDbContext> : BaseSignin<UserContext, UserAccount, UserPerson>
{
    public DbContextUser _model;
    private readonly APIActiveCollection<UserContext> _users;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SigninUser(IHttpContextAccessor http, DbContextUser model, AuthorizationUsers users, ITokenProvider tokenProvider): base(tokenProvider)
    {
        this._model = model;
        this._users = users;
        _httpContextAccessor = http;

    }

    public void UpdateUrlLocation(string url)
    {
        if(IsSignin())
        {
            using (var db = new DbContextUser())
            {
                var userId = Verify().Id;
                var user = db.UserContexts.Find(userId);
                if(user is null)
                {
                    throw new Exception($"Пользователь с ид {userId} не найден в БД");
                }
                else
                {
                    user.URL = url;
                    db.SaveChanges();
                }                
            }
        }
    }

    public bool IsSignin()
    {
        string token = this._tokenProvider.Get();
        return String.IsNullOrWhiteSpace(token) ? false: this.Validate(token);
    }
    public UserContext GetBy(UserAccount item)
    {
        return GetBy(item.Email);
    }

    public UserContext GetBy(string username)
    {
        var user = _model.UserContexts.Include(ctx => ctx.Wallet).Include(ctx => ctx.Settings).Include(ctx => ctx.Person).Include(ctx => ctx.Account).Include(ctx => ctx.BusinessFunctions).FirstOrDefault(ctx => ctx.Account.Email.ToUpper() == username.ToUpper());
        if(user is not null)
        {
            user.Roles = _model.UserRoles_.Where(r => user.BusinessFunctions.Select(bd => bd.RoleId).Contains(r.Id)).ToList();
            if (user.Roles is null)
                user.Roles = new();
        }
            
        return user;
    }

    public MethodResult<UserContext> SigninByLoginAndPassword(string email, string password)
    {
        return Signin(new UserAccount()
        {
            Email = email,
            Password = password
        });
    }

    public MethodResult<UserContext> Signin(string email, string password)
        => Signin(new UserAccount(email, password));
    public override MethodResult<UserContext> Signin(UserAccount item)
    {
        MethodResult<UserContext> result = null;
        try
        {
            var user = GetBy(item);
            if (user == null)
            {
                var ex = new Exception("Пользователь не зарегистрирован");
                result = MethodResult<UserContext>.OnError(ex);
                throw ex;
            }
            if (user != null && user.Account.Hash != item.Hash)
            {
                var ex = new Exception("Пароль задан неверно");
                result = MethodResult<UserContext>.OnError(ex);
                throw ex;
            }
            if ((((AuthorizationUsers)_users).Has(user.SecretKey)))
            {
                _users.Remove(user.SecretKey);
            }
            var key = ((AuthorizationUsers)_users).GetByEmail(item.Email);
            if (key != null)
                _users.Remove(key);
            _tokenProvider.Set(user.SecretKey = _users.Put(user));
            _model.UserContexts.Find(user.Id).SecretKey = user.SecretKey;
            user.UserAgent += _httpContextAccessor.HttpContext.Request.Headers.UserAgent;
            user.LoginCount += 1;
            _model.SaveChanges();
            result = MethodResult<UserContext>.OnResult(user);            
            return result;
        }
        catch(Exception)
        {
            throw;
        }

    }
    private bool Compare(UserAccount stored, UserAccount input) => stored.Password == input.Password;

    public bool Signout() => Signout(_tokenProvider.Get());
    public override bool Signout(string key)
    {
        if (key != null)
        {
            var user = _users.Remove(key);
            _model.UserContexts.Find(user.Id).IsActive = false;
            _model.SaveChanges();
        }
        return true;
    }
    public override bool Validate(string key)
    {
        var result = key != null && _users.GetMemory().ContainsKey(key);
        return result;
    }

    public IEnumerable<string> GetRoles(string key)
    {
        if (IsSignin() == false)
        {
            return new List<string>();
        }            
        else
        {
            if (Verify().Roles is null)
                throw new Exception("Пользователь не содержит сведения о ролях UserContext.Roles");
            return Verify().Roles.Select(role => role.Code).ToList();
        }        
    }

    public bool IsInRole(params string[] roles)
    {        
        string token = this._tokenProvider.Get();
        if(String.IsNullOrWhiteSpace(token))
        {
            return false;
        }
        else
        {
            var current = GetRoles(token);
            foreach( var role in roles ) 
            {
                if (current.Contains(role) == false)
                    return false;
            }
        }
        return true;
    }

    public UserContext Verify()
    {
        var token = this._tokenProvider.Get();
        return String.IsNullOrEmpty(token)? null: this._users.Take(token);
    }


    public UserContext GetUserByToken(string token)
    {
        return String.IsNullOrEmpty(token) ? null : this._users.Take(token);
    }

    public List<string> GetNotifications()
    {
        if (IsSignin() == false)
        {
            return new List<string>();
        }
        else
        {
            ConcurrentQueue<string> notifications = GetFromSession<ConcurrentQueue<string>>("InputNotificationsQueue");
            var arr = notifications.ToList();
            notifications.Clear();
            return arr;
        }
    }

    public int PushNotifcation(string notification)
    {
        ConcurrentQueue<string> notifications = GetFromSession<ConcurrentQueue<string>>("InputNotificationsQueue");
        notifications.Enqueue(notification);

        return notifications.Count();
    }

    
    public override void PutIntoSession (string key, object item)
    {
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            _httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = item;
            }
          
        }
    }
    public override T GetFromSession<T>(string key)
    {
      
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            _httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = typeof(T).New();
            }
            return (T)session[key];
        }
            
        
    }
    public T GetFromSession<T>(string key, T defaultValue)
    {
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            _httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = defaultValue;
            }
            return (T)session[key];
        }


    }
}
*/




/*
public sealed class SigninUser<DeliveryDbContext> : BaseSignin<UserContext, UserAccount, UserPerson>
{
    public DbContextUser _model;
    private readonly APIActiveCollection<UserContext> _users;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SigninUser(IHttpContextAccessor http, DbContextUser model, AuthorizationUsers users, ITokenProvider tokenProvider): base(tokenProvider)
    {
        this._model = model;
        this._users = users;
        _httpContextAccessor = http;

    }

    public void UpdateUrlLocation(string url)
    {
        if(IsSignin())
        {
            using (var db = new DbContextUser())
            {
                var userId = Verify().Id;
                var user = db.UserContexts.Find(userId);
                if(user is null)
                {
                    throw new Exception($"Пользователь с ид {userId} не найден в БД");
                }
                else
                {
                    user.URL = url;
                    db.SaveChanges();
                }                
            }
        }
    }

    public bool IsSignin()
    {
        string token = this._tokenProvider.Get();
        return String.IsNullOrWhiteSpace(token) ? false: this.Validate(token);
    }
    public UserContext GetBy(UserAccount item)
    {
        return GetBy(item.Email);
    }

    public UserContext GetBy(string username)
    {
        var user = _model.UserContexts.Include(ctx => ctx.Wallet).Include(ctx => ctx.Settings).Include(ctx => ctx.Person).Include(ctx => ctx.Account).Include(ctx => ctx.BusinessFunctions).FirstOrDefault(ctx => ctx.Account.Email.ToUpper() == username.ToUpper());
        if(user is not null)
        {
            user.Roles = _model.UserRoles_.Where(r => user.BusinessFunctions.Select(bd => bd.RoleId).Contains(r.Id)).ToList();
            if (user.Roles is null)
                user.Roles = new();
        }
            
        return user;
    }

    public MethodResult<UserContext> SigninByLoginAndPassword(string email, string password)
    {
        return Signin(new UserAccount()
        {
            Email = email,
            Password = password
        });
    }

    public MethodResult<UserContext> Signin(string email, string password)
        => Signin(new UserAccount(email, password));
    public override MethodResult<UserContext> Signin(UserAccount item)
    {
        MethodResult<UserContext> result = null;
        try
        {
            var user = GetBy(item);
            if (user == null)
            {
                var ex = new Exception("Пользователь не зарегистрирован");
                result = MethodResult<UserContext>.OnError(ex);
                throw ex;
            }
            if (user != null && user.Account.Hash != item.Hash)
            {
                var ex = new Exception("Пароль задан неверно");
                result = MethodResult<UserContext>.OnError(ex);
                throw ex;
            }
            if ((((AuthorizationUsers)_users).Has(user.SecretKey)))
            {
                _users.Remove(user.SecretKey);
            }
            var key = ((AuthorizationUsers)_users).GetByEmail(item.Email);
            if (key != null)
                _users.Remove(key);
            _tokenProvider.Set(user.SecretKey = _users.Put(user));
            _model.UserContexts.Find(user.Id).SecretKey = user.SecretKey;
            user.UserAgent += _httpContextAccessor.HttpContext.Request.Headers.UserAgent;
            user.LoginCount += 1;
            _model.SaveChanges();
            result = MethodResult<UserContext>.OnResult(user);            
            return result;
        }
        catch(Exception)
        {
            throw;
        }

    }
    private bool Compare(UserAccount stored, UserAccount input) => stored.Password == input.Password;

    public bool Signout() => Signout(_tokenProvider.Get());
    public override bool Signout(string key)
    {
        if (key != null)
        {
            var user = _users.Remove(key);
            _model.UserContexts.Find(user.Id).IsActive = false;
            _model.SaveChanges();
        }
        return true;
    }
    public override bool Validate(string key)
    {
        var result = key != null && _users.GetMemory().ContainsKey(key);
        return result;
    }

    public IEnumerable<string> GetRoles(string key)
    {
        if (IsSignin() == false)
        {
            return new List<string>();
        }            
        else
        {
            if (Verify().Roles is null)
                throw new Exception("Пользователь не содержит сведения о ролях UserContext.Roles");
            return Verify().Roles.Select(role => role.Code).ToList();
        }        
    }

    public bool IsInRole(params string[] roles)
    {        
        string token = this._tokenProvider.Get();
        if(String.IsNullOrWhiteSpace(token))
        {
            return false;
        }
        else
        {
            var current = GetRoles(token);
            foreach( var role in roles ) 
            {
                if (current.Contains(role) == false)
                    return false;
            }
        }
        return true;
    }

    public UserContext Verify()
    {
        var token = this._tokenProvider.Get();
        return String.IsNullOrEmpty(token)? null: this._users.Take(token);
    }


    public UserContext GetUserByToken(string token)
    {
        return String.IsNullOrEmpty(token) ? null : this._users.Take(token);
    }

    public List<string> GetNotifications()
    {
        if (IsSignin() == false)
        {
            return new List<string>();
        }
        else
        {
            ConcurrentQueue<string> notifications = GetFromSession<ConcurrentQueue<string>>("InputNotificationsQueue");
            var arr = notifications.ToList();
            notifications.Clear();
            return arr;
        }
    }

    public int PushNotifcation(string notification)
    {
        ConcurrentQueue<string> notifications = GetFromSession<ConcurrentQueue<string>>("InputNotificationsQueue");
        notifications.Enqueue(notification);

        return notifications.Count();
    }

    
    public override void PutIntoSession (string key, object item)
    {
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            _httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = item;
            }
          
        }
    }
    public override T GetFromSession<T>(string key)
    {
      
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            _httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = typeof(T).New();
            }
            return (T)session[key];
        }
            
        
    }
    public T GetFromSession<T>(string key, T defaultValue)
    {
        var session = this._users.GetSession(this._tokenProvider.Get());
        if (session == null)
        {
            _httpContextAccessor.HttpContext.Response.Redirect("/Auth/Signin");
            throw new Exception("Сессия недоступна неавторизованному контексту")
            {


            };
        }
        else
        {
            if (session.ContainsKey(key) == false)
            {
                session[key] = defaultValue;
            }
            return (T)session[key];
        }


    }
}
*/