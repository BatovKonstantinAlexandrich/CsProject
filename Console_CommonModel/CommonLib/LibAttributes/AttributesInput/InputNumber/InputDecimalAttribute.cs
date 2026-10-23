[Label("Вещественное число")]
[Description("Через аргумент можно определить кол-во десятичных знаков после запятой.")]
public class InputDecimalAttribute : BaseInputAttribute
{
    private readonly int length = 0;

    public InputDecimalAttribute(  ) : base( InputTypes.Number )
    {       
    }

    public InputDecimalAttribute(string length) : base(InputTypes.Number)
    {
        if(length.IsInt())
            this.length = int.Parse(length);
    }


    public override bool IsValidValue(object value)
    {
        return value is not null && value.ToString().IsFloat();
    }


    public override string OnValidate(object model, string property, object value)
    {
        if( value is null)
        {
            return this.GetMessage(model, property, value);
        }
        else
        {
            string text = value.ToString();
            if( text.IsFloat())
            {
                return null;
            }
            else
            {
                return GetMessage(model,property,value);
            }
        }
    }


    public override string OnGetMessage(object model, string property, object value)
    {
        return $"Значение не является десятичным числом";
    }
}