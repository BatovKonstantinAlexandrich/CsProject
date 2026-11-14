using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;

namespace 
{
    
   public partial class Validations
    {
        [SqlFunction]
        public static SqlString HelloWorld()
        {
            return new SqlString("Hello from SQL CLR!");
        }
    }
}