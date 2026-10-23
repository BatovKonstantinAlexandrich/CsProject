using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

[Label("Дата")]
public class InputDateAttribute : BaseInputAttribute, MyValidation
{
    public InputDateAttribute( ) : base(
        InputTypes.Date)
    {
        
    }

    public InputDateAttribute(string message) : base(
        InputTypes.Date)
    {
        this.ErrorMessage = message;
    }


    public string IsValidTextValue(string text)
    {
        if (text == null)
            return "Неверный формат даты";
        string message = null;
        if (text.Length > "01/01/2024".Length)
        {
            message = "Неверный формат даты";
        }
        for (int i = 0; i < text.Length; i++)
        {
            if (i == 2 || i == 4)
            {
                if (text[i] != '.' && text[i] != '-' && text[i] != '/')
                {
                    message = "Неверный формат даты";
                    break;
                }
            }
            else
            {
                if (!text[i].IsNumber())
                {
                    message = "Неверный формат даты";
                    break;
                }
            }
        }
        return message;
    }

    public override bool IsValidValue(object value)
    {
        if (value is DateTime)
            return true;
        if (value is null)
            return true;
        /*Regex regex = new Regex("(\\d{2}).(\\d{2}).(\\d{4})");
        string str = $"{value}";
        if (str.Length > "__.__.____".Length)
            str.Substring(0, "__.__.____".Length);
        if (!regex.IsMatch($"{value}"))
            return false;
        var arr = $"{value}".Split(".");
        
        bool res = (arr[0].IsInt() && arr[0].ToInt() < 31) && (arr[1].IsInt() && arr[1].ToInt() <= 12) && (arr[2].IsInt() && arr[2].ToInt() < 300);*/

        return IsValidTextValue($"{value}") == null; 
    }

    public override string OnValidate(object model, string property, object value)
    {
        return IsValidValue(value) ? null : OnGetMessage(model,property,value); 
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        return "Дата задана некорректно, формат дд.мм.гггг";
    }
}

