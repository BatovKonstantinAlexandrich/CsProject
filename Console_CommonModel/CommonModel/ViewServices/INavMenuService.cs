using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
 
public class NavMenuService 
{
    private readonly ILogger<NavMenuService> _logger;
    public NavMenuService(ILogger<NavMenuService> logger)
    {
        _logger = logger;
      
    }


    public List<MenuView> GetPageLinks(string uri)
    {
        _logger.LogInformation(uri);
        var navList = new List<MenuView>();
        string ns = "BlazorHospital.Client.Pages";
        var ids =
            uri.ReplaceAll("/", ".").ReplaceAll("\\", ".").Split(".").ToList().Where(s => string.IsNullOrEmpty(s) == false).Select(s => s.ToCapitalStyle()).ToList();
        ids.ForEach(id => ns += "." + id);
        _logger.LogInformation($"Namespace: {ns}");
        foreach (var page in Assembly.GetCallingAssembly().GetPages(ns))
        {
            var attrs = page.GetAttributes();
            if (attrs.ContainsKey("RouteAttribute"))
            {
                navList.Add(new MenuView() { Icon = "home", Text = page.GetTypeName(), Href = attrs["RouteAttribute"] });
            }
            if (attrs.ContainsKey("Route"))
            {
                navList.Add(new MenuView() { Icon = "home", Text = page.GetTypeName(), Href = attrs["Route"] });
            }

        };
        return navList.ToList();
    }

    public List<MenuView> GetDefaultMenuViews()
    {
        return new MenuView[] {
            new MenuView(){ Icon="home", Text="Домашняя", Href="/" },
            new MenuView(){ Icon="plus", Text="Версии", Href="/counter" },
            new MenuView(){ Icon="list-rich", Text="Редактор форм", Href="/input-forms" },
            new MenuView(){ Icon="person", Text="Редактор меню", Href="/nav-menu" }

        }.ToList();
    }

      
    public MenuView[] GetPageLinks()
    {
        return typeof(CommonProgram).Assembly
            .GetTypes()
            .Where(t => "BaseComponent".ToType().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetCustomAttributes<RouteAttribute>(inherit: true)
                .Select(route => new MenuView
                {
                    Text = t.Name,
                    Href = NormalizeRoute(route.Template),
                    Icon = "home"
                }))
            .Where(link => !string.IsNullOrWhiteSpace(link.Href))
            .GroupBy(link => link.Href, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(link => link.Href, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string NormalizeRoute(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return "/";
        var route = template.Trim();
        return route.StartsWith("/") ? route : "/" + route;
    }
}

public interface INavMenuService
{

    public Task OnNavTreeInit(object evt);
    public List<MenuView> GetPageLinks(string uri);
    public List<MenuView> GetPageLinks();
    public List<MenuView> GetMenuViews();
    public void Update(MenuView link);
    public void Remove(MenuView link);
    public void CreateMenuView();
}

 

public static class NavMenuServiceExtension
{
    public static IServiceCollection AddNavMenu(this IServiceCollection services)
    {
        //services.AddOpenIcons();
        services.AddScoped<NavMenuService>();
        return services;
    }
}
