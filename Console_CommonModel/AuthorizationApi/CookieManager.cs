using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

public class CookieManager: ITokenProvider
{
    //public static int Counter = StartupApplication.Add((services)=> {
    //    services.AddHttpContextAccessor();
    //    services.AddScoped<CookieManager>();
    //});


    private readonly IHttpContextAccessor _accessor;
    private readonly ConcurrentDictionary<string, string> _cookies;

    public CookieManager(IHttpContextAccessor accessor)
    {
        _cookies = new ConcurrentDictionary<string, string>();
        _accessor = accessor;
        var httpContext = _accessor.HttpContext;
        if (httpContext?.Request?.Cookies != null)
        {
            foreach (var cookie in httpContext.Request.Cookies)
            {
                _cookies[cookie.Key] = cookie.Value;
            }
        }
    }

    public void SetCookie(string key, string value)
    {
        _cookies[key] = value;
        var response = _accessor.HttpContext?.Response;
        if (response != null && response.HasStarted == false)
        {
            response.Cookies.Append(key, value);
        }
    }

    public string GetCookie(string key)
    {
        if (_cookies.ContainsKey(key))
            return _cookies[key];
        return null;
    }

    public void RemoveCookie(string cookies)
    {
        _cookies.TryRemove(cookies, out _);
        _accessor.HttpContext?.Response?.Cookies.Delete(cookies);
    }

    public string Get()
    {
        return GetCookie(GetType().Name);

    }

    public async Task Set(string token)
    {
        SetCookie(GetType().Name,token);
        await Task.CompletedTask;
    }

    public void SetDirectory(string name)
    {
    }
}
