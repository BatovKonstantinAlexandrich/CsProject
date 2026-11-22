using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Web.Http;

public class NavMenuService: INavMenuService
{
    private readonly ILogger<NavMenuService> _logger
        = LoggerFactory.Create(options => options.AddConsole()).CreateLogger<NavMenuService>();
    public Assembly _assembly;

    public NavMenuService(   )
    {
       // this._assembly = assembly;         
      
    }

    public NavMenuService(Assembly assembly)
    {
        this._assembly = assembly;         

    }


    public List<ILink> GetPageLinks(string uri)
    {
        if(uri.StartsWith("http"))
        {
            uri = uri.Substring(uri.IndexOf("://")+3);
            uri = uri.Substring(uri.IndexOf("/")+1);
        }
        if(!uri.EndsWith("/"))
        {
            uri += "/";
        }
        _logger.LogInformation(uri);
        var navList = new List<ILink>();
        string ns = "Blazor_MyApp.Components.Pages";
        var ids =
            uri.ReplaceAll("/", ".").ReplaceAll("\\", ".").Split(".").ToList().Where(s => string.IsNullOrEmpty(s) == false).Select(s => s.ToCapitalStyle()).ToList();
        ids.ForEach(id => ns += "." + id);
        _logger.LogInformation($"Namespace: {ns}");

        var types = _assembly.GetClassTypes().Where(t => t.Namespace is not null && t.Namespace.Contains("Pages"));
        this.Debug(types.Count());
        var pagesTypes = types;// Assembly.GetExecutingAssembly().GetPages(ns);
        if (pagesTypes.Count() == 0)
            throw new Exception("Не найдены компоненты страниц");
        if (!(uri.CountOfChar("/").Count() > 4) && uri.Length > 3)
        {
            string p = uri;
            if (uri.EndsWith("/"))
                p = uri.Substring(0, p.Length - 1);
            if(p.LastIndexOf("/")!=-1)
                p = p.Substring(0, p.LastIndexOf("/"));
            this.Debug(p);
            

            navList.Add(new LinkViewModel() { Icon = "home", Label = "Назад", Href = p });
        }
        foreach (var page in pagesTypes)
        {
            var attrs = page.GetAttributes();
            string href = null;
            if (attrs.ContainsKey("RouteAttribute"))
            {
                href = attrs["RouteAttribute"];                
            }
            if (attrs.ContainsKey("Route"))
            {
                href = attrs["Route"];                
            }

            this.Debug(href);
            
            if (String.IsNullOrWhiteSpace(href))
                continue;
            if (!href.EndsWith("/"))
                href = href + "/";
            var v1 = href.CountOfChar('/');
            var v2 = uri.CountOfChar('/');
            this.Debug($"{v1}={v2}");
            if ((v2+1)==v1)
            {
                navList.Add(new LinkViewModel() { Icon = "home", Label = page.GetTypeName(), Href = href });
            }            
        };
        return navList.ToList();
    }

    public List<MenuView> GetPageMenu(string uri)
    {
        _logger.LogInformation(uri);
        var navList = new List<MenuView>();
        string ns = "Blazor_MyApp.Components.Pages";
        var ids =
            uri.ReplaceAll("/", ".").ReplaceAll("\\", ".").Split(".").ToList().Where(s => string.IsNullOrEmpty(s) == false).Select(s => s.ToCapitalStyle()).ToList();
        ids.ForEach(id => ns += "." + id);
        _logger.LogInformation($"Namespace: {ns}");

        var types = _assembly.GetClassTypes().Where(t => t.Namespace is not null && t.Namespace.Contains("Pages"));
        this.Debug(types.Count());
        var pagesTypes = types;// Assembly.GetExecutingAssembly().GetPages(ns);
        if (pagesTypes.Count() == 0)
            throw new Exception("Не найдены компоненты страниц");
        foreach (var page in pagesTypes)
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

      
    public List<ILink> GetPageLinks()
    {
        return typeof(CommonProgram).Assembly
            .GetTypes()
            .Where(t => "BaseComponent".ToType().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetCustomAttributes<RouteAttribute>(inherit: true)
                .Select(route => (ILink)new LinkViewModel()
                {
                    Label = t.Name,
                    Href = NormalizeRoute(route.Template),
                    Icon = "home"
                }))
            .Where(link => !string.IsNullOrWhiteSpace(link.Href))
            .GroupBy(link => link.Href, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(link => link.Href, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string NormalizeRoute(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            return "/";
        var route = template.Trim();
        return route.StartsWith("/") ? route : "/" + route;
    }

    public Task OnNavTreeInit(object evt)
    {
        throw new NotImplementedException();
    }

    

    public List<MenuView> GetMenuViews()
    {
        throw new NotImplementedException();
    }

    public void Update(MenuView link)
    {
        throw new NotImplementedException();
    }

    public void Remove(MenuView link)
    {
        throw new NotImplementedException();
    }

    public void CreateMenuView()
    {
        throw new NotImplementedException();
    }

 

    List<ILink> INavMenuService.GetMenuViews()
    {
        throw new NotImplementedException();
    }

    public void Update(ILink link)
    {
        throw new NotImplementedException();
    }

    public void Remove(ILink link)
    {
        throw new NotImplementedException();
    }
}

public interface INavMenuService
{

    public Task OnNavTreeInit(object evt);
    public List<ILink> GetPageLinks(string uri);
    public List<ILink> GetPageLinks();
    public List<ILink> GetMenuViews();
    public void Update(ILink link);
    public void Remove(ILink link);
    public void CreateMenuView();
}

 

public static class NavMenuServiceExtension
{
    public static IServiceCollection AddNavMenu(this IServiceCollection services, Assembly assembly)
    {
        //services.AddOpenIcons();
        services.AddScoped<INavMenuService, NavMenuService>(sp => {
            var ns = new NavMenuService();
            ns._assembly = assembly;
            return ns;
        });
        return services;
    }
}



/// <summary>
/// 
/// </summary>
public class NavMenuServiceTest: TestingService<NavMenuService>
{
    public NavMenuServiceTest(IServiceProvider sp) : base(sp)
    {
    }

    public override TestingReport DoTest(bool interactive = false, bool force = false)
    {
        return base.DoTest(interactive, force);
    }

    public override bool OnServiceTest(NavMenuService service)
    {
        service._assembly = "BlazorHostingProgram".ToType().Assembly;
        this.Debug(service.GetPageLinks("/").Select(p => p.Label));
        this.Debug(service.GetPageLinks("/actions/").Select(p => p.Label));
        
        return true;
    }
}


