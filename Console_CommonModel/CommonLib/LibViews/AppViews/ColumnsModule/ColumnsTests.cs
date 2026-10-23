using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule
{
    [Label($"Тестирование {nameof(ColumnsView)}")]
    public class ColumnsTests : TestingElement
    {
        public ColumnsTests() : base()
        {
        }

        public ColumnsTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
        }
    }
}
