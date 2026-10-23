using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.CheckboxModule
{
    [Label($"Тестирование {nameof(CheckboxView)}")]
    public class CheckboxTests : ViewTesting<CheckboxView>
    {
        public CheckboxTests() : base()
        {
        }

        public CheckboxTests(IServiceProvider provider) : base(provider)
        {
        }
    }
}
