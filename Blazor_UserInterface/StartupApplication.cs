using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal class StartupApplication
{
    internal static HashSet<Action<IServiceCollection>> ConfigurationServices = new HashSet<Action<IServiceCollection>>();
    internal static HashSet<Action<IServiceCollection,IConfiguration>> ConfigurationServicesWithOptions = new HashSet<Action<IServiceCollection, IConfiguration>>();
    internal static HashSet<Action<HostBuilderContext,IServiceCollection>> ConfigurationBackgroundServices = new HashSet<Action<HostBuilderContext, IServiceCollection>>();

    internal static HashSet<Type> PageServices = new HashSet<Type>();
    internal static HashSet<Type> PageComponents = new HashSet<Type>();
    internal static HashSet<Type> SharedServices = new HashSet<Type>();
    internal static HashSet<Type> SharedComponents = new HashSet<Type>();


    internal static int AddBackground(Action<HostBuilderContext,IServiceCollection> todo) 
    {
        ConfigurationBackgroundServices.Add((context,services) => {
            Console.WriteLine($"Регистрация сервисов: ");
            todo(context,services);
        });
        return ConfigurationServices.Count();
    }

    internal static int AddWithOptions(Action<IServiceCollection, IConfiguration> todo)
    {
        ConfigurationServicesWithOptions.Add((services,configuration) => {
            Console.WriteLine($"Регистрация сервисов: ");
            todo(services, configuration);
        });
        return ConfigurationServices.Count();
    }

    internal static int Add(Action<IServiceCollection> todo)
    {
        ConfigurationServices.Add((services) => {
            Console.WriteLine($"Регистрация сервисов: ");
            todo(services);
        });
        return ConfigurationServices.Count();
    }

    internal static int AddSingletonConfiguration<TService>() where TService : class
    {
        ConfigurationServices.Add((services) => {
            Console.WriteLine($"Регистрация сервиса уровня приложения {typeof(TService).Name}");
            services.AddScoped<TService>();
        });
        return ConfigurationServices.Count();
    }

    internal static int AddTransientConfiguration<TService>() where TService : class
    {
        ConfigurationServices.Add((services) => {
            Console.WriteLine($"Регистрация изолированного сервиса {typeof(TService).Name}");
            services.AddTransient<TService>();
        });
        return ConfigurationServices.Count();
    }

    internal static int AddScopedConfiguration<TService>() where TService : class
    {
        ConfigurationServices.Add((services)=> {
            Console.WriteLine($"Регистрация сервиса уровня сеанса {typeof(TService).Name}");
            services.AddScoped<TService>();
        });
        return ConfigurationServices.Count();
    }
    internal static int AddScopedConfiguration<TService, TImpl>() where TService : class  where TImpl : class, TService
    {
        ConfigurationServices.Add((services) => {
            Console.WriteLine($"Регистрация сервиса уровня сеанса {typeof(TService).Name}");
            services.AddScoped<TService, TImpl>();
        });
        return ConfigurationServices.Count();
    }

    /// <summary>
    /// Регистрация сервисов сборки, функции регистрации определяются в
    /// статических вызовах класса сервиса
    /// </summary>
    internal static IEnumerable<ServiceDescriptor> ApplyConfigurationServices(IServiceCollection services, IConfiguration configuration)
    {
        Action<Action<IServiceCollection,IConfiguration>> ApplyWithOptions = (configure) =>
        {
            try
            {
                configure(services, configuration);
            }
            catch(Exception ex)
            {
                Writing.ToConsole(ex);
            }
        };
        Action<Action<IServiceCollection>> Apply = (configure) => configure(services);
        ConfigurationServicesWithOptions.ToList().ForEach(ApplyWithOptions);
        ConfigurationServices.ToList().ForEach(Apply);
        return services.ToArray();
    }

    internal static void OnStart()
    {
        string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        Action<Type> Registration = (type) => {
            var field = type.GetField("Counter");
            if (field != null)
            {
                Console.WriteLine($"{type.Name}: {field.GetValue(null)}");
            }
            if (string.IsNullOrEmpty(type.Namespace) == false)
            {
                if (type.Namespace.StartsWith($"{assemblyName}.Pages"))
                {
                    PageComponents.Add(type);
                }              
                if (type.Namespace.StartsWith($"{assemblyName}.Shared"))
                {
                    SharedComponents.Add(type);
                }
                if (type.Namespace.StartsWith($"{assemblyName}.Services.Pages"))
                {
                    PageServices.Add(type);
                }
                if (type.Namespace.StartsWith($"{assemblyName}.Services.Shared"))
                {
                    SharedServices.Add(type);
                }
            }


        };
        Assembly.GetExecutingAssembly().GetTypes().ToList().ForEach(Registration);
    }
}

