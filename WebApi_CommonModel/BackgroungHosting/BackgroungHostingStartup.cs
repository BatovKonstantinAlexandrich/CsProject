using WebApi_CommonModel.BackgroungHosting;

public class BackgroungHostingStartup
{
    public static BackgroundServiceContext ConfigureServices(IServiceCollection services)
    {
        var result = new BackgroundServiceContext();
        services.AddSingleton<BackgroundServiceContext>(result);
        services.AddHostedService<BackgroundServicePool>();

        return result;
    }
}