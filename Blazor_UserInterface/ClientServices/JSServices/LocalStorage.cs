using Blazor.Hospital.Client.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LocalStorage : ILocalStorage 
{
    public static int Counter = StartupApplication.Add((services) => {
        Console.WriteLine($"\t {nameof(ILocalStorage)}");
        services.AddLocalStorageService();
    });

    private readonly IJSRuntime _js;
    
    public LocalStorage(IJSRuntime js)
    {
        _js = js;
    }
    public async Task SetItem(string Key, string Value)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", Key, Value);
    }
    public async Task<TResult> GetItem<TResult>(string Key)
    {
        return await _js.InvokeAsync<TResult>("localStorage.getItem", Key);
    }
    public async Task<List<string>> GetKeys()
    {
        return await _js.InvokeAsync<List<string>>("Object.keys", "localStorage");
    }

    
}


public static class LocalStorageExtension
{
    public static IServiceCollection AddLocalStorageService(this IServiceCollection services)
    {
        Console.WriteLine("AddLocalStorageService();");
        services.AddScoped<ILocalStorage, LocalStorage>();
        return services;
    }
}