using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TableModule
{

    [Label($"Тестирование {nameof(TableView)}")]
    public class TableTests : TestingElement
    {
        public TableTests() : base()
        {
        }

        public TableTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
        }
    }
}
