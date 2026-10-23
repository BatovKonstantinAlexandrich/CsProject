using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedOrg.Shared.ViewServices
{
    public class JsonMessageHandlerService : IMessageHandlerService
    {
        public RequestMessage GetRequest(string message)
        {
            return message.FromJson<RequestMessage>();
        }
    }
}
