using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;

using Microsoft.AspNetCore.Http;

namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule.Model
{


    public interface IHttpRouter<TMiddlewareComponent> where TMiddlewareComponent : IWebApi
    {
        TMiddlewareComponent GetMiddlewareComponent();

        Task<Tuple<int, object>> OnRequest(HttpContext http);
        Task<Tuple<int, object>> OnRequest(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null);
        Task<Tuple<int, object>> OnError(Exception ex, RequestModel requestModel);


    }
}