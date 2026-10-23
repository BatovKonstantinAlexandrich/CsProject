namespace WebApi_CommonModel.WebApiModule
{
    public class WebApiComponent
    {


        private Func<HttpContext, RequestDelegate, Task> OnHttpRequest { get; set; } = async (http, next) => {
            await next.Invoke(http);
        };


        private void AddOnHttpRequest(Func<HttpContent, RequestDelegate, Task> func) { }
        private void RemoveOnHttpRequest(Func<HttpContent, RequestDelegate, Task> func) { }

        public Func<HttpContext, RequestDelegate, Task> OnHttpRequestHandler()
        {
            return OnHttpRequest;
        }
         
    }
}
