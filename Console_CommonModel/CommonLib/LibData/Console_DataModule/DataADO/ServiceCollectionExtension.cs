using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;

public static class ServiceCollectionExtension
{

    public static IServiceCollection AddDbModelContext<TDBContext>(this IServiceCollection services, string Server)
        where TDBContext : DbContextBase
    {
        //настройки соединения
        var connectionString = new SqlServerConnectionString()
        {
            Server = Server,
            Database = typeof(TDBContext).Name,
            TrustedConnection = true
        };

        return services;

    }

    /// <summary>
    /// Добавление SQLServer
    /// </summary>
    /// <typeparam name="TDBContext"></typeparam>
    /// <param name="services">набор сервисов</param>
    /// <param name="Server">имя экземпляра SQL SERVER</param>
    /// <param name="TrustedConnection">защищенное соединение</param>    
    public static IServiceCollection AddSqlServer<TDBContext>(this IServiceCollection services,string Server)
        where TDBContext : DbContextBase        
    {
        services.Info($"[Info][{typeof(TDBContext).Name}]: AddSqlServer({Server},{typeof(TDBContext).Name})");
        services.AddDbContext<TDBContext>(            
            options => {

                //настройки соединения
                var connectionString = new SqlServerConnectionString()
                {
                    Server = Server,
                    Database = typeof(TDBContext).Name,
                    TrustedConnection = true 
                };

                //проверка соединения
                var validator = new SqlServerADOService();
                if (validator.CanConnect(connectionString.ToString()))
                {
                    options.UseSqlServer(connectionString.ToString());
                }
                else
                {
                    //выполняется попытка создать базу данных
                    connectionString.Database = "Model";
                    if (validator.CanConnect(connectionString.ToString()))
                    {
                        validator.PrepareQuery(
                            connectionString.ToString(),
                            $"Create database {typeof(TDBContext).Name}");
                        connectionString.Database = typeof(TDBContext).Name;
                        options.UseSqlServer(connectionString.ToString()); ;
                    }
                    else
                    {
                        throw new System.Exception(
                            "Не удалось подключиться к источнику медицинских данных. " +
                            "Проверьте строку подключения: " + connectionString.ToString());
                    }                
                }
        });        
        return services;
    }

    

 
}