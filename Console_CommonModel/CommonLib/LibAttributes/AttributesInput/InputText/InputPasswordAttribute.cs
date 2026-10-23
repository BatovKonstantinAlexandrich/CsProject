using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Label("Пароль")]
public class InputPasswordAttribute : BaseInputAttribute
{
    public override bool IsValidValue(object? value)
    {
        if (value is null)
            return true;
        return value?.ToString()?.Length >= 8;
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        return $"Пароль должен содержать мин. 8 символов";
    }

    public override string Validate(object value)
    {
        if (value is null)
            return null;
        var mes = $"Значение {value} не является допустимым для адреса электронной почты";
        return IsValidValue(value) ? null : mes;
    }

    public override string OnValidate(object model, string property, object value)
    {
        if (IsValidValue(value) == false)
        {
            return GetMessage(model, property, value);
        }
        else
        {
            return null;
        }
    }


    public InputPasswordAttribute() : base(InputTypes.Password)
    {
        this.ErrorMessage = "Пароль не может быть меньше 8 символов";
    }

    public InputPasswordAttribute( string error = null  ) : base(InputTypes.Password)
    {
        this.ErrorMessage = String.IsNullOrWhiteSpace(error) ? "Пароль может содержать от 1 до 8 символов" : "";
    }

  
}

