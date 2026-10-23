using ApplicationDb.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class HttpDatabaseClient : IDatabaseController
{
    public static int Counter = 
        StartupApplication.AddScopedConfiguration<IDatabaseController, HttpDatabaseClient>();


    private readonly HttpClient _http;
    private readonly ILogger<HttpDatabaseClient> _logger;
    private readonly ILocalStorage _local;
    

    public HttpDatabaseClient(
            ILogger<HttpDatabaseClient> logger,
            ILocalStorage local,
            HttpClient http) {
        _http = http;
        _logger = logger;
        _local = local;
        _logger.LogInformation("Created");
    }

    public async Task<string> Take(string url)
    {
        _http.CancelPendingRequests();
        _logger.LogInformation(url);
        var response = await _http.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string result = await response.Content.ReadAsStringAsync();
            await _local.SetItem(url, result);
            _logger.LogInformation("Результат сохранен в localStorage");
            return result;
        }
        else
        {
            _logger.LogInformation("Результат получен из localStorage");
            return await _local.GetItem<string>(url);
        }
    }


    public async Task<int> Create<T>(T item) where T : BaseEntity
    {        
        var response = await _http.GetAsync("/Database/" + typeof(T).Name + "/Create?args="+ item.ToJson());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();       
    }




    public async Task<T[]> List<T>() where T:BaseEntity
    {
        var response = await _http.GetAsync("/Database/"+ typeof(T).Name +"/List");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<T[]>();
        foreach (var dataitem in data)
        {
            Console.WriteLine(dataitem.ToJson());
        }
        return data;
    }

    public async Task<T[]> Page<T>(int page, int size) where T : BaseEntity
    {
        var response = await _http.GetAsync("/Database/" + typeof(T).Name + "/Page?args={page:"+page+",size:"+size+"}");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<T[]>();
        foreach (var dataitem in data)
        {
            Console.WriteLine(dataitem.ToJson());
        }

        return data;
    }

}