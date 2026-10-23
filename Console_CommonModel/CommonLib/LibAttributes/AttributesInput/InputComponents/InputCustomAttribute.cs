using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputComponents
{
    public class InputCustomAttribute : BaseInputAttribute
    {
        private readonly string typeName;

        public InputCustomAttribute() : base() { }
        public InputCustomAttribute(string typeName) : base()
        {
            this.typeName = typeName;
        }
    }
}
