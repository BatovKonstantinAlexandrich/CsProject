using System.Collections.Concurrent;

namespace WebApi_CommonModel.MiddlewareComponents
{

    [Label("Выполняет фильтрацию запросов по IP-адресу")]
    public class IpFilterMiddleware : IMiddleware
    {
        [Label("Кол-во запросов")]
        public int MaxRequests = 100;

        [Label("Время в мс")]
        public int Timespan = 10000;

        [Description("IP-время запроса")]
        private ConcurrentDictionary<string, List<long>> rate = new();

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var ip = context.Connection.RemoteIpAddress.ToString();
            if (!rate.ContainsKey(ip))
            {
                rate[ip] = new() { DateTimeOffset.Now.ToUnixTimeMilliseconds() };
            }
            else
            {
                rate[ip].Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            }
            foreach (var p in rate[ip].Where(p => DateTimeOffset.Now.ToUnixTimeMilliseconds() - p > Timespan).ToList())
            {
                rate[ip].Remove(p);
            }
            if (rate[ip].Count() < MaxRequests)
                return next.Invoke(context);
            else return Task.CompletedTask;
        }
    }
}
