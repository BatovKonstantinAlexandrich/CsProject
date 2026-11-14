using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MarketerWeb.Authorization;

public class AuthStateProvider<TDb> : AuthenticationStateProvider, IDisposable
    where TDb : DeliveryDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SigninUser<TDb> _signinUser;

    public UserContext? CurrentUser { get; set; } = new();

    public AuthStateProvider(SigninUser<TDb> signin, IHttpContextAccessor httpContextAccessor )
    {
        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
        _httpContextAccessor = httpContextAccessor;
        _signinUser = signin;
    }

    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        var authenticationState = await task;
        if (authenticationState is not null)
        {
            //var email = authenticationState.User.Claims.FirstOrDefault(c => c.Type == Enum.GetName(ClaimTypes.Name))?.Value;
            
            //CurrentUser = FromClaimsPrincipal();
        }
    }



    public UserContext GetUserFromHttp()
    {
        var headers = _httpContextAccessor.HttpContext.Request.Headers;        
        if ( headers.ContainsKey("X-Auth-Roles") == false )
        {
            return null;
        }
        else
        {
            var roles = new List<string>();
            if(headers.ContainsKey("X-Auth-Roles") && headers["X-Auth-Roles"].Count() > 0)
            {
                foreach(var text in headers["X-Auth-Roles"].ToList())
                {
                    roles.AddRange(text.Split(",").ToList());
                }
            }
            var userId = headers.ContainsKey("X-Auth-UserId") && headers["X-Auth-UserId"].Count() > 0? headers["X-Auth-UserId"].First(): "";
            return new UserContext()
            {
                //Roles = roles,
                //UserId = userId
            };
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();        
        var user = GetUserFromHttp();
        if (user is not null)
        {
            //var  roles = CurrentUser.Roles.ToHashSet();
            //roles.ExceptWith(user.Roles);
            CurrentUser = user;
            principal = user.ToClaimsPrincipal();            
        }       
        await Task.CompletedTask;
        return new(principal);
    }

    public async Task LoginAsync()
    {
        var principal = new ClaimsPrincipal();
        var user = GetUserFromHttp();
        var headers = _httpContextAccessor.HttpContext.Request.Headers;
        headers["X-Auth-UserId"] = user == null ?
            new Microsoft.Extensions.Primitives.StringValues() :
            new Microsoft.Extensions.Primitives.StringValues(user.Id.ToString());
        headers["X-Auth-Roles"] = user == null ?
            new Microsoft.Extensions.Primitives.StringValues() :
            new Microsoft.Extensions.Primitives.StringValues(user.BusinessResources.Select(r => r.Name).ToArray());
        CurrentUser = user;

        if (user is not null)
        {
            principal = user.ToClaimsPrincipal();
        }
        await Task.CompletedTask;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task LoginAsync(string username, string password)
    {
        var principal = new ClaimsPrincipal();
        var user = await FindUserFromDatabaseAsync(username, password);
        var headers = _httpContextAccessor.HttpContext.Request.Headers;
        headers["X-Auth-UserId"] = user == null ?
            new Microsoft.Extensions.Primitives.StringValues():
            new Microsoft.Extensions.Primitives.StringValues(user.Id.ToString());
        headers["X-Auth-Roles"] = user == null?
			new Microsoft.Extensions.Primitives.StringValues(): 
			new Microsoft.Extensions.Primitives.StringValues(user.BusinessResources.Select(S=>S.Name).ToArray());
        CurrentUser = user;

        if (user is not null)
        {
            principal = user.ToClaimsPrincipal();
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private async Task<UserContext> FindUserFromDatabaseAsync(string username, string password)
    {
        using (var db = typeof(TDb).New<TDb>())
        {
            return _signinUser.FindUserContext(username, password);
        }
    }

    public async Task LogoutAsync()
    {
        // TODO: ClearBrowserUserDataAsync();
        var headers = _httpContextAccessor.HttpContext.Request.Headers;
        headers["X-Auth-UserId"] = new Microsoft.Extensions.Primitives.StringValues();
        headers["X-Auth-Roles"] = new Microsoft.Extensions.Primitives.StringValues();
        await Task.CompletedTask;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
    }

    public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
}
