using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedOrg.Shared.ViewServices
{
    public interface IMessageHandlerService
    {

        /// <summary>
        /// При
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public RequestMessage GetRequest(string message);
    }
}
