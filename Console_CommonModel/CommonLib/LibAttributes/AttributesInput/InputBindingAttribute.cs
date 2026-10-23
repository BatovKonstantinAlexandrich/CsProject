using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibAttributes.AttributesInput
{
    /// <summary>
    /// Связывание модели с FormField свойствами
    /// </summary>
    public class InputBindingAttribute : Attribute
    {
        public string ModelPropertyName { get; set; }
        public string FieldPropertyName { get; set; }
        public InputBindingAttribute(string binding)
        {
            if (!binding.Contains("="))
                throw new ArgumentException(nameof(binding),
                    "Значение binding должно содержать два имени свойства разделённых знаком равенства");
            var arr = binding.Split("=");
            ModelPropertyName = arr[0];
            FieldPropertyName = arr[1];
        }

    }
}
