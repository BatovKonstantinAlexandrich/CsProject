namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule.Model
{
    public class RequestModel
    {
        public string url { get; set; } = "/";
        public Dictionary<string, string> args { get; set; } = new();
        public Dictionary<string, string> headers { get; set; } = new();
        public string body { get; set; } = "";

        public RequestModel()
        {

        }

        public RequestModel(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null)
        {
            this.url = url;
            this.args = args;
            this.headers = headers;
            this.body = body;
        }

    }
}