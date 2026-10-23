 
using ApplicationDb.Entities;
 
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


/// <summary>
/// Служба транспортного уровня, выполняет авторизацию, регистрацию и идентификацияю пользователей
/// </summary>
public class AuthorizationService: RegistrationService, APIAuthorization
{
    //public new static int Counter = StartupApplication.AddWithOptions((services,conf)=> {
    //    services.AddAuthorizationServices(conf);
    //});

    private UserContext _user;

    private readonly ILogger<AuthorizationService> _logger;
    private readonly APIUsers _users;
    private readonly APIServices _services;
    private readonly ApplicationDbContext _context;
    private readonly CookieManager _cookieManager;

    public UserMessagesService<ApplicationDbContext> _messages { get; }

    private readonly AuthorizationOptions _options;
    private readonly IHttpContextAccessor _accessor;
    



    public AuthorizationService(
            CookieManager cookieManager,
            UserMessagesService<ApplicationDbContext> messages,
            ILogger<AuthorizationService> logger,
            ILogger<RegistrationService> registrationLogger,
            IHttpContextAccessor accessor,
       
            AuthorizationOptions options,
            IEmailService email,
            APIUsers users,
            APIServices services,
            ApplicationDbContext db ): base(registrationLogger,db, options, email) {
        
        _logger = logger;
       // _logger.LogInformation("Created");
        _users = users;
        _services = services;
        _context = db;
        _cookieManager = cookieManager;
        _messages = messages;
        _options = options;
        _accessor = accessor;            
    }


    /// <summary>
    /// Метод выхода пользователя из сеанса
    /// </summary>
    public void Signout(bool? IsFront=false)
    {
        if( _options.LoggingAuth)               
            _logger.LogInformation("Signout");
        UserContext user = Verify(IsFront);
        lock (_users)
        {
            if (user != null && _users.Has(user.SecretKey))
            {
                _users.Remove(user.SecretKey);
                _context.SaveChanges();
            }
            if (IsFront==false)
            {
                _accessor.HttpContext.Response.Redirect(_options.LoginPagePath);
            }
            
        }            
    }


    /// <summary>
    /// Авторизация пользователя в системе
    /// </summary>
    /// <param name="Email">электронный адрес</param>
    /// <param name="Password">пароль</param>
    public UserContext Signin( string Email, string Password, bool? IsFront=false )
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            if (IsFront == true)
                throw new Exception("Укажите e-mail и пароль.");
            return null;
        }

        if (_options.LoggingAuth)
            _logger.LogInformation("Signin");

        if( IsSignin())
        {
            UserContext user = Verify();
            lock (_users)
            {
                if (user != null && _users.Has(user.SecretKey))
                {
                    _users.Remove(user.SecretKey);
                    _context.SaveChanges();
                }
            }
        }

        UserContext current = GetUserByEmail(Email.Trim());
        var passwordHash = GetHashSha256(Password);
        if (current == null || current.Account == null || string.IsNullOrEmpty(current.Account.Hash) ||
            !string.Equals(passwordHash, current.Account.Hash, StringComparison.Ordinal))
        {
            if(IsFront == false)
            {
                _accessor.HttpContext?.Response?.Redirect(_options.LoginPagePath);
            }
            else
            {
                throw new Exception("Учётные данные не зарегистрированы");
            }
            return null;
        }

        if (_options.LoggingAuth)
            _logger.LogInformation("Логин и пароль правильные");

        try
        {
            current.Groups = (from g in _context.UserGroups
                              where (from p in current.UserGroups select p.GroupId).Contains(g.Id)
                              select g).ToList();
            var userGroupIds = current.Groups.Select(p => p.Id).ToList();
            //current.BusinessFunctions = (from p in _context.GroupsBusinessFunctions
            //                               .Include(b => b.BusinessFunction)
            //                               where userGroupIds.Contains(p.GroupId)
            //                               select p.BusinessFunction)
            //    .Where(b => b != null)
            //    .ToList();

            _user = current;
            string currentKey = _users.Find(current);
            current.SecretKey = currentKey != null ? currentKey : _users.Put(current);
            current.IsActive = true;
            _context.SaveChanges();

            try
            {
                //_context.LoginFacts.Add(new UserAuthEvent()
                //{
                //    Created = DateTime.Now,
                //    User = current,
                //    Calendar = GetTodayCalendar()
                //});
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось записать факт входа пользователя");
            }

            _cookieManager.SetCookie(_options.UserCookie, current.SecretKey);
        }
        catch (Exception ex) when (IsFront == true)
        {
            throw new Exception(ex.InnerException?.Message ?? ex.Message, ex);
        }

        return current;
    }

    public Calendar GetTodayCalendar()
    {

 
        
            Calendar c = (from cal in _context.Calendars where cal.Timestamp == Timing.GetTodayBeginTime() select cal).FirstOrDefault();
            DateTime p = DateTime.Now;
            if (c == null)
            {
                _context.Calendars.Add(c = new Calendar()
                {
                    Day = p.Day,
                    Quarter = p.Month < 4 ? 1 : p.Month < 7 ? 2 : p.Month < 10 ? 3 : 4,
                    Month = p.Month,
                    Week = 1,
                    Year = p.Year,
                    Timestamp = (long)((new DateTime(p.Year, p.Month, p.Day) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds)
                });
                _context.SaveChanges();
            }
            return c;
        
    }




    /// <summary>
    /// Идентификация пользователя в системе
    /// </summary>
    /// <returns>ссылка на обьект сеанса</returns>
    public UserContext Verify(bool? IsFront = false)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation("Verify");
        if ( _user != null)
        {
            return _user;
        }
        string SecretKey = _cookieManager.GetCookie(_options.UserCookie);        
        if(SecretKey == null)
        {
            if (IsFront == false)
            {
                _accessor.HttpContext?.Response?.Redirect(_options.LoginPagePath);
            }
            return null;
        }
        else
        {
            UserContext user = _users.Take(SecretKey);
            if (user == null)
            {
                var response = _accessor.HttpContext?.Response;
                if (response != null && response.HasStarted == false)
                {
                    response.Cookies.Delete(_options.UserCookie);
                    if (IsFront == false)
                    {
                        response.Redirect(_options.LoginPagePath);
                    }
                }
            }
            else
            {
                _cookieManager.SetCookie(_options.UserCookie, SecretKey);
                _user = user;
            }
            return user;
        }
    }


        


    /// <summary>
    /// Получечние текущего времени в милисекундах
    /// </summary>
    /// <returns></returns>
    private long GetTimestamp()
    {
        return (long)(((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0))).TotalMilliseconds);
    }




    /// <summary>
    /// Применение функции хэширования к последовательности символов
    /// </summary>
    /// <param name="text">последовательность</param>
    /// <returns>хэш</returns>
    private string GetHashSha256(string text)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation("GetHashSha256");
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }


    /// <summary>
    /// Проверка выполнения процедуры авторизации
    /// </summary>
    /// <returns></returns>
    public bool IsSignin()
    {
        if (_options.LoggingAuth)
            _logger.LogInformation("IsSignin");

        if (_user != null && !string.IsNullOrEmpty(_user.SecretKey) && _users.Has(_user.SecretKey))
            return true;

        string SecretKey = _cookieManager.GetCookie(_options.UserCookie);        
        if (SecretKey == null)
            return false;

        var httpContext = _accessor.HttpContext;
        UserContext user = _users.Take(SecretKey);
        if (user == null)
        {
            if (httpContext?.Response != null && httpContext.Response.HasStarted == false)
                httpContext.Response.Cookies.Delete(_options.UserCookie);
            return false;
        }

        if (httpContext?.Response != null && httpContext.Response.HasStarted == false)
            _cookieManager.SetCookie(_options.UserCookie, SecretKey);
        _user = user;
        return true;

    }


    /// <summary>
    /// Метод получения атрибутов сеанса
    /// </summary>
    /// <returns></returns>
    public ConcurrentDictionary<string, object> Session()
    {
        if (_options.LoggingAuth)
            _logger.LogInformation("Session");
        string SecretKey = null;
        _accessor.HttpContext.Request.Cookies.TryGetValue(_options.UserCookie, out SecretKey);
        if (SecretKey == null)
        {
            return null;
        }
        else
        {
            ConcurrentDictionary<string, object> session = _users.GetSession(SecretKey);
            if (session == null)
            {
                _accessor.HttpContext.Response.Cookies.Delete(_options.UserCookie);
            }
            else
            {
                if (_accessor.HttpContext.Response.HasStarted == false)
                {                    
                    _cookieManager.SetCookie(_options.UserCookie, SecretKey);
                }
                
            }
            return session;
        }
    }



    /// <summary>
    /// Проверка принадлежности пользователя к роли
    /// </summary>
    /// <param name="roleName">наменование роли</param>
    /// <returns></returns>
    public bool InRole(string roleName)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation("InRole");
        UserContext user = this.Verify();
        if (user == null)
        {
            return false;
        }
        else
        {
            //BusinessResource prole = user.Role;
            //while( prole != null)
            //{
            //    if (prole.Code == roleName)
            //    {
            //        return true;
            //    }
            //    if( prole.ParentId == null)
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        if(prole.ParentId == prole.Id)
            //        {
            //            break;
            //        }
            //        prole = _context.BusinessResources.Find((int)prole.ParentId);
            //    }
            //}
            return false;
        }
    }


    /// <summary>
    /// Проверка активации учетной записи
    /// </summary>
    /// <returns></returns>
    public bool IsActivated()
    {
        _logger.LogInformation("IsActivated");
        UserContext user = Verify();
        if( user == null)
        {
            return false;
        }
        else
        {
            return _context.UserAccounts.Find(user.Account.Id).Activated != null;
        }
    }

    public UserContext Signin(string RFIdLabel)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation("Signin");
        if (IsSignin())
        {
            Signout();
        }

        UserContext current = (from user
                        in _context.UserContexts
                            .Include(a => a.Account)
                            .Include(a => a.Settings)
                            .Include(a => a.Person)
                            //.Include(a => a.Role)
                            .Include(a => a.UserGroups)
                        where user.Account.RFId == RFIdLabel
                        select user).FirstOrDefault();
        if (current == null )
        {            
            throw new AccessViolationException("Учетные данные не зарегистрированы");
        }
        else
        {
            _user = current;
            current.Groups = (from grs
                                in _context.UserGroups
                                where (from ug in current.UserGroups select ug.GroupId).Contains(grs.Id)
                                select grs).ToList();
            string currentKey = _users.Find(current);
            current.SecretKey = currentKey != null ? currentKey : _users.Put(current);
            current.IsActive = true;
            long timestamp = Timing.GetTodayBeginTime();
            _context.SaveChanges();
            //_context.LoginFacts.Add(new UserAuthEvent()
            //{
            //    Created = DateTime.Now,
            //    User = current,
            //    Calendar = (from cal in _context.Calendars where cal.Timestamp == timestamp select cal).First()
            //});
            _context.SaveChanges();
            _cookieManager.SetCookie(_options.UserCookie, current.SecretKey);            
            return current;
        }
            
    }

    public string Login(string Email, string Password)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation("Login");

        if (IsSignin())
        {
            UserContext user = Verify();
            lock (_users)
            {
                if (user != null && _users.Has(user.SecretKey))
                {
                    _users.Remove(user.SecretKey);
                    _context.SaveChanges();
                }
            }
        }

        UserContext current = GetUserByEmail(Email?.Trim());
        var passwordHash = GetHashSha256(Password);
        if (current == null || current.Account == null || string.IsNullOrEmpty(current.Account.Hash) ||
            !string.Equals(passwordHash, current.Account.Hash, StringComparison.Ordinal))
        {
            return null;
        }

        current.Groups = (from g in _context.UserGroups
                          where (from p in current.UserGroups select p.GroupId).Contains(g.Id)
                          select g).ToList();
        var userGroupIds = current.Groups.Select(p => p.Id).ToList();
        //current.BusinessFunctions = (from p in _context.GroupsBusinessFunctions
        //                               .Include(b => b.BusinessFunction)
        //                               where userGroupIds.Contains(p.GroupId)
        //                               select p.BusinessFunction)
        //    .Where(b => b != null)
        //    .ToList();

        _user = current;
        string currentKey = _users.Find(current);
        current.SecretKey = currentKey != null ? currentKey : _users.Put(current);
        current.IsActive = true;
        _context.SaveChanges();

        try
        {
            //_context.LoginFacts.Add(new UserAuthEvent()
            //{
            //    Created = DateTime.Now,
            //    User = current,
            //    Calendar = GetTodayCalendar()
            //});
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось записать факт входа пользователя");
        }

        _cookieManager.SetCookie(_options.UserCookie, current.SecretKey);
        return current.SecretKey;
    }

    public IReadOnlyList<ServiceContext> GetConnectedServices()
    {
        //var user = Verify(true);
        //if (user == null)
        //    return Array.Empty<ServiceContext>();

        //return _context.ServiceContexts
        //    .AsNoTracking()
        //    .Include(s => s.Account)
        //    .Where(s => s.PersonId == user.PersonId)
        //    .OrderBy(s => s.Name)
        //    .ToList();
        throw new NotImplementedException();
    }

    public ServiceContext RegisterService(string name, string email, string password, string description = null)
    {
        //if (string.IsNullOrWhiteSpace(name))
        //    throw new Exception("Укажите имя сервиса.");
        //if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        //    throw new Exception("Укажите e-mail и пароль сервиса.");

        //var user = Verify(true);
        //if (user == null)
        //    throw new Exception("Требуется авторизация пользователя.");

        //var normalizedEmail = email.Trim().ToLower();
        //if (_context.ServiceContexts.Any(s => s.Account != null && s.Account.Email != null && s.Account.Email.ToLower() == normalizedEmail))
        //    throw new Exception("Сервис с таким e-mail уже зарегистрирован.");

        //var account = new UserAccount
        //{
        //    Email = email.Trim(),
        //    Password = password,
        //    Hash = GetHashSha256(password),
        //    RFId = Guid.NewGuid().ToString("N"),
        //    ActivationKey = Guid.NewGuid().ToString("N"),
        //    Activated = DateTime.UtcNow
        //};

        //var settings = new UserSettings
        //{
        //    ColorTheme = "Light",
        //    VertialLayout = false,
        //    FocusControls = false,
        //    PublicOperations = false,
        //    SendNewsToEmail = false,
        //    ShowHelp = true,
        //    EvaluateMe = false
        //};

        //var service = new Service
        //{
        //    Name = name.Trim(),
        //    Description = description,
        //    Account = account,
        //    Settings = settings,
        //    Role = user.Role,
        //    Person = user.Person,
        //    SecretKey = GuId.NewGuId().ToString("N"),
        //    LastActive = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds,
        //    IsActive = false,
        //    LoginCount = 0
        //};

        //_context.UserAccounts.Add(account);
        //_context.Settings.Add(settings);
        //_context.Services.Add(service);
        //_context.SaveChanges();

        //return service;
        throw new NotImplementedException();
    }

    public ServiceContext SigninService(string email, string password, bool? IsFront = false)
    {
        //if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        //{
        //    if (IsFront == true)
        //        throw new Exception("Укажите e-mail и пароль сервиса.");
        //    return null;
        //}

        //var normalizedEmail = email.Trim().ToLower();
        //var current = _context.Services
        //    .Include(s => s.Account)
        //    .Include(s => s.Role)
        //    .Include(s => s.Person)
        //    .Include(s => s.Settings)
        //    .FirstOrDefault(s => s.Account != null && s.Account.Email != null && s.Account.Email.ToLower() == normalizedEmail);

        //var passwordHash = GetHashSha256(password);
        //if (current == null || current.Account == null || string.IsNullOrEmpty(current.Account.Hash) ||
        //    !string.Equals(passwordHash, current.Account.Hash, StringComparison.Ordinal))
        //{
        //    if (IsFront == true)
        //        throw new Exception("Учётные данные сервиса не зарегистрированы");
        //    return null;
        //}

        //string currentKey = _services.Find(current);
        //current.SecretKey = currentKey ?? _services.Put(current);
        //current.IsActive = true;
        //_context.SaveChanges();
        //_cookieManager.SetCookie(_options.ServiceCookie, current.SecretKey);
        //return current;
        throw new NotImplementedException();
    }

    public ServiceContext SinginService(string email, string password, bool? IsFront = false)
    {
        return SigninService(email, password, IsFront);
    }

    public bool InBusinessResource(string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<UserAccount> GetAccountById(int iD)
    {
        throw new NotImplementedException();
    }

    public string GetUserHomeUrl()
    {
        throw new NotImplementedException();
    }
}

