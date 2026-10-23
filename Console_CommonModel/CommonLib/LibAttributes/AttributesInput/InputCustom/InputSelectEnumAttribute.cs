using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

[Label("Выбор из перечисленного")]
public class InputSelectEnumAttribute : InputSelectAttribute
{
    public InputSelectEnumAttribute() : base(InputTypes.Custom) { }
   
    public InputSelectEnumAttribute(string enumType) : base(enumType)
    {
        this.EnumType = enumType;        
        foreach (var item in Enum.GetValues(this.EnumType.ToType()))
        {
            this.Options.Add(item.ToString());
        }
    }

    public string EnumType { get; }

    public override bool IsValidValue(object value)
        => value != null && Enum.GetNames(EnumType.ToType()).Contains(value.ToString());
}
