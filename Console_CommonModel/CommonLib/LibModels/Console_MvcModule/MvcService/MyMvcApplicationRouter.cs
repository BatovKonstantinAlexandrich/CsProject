using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;
using Console_CommonModel.CommonLib.LibModels.Console_WebModule.Model;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService
{
    public class MyMvcApplicationRouter : IHttpRouter<BusinessDatasourceService>
    {
        private IWebApi webApi;



        [NotNullNotEmpty]
        [InputDictionary($"{nameof(BusinessDatasource)},{nameof(BusinessDatasource.Name)}")]
        public int BusinessDatasourceId { get; set; }
        public BusinessDatasource BusinessDatasource { get; set; }
        private BusinessDatasourceService BusinessService;




        public BusinessDatasourceService GetMiddlewareComponent()
        {
            if (BusinessDatasource is null)
            {
                throw new ArgumentNullException(nameof(BusinessDatasource));
            }
            if (BusinessService is null)
            {
                BusinessService = new BusinessDatasourceService();
                BusinessService.Setup(BusinessDatasource);
            }
            return BusinessService;
        }


        public async Task OnResponse(HttpContext http, RequestModel request, Tuple<int, object> result)
        {
            try
            {
                http.Response.StatusCode = result.Item1;
                var responseText = result.Item2.ToJsonOnScreen();
                await http.Response.WriteAsync(responseText);
            }
            catch (Exception ex)
            {
                OnError(ex, request);
                await http.Response.WriteAsync(ex.Message);
            }
        }

        public async Task OnRequest(HttpContext http)
        {
            var request = GetRequestModel(http);
            var result = await OnRequest(request);
            await OnResponse(http, request, result);
        }

        public string ReadBody(HttpContext context)
        {
            try
            {
                // Проверяем, что тело запроса можно читать
                if (context?.Request?.Body == null || !context.Request.Body.CanRead)
                    return string.Empty;

                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true
                );

                var body = reader.ReadToEnd();

                // Сбрасываем позицию потока для возможного повторного чтения
                context.Request.Body.Position = 0;

                return body;
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                this.Error($"Ошибка чтения тела запроса: {ex.Message}");
                return string.Empty;
            }
        }

        private RequestModel GetRequestModel(HttpContext http)
        {
            var requestModel = new RequestModel();
            requestModel.url = http.Request.Path.ToString();
            requestModel.headers = http.Request.Headers.ToDictionary(
                    h => h.Key,
                    h => string.Join(", ", h.Value)
            );
            requestModel.args = http.Request.Query.ToDictionary(
                    h => h.Key,
                    h => string.Join(", ", h.Value)
            );
            requestModel.body = ReadBody(http);
            return requestModel;
        }

        public async Task<Tuple<int, object>> OnRequest(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null)
        {
            var requestModel = new RequestModel(url, args, headers, body);
            return await OnRequest(requestModel);
        }


        private async Task<Tuple<int, object>> OnRequest(RequestModel requestModel)
        {
            Tuple<int, object> methodResult = null;
            try
            {
                return methodResult = await GetMiddlewareComponent().Request(requestModel.url, requestModel.args, requestModel.headers, requestModel.body);
            }
            catch (Exception ex)
            {
                this.Error($"Исключение при обработки запроса => {requestModel.ToJson()} => {ex.Message}", ex);
                try
                {
                    return methodResult = await OnError(ex, requestModel);
                }
                catch (Exception exx)
                {
                    this.Error($"Исключение при обработки ошибки => {ex.Message} => {exx.Message}", exx);
                    return methodResult = new Tuple<int, object>(500, null);
                }
            }
        }

        public async Task<Tuple<int, object>> OnError(Exception ex, RequestModel requestModel)
        {
            await Task.CompletedTask;
            return new Tuple<int, object>(500, new
            {
                message = ex.Message,
                requestModel
            });
        }

        Task<Tuple<int, object>> IHttpRouter<BusinessDatasourceService>.OnRequest(HttpContext http)
        {
            throw new NotImplementedException();
        }
    }
}
