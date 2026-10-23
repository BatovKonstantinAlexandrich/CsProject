using Parlot;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule
{
    internal class WebApiDetaliusBotTestsClient
    {
        private string v;
        private HttpClient http;

        public WebApiDetaliusBotTestsClient(string v, HttpClient http)
        {
            this.v = v;
            this.http = http;
        }

        
        internal void SetToken(object access_token)
        {
            throw new NotImplementedException();
        }
    }
}