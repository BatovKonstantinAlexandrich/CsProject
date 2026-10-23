using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassModelLib.ActionsClient
{
    public interface IQueueConsumer

    {
        public ActionRequest TakeNext();

    }
}
