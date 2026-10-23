
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;

using Org.BouncyCastle.Asn1.X509;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.FormModule
{
    [Label("Абстракция ввода")]
    public abstract class AbstractInputView<TFormInputField> : AppLayoutView where TFormInputField : InputFormField

    {
        [Label("Error Message")]
        public string ErrorMessage { get; set; }
        public object target;
        public List<string> _properties = new();

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<TFormInputField> fields;


        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private Type ptype;

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<string> properties
        {
            get => _properties;
            set
            {
                // Оптимизация: избегаем лишнего ToList() если value уже List
                _properties = value is List<string> list ? list : value is null ? null : value.ToList();

            }
        }

        //public override Control ToControl()
        //{
        //    return base.ToControl<LayoutView>();
        //}

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<string, string> OnInput = (k, v) =>
        {
            Console.WriteLine($"{k}={v}");
        };

        protected AbstractInputView() : this(new UserAccount(), null) { }

        public AbstractInputView(object p, List<string> properties)
        {
            IsVertical = true;
            target = p is Type ? ((Type)p).New() : ptype;
            ptype = p is Type ? (Type)p : p.GetType();
            this.properties = properties is null || properties.Count() == 0 ?
                ptype.GetInputProperties() :
                properties;
            InitFormFields();
            var errors = target.Validate();
            var valid = !errors.Any(kv => kv.Value.Any(err => !string.IsNullOrWhiteSpace(err)));
            UpdateErrorMessages(errors);
        }

        public void UpdateFormFields(Dictionary<string, TFormInputField> fields)
        {
            this.fields = fields.Values.ToList();
            UpdateFormFields();
        }

        public void InfoTime(string text, Action todo)
        {
            var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            todo();
            var ended = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.Info($"{text}: {ended - started} мс.");

        }
        public void UpdateFormFields()
        {
            InfoTime("Обработка свойств объекта", () =>
            {

                // Оптимизация: используем for вместо foreach для List (немного быстрее)
                var fieldsCount = fields.Count;
                for (int i = 0; i < fieldsCount; i++)
                {

                    var field = fields[i];
                    if (field.Name == "Item")
                    {
                        int x = 0;
                        this.Info(x);
                    }
                    try
                    {

                        this.Info(field.Label);
                        AddFormField(field);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Исключение при добавлении свойства: {field.Name} для типа {target.GetType().GetTypeName()}: {ex.Message}\n {ex.StackTrace}");
                    }
                }

            });
        }
        public void Update(string key, string value)
        {
            try
            {
                TextDataSetter.SetValue(target, key, value);
                var errorsMessages = target.Validate(key);

                // Оптимизация: используем StringBuilder для конкатенации строк
                var messageBuilder = new System.Text.StringBuilder();
                foreach (string error in errorsMessages)
                {
                    messageBuilder.Append(error).Append("; ");
                }
                var field = fields.FirstOrDefault(f => f.Name == key);
                if (field != null)
                {
                    field.SetErrors(messageBuilder.ToString());
                }
                var errors = target.Validate();
                this.Debug(errors);

                var valid = !errors.Any(kv => kv.Value.Any(err => !string.IsNullOrWhiteSpace(err)));
                //this.Info(errors);
                //ok.Enabled = valid;
                UpdateErrorMessages(errors);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }
        public void OnError(Exception ex)
        {
            this.Error(ex);
            //var dialog = new DialogService();
            //var factory = new global::ControlsFactory();
            //var text = ex.ToTextDocument();
            //var ctrl = factory.CreateMultilineTextBox(text, changed => { });
            //var form = dialog.CreateSubmitDialog("Перехвачено исключение", ctrl, submited => {
            //});
            //form.ShowDialog();
        }

        public abstract void AddFormField(TFormInputField field);

        private void InitFormFields()
        {
            fields = GetFormFields(ptype, properties).ToList();
            foreach (var field in fields)
                field.Target = target;
            UpdateFormFields();

            OnInput += (k, v) =>
            {
                Update(k, v);
            };
        }

        public IEnumerable<TFormInputField> GetFormFields(Type ptype, IEnumerable<string> properties = null)
        {
            if (properties is null)
                properties = ptype.GetInputPropertiesSorted();
            var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var key = ptype.GetTypeName();


            var ctrl = this;
            TFormInputField pfield = null;
            fields = new List<TFormInputField>();
            var tasks = new List<Task>();
            foreach (var property in properties)
            {
                //tasks.Add(Task.Run(() => {
                try
                {
                    this.Info(property);
                    string pname = property;
                    if (pname == "Item")
                    {
                        this.Info(1);
                    }
                    pfield = GetFormField(ptype, property, val =>
                    {
                        try
                        {
                            if (target is not null)
                            {
                                TextDataSetter.SetValue(target, pname, val);
                            }
                            ctrl.OnInput(property, val);
                        }
                        catch (Exception e)
                        {
                            this.Error($"Попытка установить значение свойства {property} для типа {ptype.GetTypeName()} со значением {val} вызвало исключение: {e.ToTextDocument()}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при создании поля формы для свойства {property}, определённого в типе {ptype.GetTypeName()}:\n{ex.ToTextDocument()}");
                }
                //}));                      
            }
            Task.WaitAll(tasks.ToArray());
            long ms = DateTimeOffset.Now.ToUnixTimeMilliseconds() - started;
            /*fields.Sort((p1, p2) =>
            {
                return
                p1.Order != p2.Order ? p1.Order - p2.Order :
                p1.Order == p2.Order ?
                p1.IsCollection && !p2.IsCollection ? -1 :
                !p1.IsCollection && p2.IsCollection ? 1 :
                    0 : 0;
            });*/
            return fields;
        }


        public TFormInputField GetFormField(Type ptype, string property, Action<string> onchange)
        {
            TFormInputField pfield;
            TFormInputField p = null;
            fields.Add(p = pfield = typeof(TFormInputField).New<TFormInputField>());
            pfield.Name = property;
            pfield.Help = ptype.GetPropertyHelp(property);
            pfield.Description = ptype.GetPropertyDescription(property);
            var pp = ptype.GetProperty(property);
            if (pp is null)
                throw new Exception($"Не найдено свойство {property} в типе {ptype.GetTypeName()}");
            pfield.IsCollection = pp.PropertyType.IsCollectionType();
            pfield.Order = 0;
            pfield.Icon = ptype.GetPropertyIcon(property);
            pfield.Label = ptype.GetPropertyLabel(property);
            pfield.Attributes = ptype.GetPropertyAttributes(property);
            pfield.OnInput = text =>
            {
                pfield.SetErrors("");
                if (target is not null && $"{target.GetValue(p.Name)}" != text)
                {
                    TextDataSetter.SetValue(target, p.Name, text);
                    onchange(text);
                }
            };
            pfield.Type = GetInputType(ptype, property);
            if (pfield.Type.IndexOf("collection") != -1)
            {
                pfield.ValueType = GetValueType(ptype, property);
            }

            if (target is null)
                target = this.ptype.New();
            try
            {
                pfield.Value = target.GetValue(property);
            }
            catch (Exception ex)
            {
                this.Warn($"не удалось получить значение свойства {property}");
            }
            if (target is ChangeSupport)
            {

                ((ChangeSupport)target).OnChange += (evt) =>
                {
                    var field = fields.FirstOrDefault(f => f.Name == evt.Property);
                    if (field is not null && field.Value != evt.After)
                    {
                        field.Value = evt.After;
                    }
                };
            }

            pfield.CtrlType = target.GetType().GetPropertyControl(property);
            if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
                p.Control = pfield.CtrlType.CreateControl(p);
            var orederStr = ptype.GetPropertyAttributeValue<InputOrderAttribute>(property, "0");

            var binding = ptype.GetPropertyAttribute<InputBindingAttribute>(property);
            if (binding is not null)
            {
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
            return pfield;
        }

        private string GetValueType(Type ptype, string property)
        {
            var propertyType = ptype.GetProperty(property).PropertyType;
            this.Info(propertyType.GetTypeName());
            bool isCollection = propertyType.IsCollectionType();
            return isCollection ?
                CommonUtils.ParseCollectionType(propertyType) :
                ptype.GetProperty(property).PropertyType.GetTypeName();
        }


        public string GetInputType(Type type, string property)
        {
            var attribute = type.GetPropertyAttributes(property).Where(kv => kv.Key.ToType().IsExtends(typeof(BaseInputAttribute))).Select(kv => kv.Key.Replace("Input", "").Replace("Attribute", "").ToLower()).FirstOrDefault();
            return GetInputType(type, property, attribute);
        }

        private static string GetInputType(Type type, string property, string attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute))
            {
                var propertyType = type.GetProperty(property).PropertyType;
                if (propertyType.IsCollectionType())
                {
                    var elementType = propertyType.GetElementType();
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
            else
            {
                return attribute;
            }
        }

        public void UpdateErrorMessages(Dictionary<string, List<string>> errors)
        {
            // Оптимизация: используем StringBuilder для конкатенации строк
            var messageBuilder = new System.Text.StringBuilder();
            foreach (var field in fields)
            {
                messageBuilder.Clear();
                if (errors.TryGetValue(field.Name, out var fieldErrors))
                {
                    foreach (string error in fieldErrors)
                    {
                        messageBuilder.Append(error).Append("; ");
                    }
                }
                field.SetErrors(messageBuilder.ToString());
            }
            if (errors.TryGetValue("this", out var thisErrors))
            {
                messageBuilder.Clear();
                foreach (string error in thisErrors)
                {
                    messageBuilder.Append(error).Append("; ");
                }
                ErrorMessage = messageBuilder.ToString();
            }
        }
    }
}
