using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

public class ControlTypes
{
    public const string Select = "Select";
    public const string Combo = "Combo";
    public const string Check = "Select";
    public const string Switch = "Select";
    public const string SwitchMulti = "SwitchMulti";
}

public abstract class ControlAttribute  : BaseInputAttribute
{

    //public abstract Control CreateWinFormsControl(InputField field);

    
    public string ControlType { get; set; }

    protected ControlAttribute() : base(InputTypes.Custom)
    {
        ControlType = ControlTypes.Select;
    }

    public ControlAttribute(string ControlType) : base(InputTypes.Custom)
    {
        this.ControlType = ControlType;
    }


    public virtual AppViewBase CreateControl(InputFormField field) =>
        this.CreateView(field);
  


    public virtual void Layout(object form) { }

    public override bool IsValidValue(object value) => true;

    public abstract AppViewBase CreateView(InputFormField field);

    //public virtual TControl CreateWinFormsControl(InputFormField field)
    //    => CreateView(field).ToControl();
}

public class FormModel : FormModel<object>
{
    public FormModel(object target)
    {
        this.Item = target;
    }
}
public class FormModel<T> where T : class
{
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Message { get; set; }

    public List<InputFormField> FormFields { get; set; } = new();

    public T Item { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; }
    [InputBool]
    public bool IsValid { get; set; } = false;
}

public enum FormFieldType
{
    Select, Checkbox, Text, Number, Data, DateTime, File, Image, Textarea
}
public class ParameterAttribute: Attribute
{

}

 