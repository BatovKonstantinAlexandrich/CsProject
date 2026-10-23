 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreConstructorAngular.Data;
using NetCoreConstructorAngular.Data.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 

public static class DataServicesExtensions
{

    /*public static IServiceCollection AddSqlDataContext<T>(this IServiceCollection services, string sqlserver) where T: DbContext
    {
        var connectionModel = new SqlServerADOConnectionStringModel()
        {
            Server = sqlserver,
            TrustedConnection = true,
            DataBase = typeof(T).Name
        };
        var ado = new SqlServerADOService();
        if (ado.CanConnect(connectionModel.ToString()) == false)
        {
            throw new Exception($"Сервер {sqlserver} не доступен") ;
        }        
        services.AddDbContext<T>(options =>
        {            
            options.UseSqlServer(@"Server=KEST\SQLSERVER;Database=" + nameof(ApplicationDbContext) +
                @";Trusted_Connection=True;MultipleActiveResultSets=true;");
        });
        return services;
    }*/
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {



       /* services.AddScoped<SqlServerADOService>();
        services.AddScoped<MysqlADOConnectionStringModel>();
        services.AddScoped<SqlServerADOService>();
        services.AddScoped<DatabaseManager>();
        services.AddScoped<DatabaseManager>();*/
        services.AddScoped<DataValidationService>();
        //services.AddScoped<AnaliticsService>();
        services.AddScoped<UserRolesService>();
        services.AddScoped<ITStatistics>();
        services.AddScoped<StatisticsService>();
        services.AddScoped<UserGroupsService>();
        services.AddScoped<ListService>();
        //services.AddDataConverters();
        services.AddScoped<InvokationService>();
        
    
        return services;
    }
}