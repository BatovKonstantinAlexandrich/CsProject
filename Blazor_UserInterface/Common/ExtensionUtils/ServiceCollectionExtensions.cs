using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class ServiceCollectionExtensions
{
    public static void AddDescriptors(this IServiceCollection services, IConfiguration configuration)
    {
        IEnumerable<ServiceDescriptor> descriptors = 
            StartupApplication.ApplyConfigurationServices(services, configuration);
        services.AddScoped(typeof(IEnumerable<ServiceDescriptor>), (provide) => {
            return descriptors;
        });
    }
}

