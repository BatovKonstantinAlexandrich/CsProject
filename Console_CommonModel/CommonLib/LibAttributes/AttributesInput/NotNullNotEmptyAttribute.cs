
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

public class NotNullNotEmptyAttribute : BaseValidationAttribute, MyValidation
{
    protected string _errorMessage;

    public NotNullNotEmptyAttribute() : base()
    {
        ErrorMessage = _errorMessage = "Не установлено обязательное свойство";
    }
    public NotNullNotEmptyAttribute(string error) : base()
    {
        if (String.IsNullOrWhiteSpace(error))
            error = "Не установлено обязательное свойство";
        ErrorMessage = _errorMessage = error;
    }


    public override string Validate(object model, string property, object value)
    {
        if (value is null)
            return _errorMessage;
        if( value is string && String.IsNullOrWhiteSpace(value.ToString()))
        {
            return _errorMessage;
        }
        if (value != null && value is byte[] )
        {
            return null;
        }

        if(value is DateTime)
        {
            if(((DateTime)value).Year == 1 && ((DateTime)value).Month == 1 && ((DateTime)value).Day == 1)
            {
                return _errorMessage;
            }
        }
        if (value == null || value.ToString().Trim() == "")
            return _errorMessage;
        return null;
    }

    public string GetMessage(object model, string property, object value)
    {
        if (string.IsNullOrEmpty(ErrorMessage))
        {
            return "Свойство "+property+" необходимо определить как действительное значение";
        }
        else
        {
            return ErrorMessage;
        }
    }
}


 