[Label("Целое число")]
[Description("Используется для маркировки свойства, которое" +
    " должно содержать целое число.")]
public class InputIntAttribute: BaseInputAttribute
{
    [Label("Минимальное значение")]
    public int MinValue { get; set; } = int.MinValue;

    [Label("Максимальное значение")]
    public int MaxValue { get; set; } = int.MaxValue;


    public InputIntAttribute( ) : base(InputTypes.Number)
    {
    }

    public InputIntAttribute(string exp) : base(InputTypes.Number)
    {
    }


    public override bool IsValidValue(object value)
    {
        if(value is not null)
        {
            var strValue = value.ToString();
            if (!strValue.IsInt())
            {
                return false;
            }
            else if(MinValue>=strValue.ToInt() || MaxValue<=strValue.ToInt())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }        
    }

    public override string OnValidate(object model, string property, object value)    
        => IsValidValue(value) ? null : GetMessage(model, property, value);

    

    public override string OnGetMessage(object model, string property, object value)    
        => $"Значение свойства {model.GetType().Name}.{property} не является целочисленным";

    
}

