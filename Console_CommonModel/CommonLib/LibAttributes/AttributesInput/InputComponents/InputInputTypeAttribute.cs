using System.Collections.Generic;

[Label("Указывает на выбор из списка InputType")]
public class InputInputTypeAttribute : InputSelectAttribute
{
    public static IEnumerable<string> GetInputTypes() =>
        BaseInputAttribute.GetInputTypes().ToList();
    public static string GetInputTypesString()
    {
        string text = "";
        foreach (string controlType in GetInputTypes())
        {
            text += $"{controlType},";
        }
        if (text.EndsWith(","))
            text = text.Substring(0, text.Length - 1);
        return text;
    }
    public InputInputTypeAttribute() : base(GetInputTypesString())
    {
    }
    public InputInputTypeAttribute(string exp) : base(GetInputTypesString())
    {
    }
}
