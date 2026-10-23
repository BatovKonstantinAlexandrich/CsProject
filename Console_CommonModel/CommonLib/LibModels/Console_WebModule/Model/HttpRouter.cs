using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;

using Microsoft.AspNetCore.Http;

namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule.Model
{
    public class HttpRouter : IHttpRouter<IWebApi>
    {
        /// <summary>
        /// Устанавливает режим усиленной навигации, в случае 404 статуса 
        /// предлагаются варианты достоверных путей URL и прочей инф-ии.
        /// </summary>
        public bool SupportUsing { get; set; }


        /// <summary>
        /// Поддержка запросов PUTCH для изменения API.
        /// </summary>
        public bool PatchEnable { get; set; }


        public IWebApi GetMiddlewareComponent()
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<int, object>> OnRequest(HttpContext http)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<int, object>> OnRequest(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<int, object>> OnError(Exception ex, RequestModel requestModel)
        {
            throw new NotImplementedException();
        }
    }
}
