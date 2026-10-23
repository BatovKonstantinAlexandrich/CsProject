using Blazor.Hospital.Client.Services.JSServices;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 
public class AngularJSService: IAngularJS
{
    public static int Counter = StartupApplication.Add((services)=> {
        Console.WriteLine($"\t {nameof(IAngularJS)}");
        services.AddAngularJS();
    });

    private readonly IJS _js;

    public AngularJSService( IJS js )
    {
        _js = js;
    }
    public void CompileNode(string nodeId)
    {
        _js.Eval<string>(@$"
            let pnode = document.getElementById('{nodeId}');
            $compile(pnode);
            return true;
        ");
    }
}
 

public static class AngularJSServiceExtension
{
    public static void AddAngularJS( this IServiceCollection services)
    {
        services.AddScoped<IJS, JSService>();
        services.AddScoped<IAngularJS, AngularJSService>();
    }
}
