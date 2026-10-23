using Blazor.TreeEditor.Models;
using Blazor.TreeEditor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

 
public class PagesServices
{
    public static int Counter = StartupApplication.AddSingletonConfiguration<PagesServices>();


    private IEnumerable<RouterConfig> Routes;
    private NavigationManager _navigationManager;
    private ApplicationSignleton _applicationSignleton;

    public PagesServices( 
            ApplicationSignleton applicationSignleton,
            NavigationManager navigationManager) 
    {
        _applicationSignleton = applicationSignleton;
        this.Routes = StartupApplication.PageComponents.Select(pageType => new RouterConfig()
        {
            Label = pageType.GetTypeName(),
            Icon = Attrs.IconFor(pageType),
            Href = Attrs.GetTypeAttrValue(pageType, "RouteAttribute"),
                
        }).Where(route=>string.IsNullOrEmpty(route.Href)==false).ToArray();                
        _navigationManager = navigationManager;
    }

    public string ParseUri(string location)
    {
        location = location.EndsWith("/") == false ? location + "/" : location;
        int i = location.IndexOf(":");
        i += location.Substring(i+1).IndexOf(":")+1;
        i += location.Substring(i+1).IndexOf("/")+1;
        return location.Substring(i);
    }


    public void OnLocationChanged(object sender, LocationChangedEventArgs changes) { 
    }


    public IEnumerable<ActionLink> InitializeNavigationLinks()
    {
        return new List<ActionLink>() {
            new ActionLink(){
                Label = "Авторизация",
                Icon = "home",
                Href = "/signin",
                Tooltip = "Переход на страницу авторизации",
                OnMessage = ( evt )=>{
                    _navigationManager.NavigateTo("/signin");
                }
            },
            new ActionLink(){
                Label = "Регистрация",
                Icon = "person",
                Href = "/signup",
                Tooltip = "Переход на страницу регистрации",
                OnMessage = ( evt )=>{
                    _navigationManager.NavigateTo("/signup");
                }
            },
        }.ToArray();
    }

    private IEnumerable<Type> GetPageComponents()
    {
        return StartupApplication.PageComponents;
    }

    private ActionLink ToActionLink( global::NavLink navLink)
    {
        return new ActionLink()
        {
            Href = navLink.Href,
            Label = navLink.Label,
            Icon = navLink.Icon,
            Tooltip = navLink.Tooltip,
            OnMessage = async (evt) => {
                _navigationManager.NavigateTo(navLink.Href);
            }
        };
    }



    /// <summary>
    /// Убирает завершающие слэши, чтобы глубина пути совпадала с объявленными Route (иначе после перехода
    /// ParseUri даёт "/page/", а Href — "/page", CountOfChar('/') расходится и меню становится пустым).
    /// </summary>
    private static string NormalizeNavPath(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return "/";
        location = location.Trim();
        while (location.Length > 1 && location.EndsWith("/"))
            location = location.TrimEnd('/');
        return string.IsNullOrEmpty(location) ? "/" : location;
    }

    private static string GetParentNavPath(string location)
    {
        location = NormalizeNavPath(location);
        if (location == "/")
            return "/";
        var i = location.LastIndexOf('/');
        if (i <= 0)
            return "/";
        return NormalizeNavPath(location.Substring(0, i));
    }

    public IEnumerable<ActionLink> GetNavigationLinks( string location )
    {
        Console.WriteLine(location);
        if (string.IsNullOrEmpty(location))
            location = "/";
        if (location.StartsWith("/") == false)
        {
            throw new Exception("Проверьте данные переданные фактически в аргумент location: "+ location);
        }

        var path = NormalizeNavPath(location);
        var links = this.Routes
            .Where(route => route.Href != null)
            .Select(route => new { route, href = NormalizeNavPath(route.Href) })
            .Where(x => x.href.StartsWith(path, StringComparison.Ordinal) && x.href.CountOfChar('/') == path.CountOfChar('/'))
            .Select(x => x.route.toActionLink((context)=> {
                return _navigationManager;
            }))
            .ToList();

        if (path != "/")
        {
            var parent = GetParentNavPath(path);
            links.Insert(0, new ActionLink()
            {
                Label = "НАЗАД",
                Icon = "arrow-left",
                Href = parent,
                Tooltip = "Вернуться на предыдущий уровень",
                OnMessage = (evt) => { _navigationManager.NavigateTo(parent); }
            });
        }

        return links;
    }

     
}
