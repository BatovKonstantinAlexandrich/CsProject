using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Console_CommonModel.CommonLib.Others
{
    public class ConsoleUnit : TestingUnit
    {
        public ConsoleUnit()
        {
            InitTests();
        }

        public ConsoleUnit(IServiceProvider provider) : base(provider)
        {
            InitTests();
        }

        private void InitTests()
        {
            //dAppend(new ConsoleClassUnit(provider));

        }
    }
}
