using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.List
{

    public class IChangingListEvent : CommonEventMessage<IChangingListMessage>
    {
        public IChangingListEvent(IChangingListMessage Target) : base(Target)
        {
        }

        public IChangingListMessage GetChanges()
        {
            return Target;
        }
    }
}
