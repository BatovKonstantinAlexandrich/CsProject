using NSwag;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule
{
    public interface IServiceBuilder<TServiceModel> where TServiceModel : ServiceModel
    {
        void AddOpenApi(string url);
        void AddWsdl(string url);
        Func<Dictionary<string, object>, MethodResult<object>> CreateCaller(Type serviceType, string action);
        OpenApiDocument CreateOpenApiForUrl(string url);
        void CreateServiceModel(TServiceModel model);
        Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>> CreateValidator(Type serviceType, string action);
        MethodResult<object> ExecuteEndpoint(string key, Dictionary<string, object> args);
        List<string> GetAllRouteOptions();
        List<string> GetRouteOptions(string path = "/");
        void Init();
    }
}