using System.Collections.Concurrent;


namespace WebApi_CommonModel.MiddlewareComponents
{


    [Label("Выполняет фильтрацию запросов по IP-адресу")]
    [Description("Ведёт подсчёт запросов по IP адресам, после превышения лимита - запрос не обрабатывается")]
    public class IpFilterMiddleware : IMiddleware
    {

        [Label("Кол-во запросов")]
        public int MaxRequests = 100;

        [Label("Время в мс")]
        public int Timespan = 10000;

        [NotInput]
        [Description("IP-время запроса")]
        private ConcurrentDictionary<string, List<long>> rate = new();

        public IpFilterMiddleware()
        {
        }

        [Label("Установить кол-во запросов")]
        public void SetMaxRequests([InputPositiveInt][Label("Кол-во запросов")] int max) => MaxRequests = max;


        [Label("Установить время в мс")]
        public void SetTimespan([InputPositiveInt][Label("Время в мс")] int max) => Timespan = max;


        [NotInput]
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
