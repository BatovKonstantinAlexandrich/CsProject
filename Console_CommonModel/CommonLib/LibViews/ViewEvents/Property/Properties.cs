using Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModels.Property;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.Property
{
    public class Properties : ConcurrentDictionary<string, Property<PropertyValue>>
    {
    }
}
