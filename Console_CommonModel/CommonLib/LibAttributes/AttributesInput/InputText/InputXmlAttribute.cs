using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System.Xml.Linq;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;


[Label("XML-разметка")]
public class InputXmlAttribute : ControlAttribute
{
    public InputXmlAttribute() : base(InputTypes.Xml)
    {
        _InputType = InputTypes.Xml;
    }

    public InputXmlAttribute(string argument) : base(InputTypes.Xml)
    {
        _InputType = InputTypes.Xml;
    }

    public override AppViewBase CreateView(InputFormField field)
    {
        var textView = new MultiLineTextView
        {
            Text = field.Value is null ? string.Empty : $"{field.Value}",
            MinHeight = 200
        };

        field.AddPropertyChangeListener<object>(nameof(field.Value), val =>
            textView.Text = val is null ? string.Empty : $"{val}");

        textView.OnTextChanged += text =>
        {
            field.Value = text;
            field.OnInput?.Invoke(text);
        };

        return textView;
    }

    //public override Control CreateWinFormsControl(InputFormField field)
    //{
    //    var factory = new ControlsFactory();
    //    var text = field.Value is null ? string.Empty : $"{field.Value}";
    //    var control = factory.CreateMultilineTextBox(text, field.OnInput);
    //    return control;
    //}

    public override string OnValidate(object model, string property, object value)
    {
        if (value is null || string.IsNullOrWhiteSpace($"{value}"))
            return null;

        try
        {
            XDocument.Parse($"{value}");
            return null;
        }
        catch (Exception ex)
        {
            return $"Некорректный XML: {ex.Message}";
        }
    }

    public override bool IsValidValue(object value)
    {
        if (value is null || string.IsNullOrWhiteSpace($"{value}"))
            return true;

        try
        {
            XDocument.Parse($"{value}");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override string OnGetMessage(object model, string property, object value)
        => $"Свойство {property} должно содержать корректную XML-разметку.";
}
