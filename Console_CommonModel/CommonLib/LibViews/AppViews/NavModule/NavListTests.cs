using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.NavModule
{
    [Label($"Тестирование {nameof(NavView)}")]
    public class NavListTests : TestingElement
    {
        public NavListTests() : base()
        {
        }

        public NavListTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
        }
    }
}
