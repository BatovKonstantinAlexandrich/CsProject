/*

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;

using Newtonsoft.Json.Linq;

using Parlot.Fluent;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LocalStorage : ILocalStorage
{
    private static string KEY = "PublicKey";

    private readonly IJSRuntime _jsr;
    private readonly IJSInvoke _jss;

    public LocalStorage(IJSRuntime jsr, IJSInvoke jss)
    {
        _jsr = jsr;
        _jss = jss;
    }
    public async Task SetItem(string Key, string Value)
    {
        string value = await _jss.EvalAsync<string>($"localStorage.setItem('{Key}','{Value}'); return localStorage.getItem('{Key}');");
        if (value != Value)
        {
            throw new Exception("В локальное хранилище данные записаны неверно. Ключ " + Key);
        }
    }
    public async Task<string> GetItem(string Key)
    {
        return await _jss.EvalAsync<string>($"return localStorage.getItem('{Key}');");
    }
    public async Task<List<string>> GetKeys()
    {
        return await _jss.EvalAsync<List<string>>("return Object.getOwnPropertyNames(localStorage);");
    }

    public Task SetToken(string token)
    {
        return this.SetItem(KEY, token);
    }

    public Task<string> GetToken()
    {
        return this.GetItem(KEY);
    }
}

*/
using System.Collections.Concurrent;
using System.Threading.Tasks;

public interface ITokenProvider
{
    public string Get();
    public Task Set(string token);
    void SetDirectory(string name);
}/*
public class HttpHeaderTokenProvider : ITokenProvider
{ 
    private readonly IHttpContextAccessor _http;

    public HttpHeaderTokenProvider(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string Get()
    {       
        return _http.HttpContext.Request.Headers.ContainsKey("Authorization") ?
            _http.HttpContext.Request.Headers["Authorization"].ToString() : null;
    }
    public async Task Set(string id)
    {

        this.Info($"{GetType().GetTypeName()} Set {id}");
        _http.HttpContext.Request.Headers["Authorization"] = $"Basic {id}";
        / *_http.HttpContext.Response.Cookies.Append(
                           "Authorization", id,
                           new CookieOptions() { SameSite = SameSiteMode.Unspecified });* /

    }    
}
*/
/*

public class HttpTokenProvider : ITokenProvider
{

    private readonly IHttpContextAccessor _accessor;
    private readonly ConcurrentDictionary<string, string> _cookies;
    public HttpTokenProvider(IHttpContextAccessor accessor)
    {
        _cookies = new ConcurrentDictionary<string, string>();
        _accessor = accessor;
        this.Init();
    }

    private void Init()
    {
        foreach (var cookie in _accessor.HttpContext.Request.Cookies)
        {
            _cookies[cookie.Key] = cookie.Value;
        }
    }

    public void SetCookie(string key, string value)
    {
        _accessor.HttpContext.Response.Cookies.Append(key, value);
        _cookies[key] = value;
    }

    public string GetCookie(string key)
    {
        //string value = null;
        //_accessor.HttpContext.Request.Cookies.TryGetValue(key, out value);
        //return value;
        if (_cookies.ContainsKey(key))
            return _cookies[key];
        else return null;
    }

    public string Get()
    {
        return GetCookie("Authorization");
    }

    public async Task Set(string token)
    {
        await Task.CompletedTask;
        SetCookie("Authorization", token);
    }
}
*/
public class ClientLocalStorageTokenProvider :  ITokenProvider
{ 

    private string KEY = "Authorization";
    private readonly ILocalStorage _localStorage;


    public ClientLocalStorageTokenProvider(ILocalStorage localStorage)
    {
        _localStorage = localStorage;

    }

    public async Task<string> Get()
    {
     
        string token = await _localStorage.GetItem(KEY);
        return token;
    }

    public async Task Set(string token)
    {
        await _localStorage.SetItem(KEY, token);
    }

    string ITokenProvider.Get()
    {
        string token = _localStorage.GetItem(KEY).Result;
        return token;
    }

    public void SetDirectory(string name)
    {
        this.KEY = name;
    }
}
public class MemoryTokenProvider : ITokenProvider
{
    private static ConcurrentDictionary<string,string> tokens = new();

    public MemoryTokenProvider()
    {
    }
    static int counter = 1;

  
    public string KEY = $"{counter++}";

    public string Get()
    {
        return tokens.ContainsKey(KEY)? tokens[KEY]: null;
    }

    public async Task Set(string token)
    {
        tokens[KEY] = token;
        await Task.CompletedTask;
    }

    public void SetDirectory(string name)
    {
        this.KEY = name;
    }
}