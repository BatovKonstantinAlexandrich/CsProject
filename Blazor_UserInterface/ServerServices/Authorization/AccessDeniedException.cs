using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedOrg.Server.DataServices.Authorization
{
    public class AccessDeniedException: System.Exception
    {
        public AccessDeniedException(string message):base(message)
        {
            
        }
    }
}
