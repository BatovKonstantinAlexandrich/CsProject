using Blazor.Hospital.Client.Services.JSServices;
 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Hospital.Client.Services.JSServices
{
    public class JSService: IJS
    {
        private IJSRuntime _jsr;
        private ILogger<JSService> _logger;

        public JSService(
                ILogger<JSService> logger,
                IJSRuntime JSR ) {
            this._jsr = JSR;
            this._logger = logger;
        }

        protected void Info(string message)
        {
            _logger.LogInformation(message);
        }


        public async Task<T> Eval<T>(string js)
        { 
            return await _jsr.InvokeAsync<T>("eval", @"(function () {try{" + js + @"} catch (e) { alert('Ошибка при разборе JSON: ' + e); } })()");
        }

        public async Task<T> EvalAsync<T>(string js)
        {
            return await _jsr.InvokeAsync<T>("eval", @"(function () { try {" + js + @" } catch (e) { alert('Ошибка при разборе JSON: ' + e); }  })()");
        }
    }
}
public static class JSServiceExtension
{
    public static void AddJS(this IServiceCollection services)
    {
        
 
        //
    }
}
