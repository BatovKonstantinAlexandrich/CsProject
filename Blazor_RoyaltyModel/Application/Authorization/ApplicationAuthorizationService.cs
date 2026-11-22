using System.Security.Claims;
using Console_RoyaltyData.RoyaltyData;
using Console_RoyaltyData.RoyaltyEntities;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor_SiteModel.Application.Authorization;

public class ApplicationAuthorizationService(

    RoyaltyDbContext royaltyDbContext,
    IHttpContextAccessor httpContextAccessor,
    AuthenticationStateProvider authenticationStateProvider)
{
    private const string AdLoginClaim = "ad_login";

    public bool IsSigned() =>
        CurrentUser().Identity?.IsAuthenticated == true;

    public string GetAdLogin()
    {
        if (!IsSigned())
            throw new InvalidOperationException("Пользователь не авторизован.");

        var login = CurrentUser().FindFirst(AdLoginClaim)?.Value;
        if (string.IsNullOrWhiteSpace(login))
            throw new InvalidOperationException("В токене Keycloak отсутствует claim ad_login.");

        return login;
    }

    private ClaimsPrincipal CurrentUser()
    {
        var httpUser = httpContextAccessor.HttpContext?.User;
        if (httpUser?.Identity?.IsAuthenticated == true)
            return httpUser;

        return authenticationStateProvider
            .GetAuthenticationStateAsync()
            .GetAwaiter()
            .GetResult()
            .User;
    }


    public Employee GetEmployee()
    {
        return royaltyDbContext.Employees.FirstOrDefault(e => e.AdLogin == this.GetAdLogin());        
    }

    public Dictionary<string, string> GetEmployeeMainMenu()
    {
        return new Dictionary<string, string>
        {
            ["Расчёт роялти"] = "/royalty",
            ["Основные параметры"] = "/royalty/main-settings",
            ["Карточка предприятий"] = "/royalty/company-card",
            ["Полученные документы"] = "/royalty/received-documents"
        };
    }
}
