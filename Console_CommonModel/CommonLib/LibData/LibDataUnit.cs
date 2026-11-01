using Console_CommonModel.CommonLib.LibData.RepositoryPattern;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData
{
    public class LibDataUnit : TestingUnit
    {
        public LibDataUnit(IServiceProvider parent) : base(parent)
        {
            this.Append(new RepositoryTest(parent));
            this.Append(new DbContextUserTest(parent));
        }

        
    }
}
