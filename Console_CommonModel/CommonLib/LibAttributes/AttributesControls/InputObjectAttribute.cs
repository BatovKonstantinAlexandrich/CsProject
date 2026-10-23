using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;

using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;


public class InputObjectAttribute:  ControlAttribute 
{
    private readonly string _typeName;

    public InputObjectAttribute() : base(InputTypes.Object)
    {
        _InputType = InputTypes.Object;
    }

    public InputObjectAttribute(string typeName) : base(InputTypes.Object)
    {
        _InputType = InputTypes.Object;
        _typeName = typeName;
    }
    private Type ResolveItemType(InputFormField field)
    {
        if (!string.IsNullOrWhiteSpace(_typeName))
            return _typeName.ToType();

        var attrKey = field.Attributes?.Keys.FirstOrDefault(key =>
            key.ToType().IsExtends(typeof(InputObjectAttribute)));
        if (attrKey is not null
            && field.Attributes.TryGetValue(attrKey, out var arg)
            && !string.IsNullOrWhiteSpace(arg))
        {
            return arg.Trim().ToType();
        }

        if (!string.IsNullOrWhiteSpace(field.ValueType))
            return field.ValueType.ToType();

        if (field.Value is not null)
            return field.Value.GetType();

        return typeof(object);
    }
    public override AppViewBase CreateView(InputFormField field)
    {
        var root = new AppViewBase { AutoSize = false };
        var itemType = ResolveItemType(field);
        var instance = field.Value ?? itemType.New();
        field.Value = instance;
        if (field.Value is null)
        {
            throw new ArgumentNullException($"field.Value");
        }
        if (field.Value.GetType().IsPrimitiveForType())
        {
            var editor = new TextBoxView();
            editor.Text = $"{field.Value}";
            field.AddPropertyChangeListener<object>(nameof(field.Value), val => editor.Text = $"{val}");
            editor.OnTextChanged += (text) =>
            {
                if (field.OnInput is not null)
                {
                    field.OnInput(text);
                }
            };
            editor.MinHeight = 280;
            editor.Height = 280;
            editor.Dock = "Fill";
            root.Controls.Add(editor);
            root.MinHeight = 280;
            root.Height = 280;
            return editor;
        }
        else
        {
            var editor = EditView.CreateEditViewBase(instance, (success, model) =>
            {
                if (!success || model is null)
                    return;

                field.Value = model;
                field.Target?.SetValue(field.Name, model);
                try
                {
                    field.OnInput?.Invoke(model.ToJson());
                }
                catch
                {
                    field.OnInput?.Invoke($"{model}");
                }
            });
            //editor.submit.ShowButtons = false;
            editor.MinHeight = 280;
            editor.Height = 280;
            editor.Dock = "Fill";
            root.Controls.Add(editor);
            root.MinHeight = 280;
            root.Height = 280;
            return root;
        }
    }
}
