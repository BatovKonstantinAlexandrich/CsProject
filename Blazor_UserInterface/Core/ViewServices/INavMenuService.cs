using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
 
public class NavMenuService : INavMenuService
{
    private readonly ILogger<NavMenuService> _logger;
    private readonly NavMenuDbContext _context;
    public NavMenuService(ILogger<NavMenuService> logger, NavMenuDbContext context)
    {
        _logger = logger;
        _context = context;
        if (GetNavLinks().Length == 0)
        {
            foreach (var navLink in GetDefaultNavLinks())
            {
                _context.NavLinks.Add(navLink);
                _context.SaveChanges();
            }
        }
    }
    public NavLink[] GetNavLinks()
    {
        return _context.NavLinks.ToArray();
    }

    public void CreateNavLink()
    {
        _context.NavLinks.Add(new global::NavLink() { Icon = "oi-home", Label = "Домашняя", Href = "/" });
        _context.SaveChanges();
    }

    public void Update(NavLink link)
    {
        _context.Update(link);
        _context.SaveChanges();
    }
    public void Remove(NavLink link)
    {
        _context.NavLinks.Remove(link);
        _context.SaveChanges();
    }

 

    public NavLink[] GetPageLinks(string uri)
    {
        _logger.LogInformation(uri);
        var navList = new List<NavLink>();
        string ns = "BlazorHospital.Client.Pages";
        var ids =
            uri.ReplaceAll("/", ".").ReplaceAll("\\", ".").Split(".").ToList().Where(s => string.IsNullOrEmpty(s) == false).Select(s => s.ToCapitalStyle()).ToList();
        ids.ForEach(id => ns += "." + id);
        _logger.LogInformation($"Namespace: {ns}");
        foreach (var page in Assembly.GetCallingAssembly().GetPages(ns))
        {
            var attrs = page.GetAttrs();
            if (attrs.ContainsKey("RouteAttribute"))
            {
                navList.Add(new NavLink() { Icon = "home", Label = page.GetTypeName(), Href = attrs["RouteAttribute"] });
            }
            if (attrs.ContainsKey("Route"))
            {
                navList.Add(new NavLink() { Icon = "home", Label = page.GetTypeName(), Href = attrs["Route"] });
            }

        };
        return navList.ToArray();
    }

    public NavLink[] GetDefaultNavLinks()
    {
        return new NavLink[] {
            new NavLink(){ Icon="home", Label="Домашняя", Href="/" },
            new NavLink(){ Icon="plus", Label="Версии", Href="/counter" },
            new NavLink(){ Icon="list-rich", Label="Редактор форм", Href="/input-forms" },
            new NavLink(){ Icon="person", Label="Редактор меню", Href="/nav-menu" }

        };
    }


    public async Task OnNavTreeInit(object evt)
    {
        _logger.LogInformation("OnNavTreeInit(evt)");
        //NavTree tree = (NavTree)evt;
        //tree.Reset(GetPageLinks());
    }

    public NavLink[] GetPageLinks()
    {
        return typeof(RootForms.Program).Assembly
            .GetTypes()
            .Where(t => typeof(ComponentBase).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetCustomAttributes<RouteAttribute>(inherit: true)
                .Select(route => new NavLink
                {
                    Label = t.Name,
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
    public NavLink[] GetPageLinks(string uri);
    public NavLink[] GetPageLinks();
    public NavLink[] GetNavLinks();
    public void Update(NavLink link);
    public void Remove(NavLink link);
    public void CreateNavLink();

}
public class NavMenuDbContext : DbContext
{
    public DbSet<NavLink> NavLinks { get; set; }
    public NavMenuDbContext(DbContextOptions<NavMenuDbContext> options) : base(options) { }
}
public static class NavMenuServiceExtension
{
    public static IServiceCollection AddNavMenu(this IServiceCollection services)
    {
        services.AddOpenIcons();
        services.AddScoped<INavMenuService, NavMenuService>();
        services.AddDbContext<NavMenuDbContext>(options => options.UseInMemoryDatabase(nameof(NavMenuDbContext)));
        return services;
    }
}
