using Console_RoyaltyData.RoyaltyData;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Console_RoyaltyData.RoyaltyServices
{
    public class RoyaltyStartup
    {
        public void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(typeof(RoyaltyDbContext).Name);
            if(String.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Конфигурация приложени не содержит строку соединения {typeof(RoyaltyDbContext).Name}");
            services.AddDbContext<RoyaltyDbContext>(options => {
                options.UseSqlServer(connectionString);
            });
        }
    }
}
