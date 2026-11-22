using Blazor_SiteModel.Application.Authorization;
using Blazor_SiteModel.Models;

using System.Linq;

namespace Blazor_SiteModel.Services;

public interface IMenuService
{
    IReadOnlyList<MenuLink> GetMainMenu();

    IReadOnlyList<MenuLink> GetFooterClients();

    IReadOnlyList<MenuLink> GetFooterCompany();

    IReadOnlyList<MenuLink> GetLegalLinks();
}

public sealed class MenuService(ApplicationAuthorizationService applicationAuthorizationService) : IMenuService
{
    
    public IReadOnlyList<MenuLink> GetMainMenu()
    {
        Dictionary<string,string> mainMenu = applicationAuthorizationService.GetEmployeeMainMenu();
        return mainMenu.Select(kv => new MenuLink(kv.Key, kv.Value)).ToList();
        
    }

    public IReadOnlyList<MenuLink> GetFooterClients() =>
    [
        new("Акции", "/promotions")
    ];

    public IReadOnlyList<MenuLink> GetFooterCompany() =>
    [
        new("Вакансии", "/vacancies")
    ];

    public IReadOnlyList<MenuLink> GetLegalLinks() =>
    [
        new("Контакты", "/info/kontakty")
    ];
}
