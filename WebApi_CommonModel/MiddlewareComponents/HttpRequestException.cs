namespace WebApi_CommonModel.MiddlewareComponents
{
    public class HttpRequestException : Exception
    {
        public readonly int _status;

        public HttpRequestException(int status, string message) : base(message)
        {
            this._status = status;
        }
    }
}
