[Label("Путь к файлу")]
public class InputFilePathAttribute : BaseInputAttribute
{
    public string Extensions { get; }

    public InputFilePathAttribute() : base(InputTypes.FilePath) { }

    public InputFilePathAttribute(string exts) : base(InputTypes.FilePath)
    {
        Extensions = exts;
    }

    public override bool IsValidValue(object value)
    {
        if (value is null)
            return true;
        var path = value.ToString();
        return string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path);
    }

    public override string OnValidate(object model, string property, object value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return null;
        if (System.IO.File.Exists(value.ToString()))
            return null;
        return "Путь задан неверно";
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        return "Укажите существующий путь к файлу";
    }
}
