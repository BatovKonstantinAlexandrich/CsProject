[Label("Камера")]
[Description("Ввод идентификатора камеры с предпросмотром потока в форме")]
public class InputCameraAttribute : BaseInputAttribute, MyValidation
{
    public InputCameraAttribute() : base(InputTypes.Camera)
    {
    }

    public InputCameraAttribute(string argument) : base(InputTypes.Camera)
    {
    }

    public override bool IsValidValue(object value)
    {
        return value is null || value is string;
    }
}
