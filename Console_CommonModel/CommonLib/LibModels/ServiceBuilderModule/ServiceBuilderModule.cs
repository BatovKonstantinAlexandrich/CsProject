using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule
{


    /// <summary>
    /// Модуль для динамического подключения служб
    /// </summary>
    public class ServiceBuilderModule
    {

        /// <summary>
        /// Создаёт функцию обработки запросов
        /// </summary>      
        public static Func<HttpContext, RequestDelegate, Task> CreateHttpHandler(DbContextService serviceBuilder)
        {
            return (http, next) =>
            {
                var path = http.Request.Path.ToString();
                var routes = serviceBuilder.GetRouteOptions(path);
                if (routes.Count() == 1)
                {
                    http.Response.ContentType = "text/plain; charset=utf-8";
                    var target = routes.FirstOrDefault();
                    var args = new Dictionary<string, object>();
                    var methodResult = serviceBuilder.ExecuteEndpoint(target, args);
                    if (methodResult.Succeeded)
                    {
                        http.Response.WriteAsync(methodResult.Result.ToJsonOnScreen()).Wait();
                    }
                    else
                    {
                        http.Response.WriteAsync(methodResult.ToJsonOnScreen()).Wait();
                    }
                    return Task.CompletedTask;
                }
                else if (routes.Count() > 1)
                {
                    http.Response.ContentType = "text/plain; charset=utf-8";
                    http.Response.WriteAsync(routes.ToJsonOnScreen()).Wait();
                    return Task.CompletedTask;
                }
                else
                {
                    return next.Invoke(http);
                }


            };
        }
    }
}
