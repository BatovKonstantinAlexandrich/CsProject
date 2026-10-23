using System;

public class SelectControlWithoutValidation : SelectControlAttribute
{
    public SelectControlWithoutValidation(): base()
    {
        Writing.ToConsole("Create");
    }

    public SelectControlWithoutValidation(string expression) : base(expression)
    {
        Writing.ToConsole("Create");
    }
    public override string GetMessage(object model, string property, object value)
    {
        return null;
    }

    public override string Validate(object model, string property, object value)
    {
        return null;
    }
}
 