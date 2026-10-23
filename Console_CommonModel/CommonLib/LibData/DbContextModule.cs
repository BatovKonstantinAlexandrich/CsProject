using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel;

namespace Console_CommonModel.CommonLib.LibData
{
    /// <summary>
    /// Предоставляет сервисы для работы с базой данных
    /// </summary>    
    public class DbContextModule<TDbContext>
        where TDbContext : DbContextBase
    {

        /// <summary>
        /// Решистрация контекста данных
        /// </summary>
        public void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TDbContext>(options =>
            {
                var databaseName = typeof(TDbContext).GetType().GetTypeName().Replace("<", "_").Replace(">", "");
                options.UseSqlServer(
                    $"Data Source=DESKTOP-IHJM9RD;" +
                    $"Initial Catalog={databaseName};" +
                    $"Integrated Security=True;" +
                    $"Connect Timeout=30;" +
                    $"Encrypt=False;" +
                    $"Application Intent=ReadWrite;" +
                    $"Multi Subnet Failover=False");
            });


            ///
            /*services.AddDbContext<MessageDbContext<TDbContext>>(options =>
            {
                var databaseName = typeof(MessageDbContext<TDbContext>).GetType().GetTypeName().Replace("<", "_").Replace(">", "");
                options.UseSqlServer(
                    $"Data Source=DESKTOP-IHJM9RD;" +
                    $"Initial Catalog={databaseName};" +
                    $"Integrated Security=True;" +
                    $"Connect Timeout=30;" +
                    $"Encrypt=False;" +
                    $"Application Intent=ReadWrite;" +
                    $"Multi Subnet Failover=False");
            });*/


        }
    }
}
