using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibExtensions
{
    [Label("Модуль тестирования функций расширения типов")]
    public class LibExtensionsUnit : TestingUnit
    {     
        public LibExtensionsUnit(IServiceProvider parent) : base(parent)
        {                        
            this.Append(new TextConvertExtensionsTest(this.provider));
            this.Append(new TextCountingExtensionsTest(this.provider));
            this.Append(new TextExtensionsTest(this.provider));
            this.Append(new TextFactoryExtensionsTest(this.provider));
            this.Append(new TextIOExtensionsTest(this.provider));
            this.Append(new TextLangExtensionsTest(this.provider));
            this.Append(new TextNamingExtensionsTest(this.provider));
            this.Append(new TextNamingTest(this.provider));
            this.Append(new TextValueExtensionsTest(this.provider));
            this.Append(new FunctionExtensionsTest(this.provider));
        }
    }
}
