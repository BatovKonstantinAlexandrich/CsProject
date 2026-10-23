using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.ViewEvents
{
    [Label("Тестирование событий AppViewBase")]
    public class ViewEventsUnit : TestingUnit
    {
        public ViewEventsUnit(IServiceProvider sp)
        {
            Append(new ChangingListTest(sp));
            Append(new ChangingDictionaryTest(sp));
        }
    }
}
