using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System.IO;

using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;


[Label("Файл")]
public class InputFileAttribute : ControlAttribute
{
    protected readonly string _extensions;

    public InputFileAttribute() : base(InputTypes.File)
    {
        _InputType = InputTypes.File;
    }

    public InputFileAttribute(string exts) : base(InputTypes.File)
    {
        _InputType = InputTypes.File;
        _extensions = exts;
    }

    public override AppViewBase CreateView(InputFormField field)
    {
        EnsureByteArrayValue(field);

        var pane = new AppViewBase { AutoSize = false, MinHeight = 56 };
        var status = new TextView { Text = GetStatusText(field.Value), MinHeight = 24 };
        pane.LayoutTop(status);

        Action pickFile = null;
        pickFile = () =>
        {
            
            this.OpenFile(
                "Выберите файл",
                string.IsNullOrWhiteSpace(_extensions) ? "All files (*.*)|*.*" : _extensions,
                Directory.GetCurrentDirectory(),
                file =>
                {
                    var data = File.ReadAllBytes(file);
                    field.Value = data;
                    field.Target?.SetValue(field.Name, data);
                    status.Text = GetStatusText(data, file);
                    try
                    {
                        field.OnInput?.Invoke(Convert.ToBase64String(data));
                    }
                    catch
                    {
                        field.OnInput?.Invoke($"{data.Length}");
                    }
                });
        };

        var buttonText = GetBytes(field.Value).Length > 0 ? "Заменить файл" : "Выбрать файл";
        pane.LayoutTop(pane.CreateButton(buttonText, pickFile));
        return pane;
    }

    private void OpenFile(string v1, string v2, string v3, Action<string> value)
    {
        throw new NotImplementedException();
    }

    //public override Control CreateWinFormsControl(InputFormField field)
    //{
    //    var panel = new FlowLayoutPanel
    //    {
    //        AutoSize = true,
    //        FlowDirection = FlowDirection.TopDown,
    //        WrapContents = false
    //    };

    //    var status = new Label
    //    {
    //        AutoSize = true,
    //        Text = GetStatusText(field.Value)
    //    };
    //    panel.Controls.Add(status);

    //    var button = new Button { Text = GetBytes(field.Value).Length > 0 ? "Заменить файл" : "Выбрать файл" };
    //    button.Click += (_, _) =>
    //    {
    //        var fds = new FileDialogService();
    //        fds.OpenFile(
    //            "Выберите файл",
    //            string.IsNullOrWhiteSpace(_extensions) ? "All files (*.*)|*.*" : _extensions,
    //            Directory.GetCurrentDirectory(),
    //            file =>
    //            {
    //                var data = File.ReadAllBytes(file);
    //                field.Value = data;
    //                status.Text = GetStatusText(data, file);
    //                button.Text = "Заменить файл";
    //                field.OnInput?.Invoke(Convert.ToBase64String(data));
    //            });
    //    };
    //    panel.Controls.Add(button);
    //    return panel;
    //}

    public override bool IsValidValue(object value) => true;

    public override string OnValidate(object model, string property, object value)
        => null;

    public override string OnGetMessage(object model, string property, object value)
        => "";

    protected static void EnsureByteArrayValue(InputFormField field)
    {
        if (field.Value is byte[])
            return;

        field.Value = field.Value is null ? Array.Empty<byte>() : field.Value;
        if (field.Value is not byte[])
            field.Value = Array.Empty<byte>();
    }

    protected static byte[] GetBytes(object value)
        => value is byte[] bytes ? bytes : Array.Empty<byte>();

    protected static string GetStatusText(object value, string filePath = null)
    {
        var bytes = GetBytes(value);
        if (bytes.Length == 0)
            return "Файл не выбран";

        var size = bytes.Length >= 1024
            ? $"{bytes.Length / 1024.0:0.#} КБ"
            : $"{bytes.Length} байт";

        if (!string.IsNullOrWhiteSpace(filePath))
            return $"Файл: {Path.GetFileName(filePath)} ({size})";

        return $"Загружено: {size}";
    }
}
