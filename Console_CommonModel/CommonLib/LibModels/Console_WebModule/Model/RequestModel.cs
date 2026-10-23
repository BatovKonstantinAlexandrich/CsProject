namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule.Model
{
    public class RequestModel
    {
        public string url;
        public Dictionary<string, string> args = null;
        public Dictionary<string, string> headers = null;
        public string body = null;

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