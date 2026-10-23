using Microsoft.AspNetCore.Http;

namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule;

[Label("WebModule: OdbcMiddleware реализует IMiddleware")]
[Description("OdbcMiddleware зарегистрирован как IMiddleware для конвейера ASP.NET Core.")]
public class WebModuleOdbcMiddlewareContractTest : TestingElement
{
    public WebModuleOdbcMiddlewareContractTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("Проверка OdbcMiddleware : IMiddleware");
        Assert(_ =>
        {
            var ok = typeof(IMiddleware).IsAssignableFrom(typeof(OdbcMiddleware));
            LogMessage($"IsAssignableFrom(IMiddleware) = {ok}");
            return ok;
        },
            "OdbcMiddleware реализует контракт IMiddleware",
            "OdbcMiddleware не реализует IMiddleware");
    }
}
