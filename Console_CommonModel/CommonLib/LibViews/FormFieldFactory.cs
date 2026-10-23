using System.Collections.Concurrent;
using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.PrimitiveCollectionModule;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;
using Console_CommonModel.CommonLib.LibViews.AppViews.LineModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.FormView;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;
using Microsoft.EntityFrameworkCore;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.LibAttributes;
using WinFormsApp.Icons;

namespace Console_CommonModel.CommonLib.LibViews
{
}





public class FormFieldFactory
{

    public List<InputFormField> CreateFields(object after, Action<string, string> OnFormFieldValueChanged)
    {
        return this.LogPerf($"CreateFields → {after.GetType().Name}", () => CreateFieldsCore(after, OnFormFieldValueChanged));
    }

    private List<InputFormField> CreateFieldsCore(object after, Action<string, string> OnFormFieldValueChanged)
    {
        //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(after);
        var properties = after.GetInputPropertiesSorted();
        var fieldsList = new List<InputFormField>(properties.Count);
        foreach (var property in properties)
        {
            var pvalue = after.GetValue(property);
            var pattrs = after.GetType().GetPropertyAttributes(property);
            var pinfo = after.GetType().GetProperty(property);
            fieldsList.Add(CreateField(after, property, pvalue, text =>
            {
                TextDataSetter.SetValue(after, property, text);
            }, pinfo, pattrs, text =>
            {
                if (OnFormFieldValueChanged is not null)
                    OnFormFieldValueChanged(property, text);
            }));
        }
        fieldsList.Sort((p1, p2) =>
        {
            return
            p1.Order != p2.Order ? p1.Order - p2.Order :
            p1.Order == p2.Order ?
            p1.IsCollection && !p2.IsCollection ? -1 :
            !p1.IsCollection && p2.IsCollection ? 1 :
                0 : 0;
        });
        return fieldsList;
    }


    /// <summary>
    /// Используется в FormView
    /// </summary>
    public InputFormField GetInputFormField(object target, string property, Action<string> onchange)
    {
        if (!target.GetType().GetPropertyNames().Contains(property))
            throw new ArgumentException(nameof(property));
        var val = target.GetValue(property);
        var attrs = target.GetType().GetPropertyAttributes(property);
        var prop = target.GetType().GetProperty(property);
        return CreateField(
            target,
            property,
            val,
            text => { target.SetValue(property, text); },
            prop,
            new(attrs),
            onchange);
    }
    private AppViewBase CreateField(InputFormField view)
    {
        try
        {
            var fff = new FormFieldFactory();
            switch (view.Type)
            {
                case "primitivecollection": fff.CreatePrimitiveCollection(view); break;
                case "structurecollection": fff.CreateStructuredCollection(view); break;
                case "structuredcollection": fff.CreateStructuredCollection(view); break;
                case "color": fff.CreateColor(view); break;
                case "audio": fff.CreateAudio(view); break;
                case "video": fff.CreateVideo(view); break;
                case "camera": fff.CreateCamera(view); break;
                case "signalrvideo":
                case "signalr_video": fff.CreateSignalRVideo(view); break;
                case "url": fff.CreateUrl(view); break;
                case "image": fff.CreateImage(view); break;
                case "icon": fff.CreateIcon(view); break;
                case "int": fff.CreateInt(view); break;
                case "percent": fff.CreatePercent(view); break;
                case "money": fff.CreateMoney(view); break;
                case "creditcard": fff.CreateCreditCard(view); break;
                case "natural": fff.CreateNatural(view); break;
                case "positiveint":
                case "positive": fff.CreatePositiveInt(view); break;
                case "bool": fff.CreateBool(view); break;
                case "number": fff.CreateNumber(view); break;
                case "boolean": fff.CreateBool(view); break;
                case "logic": fff.CreateBool(view); break;
                case "checkbox": fff.CreateBool(view); break;
                case "dictionary": fff.CreateDictionary(view); break;
                case "entitytype": fff.CreateEntityType(view); break;
                case "select": fff.CreateSelect(view); break;
                case "selectexpression": fff.CreateSelectExpression(view); break;
                case "selectdictionary": fff.CreateSelectDictionary(view); break;
                case "combobox": fff.CreateCombobox(view); break;
                //case "entitytype": fff.CreateEntityType(view); break;
                case "engword": fff.CreateEngWord(view); break;
                case "engtext": fff.CreateEngText(view); break;
                case "rustext": fff.CreateRusText(view); break;
                case "rusword": fff.CreateRusWord(view); break;
                case "phone": fff.CreatePhone(view); break;
                case "email": fff.CreateEmail(view); break;
                case "date": fff.CreateDate(view); break;
                case "datetime": fff.CreateDateTime(view); break;
                case "multilinetext": fff.CreateMultilineText(view); break;
                case "text": fff.CreateText(view); break;
                case "password": fff.CreatePassword(view); break;
                case "xml": fff.CreateXml(view); break;
                case "upload":
                case "file":
                case "filedata": fff.CreateUpload(view); break;
                case "decimal": fff.CreateNumber(view); break;
                case "filepath":
                case "file_path": fff.CreateFilePath(view); break;
                case "tcpipaddress": fff.CreateTcpIpAddress(view); break;
                case "duration":
                case "directory":
                case "fileitem":
                case "punctuation":
                case "confirmation": fff.CreateText(view); break;
                case "selectenum":
                case "enum": fff.CreateSelect(view); break;
                case "custom":

                    break;

            }

            //var b = new ButtonControl();
            //b.Text = view.Label;
            //view.control = b;
            //b.viewModel = view;
            return new ButtonView()
            {
                Text = view.Label
            };
        }
        catch (Exception ex)
        {
            this.Error($"Исключение при создании элемента управления InputFormField => {view.Name} => {ex.Message}");
            throw new Exception($"Исключение при создании элемента управления InputFormField => {view.Name} => {ex.Message}");
        }
    }


    public InputFormField CreateField(object target, string property, object value, Action<string> setter, PropertyInfo info, Dictionary<string, string> attributes, Action<string> onchange)
    {
        try
        {
            string pname = property;
            if (property == "XmlText")
            {
                this.Info(1);
            }
            InputFormField pfield = new();
            pfield.Value = target.GetProperty(property);
            pfield.Name = property;

            pfield.Help = attributes.ContainsKey(nameof(HelpMessageAttribute)) ? attributes[nameof(HelpMessageAttribute)] : "";
            pfield.Description = attributes.ContainsKey(nameof(DescriptionAttribute)) ? attributes[nameof(DescriptionAttribute)] : "";
            pfield.DataType = info.PropertyType.GetTypeName();
            pfield.IsCollection = info.PropertyType.IsCollectionType();
            pfield.Order = 0;
            pfield.Icon = attributes.ContainsKey(nameof(IconAttribute)) ? attributes[nameof(IconAttribute)] : "";
            pfield.Label = attributes.ContainsKey(nameof(LabelAttribute)) ? attributes[nameof(LabelAttribute)] : property;
            pfield.Attributes = attributes;
            pfield.OnInput = text =>
            {
                if (target is not null && $"{target.GetValue(property)}" != text)
                {

                    target.SetTextValue(property, text);
                    pfield.Errors.Clear();
                    pfield.Errors.AddRange(target.Validate(property));
                    pfield.ValidationResults[pfield.Name] = pfield.Errors.ToList();
                    pfield.ValidationSuccess = pfield.ValidationResults.All(kv => kv.Value.Count() == 0);
                    onchange(text);
                }
            };
            pfield.Type = GetInputType(info.PropertyType, attributes);
            if ((pfield.Type.StartsWith("structured") || pfield.Type.StartsWith("primitive")) && pfield.Type.Contains("<") && pfield.Type.Contains(">"))
            {
                var typeName = pfield.Type.Substring(pfield.Type.IndexOf("<") + 1);
                typeName = typeName.Substring(0, typeName.LastIndexOf(">") - 1);
                pfield.ValueType = GetValueType(info.PropertyType, property);
            }
            else
            {
                pfield.ValueType = GetValueType(info.PropertyType, property);
            }


            if (target is ChangeSupport)
            {

                ((ChangeSupport)target).OnChange += (evt) =>
                {
                    /*var field = pfield;
                    if(evt.Property == property)
                    {
                        if (field is not null && field.Value != evt.After)
                        {
                            field.Value = evt.After;
                        }
                    }*/
                };
            }
            pfield.CtrlType = target.GetType().GetPropertyControl(property);
            var orederStr = attributes.ContainsKey(nameof(InputOrderAttribute)) ? attributes[nameof(InputOrderAttribute)] : "0";
            var bindingValue = attributes.ContainsKey(nameof(InputBindingAttribute)) ? attributes[nameof(InputBindingAttribute)] : null;
            if (bindingValue is not null)
            {
                var binding = new InputBindingAttribute(bindingValue);
                if (target is not ChangeSupport)
                    throw new Exception("Атрибут InputBinding можно использовать только в моделях типа ChangeSupport");
                ChangeSupport support = (ChangeSupport)target;
                support.AddPropertyChangeListener(binding.ModelPropertyName, evt =>
                {
                    //this.Info($"{binding.FieldPropertyName}={evt.After}");
                    pfield.SetValue(binding.FieldPropertyName, evt.After);
                });
                pfield.AddPropertyChangeListener(binding.FieldPropertyName, evt =>
                {
                    //UpdateFormFields();
                });
            }
            pfield.Order = orederStr.ToInt();
            pfield.OnInput += (text) => { };
            if (pfield.Type.Contains("collection") && pfield.Type.Contains("<"))
            {
                var a = pfield.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputStructureCollectionAttribute)));
                var b = pfield.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputPrimitiveCollectionAttribute)));
                if(a is null && b is null)
                {
                    string typeName = pfield.Type;
                    typeName = typeName.Substring(typeName.IndexOf("<"));
                    typeName = typeName.Substring(0, typeName.IndexOf(">"));
                    typeName = typeName.Replace("<", "").Replace(">", "");
                    pfield.ValueType = typeName;
                }
                else
                {
                    string typeName = a is not null ? a : b;
                    typeName = typeName.Substring(typeName.IndexOf("<"));
                    typeName = typeName.Substring(0, typeName.IndexOf(">"));
                    typeName = typeName.Replace("<", "").Replace(">", "");
                    pfield.ValueType = typeName;
                }
                

            }
            ApplyCollectionFieldMetadata(pfield, info.PropertyType, attributes);
            ApplyObjectFieldMetadata(pfield, info.PropertyType, attributes);
            ApplyInterfaceCollectionFieldMetadata(pfield, info.PropertyType, attributes);
            ApplyFileDataFieldMetadata(pfield, info.PropertyType, attributes);
            return pfield;
        }
        catch (Exception ex)
        {
            var message = $"Исключение при создании поля формы для свойства {property}, определённого в типе {target.GetType().GetProperties().First(p => p.Name == property).PropertyType.GetTypeName()}:\n{ex.ToTextDocument()}";
            this.Error(message);
            throw new Exception(message);
        }
    }

    //for methods
    public InputFormField CreateField(string name, string type, Dictionary<string, string> attributes, Action<string> onchange)
    {
        try
        {
            //this.Info(property);
            string pname = name;
            InputFormField pfield = new();

            pfield.Name = name;

            pfield.Help = attributes.ContainsKey(nameof(HelpMessageAttribute)) ? attributes[nameof(HelpMessageAttribute)] : "";
            pfield.Description = attributes.ContainsKey(nameof(DescriptionAttribute)) ? attributes[nameof(DescriptionAttribute)] : "";
            pfield.DataType = type;
            pfield.IsCollection = type.ToType().IsCollectionType();
            pfield.Order = 0;
            pfield.Icon = attributes.ContainsKey(nameof(IconAttribute)) ? attributes[nameof(IconAttribute)] : "";
            pfield.Label = attributes.ContainsKey(nameof(LabelAttribute)) ? attributes[nameof(LabelAttribute)] : name;
            pfield.Attributes = attributes;
            pfield.OnInput = text =>
            {

                onchange(text);
                pfield.Errors.Clear();


                pfield.Errors.AddRange(ValidateByAttributes(type, attributes, text));
                pfield.ValidationResults[pfield.Name] = pfield.Errors.ToList();
                pfield.ValidationSuccess = pfield.ValidationResults.All(kv => kv.Value.Count() == 0);



            };
            var propType = type.ToType();
            pfield.Type = GetInputType(propType, attributes);
            pfield.ValueType = GetValueType(propType, name);


            //pfield.CtrlType = target.GetType().GetPropertyControl(property);
            //if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
            //pfield.Control = pfield.CtrlType.CreateControl(pfield);
            var orederStr = attributes.ContainsKey(nameof(InputOrderAttribute)) ? attributes[nameof(InputOrderAttribute)] : "0";

            var bindingValue = attributes.ContainsKey(nameof(InputBindingAttribute)) ? attributes[nameof(InputBindingAttribute)] : null;

            pfield.Order = orederStr.ToInt();
            pfield.OnInput += (text) => { };
            if (pfield.Type.Contains("collection") && pfield.Type.Contains("<"))
            {
                var a = pfield.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputStructureCollectionAttribute)));
                var b = pfield.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputPrimitiveCollectionAttribute)));
                string typeName = a is not null ? a : b;
                typeName = typeName.Substring(typeName.IndexOf("<"));
                typeName = typeName.Substring(0, typeName.IndexOf(">"));
                typeName = typeName.Replace("<", "").Replace(">", "");
                pfield.ValueType = typeName;

            }
            return pfield;
        }
        catch (Exception ex)
        {
            throw new Exception($"Исключение при создании поля формы для параметра {name}:\n{ex.ToTextDocument()}");
        }
    }

    private IEnumerable<string> ValidateByAttributes(string type, Dictionary<string, string> attributes, string text)
    {
        throw new NotImplementedException();
    }

    public InputFormField CreateParameterField(string name, string type, object value, Dictionary<string, string> attributes, ControlAttribute control, Action<string> onchange)
    {
        try
        {

            InputFormField pfield = new();
            pfield.Value = value;
            pfield.Name = name;

            pfield.Help = attributes.ContainsKey(nameof(HelpMessageAttribute)) ? attributes[nameof(HelpMessageAttribute)] : "";

            pfield.Description = attributes.ContainsKey(nameof(DescriptionAttribute)) ? attributes[nameof(DescriptionAttribute)] : "";
            pfield.DataType = type;
            pfield.IsCollection = type.ToType().IsCollectionType();
            pfield.Order = 0;
            pfield.Label = name;
            pfield.Attributes = attributes;
            pfield.OnInput = text =>
            {
                if (onchange is not null)
                    onchange(text);
            };
            pfield.Type = GetInputType(type.ToType(), attributes);
            pfield.ValueType = GetValueType(type.ToType(), name);


            pfield.CtrlType = control;
            if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
                pfield.Control = pfield.CtrlType.CreateControl(pfield);
            var orederStr = attributes.ContainsKey(nameof(InputOrderAttribute)) ? attributes[nameof(InputOrderAttribute)] : "0";

            /*var bindingValue = attributes.ContainsKey(nameof(InputBindingAttribute)) ? attributes[nameof(InputBindingAttribute)] : null;
            if (bindingValue is not null)
            {
                var binding = new InputBindingAttribute(bindingValue);
                if (target is not ChangeSupport)
                    throw new Exception("Атрибут InputBinding можно использовать только в моделях типа ChangeSupport");
                ChangeSupport support = (ChangeSupport)target;
                support.AddPropertyChangeListener(binding.ModelPropertyName, evt =>
                {
                    this.Info($"{binding.FieldPropertyName}={evt.After}");
                    pfield.SetValue(binding.FieldPropertyName, evt.After);
                });
                pfield.AddPropertyChangeListener(binding.FieldPropertyName, evt =>
                {
                    //UpdateFormFields();
                });
            }*/
            pfield.Order = orederStr.ToInt();
            pfield.OnInput += (text) => { };
            if (pfield.Type.Contains("collection") && pfield.Type.Contains("<"))
            {
                var a = pfield.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputStructureCollectionAttribute)));
                var b = pfield.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputPrimitiveCollectionAttribute)));
                string typeName = a is not null ? a : b;
                typeName = typeName.Substring(typeName.IndexOf("<"));
                typeName = typeName.Substring(0, typeName.IndexOf(">"));
                typeName = typeName.Replace("<", "").Replace(">", "");
                pfield.ValueType = typeName;

            }
            return pfield;
        }
        catch (Exception ex)
        {
            throw new Exception($"Исключение при создании поля формы для свойства {name}: {ex.Message}");
        }
    }

    private string GetValueType(Type propertyType, string property)
    {
        //this.Info(propertyType.GetTypeName());
        if (propertyType.GetTypeName().EndsWith("[]"))
        {
            return propertyType.GetTypeName().Substring(0, propertyType.GetTypeName().Length - 2);
        }


        bool isCollection = propertyType.IsCollectionType();
        if (propertyType.GetGenericArguments().Count() == 0)
            return typeof(object).GetTypeName();
        return isCollection ?
            CommonUtils.ParseCollectionType(propertyType) :
            propertyType.GetTypeName();
    }

    public string GetInputType(Type propertyType, Dictionary<string, string> attrs)
    {
        var key = attrs.Keys.FirstOrDefault(key => key.ToType().IsExtends(typeof(BaseInputAttribute)));
        if (key != null)
        {
            var type = key.ToType();
            var attrValue = attrs[key];
            if (string.IsNullOrWhiteSpace(attrValue))
                type.New<BaseInputAttribute>(Array.Empty<object>());
            else
                type.New<BaseInputAttribute>(new object[] { attrValue });
            return key.Replace("Input", "").Replace("Attribute", "").ToLower();
        }

        if (propertyType.IsCollectionType())
        {
            var elementType = CommonUtils.ParseCollectionType(propertyType).ToType();
            //this.Info($"{elementType.GetTypeName()}");
            return elementType.IsPrimitiveForType() ?
                nameof(InputPrimitiveCollectionAttribute).Replace("Input", "").Replace("Attribute", "").ToLower() :
                nameof(InputStructureCollectionAttribute).Replace("Input", "").Replace("Attribute", "").ToLower();
        }
        else
        {
            if (!propertyType.IsPrimitiveForType())
            {
                return "custom";
            }
            else
            {
                switch (propertyType.GetTypeName())
                {
                    case "System.Byte[]": return "upload";
                    case "Byte[]": return "upload";
                    case "String": return "text";
                    case "Boolean": return "checkbox";
                    case "System.String": return "text";
                    case "string": return "text";
                    case "int": return "number";
                    case "long": return "number";
                    case "float": return "number";
                    case "Nullable<System.Boolean>": return "number";
                    case "Double": return "number";
                    case "Nullable<System.Double>": return "number";
                    case "Int16": return "number";
                    case "Nullable<Int16>": return "number";
                    case "Int32": return "number";
                    case "Nullable<System.Int32>": return "number";
                    case "UInt16": return "number";
                    case "UInt32": return "number";
                    case "UInt64": return "number";
                    case "DateTime": return "datetime";
                    case "Nullable<System.DateTime>": return "datetime";
                    case "text": return "text";
                    default: return "text";
                }
            }
        }

    }
    public TextBoxView CreateEngWord(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }

    internal AppViewBase CreateEngText(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }


    internal AppViewBase CreateRusWord(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }
    internal AppViewBase CreateUrl(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }
    internal AppViewBase CreateRusText(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }


    internal AppViewBase CreateEmail(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value),
            val =>
            {
                //textView.Text = $"{val}";
            }
        );
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }
    internal AppViewBase CreatePhone(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }


    internal AppViewBase CreateMultilineText(InputFormField view)
    {
        var textView = new MultiLineTextView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        /*textView.OnLayout += (ViewNode panel) => {
            ((AppViewBase)panel).Height = textView.Height;
        };*/
        return textView;
    }

    internal AppViewBase CreateXml(InputFormField view)
    {
        var skv = view.Attributes.FirstOrDefault(KV => KV.Key.ToType().IsExtends(typeof(InputXmlAttribute)));
        var attr = skv.Key is null
            ? new InputXmlAttribute()
            : string.IsNullOrWhiteSpace(skv.Value)
                ? skv.Key.ToType().New<InputXmlAttribute>()
                : skv.Key.ToType().New<InputXmlAttribute>(skv.Value);
        return attr.CreateView(view);
    }


    internal AppViewBase CreateText(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Parent = view;
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener(nameof(view.Value), evt => { textView.Text = $"{view.Value}"; });
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }


    internal AppViewBase CreatePassword(InputFormField view)
    {
        var textboxView = new InputPasswordView();
        textboxView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        textboxView.Text = $"{view.Value}";
        view.AddPropertyChangeListener(nameof(view.Value), evt =>
        {
            if (textboxView.Text != $"{view.Value}")
            {
                textboxView.Text = $"{view.Value}";
            }
        });
        view.MinHeight = 200;
        return textboxView;
    }

    internal AppViewBase CreateEntityType(InputFormField view)
        => view;

    internal AppViewBase CreateCombobox(InputFormField field)
    {
        var skv = field.Attributes.FirstOrDefault(KV =>
            KV.Key.ToType().IsExtends(typeof(NewComboboxAttribute)));
        if (skv.Key is null)
            throw new Exception($"Поле {field.Name}: не найден {nameof(NewComboboxAttribute)}");

        var attr = string.IsNullOrWhiteSpace(skv.Value)
            ? skv.Key.ToType().New<NewComboboxAttribute>()
            : skv.Key.ToType().New<NewComboboxAttribute>(skv.Value);
        var expression = attr.GetExpression();
        var root = new AppViewBase { AutoSize = false, MinHeight = 56 };
        var row = new AppViewBase { AutoSize = false };

        ComboboxView combobox;
        try
        {
            combobox = BuildCombobox(field, out _, expression);
        }
        catch (Exception ex)
        {
            this.Error($"Ошибка при загрузке справочника: {ex.Message}");
            combobox = new ComboboxView { Text = field.Label };
        }

        row.LayoutFill(combobox);
        row.LayoutRight(row.CreateButton("Создать", () => OnCreateEntity(field, combobox, expression)));
        root.LayoutFill(row);
        return root;
    }
    public Dictionary<int, object> SelectDictionaryData(string entityTypeName)
    {
        Dictionary<int, object> result = new();
        var entityType = entityTypeName.ToType();
        var dbContextType = entityType.GetDbContextWithEntityType();
        using (var db = dbContextType.New<DbContext>())
        {
            foreach (object p in db.GetDbSet(entityTypeName))
            {
                object next = p;
                object id = next.GetValue("Id");
                if (id is null)
                {
                    id = next.GetValue("id");
                }
                result[id.ToString().ToInt()] = next;
            }
        }
        return result;
    }
    public Dictionary<int, object> GetDataDictionary(string expression)
    {
        Type entityType = GetEntityType(expression);
        Dictionary<int, object> dataset = SelectDictionaryData(entityType.GetTypeName());
        return dataset;
    }

    public string GetPropertyName(string expression)
    {
        return expression.Split(",")[1].Replace("\"", "");
    }

    public Type GetEntityType(string expression)
    {
        var entityTypeName = expression.Split(",")[0];
        var entityType = entityTypeName.Replace("\"", "").ToType();
        return entityType;
    }

    private ComboboxView BuildCombobox(InputFormField field, out Dictionary<int, string> options, string expression)
    {
        var combobox = new ComboboxView { Text = field.Label };
        var dataset = GetDataDictionary(expression);
        var propertyBinding = GetPropertyName(expression);
        options = dataset.ToDictionary(
            kv => kv.Key,
            kv => $"{kv.Value?.GetProperty(propertyBinding)}");

        foreach (var text in options.Values)
            combobox.Options.Add(text);

        if (field.Value is not null)
        {
            var current = $"{field.Value}";
            if (options.Values.Contains(current))
                combobox.Value = current;
            else if (int.TryParse(current, out var id) && options.TryGetValue(id, out var text))
                combobox.Value = text;
        }
        var p = options;
        combobox.OnSelected += text =>
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var id = p.FirstOrDefault(kv => kv.Value == text).Key;
            field.Value = id;
            field.Target?.SetValue(field.Name, id);
            field.OnInput?.Invoke(id.ToString());
        };

        return combobox;
    }
    private void OnCreateEntity(InputFormField field, ComboboxView combobox, string expression)
    {
        var entityType = GetEntityType(expression);
        var entityRef = entityType.New();
        ToEditDialog(entityRef, (success, model) =>
        {
            if (!success)
                return;

            if (model is BaseEntity entity)
                entity.Create();

            //RefreshCombobox(field, combobox);
        });
    }

    private void ToEditDialog(object entityRef, Action<bool, object> value)
    {
        throw new NotImplementedException();
    }

    internal AppViewBase CreateSelectDictionary(InputFormField view)
    {
        var skv = view.Attributes.First(KV => KV.Key.ToType().IsExtends(typeof(SelectDictionaryAttribute)));
        var attr = string.IsNullOrWhiteSpace(skv.Value)
            ? skv.Key.ToType().New<SelectDictionaryAttribute>()
            : skv.Key.ToType().New<SelectDictionaryAttribute>(skv.Value);
        return attr.CreateView(view);
    }

    internal AppViewBase CreateSingleSelectApi(InputFormField view)
    {
        var skv = view.Attributes.First(KV => KV.Key.ToType().IsExtends(typeof(SingleSelectApiAttribute)));
        var attr = string.IsNullOrWhiteSpace(skv.Value)
            ? skv.Key.ToType().New<SingleSelectApiAttribute>()
            : skv.Key.ToType().New<SingleSelectApiAttribute>(skv.Value);
        return attr.CreateView(view);
    }
    public AppViewBase CreateSelectEnum(InputFormField view)
    {
        var combobox = new ComboboxView() { Text = view.Label };
        var skv = view.Attributes.First(KV => KV.Key.ToType().IsExtends(typeof(InputSelectEnumAttribute)));
        var attr = skv.Key.ToType().New<InputSelectAttribute>(skv.Value);
        combobox.Options.AddRange(attr.Options);
        combobox.OnSelected += val => view.OnInput(val);
        return combobox;
    }

    internal AppViewBase CreateSelect(InputFormField view)
    {
        var combobox = new ComboboxView() { Text = view.Label };
        var skv = view.Attributes.First(KV => KV.Key.ToType().IsExtends(typeof(InputSelectAttribute)));
        var attr = skv.Key.ToType().New<InputSelectAttribute>(skv.Value);
        combobox.Options.AddRange(attr.Options);
        combobox.OnSelected += val => view.OnInput(val);
        return combobox;
    }

    internal AppViewBase CreateDictionary(InputFormField view)
    {
        var kv = view.Attributes.FirstOrDefault(kv => kv.Key == nameof(InputDictionaryAttribute));
        if (kv.Value is null)
            throw new Exception($"Не удалось найди среди атрибутов {nameof(InputDictionaryAttribute)} для свойства {view.Name}");

        var select = kv.Key.ToType().New<InputDictionaryAttribute>(new object[] { kv.Value });
        var entityType = select.Entity.ToType();

        var root = new AppViewBase { AutoSize = false };
        var typeRow = new AppViewBase { AutoSize = false };
        var editorHost = new AppViewBase { AutoSize = false, Visible = false };
        EditView entityEditor = null;
        ComboboxView combobox = null;

        void ApplyDictionaryHeight()
        {
            ApplyDictionaryPanelHeight(view, root, typeRow, editorHost, entityEditor is not null);
        }

        void OnDictionaryValueSelected(string entityId)
        {
            view.OnInput(entityId);
            if (string.IsNullOrWhiteSpace(entityId))
            {
                editorHost.Visible = false;
                entityEditor?.RemoveFromParent();
                entityEditor = null;
                ApplyDictionaryHeight();
                return;
            }

            try
            {
                var entity = LoadDictionaryEntity(entityType, entityId);
                if (entity is null)
                    return;

                if (entityEditor is null)
                {
                    entityEditor = EditView.CreateEditViewBase(entity, (success, model) =>
                    {
                        if (success && model is not null)
                            view.Target?.SetValue(view.Name, select.Map.FirstOrDefault(p => $"{p.Value}" == $"{entityId}").Value ?? entityId);
                    });
                    //entityEditor.submit.ShowButtons = false;
                    entityEditor.MinHeight = DictionaryEditorHeight;
                    entityEditor.Height = DictionaryEditorHeight;
                    editorHost.Controls.Clear();
                    entityEditor.Dock = "Fill";
                    editorHost.Controls.Add(entityEditor);
                }
                else
                {
                    entityEditor.BindEditControls(entity);
                }

                editorHost.Visible = true;
                ApplyDictionaryHeight();
            }
            catch (Exception ex)
            {
                view.Error($"Не удалось открыть редактор для {view.Name}: {ex.Message}");
            }
        }

        ButtonView createButton = null;
        ComboboxView BuildCombobox(Dictionary<string, string> map)
            => typeRow.CreateComboboxForDictionary($"{view.Label}", map, OnDictionaryValueSelected);

        combobox = BuildCombobox(select.Map);
        createButton = typeRow.CreateButton("Создать", () =>
        {
            var pentity = entityType.New();
            var edit = EditView.CreateEditViewBase(pentity, (success, _) =>
            {
                if (!success)
                    return;

                var refreshed = kv.Key.ToType().New<InputDictionaryAttribute>(new object[] { kv.Value });
                combobox = BuildCombobox(refreshed.Map);
                LayoutDictionaryTypeRow(typeRow, combobox, createButton);
                ApplyDictionaryHeight();
            });
            edit.ShowDialog();
        });
        createButton.Width = 120;

        LayoutDictionaryTypeRow(typeRow, combobox, createButton);
        root.Controls.Clear();
        typeRow.Dock = "Top";
        editorHost.Dock = "Fill";
        root.Controls.Add(typeRow);
        root.Controls.Add(editorHost);
        ApplyDictionaryHeight();
        return root;
    }

    private const int DictionaryFieldRowHeight = 51;
    private const int DictionaryEditorHeight = 280;
    private const int DictionaryFieldChromeHeight = 52;

    private static void LayoutDictionaryTypeRow(AppViewBase typeRow, ComboboxView combobox, ButtonView createButton)
    {
        ApplyDictionaryFieldRowLayout(combobox);
        ApplyDictionaryFieldRowLayout(createButton);
        typeRow.Controls.Clear();
        createButton.Dock = "Right";
        combobox.Dock = "Fill";
        typeRow.Controls.Add(createButton);
        typeRow.Controls.Add(combobox);
        typeRow.MinHeight = DictionaryFieldRowHeight;
        typeRow.Height = DictionaryFieldRowHeight;
    }

    private static void ApplyDictionaryFieldRowLayout(ViewItem item)
    {
        item.AutoSize = false;
        item.MinHeight = DictionaryFieldRowHeight;
        item.Height = DictionaryFieldRowHeight;
    }

    private static void ApplyDictionaryPanelHeight(
        InputFormField view,
        AppViewBase root,
        AppViewBase typeRow,
        AppViewBase editorHost,
        bool editorVisible)
    {
        typeRow.MinHeight = typeRow.Height = DictionaryFieldRowHeight;
        if (editorVisible)
        {
            editorHost.MinHeight = editorHost.Height = DictionaryEditorHeight;
            root.MinHeight = root.Height = DictionaryFieldRowHeight + DictionaryEditorHeight;
        }
        else
        {
            editorHost.MinHeight = editorHost.Height = 0;
            root.MinHeight = root.Height = DictionaryFieldRowHeight;
        }

        view.AutoSize = false;
        view.MinHeight = view.Height = root.Height + DictionaryFieldChromeHeight;
        SyncInputFormFieldControlHeight(view);
    }

    private static object LoadDictionaryEntity(Type entityType, string entityId)
    {
        var facade = entityType.GetEntityFasadeForType();
        var idProperty = entityType.GetProperty("Id");
        if (idProperty is null)
            return facade.GetAll().Cast<object>().FirstOrDefault();

        var idType = Nullable.GetUnderlyingType(idProperty.PropertyType) ?? idProperty.PropertyType;
        var id = Convert.ChangeType(entityId, idType);
        return facade.Get(id);
    }

    private static void SyncInputFormFieldControlHeight(InputFormField view)
    {
        view.RunOnUiThread(() =>
        {
            //if (view.control is InputFormFieldControl fieldControl)
            //    fieldControl.SyncContentHeight(view);
        });
    }

    internal AppViewBase CreateBool(InputFormField view)
    {
        var p = new CheckboxView();
        p.Text = view.Label;
        p.Checked = view.Value is null ? false :
            view.Value is bool ? (bool)view.Value :
            throw new Exception("Тип данных не поддерживается");
        p.OnCheckedChanged = (s, e) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput($"{e.After}");
            }

        };
        return p;

    }

    public AppViewBase CreateDecimal(InputFormField view)
    {
        var textView = new InputDecimalView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }

    internal AppViewBase CreateNumber(InputFormField view)
    {
        var textView = new InputNumberView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }

    /// <summary>
    /// Поля с <see cref="InputPositiveIntAttribute"/> / <see cref="InputPositiveAttribute"/> (типы <c>positiveint</c>, <c>positive</c> в форме).
    /// Ввод как у числа; проверка — атрибутом на модели (<see cref="InputPositiveIntAttribute.Validate"/>).
    /// </summary>
    internal AppViewBase CreatePositiveInt(InputFormField view) => CreateNumber(view);

    internal AppViewBase CreateNatural(InputFormField view)
    {
        var textView = new InputNaturalView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }

    internal AppViewBase CreateMoney(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }

    internal AppViewBase CreatePercent(InputFormField view)
    {
        var textView = new InputPercentView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            this.Info(text);
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }


        };
        return textView;
    }

    internal AppViewBase CreateInt(InputFormField view)
    {
        var textView = new TextBoxView();
        textView.Text = $"{view.Value}";
        view.AddPropertyChangeListener<object>(nameof(view.Value), val => textView.Text = $"{val}");
        textView.OnTextChanged += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
            }
        };
        return textView;
    }

    internal AppViewBase CreateIcon(InputFormField view)
    {

        var comboboxView = new ComboboxView();
        comboboxView.Options.AddRange(GetIconNames());
        comboboxView.Value = $"{view.Value}";
        comboboxView.OnSelected += (text) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(text);
                view.Target.SetValue(view.Name, text);
            }
        };
        comboboxView.MinHeight = 100;
        view.MinHeight = 300;
        return comboboxView;
    }

    private IEnumerable<string> GetIconNames()
    {
        return new IconsManager().GetIconNames();
    }

    internal AppViewBase CreateImage(InputFormField view)
    {
        var pane = new AppViewBase();
        if (view.Value is null)
        {
            ImageView image = new() { Data = File.ReadAllBytes("C:\\Projects\\CursorApp\\Icons\\folder.png"), KeepAspectRatio = true };
            ButtonView button = null;
            AppViewBase last = button;
            Action todo = null;
            todo = () =>
            {
                OpenFile("", "", "", file =>
                {
                    var data = File.ReadAllBytes(file);
                    view.Value = data;
                    image.Data = data;
                    view.Target.SetValue(view.Name, image.Data);
                    last = view.CreateImage((byte[])view.Value);
                    last.OnClick = (s, e) => { todo(); };
                    pane.LayoutFill(last);
                });
            };
            button = pane.CreateButton("Загрузить", todo);
            pane.LayoutFill(button);
            pane.MinHeight = 200;
        }
        else
        {
            pane.LayoutFill(view.CreateImage((byte[])view.Value));
        }
        pane.MinHeight = 200;
        return pane;


    }

    private void OpenFile(string v1, string v2, string v3, Action<string> value)
    {
        this.Warn("OpenFile not implemented");
    }

    public AppViewBase CreateColor(InputFormField view)
    {
        var p = new InputColorView();
        p.Text = view.Label;
        p.OnColorChanged = (color) =>
        {
            if (view.OnInput is not null)
            {
                view.OnInput(color);
            }

        };
        return p;
    }

    public AppViewBase CreateFilePath(InputFormField view)
    {
        var input = new InputFilePathView();
        if (!string.IsNullOrWhiteSpace(view.Label))
            input.Label = view.Label;
        if (view.Value is not null)
            input.Path = $"{view.Value}";

        var attrKv = view.Attributes?.FirstOrDefault(kv =>
        {
            try { return kv.Key.ToType().IsExtends(typeof(InputFilePathAttribute)); }
            catch { return false; }
        }) ?? default;
        if (!string.IsNullOrWhiteSpace(attrKv.Value))
            input.Filter = InputFilePathView.BuildOpenFileFilter(attrKv.Value);

        input.OnPathChanged = path =>
        {
            view.Value = path;
            view.Target?.SetValue(view.Name, path);
            view.OnInput?.Invoke(path ?? "");
        };
        return input;
    }

    public AppViewBase CreateTcpIpAddress(InputFormField view)
    {
        var input = new InputTcpIpAddressView();
        if (!string.IsNullOrWhiteSpace(view.Label))
            input.Label = view.Label;
        if (view.Value is not null)
            input.Address = $"{view.Value}";

        var attrKv = view.Attributes?.FirstOrDefault(kv =>
        {
            try { return kv.Key.ToType().IsExtends(typeof(InputTcpIpAddressAttribute)); }
            catch { return false; }
        }) ?? default;
        if (!string.IsNullOrWhiteSpace(attrKv.Value))
            input.Mask = attrKv.Value;

        input.OnAddressChanged = address =>
        {
            view.Value = address;
            view.Target?.SetValue(view.Name, address);
            view.OnInput?.Invoke(address ?? "");
        };
        return input;
    }

    public AppViewBase CreateCreditCard(InputFormField view)
    {
        var input = new InputCreditCardView();
        if (!string.IsNullOrWhiteSpace(view.Label))
            input.Label = view.Label;
        if (view.Value is not null)
            input.Number = InputCreditCardAttribute.FormatCardNumber($"{view.Value}");

        input.OnNumberChanged = number =>
        {
            view.Value = number;
            view.Target?.SetValue(view.Name, number);
            view.OnInput?.Invoke(number ?? "");
        };
        return input;
    }

    public AppViewBase CreateAudio(InputFormField view)
    {
        var a = new AudioRecorderView { Text = view.Label };
        a.MinHeight = Math.Max(a.MinHeight, 96);
        if (view.Value is not null)
            a.OutputPath = $"{view.Value}";
        a.AddPropertyChangeListener(nameof(AudioRecorderView.OutputPath), evt =>
        {
            if (view.OnInput is not null)
                view.OnInput($"{evt.After}");
        });
        return a;
    }

    public AppViewBase CreateVideo(InputFormField view)
    {
        var v = new VideoView { Text = view.Label };
        if (view.Value is not null)
            v.Source = $"{view.Value}";
        v.AddPropertyChangeListener(nameof(VideoView.Source), evt =>
        {
            if (view.OnInput is not null)
                view.OnInput($"{evt.After}");
        });
        return v;
    }

    public AppViewBase CreateCamera(InputFormField view)
    {
        var c = new CameraView { Text = view.Label };
        if (view.Value is not null)
            c.DeviceId = $"{view.Value}";
        c.AddPropertyChangeListener(nameof(CameraView.DeviceId), evt =>
        {
            if (view.OnInput is not null)
                view.OnInput($"{evt.After}");
        });
        return c;
    }

    public AppViewBase CreateSignalRVideo(InputFormField view)
    {
        var sv = new SignalRVideoView { Text = view.Label };
        if (view.Value is not null)
            sv.HubUrl = $"{view.Value}";
        sv.AddPropertyChangeListener(nameof(SignalRVideoView.HubUrl), evt =>
        {
            if (view.OnInput is not null)
                view.OnInput($"{evt.After}");
        });
        sv.AddPropertyChangeListener(nameof(SignalRVideoView.StreamId), evt =>
        {
            if (view.OnInput is not null)
                view.OnInput($"{evt.After}");
        });
        return sv;
    }


    internal AppViewBase CreateSelectExpression(InputFormField field)
    {

        var ctrl = typeof(InputSelectExpressionAttribute).New<InputSelectExpressionAttribute>(new object[] { field.Attributes.First(kv => kv.Key == nameof(InputSelectExpressionAttribute)).Value });
        var value = ctrl.GetOptions(field.Target);
        var selectOptions = value is ChangingList<string> ? value : new ChangingList<string>();
        if (!(value is ChangingList<string>))
        {
            foreach (var item in (dynamic)value)
            {
                selectOptions.Add($"{item}");
            }
        }
        var result = field.CreateCombobox($"{field.Value}", selectOptions, field.OnInput);

        return result;
    }




    internal AppViewBase CreateDate(InputFormField view)
    {
        var input = new InputDateTimeView();
        input.Value = view.Value is null ? DateTime.Now : (DateTime)view.Value;
        input.OnValueChanged += (dt) => { if (dt is not null) view.OnInput(((DateTime)dt).ToString("yyyy-MM-dd")); };
        return input;
    }

    internal AppViewBase CreateDateTime(InputFormField view)
    {

        var combobox = new ComboboxView() { Text = view.Label };
        combobox.Options.AddRange(Enumerable.Range(2100, 1900).Select(p => p.ToString()));
        combobox.OnSelected += val => view.OnInput(val);
        return combobox;
    }



    public AppViewBase CreatePrimitiveCollection(InputFormField view)
    {
        var result = new CollectionsView();
        try
        {
            result.Type = view.ValueType;
            result.Attributes = view.Attributes;
            if (view.Value is not null)
                foreach (var value in (dynamic)view.Value)
                {
                    result.values.Add($"{value}");
                }
            if (view.Target is EventSupport es)
            {
                es.AddPropertyChangeListener(view.Name, evt =>
                {
                    result.values.Clear();
                    if (evt.After is not null)
                    {
                        foreach (var value in (dynamic)evt.After)
                        {
                            result.values.Add(value);
                        }
                    }
                });
            }
            result.OnAdded += (i, p) =>
            {
                try
                {
                    var ptarget = TextDataSetter.FromText(p, view.ValueType);
                    view.Value.Invoke("Add", new object[] { ptarget });
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при обработки события OnAdded() => {ex.Message}");
                }
            };
            result.OnRemoved += (i, p) =>
            {
                try
                {
                    var ptarget = TextDataSetter.FromText(p, view.ValueType);
                    view.Value.Invoke("Remove", new object[] { ptarget });
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при обработки события OnRemoved() => {ex.Message}");
                }

            };
            result.OnValueChanged += (i, p) =>
            {
                try
                {
                    view.Value.Invoke("RemoveAt", new object[] { i });
                    var pvalue = TextDataSetter.FromText(p, view.ValueType);
                    view.Value.Invoke("Insert", new object[] { i, pvalue });
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при передачи значения {p} в коллекцию: {ex.Message}");
                }
            };
        }
        catch (Exception ex)
        {
            result.LayoutFill(result.CreateText($"Исключение при создании primitivecollection => {ex.Message}"));
        }
        return result;
    }

    public AppViewBase CreateStructuredCollection(InputFormField view)
    {
        var result = new InputStructuredCollectionView();

        result.Label = view.Label;
        result.Icon = view.Label;
        EnsureStructuredCollectionValue(view);
        result.ItemType = view.ValueType;
        if (!string.IsNullOrWhiteSpace(view.ValueType))
        {
            try { new EditView().WarmFieldCache(view.ValueType.ToType()); }
            catch { /* ignored */ }
        }
        if (view.Value is null)
        {
            view.Value = view.DataType.ToType().IsCollectionType()
                ? view.DataType.ToType().New()
                : null;
        }
        if (view.Value is not null)
        {
            foreach (var item in (dynamic)view.Value)
            {
                result.Items.Add(item);
                result.tableView.DataRows.Add(item);
            }
        }

        //result.tableView.SetDataRows(
        //    view.ValueType.ToType().GetOwnInputSimplePropertyNames(),
        //    result.tableView.DataRows);
        result.OnItemAdded += (o, e) =>
        {
            try
            {
                view.Value.Call("Add", new object[] { e });
                view.OnInput(view.Value.ToJson());
            }
            catch (Exception ex)
            {
                result.LayoutFill(new TextView() { Text = $"Исключение при добавлении элемента {ex.Message}" });
            }
        };
        result.OnItemRemoved += (o, e) =>
        {
            try
            {
                view.Value.Call("Remove", new object[] { e });
                view.OnInput(view.Value.ToJson());
            }
            catch (Exception ex)
            {
                result.LayoutFill(new TextView() { Text = $"Исключение при добавлении элемента {ex.Message}" });
            }
        };

        ApplyCollectionPanelHeight(view, result, result.tableView.DataRows.Count());
        result.tableView.DataRows.OnChanged += (_, __) =>
        {
            ApplyCollectionPanelHeight(view, result, result.tableView.DataRows.Count());
            //if (view.control is InputFormFieldControl fieldControl)
            //fieldControl.SyncContentHeight(view);
        };
        return result;
    }

    public AppViewBase CreateInterfaceCollection(InputFormField view)
    {
        var (expression, options, interfaceType) = ResolveInterfaceCollectionAttribute(view);
        EnsureInterfaceCollectionValue(view, interfaceType);

        var result = new InputInterfaceCollectionView
        {
            Expression = expression,
            TypeOptions = options
        };

        if (view.Value is not null)
        {
            foreach (var item in (dynamic)view.Value)
            {
                result.Items.Add(item);
                result.tableView.DataRows.Add(item);
            }
        }

        result.OnItemAdded += (_, item) =>
        {
            try
            {
                view.Value.Call("Add", new object[] { item });
                view.OnInput(view.Value.ToJson());
            }
            catch (Exception ex)
            {
                result.LayoutFill(new TextView() { Text = $"Исключение при добавлении элемента: {ex.Message}" });
            }
        };
        result.OnItemRemoved += (_, item) =>
        {
            try
            {
                view.Value.Call("Remove", new object[] { item });
                view.OnInput(view.Value.ToJson());
            }
            catch (Exception ex)
            {
                result.LayoutFill(new TextView() { Text = $"Исключение при удалении элемента: {ex.Message}" });
            }
        };

        ApplyCollectionPanelHeight(view, result, result.tableView.DataRows.Count());
        result.tableView.DataRows.OnChanged += (_, __) =>
        {
            ApplyCollectionPanelHeight(view, result, result.tableView.DataRows.Count());
            //if (view.control is InputFormFieldControl fieldControl)
            //fieldControl.SyncContentHeight(view);
        };
        return result;
    }

    /// <summary>
    /// Высота редактора коллекции: панель инструментов + таблица с учётом числа строк.
    /// </summary>
    protected static void ApplyStructuredCollectionSizing(InputFormField field, InputStructuredCollectionView collectionView)
        => ApplyCollectionPanelHeight(field, collectionView, collectionView.tableView.DataRows.Count());

    private static void ApplyCollectionPanelHeight(InputFormField field, AppViewBase collectionView, int rowCount)
    {
        const int toolsHeight = 44;
        const int tableHeaderHeight = 32;
        const int rowHeight = 26;
        const int minTableHeight = 120;
        const int labelAndErrors = 52;

        rowCount = Math.Max(rowCount, 1);
        var tableHeight = Math.Max(minTableHeight, tableHeaderHeight + rowCount * rowHeight);
        var contentHeight = toolsHeight + tableHeight + 8;

        collectionView.AutoSize = false;
        collectionView.MinHeight = contentHeight;
        collectionView.Height = contentHeight;

        var fieldHeight = contentHeight + labelAndErrors;
        field.AutoSize = false;
        field.MinHeight = fieldHeight;
        field.Height = fieldHeight;
    }

    private static (string expression, List<string> options, Type interfaceType) ResolveInterfaceCollectionAttribute(InputFormField view)
    {
        var attrKey = view.Attributes.Keys.FirstOrDefault(k =>
            k.ToType().Name.StartsWith(nameof(InputInterfaceCollectionAttribute), StringComparison.Ordinal));
        if (attrKey is null)
            return ("ToString()", new List<string>(), typeof(object));

        var attrType = attrKey.ToType();
        view.Attributes.TryGetValue(attrKey, out var attrValue);
        object attr = string.IsNullOrWhiteSpace(attrValue)
            ? Activator.CreateInstance(attrType)!
            : Activator.CreateInstance(attrType, attrValue)!;

        var options = (List<string>)attrType.GetMethod("GetOptions")!.Invoke(attr, null)!;
        var expression = attrType.GetProperty("Expression")!.GetValue(attr) as string ?? "ToString()";
        var interfaceType = attrType.IsGenericType
            ? attrType.GetGenericArguments()[0]
            : typeof(object);
        return (expression, options, interfaceType);
    }

    private static void EnsureInterfaceCollectionValue(InputFormField view, Type interfaceType)
    {
        if (view.Value is System.Collections.IEnumerable && view.Value is not string)
            return;

        if (view.DataType.ToType().IsCollectionType())
            view.Value = view.DataType.ToType().New();
        else
            view.Value = typeof(List<>).MakeGenericType(interfaceType).New();
    }

    internal AppViewBase CreateUpload(InputFormField view)
    {
        var skv = view.Attributes.FirstOrDefault(KV =>
        {
            var attrType = KV.Key.ToType();
            if (attrType.IsExtends(typeof(InputFileDataAttribute)))
                return true;
            if (attrType.IsExtends(typeof(InputImageAttribute)))
                return false;
            return attrType.IsExtends(typeof(InputFileAttribute));
        });

        if (skv.Key is not null)
        {
            var attrType = skv.Key.ToType();
            object attr = string.IsNullOrWhiteSpace(skv.Value)
                ? Activator.CreateInstance(attrType)!
                : Activator.CreateInstance(attrType, skv.Value)!;
            return ((ControlAttribute)attr).CreateView(view);
        }

        return new InputFileAttribute().CreateView(view);
    }

    internal AppViewBase CreateObject(InputFormField view)
    {
        var skv = view.Attributes.FirstOrDefault(KV => KV.Key.ToType().IsExtends(typeof(InputObjectAttribute)));
        var attr = skv.Key is null
            ? new InputObjectAttribute()
            : string.IsNullOrWhiteSpace(skv.Value)
                ? new InputObjectAttribute()
                : new InputObjectAttribute(skv.Value);
        return attr.CreateView(view);
    }

    private static void ApplyCollectionFieldMetadata(
        InputFormField field,
        Type propertyType,
        Dictionary<string, string> attributes)
    {
        if (!field.Type.Contains("collection", StringComparison.OrdinalIgnoreCase))
            return;

        if (propertyType.IsCollectionType())
        {
            field.IsCollection = true;
            var typeName = propertyType.GetTypeName();
            if (!typeName.Contains("<"))
            {
                typeName = propertyType.GetExtendedTypes().FirstOrDefault(p => p.GetTypeName().Contains("<")).GetTypeName();
            }
            field.ValueType = CommonUtils.ParseCollectionType(typeName.ToType());
            field.Value ??= propertyType.New();
            return;
        }

        var structureAttr = attributes.Keys.FirstOrDefault(key =>
            key.ToType().IsExtends(typeof(InputStructureCollectionAttribute)));
        var primitiveAttr = attributes.Keys.FirstOrDefault(key =>
            key.ToType().IsExtends(typeof(InputPrimitiveCollectionAttribute)));
        var attrKey = structureAttr ?? primitiveAttr;
        if (attrKey is null)
            return;

        var itemTypeName = attributes.TryGetValue(attrKey, out var arg) && !string.IsNullOrWhiteSpace(arg)
            ? arg.Split(',')[0].Trim().Replace("\"", "")
            : typeof(string).GetTypeName();

        field.IsCollection = true;
        field.ValueType = itemTypeName;
        field.Value ??= typeof(List<>).MakeGenericType(itemTypeName.ToType()).New();
    }

    private static void EnsureStructuredCollectionValue(InputFormField view)
    {
        if (view.Value is System.Collections.IEnumerable && view.Value is not string)
            return;

        if (!string.IsNullOrWhiteSpace(view.ValueType))
        {
            view.Value = typeof(List<>).MakeGenericType(view.ValueType.ToType()).New();
            return;
        }

        if (view.DataType.ToType().IsCollectionType())
            view.Value = view.DataType.ToType().New();
    }

    private static void ApplyObjectFieldMetadata(
        InputFormField field,
        Type propertyType,
        Dictionary<string, string> attributes)
    {
        if (!field.Type.Equals("object", StringComparison.OrdinalIgnoreCase))
            return;

        var attrKey = attributes.Keys.FirstOrDefault(key =>
            key.ToType().IsExtends(typeof(InputObjectAttribute)));
        if (attrKey is null)
            return;

        if (attributes.TryGetValue(attrKey, out var typeName) && !string.IsNullOrWhiteSpace(typeName))
            field.ValueType = typeName.Trim();

        if (field.Value is not null)
            return;

        if (!string.IsNullOrWhiteSpace(field.ValueType))
            field.Value = field.ValueType.ToType().New();
        else if (propertyType != typeof(object))
            field.Value = propertyType.New();
    }

    private static void ApplyInterfaceCollectionFieldMetadata(
        InputFormField field,
        Type propertyType,
        Dictionary<string, string> attributes)
    {
        if (!field.Type.Contains("interfacecollection", StringComparison.OrdinalIgnoreCase))
            return;

        var attrKey = attributes.Keys.FirstOrDefault(k =>
            k.ToType().Name.StartsWith(nameof(InputInterfaceCollectionAttribute), StringComparison.Ordinal));
        if (attrKey is null)
            return;

        var attrType = attrKey.ToType();
        if (attrType.IsGenericType)
        {
            var itemType = attrType.GetGenericArguments()[0];
            field.IsCollection = true;
            field.ValueType = itemType.GetTypeName();
            if (field.Value is null)
                field.Value = propertyType.IsCollectionType()
                    ? propertyType.New()
                    : typeof(List<>).MakeGenericType(itemType).New();
        }
    }

    private static void ApplyFileDataFieldMetadata(
        InputFormField field,
        Type propertyType,
        Dictionary<string, string> attributes)
    {
        if (!field.Type.Equals("filedata", StringComparison.OrdinalIgnoreCase)
            && !field.Type.Equals("file", StringComparison.OrdinalIgnoreCase)
            && !field.Type.Equals("upload", StringComparison.OrdinalIgnoreCase))
            return;

        field.DataType = typeof(byte[]).GetTypeName();
        if (field.Value is byte[])
            return;

        field.Value = Array.Empty<byte>();
    }
}