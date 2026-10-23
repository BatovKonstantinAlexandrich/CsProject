using Newtonsoft.Json.Linq;

using System.Collections.Generic;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModels.CoreMessaging
{
    public class RequestParams
    {
        public string path { get; set; }
        public JObject pars { get; set; }

        public byte[] blob { get; set; }
    }
}
