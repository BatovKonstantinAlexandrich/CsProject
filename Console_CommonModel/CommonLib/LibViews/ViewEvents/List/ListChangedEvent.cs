using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.List
{

    public class ListChangedEvent : CommonEventMessage<ChangingListMessage>
    {
        public ListChangedEvent(ChangingListMessage Target) : base(Target)
        {
        }

        public ChangingListMessage GetChanges()
        {
            return Target;
        }
    }
}
