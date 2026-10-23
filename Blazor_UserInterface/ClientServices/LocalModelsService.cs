using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LocalModelsService: IUserModelsService
{
    private readonly ILocalStorage _storage;

    public LocalModelsService()
    {
        //_storage = storage;
    }
    public async Task Save(object target)
    {
        if(target is ViewItem)
        {
            ViewItem item = (ViewItem)target;
            if (item.hashcode == 0)
            {
                item.hashcode++;
            }
            await _storage.SetItem("view-" + item.hashcode, item.ToJson());
        }
    }
}

public static class LocalModelsServiceExtensions
{
    public static IServiceCollection AddUserModelsService( this IServiceCollection services )
    {
        Console.WriteLine("AddLocalStorageService();");
        
        services.AddScoped<LocalModelsService>();
        return services;
    }
}