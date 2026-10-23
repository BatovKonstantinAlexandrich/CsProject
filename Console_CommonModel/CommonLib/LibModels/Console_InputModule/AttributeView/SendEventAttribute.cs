using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.AttributeView
{
    public class SendEventAttribute : ViewAttribute
    {
        private string _eventName;

        public SendEventAttribute(string eventName)
        {
            _eventName = eventName;
        }
    }
}
