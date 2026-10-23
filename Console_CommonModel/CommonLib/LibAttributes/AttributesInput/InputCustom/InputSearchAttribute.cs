using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom
{

    [Label("Поиск значения")]
    [Description("Атрибут указывает системе компановки на необходимость выбора значения с помощью" +
        " представления поиска с широким набором функциональности")]
    public class InputSearchAttribute<TDbContext, TEntity> : BaseInputAttribute
    {
        public InputSearchAttribute() : base(InputTypes.Custom) { }

        public InputSearchAttribute(string inputType) : base(inputType)
        {
        }

        public Type GetDbContextType() => typeof(TDbContext);
        public Type GetEntityType() => typeof(TEntity);
    }
}
