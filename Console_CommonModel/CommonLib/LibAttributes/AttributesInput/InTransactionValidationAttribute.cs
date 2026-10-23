using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibAttributes.AttributesInput
{
    public abstract class InTransactionValidationAttribute: BaseValidationAttribute
    {


        public abstract string Validate(object model, string property, object value, DbContext dbContext);

    }
}
