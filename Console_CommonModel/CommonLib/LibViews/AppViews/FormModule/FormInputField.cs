using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.FormModule
{
    /// <summary>
    /// Поле формы, содержит элементы управления для связанных объектов
    /// </summary>
    [Label("Поле формы")]
    [Description("Представление поля формы ввода, содержащее метку, элемент управления для ввода данных, обработчик изменений и отображение ошибок валидации")]
    public class FormInputField : InputFormField
    {
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Надпись")]

        public TextView label;

        [Label("Ошибки валидации")]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public TextView errors;

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public ViewItem input;

        public FormInputField() { }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">наименование поля</param>
        /// <param name="ptype">тип данных</param>
        /// <param name="attrs">атрибуты</param>
        public FormInputField(string name, Type ptype, Dictionary<string, string> attrs)
        {
            FormInputField pfield = this;
            pfield.Name = name;
            pfield.Help = attrs.ContainsKey(nameof(HelpMessageAttribute)) ? attrs[nameof(HelpMessageAttribute)] : null;

            pfield.Description = attrs.ContainsKey(nameof(DescriptionAttribute)) ? attrs[nameof(DescriptionAttribute)] : null;
            pfield.IsCollection = ptype.IsCollectionType();
            pfield.Order = 0;
            pfield.Icon = attrs.ContainsKey(nameof(IconAttribute)) ? attrs[nameof(IconAttribute)] : null;
            pfield.Label = attrs.ContainsKey(nameof(LabelAttribute)) ? attrs[nameof(LabelAttribute)] : null;
            pfield.Attributes = attrs;
            pfield.OnInput = text =>
            {

            };
            var attribute = attrs.Keys.FirstOrDefault(key => key.ToType().IsExtends(typeof(BaseInputAttribute)));
            if (string.IsNullOrWhiteSpace(attribute))
            {
                var propertyType = ptype;
                if (propertyType.IsCollectionType())
                {
                    var elementType = propertyType.GetElementType();
                    pfield.Type = elementType.IsPrimitiveForType() ?
                        nameof(InputPrimitiveCollectionAttribute).Replace("Input", "").Replace("Attribute", "").ToLower() :
                        nameof(InputStructureCollectionAttribute).Replace("Input", "").Replace("Attribute", "").ToLower();
                }
                else
                {
                    if (!propertyType.IsPrimitiveForType())
                    {
                        pfield.Type = "custom";
                    }
                    else
                    {
                        switch (propertyType.GetTypeName())
                        {
                            case "System.Byte[]": pfield.Type = "upload"; break;
                            case "Byte[]": pfield.Type = "upload"; break;
                            case "String": pfield.Type = "text"; break;
                            case "Boolean": pfield.Type = "checkbox"; break;
                            case "System.String": pfield.Type = "text"; break;
                            case "string": pfield.Type = "text"; break;
                            case "int": pfield.Type = "number"; break;
                            case "long": pfield.Type = "number"; break;
                            case "float": pfield.Type = "number"; break;
                            case "Nullable<System.Boolean>": pfield.Type = "number"; break;
                            case "Double": pfield.Type = "number"; break;
                            case "Nullable<System.Double>": pfield.Type = "number"; break;
                            case "Int16": pfield.Type = "number"; break;
                            case "Nullable<Int16>": pfield.Type = "number"; break;
                            case "Int32": pfield.Type = "number"; break;
                            case "Nullable<System.Int32>": pfield.Type = "number"; break;
                            case "UInt16": pfield.Type = "number"; break;
                            case "UInt32": pfield.Type = "number"; break;
                            case "UInt64": pfield.Type = "number"; break;
                            case "DateTime": pfield.Type = "datetime"; break;
                            case "Nullable<System.DateTime>": pfield.Type = "datetime"; break;
                            case "text": pfield.Type = "text"; break;
                            default: pfield.Type = "text"; break;
                        }
                    }
                }
            }
            else
            {
                pfield.Type = attribute;
            }

            pfield.ValueType = ptype.IsCollectionType() ?
                CommonUtils.ParseCollectionType(ptype) :
                ptype.GetTypeName();
            pfield.Value = null;

            try
            {
                string controlType = attrs.Keys.FirstOrDefault(key => key.ToType().IsExtends(typeof(ControlAttribute)));
                if (!string.IsNullOrWhiteSpace(controlType))
                {
                    pfield.CtrlType = null;
                }

                if (controlType is not null)
                {
                    pfield.CtrlType = controlType.New<ControlAttribute>(new object[] { attrs[controlType] });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Свойство типа {ptype.GetTypeName()}  с именем  исключено для получения элемента управления: " + ex.Message);
            }

            //if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
            //    p.Control = pfield.CtrlType.CreateControl(p);

            pfield.Order = attrs.ContainsKey(nameof(InputOrderAttribute)) ? attrs[nameof(InputOrderAttribute)].ToInt() : 0;
            pfield.OnInput += (text) => { };
            if (pfield.IsCollection && CommonUtils.IsPrimitive(pfield.ValueType.ToType()))
            {
                pfield.Type = "primitivecollection";
            }


        }


        /// <summary>
        /// Создание атрибута типа
        /// </summary>    
        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            var kv = Attributes.Keys.FirstOrDefault(kv => kv.ToType().IsExtends(typeof(TAttribute)));
            if (kv is null)
                throw new ArgumentException($"Не удалось найти атрибут типа {typeof(TAttribute).GetTypeName()}");
            return kv.ToType().New<TAttribute>(new object[] { Attributes[kv] });
        }



        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Ctrl Type")]
        public ControlAttribute CtrlType { get; internal set; }

        public IEnumerable<string> GetSelectOptions()
        {
            var kv = Attributes.FirstOrDefault(kv => kv.Key == nameof(InputSelectAttribute));
            if (kv.Value is null)
            {
                throw new Exception($"Не удалось найди среди атрибутов {nameof(InputSelectAttribute)} для свойства {Name}");
            }
            var select = kv.Key.ToType().New<InputSelectAttribute>(new object[] { kv.Value });
            return select.Options;
        }

        public void SetErrors(TextView errors)
        {
            if (this.errors is not null)
            {
                return;
            }
            this.errors = errors;
        }
        public void SetErrors(string text)
        {
            if (errors is not null)
            {
                errors.Text = text;
            }

        }
    }
}
