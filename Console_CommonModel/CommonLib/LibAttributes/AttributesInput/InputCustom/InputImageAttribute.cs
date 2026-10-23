[Label("Изображение")]
public class InputImageAttribute : InputFileAttribute
{
    public InputImageAttribute() : base(InputTypes.Image) { }

    public InputImageAttribute(string argument) : base(InputTypes.Image) { }

   
} 