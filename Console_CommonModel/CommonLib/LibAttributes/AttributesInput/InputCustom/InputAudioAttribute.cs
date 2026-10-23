[Label("Аудио")]
[Description("Ввод пути или URL к аудиофайлу с воспроизведением в форме")]
public class InputAudioAttribute : BaseInputAttribute, MyValidation
{
    public InputAudioAttribute() : base(InputTypes.Audio)
    {
    }

    public InputAudioAttribute(string argument) : base(InputTypes.Audio)
    {
    }

    public override bool IsValidValue(object value)
    {
        return value is null || value is string;
    }
}
