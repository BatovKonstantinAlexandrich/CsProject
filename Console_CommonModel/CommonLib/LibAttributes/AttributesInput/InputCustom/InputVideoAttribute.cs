[Label("Видео")]
[Description("Ввод пути или URL к видеофайлу с встроенным воспроизведением в форме")]
public class InputVideoAttribute : BaseInputAttribute, MyValidation
{
    public InputVideoAttribute() : base(InputTypes.Video)
    {
    }

    public InputVideoAttribute(string argument) : base(InputTypes.Video)
    {
    }

    public override bool IsValidValue(object value)
    {
        return value is null || value is string;
    }
}
