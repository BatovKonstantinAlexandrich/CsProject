[Label("Натуральное число")]
public class InputPositiveIntAttribute : InputNumberAttribute
{
    protected string _message;

    public InputPositiveIntAttribute() : base(InputTypes.Number) { }

    public InputPositiveIntAttribute(string message) : base(InputTypes.Number)
    {
        _message = message;
    }
    public override string GetMessage(object model, string property, object value)
    {
        if (string.IsNullOrEmpty(_message))
        {
            return "Значение должно быть положительным";
        }
        else
        {
            return _message;
        }
    }
    //Validate(new object (), "", value);
 
    public string Validate(object value) => ValidateValue(value);
    public override string Validate(object model, string property, object value)
    {
        if (value != null)
        {            
            return value.ToString().ValidateIsPositiveInt();
        }
        else
        {
            return null;
        }
    }

    public string ValidateValue(object value) => Validate(new object(), "", value);
}