using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MedOrg.Client.Services
{
    public class ClientHttp: IClientHttp
    {

        private readonly HttpClient _http;
        private readonly ILogger<ClientHttp> _logger;
        private readonly ILocalStorage _local;
        


        public ClientHttp(
                ILogger<ClientHttp> logger,
                ILocalStorage local,
                HttpClient http)
        {
            _http = http;
            _logger = logger;
            _local = local;
            _logger.LogInformation("Created");
        }



        public async Task<TResponse> DoPost<TResponse>(string url, object request = null)
        {
            _http.CancelPendingRequests();
            _logger.LogInformation(url);
            var response = await _http.GetAsync(WebUtility.UrlEncode(url));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string textResult = await response.Content.ReadAsStringAsync();
                TResponse result = textResult.FromJson<TResponse>();
                await _local.SetItem(url, textResult);
                _logger.LogInformation("Результат сохранен в localStorage");
                return result;
            }
            else
            {
                _logger.LogInformation("Результат получен из localStorage");
                string textResult = await _local.GetItem<string>(url);
                return textResult.FromJson<TResponse>();
            }
        }
         
        public async Task<TResponse> DoGet<TResponse>(string url)
        {
            _http.CancelPendingRequests();
            
            _logger.LogInformation(url);
            var response = await _http.GetAsync(WebUtility.UrlEncode(url));

            _logger.LogInformation(response.StatusCode.ToString());
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string textResult = await response.Content.ReadAsStringAsync();
                TResponse result = textResult.FromJson<TResponse>();
                await _local.SetItem(url, textResult);
                _logger.LogInformation("Результат сохранен в localStorage");
                return result;
            }
            else
            {
                _logger.LogInformation("Результат получен из localStorage");
                string textResult = await _local.GetItem<string>(url);
                return textResult.FromJson<TResponse>();
            }
        }
    }
}
