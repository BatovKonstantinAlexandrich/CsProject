[Label("Файл")]
[Description("Свойство должно быть типа byte[]")]
public class InputFileDataAttribute : InputFileAttribute
{
    public InputFileDataAttribute() : base() { }

    public InputFileDataAttribute(string exts) : base(exts) { }

    public override bool IsValidValue(object value)
    {
        if (value is null)
            return true;

        return value is byte[] bytes && bytes.Length > 0;
    }

    public override string OnValidate(object model, string property, object value)
        => IsValidValue(value) ? null : GetMessage(model, property, value);

    public override string OnGetMessage(object model, string property, object value)
        => $"Свойство {property} должно содержать данные файла.";
}
