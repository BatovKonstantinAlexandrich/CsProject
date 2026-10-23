using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;


[Label("Поле для ввода данных")]

public class InputFormField: AppViewBase
{


    /// <summary>
    /// Вызывается по событию ввода
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<string> OnInput = (text) => { };


    /// <summary>
    /// Получение атрибута
    /// </summary>
    public TAttribute GetFieldAttribute<TAttribute>() where TAttribute: Attribute
    {
        
        if (!this.Attributes.ContainsKey(typeof(TAttribute).GetTypeName()))
        {            
            throw new ArgumentException("GenericType задан неверно");
        }
        else
        {
            var argv = this.Attributes[typeof(TAttribute).GetTypeName()];
            return typeof(TAttribute).New<TAttribute>(new object[] { argv });
        }
    }

 
    
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput()]
    [Label("Ссылка на объект")]
    public object Target { get; set; }

    
    [NotInput]
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [Label("Input Form Model")]
    public virtual InputFormModel InputFormModel { get; set; }

    [InputHidden]
    [InputNumber]
    [NotInput]
    [Label("Input Form Model Id")]
    public int InputFormModelId { get; set; }

    [NotMapped()]
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [Label("Control")]
    public AppViewBase Control { get; set; }

    [InputPositiveInt()]
    [NotNullNotEmpty()]
    [Label("Приоритет вывода")]
    public int Order { get; set; }

    [InputBool]
    [Label("Видно на экране")]
    public bool Visible { get => Get<bool>(nameof(Visible)); set => Set<bool>(nameof(Visible),value); }


    
    [Label("Атрибуты")]
    [NotMapped]
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();


    
    [Label("Атрибуты")]
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]
    public Dictionary<string, List<object>> CustomValidators { get; set; } = new Dictionary<string, List<object>>();
    [Label("Наименование")]
    [NotNullNotEmpty]
    [InputEngWord]
    public string Name { get; set; } = "Undefined";

    [Label("Иконка")]       
    
    public string Icon { get; set; } = "home";

    [NotInput]
    [Label("Text Value")]
    
    public string TextValue { get; set; }



    [Label("Описание")]
    [InputMultilineText]
    public string Description { get; set; } = "Нет подробного описания";

    [Label("Тип ввода")]
    [InputInputTypeAttribute]
    public string Type { get; set; } = "Text";
    [Label("Value Type")]
    
    public string ValueType { get; set; }

    
    [Label("Надпись")]
    public string Label { get => Get<string>("Label"); set => Set<string>("Label",value); }

    [Label("Разрешено изменение")]
    [InputBool]
    public bool Edited { get; set; }


    
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [Label("Value")]
    public object Value { get => Get<object>(nameof(Value)); set=>Set<object>(nameof(Value),value); }


    [Label("Is Primitive")]
    [InputBool]
    public bool IsPrimitive { get; set; }
    [Label("Is Collection")]
    [InputBool]
    public bool IsCollection { get; set; } = false;

    [Label("Размер")]
    [InputSelect("small,normal,big")]
    public string Size { get; set; } = "normal";


    [Label("Состояние")]
    [InputSelect("valid,invalid,undefined")]
    public string State { get; set; } = "undefined";

    [NotMapped]
    [Label("Подсказка")]
    
    public string Help { get; set; } = "";

    
    [Label("Ошибки")]
    [NotMapped]
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ChangingList<string> Errors { get; set; } = new ChangingList<string>();

    
    [NotMapped]
    [NotInput]
    [Label("Warnings")]
    public List<string> Warnings { get; set; } = new List<string>();



    [Label("Тип данных свойства или параметра")]
    [NotInput]
    [NotNullNotEmpty]
    
    public string DataType { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]

    
    [Label("Ctrl Type Ref")]
    public object CtrlTypeRef { get; internal set; }
    
    [Label("Ctrl Type")]
    public ControlAttribute CtrlType { get; internal set; }

    public InputFormField() : base()
    {
        this.Visible = true;
        this.Label = "Неизвестно";
        this.AutoSize = true;
        this.MinHeight = 0;        
    }

    public InputFormField(Type model, string name): this()
    {
        this.Value = "";
        this.Visible = true;
        this.AutoSize = true;
        this.Errors.Add("test");
        this.Icon = model.GetPropertyIcon(name);
        this.Name = model.GetPropertyIcon(name);
        this.Type = model.GetProperty(name).PropertyType.GetTypeName();
        this.State = "undefined";
        this.Label = model.GetPropertyLabel(name);
        this.Description = model.GetPropertyDescription(name);
        this.Help = model.GetPropertyHelp(name);
        this.MinHeight = 0;
    }

    public InputFormField(string property, Type type, Dictionary<string, string> dictionary)
    {
    }

    public string GetHtmlInputValue()
    {
        InputFormField Model = this;
        switch (Type.ToLower())
        {
            case "date":
                string year = "XXXX";
                string month = "XX";
                string day = "XX";
                if (Model.Value != null)
                {
                    System.DateTime? dateNullable = null;
                    if (Model.Value is System.DateTime)
                    {
                        dateNullable = (System.DateTime)Model.Value;
                    }
                    else
                    {
                        try
                        {
                            var str = Model.Value.ToString();
                            dateNullable = str.ToDate();
                        }catch(Exception)
                        {                            
                        }
                    }
                    if (dateNullable == null)
                    {
                        //throw new Exception("Значение DateTime не получено в итоге");
                        return "";
                    }
                    System.DateTime date = (System.DateTime)dateNullable;
                    year = date.Year.ToString();
                    month = (date.Month >= 10) ? date.Month.ToString() : ("0" + date.Month.ToString());
                    day = (date.Day >= 10) ? date.Day.ToString() : ("0" + date.Day.ToString());
                }
                return $"{year}-{month}-{day}";
            default: return Value != null ? Value.ToString() : "";

        }

    }

    public string GetInputId() => Name + "Input-" + GetHashCode();
    public InputFormField SetEdited(bool val)
    {
        this.Edited = val;
        return this;
    }
    public string GetInputName() => Name + "Input";

    public string GetErrorMessage()
    {
        string message = "";
        foreach(var error in Errors)
        {
            message += error + "\n";
        }
        return message;
    }

    internal void SetErrors(params string[] messages)
    {
        this.Errors.Clear();
        this.Errors.AddRange(messages);
    }

    internal bool IsUniq()
    {
        return false;
    }

    public object GetInputAttribute()
    {
        var a = Attributes.FirstOrDefault(a => a.Key.ToType().IsExtends(typeof(BaseInputAttribute)));
        return a.Key.ToType().New(new object[] { a.Value });
    }

    internal void ToEditDialog(Action<object, object> value)
    {
        throw new NotImplementedException();
    }
}
