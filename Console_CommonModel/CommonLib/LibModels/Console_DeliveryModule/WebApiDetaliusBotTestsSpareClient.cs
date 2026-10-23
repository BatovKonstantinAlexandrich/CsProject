using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule
{
    internal class WebApiDetaliusBotTestsSpareClient
    {
        private string v;
        private HttpClient http;

        public WebApiDetaliusBotTestsSpareClient(string v, HttpClient http)
        {
            this.v = v;
            this.http = http;
        }

        internal async Task<List<SparePart>> PartAsync(object product_id)
        {
            throw new NotImplementedException();
        }
    }
}