using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;


/// <summary>
/// Модель данных отображаемых в представлении
/// формы ввода
/// </summary>
[Label("Модель формы ввода")]
public class InputFormModel: EditView
{

    [NotInput]
    [Label("Json")]
    
    public string Json { get; set; }


    [Label("Заголовок формы")]
    
    public string Title { get; set; }

    [Label("Описани формы")]
    
    public string Description { get; set; }


    [NotInput]
    [Label("Сообщение об ошибке")]
    [InputRusWord]
    public string Error { get; set; }

    [Label("Контейнер")]
    [InputSelect("h-group,v-group")]
    public string Container { get; set; } = "v-group";
   
    [Label("Размер")]
    [InputSelect("small,normal,big")]
    public string Size { get; set; }

    
    [Label("Ссылка на объект")]
    [NotMapped]
    [NotInput]    
    public object Item { get; set; }

    [NotNullNotEmpty]
    [Label("Item Type")]
    
    public string ItemType { get; set; }
    public string Icon { get; set; }

    [Label("Поля ввода")]
    [InputStructureCollection(nameof(InputFormField))]
    [NotMapped]
    public List<InputFormField> FormFields { get; set; } = new List<InputFormField>();


    
    [Label("Действие по завершению")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput("")]
    [NotMapped]
    public Action<object> OnComplete { get; set; }


    [Label("Разрешено изменение")]
    [NotInput]
    [InputBool]
    public bool Edited { get; set; }


    [NotMapped]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    public Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();


    [InputBool]
    [Label("Состояние")]
    [NotInput]
    [UpdateWhenChanged(false)]    
    public bool IsValid { get => Get<bool>(nameof(IsValid)); set => Set<bool>(nameof(IsValid),value); }

    
    [NotMapped]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    [Label("On Validation State Changed")]
    public Action<bool> OnValidationStateChanged { get; set; }


    [InputPrimitiveCollection()]
    [Label("Properties")]
    public List<string> Properties { get; set; } = new();


    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<string, string> OnInput = (string name, string text) =>
    {
    };



    public InputFormModel CreateSubmitInputForm(Type modelType, Action<bool, object> oncompleted)
    {
        var target = modelType.New();
        var form = new InputFormModel(target);
        var panel = new AppViewBase();
        var okButton = panel.CreateButton("Ok", () => { oncompleted(true,target);  });
        
        var cancelButton = panel.CreateButton("Cancel", () => { oncompleted(false, null); });
        form.OnValidationStateChanged += is_valid_now => {
            okButton.Enabled = is_valid_now;
        };
        form.LayoutBottom(panel);
        
        return form;
    }

    public InputFormModel() : base()
    {
        
        IsValid = false;
        Title = "Заголовок";
        Description = "Описание";
        Error = "";
        Container = "v-group";
        Size = "normal";
        OnValidationStateChanged = v => { };
        this.AddPropertyChangeListener<bool>(nameof(IsValid), validatedSuccess => OnValidationStateChanged(validatedSuccess));
        this.MinHeight = this.MinWidth = 320;
        this.BorderWidth = 1;
        //IsValid = this.Errors.Where(kv => kv.Value.Count() > 0).Count() == 0;
    }


    

    public void AddFormFieldsFor<TImplentedType>(TImplentedType target)
    {
        this.Item = target;
        var properties = typeof(TImplentedType).GetOwnPropertyNames();
        if(properties.Count()==0)
        {
            throw new ArgumentException($"Не получены свойства на редактирования для {typeof(TImplentedType).GetTypeName()}");
        }
        this.AddFormFields(properties);
    }


    public InputFormModel(object item) : this()
    {
        if (item == null)
            throw new Exception("Аргумент item на задан");
        SetItem(item);
    }

    public void SetItem(object item)
    {
        try
        {

            IsValid = false;
            OnValidationStateChanged = v => { };
            this.AddPropertyChangeListener<bool>(nameof(IsValid), validatedSuccess => OnValidationStateChanged(validatedSuccess));
            Item = item;
            ItemType = item.GetTypeName();
            Title = item.GetType().GetLabel();
            Description = item.GetType().GetDescription();
            var ptype = item.GetType();
            this.Properties = ptype.GetInputProperties();
            if (this.Properties.Count() == 0)
                throw new Exception($"Не удалось получить свойства для реадктирования для типа {ptype.GetTypeName()}");
            AddFormFields(Properties.ToArray());
            this.Json = this.Item.ToJsonOnScreen();
        }
        catch (Exception ex)
        {
            this.Error($"Ошибка при инициализации {GetType().GetTypeName()}: {ex}");
        }
        this.Errors = this.Item.Validate(this.Properties);
        IsValid = this.Errors.Where(kv => kv.Value.Count() > 0).Count() == 0;
        this.OnInput += (name, value) =>
        {
            this.Errors = this.Item.Validate();
        };
    }

    public InputFormModel(object item, params string[] propertyNames) : this()
    {
        if (item == null)
            throw new Exception("Аргумент item на задан");
        IsValid = false;
        OnValidationStateChanged = v => { };
        this.AddPropertyChangeListener<bool>(nameof(IsValid), validatedSuccess => OnValidationStateChanged(validatedSuccess));
        ItemType = item.GetTypeName();
        Title = item.GetType().GetLabel();
        Description = item.GetType().GetDescription();
        Item = item;
        AddFormFields(propertyNames);
        this.Errors = this.Item.Validate(this.Properties);
        IsValid = this.Errors.Where(kv => kv.Value.Count() > 0).Count() == 0;
    }





    public void EnsureThatItemIsValide()
    {
        if (Item is MyValidatableObject)
        {
            ((MyValidatableObject)Item).EnsureIsValide();
        }
        else
        {
            throw new Exception($"Объект не наследует тип {nameof(MyValidatableObject)}");
        }
    }




    /// <summary>
    /// Обновление изолированной области в модели
    /// </summary>
    /// <param name="model"></param>
    public object AddFormFields(IEnumerable<string> propertyNames)
    {
        var item = Item;
        var form = this;
        if (item == null)
        {
            throw new Exception("Установите свойство Item для модели " + this.GetType().Name);
        }
        form.Item = item;
        form.Title = form.Title == null ? AttrsUtil.LabelFor(item) : form.Title;
        form.Description = AttrsUtil.DescriptionFor(item);
        var typeName = item.GetTypeName();
        this.Info(typeName);
        if (typeName.Contains("Dictionary")== true)
        {
            form.FormFields = CreateFormFields(item.GetType(), propertyNames, item);
        }
        else
        {
            FormFields = CreateFormFields(Item.GetType(), propertyNames, Item);
        }
        FormFields.ForEach(field => field.OnInput += (text) => {
            form.OnInput(field.Name, text);
        });
        FormFields.Sort((f1, f2) =>
        {
            return f1.Order - f2.Order;
        });
        this.OnInput += (k, v) => 
        {
            TextDataSetter.SetValue(this.Item, k, v);
            this.InfoTime("", () => {
                this.Errors = this.Item.Validate(this.Properties);
                this.IsValid = this.Errors.Where(kv => kv.Value.Count() > 0).Count() == 0;

                foreach (var kv in Errors)
                {
                    var field = this.FormFields.FirstOrDefault(f => f.Name == kv.Key);
                    if (field is not null)
                    {
                        field.Errors.Clear();
                        field.Errors.AddRange(kv.Value);
                    }
                }
            });
                
            /*var validationResult = this.Item.Validate();
            */
          
        };
        return this;
    }







    public int OnSort(InputFormField left, InputFormField right)
    {
        if (left.Type.ToLower() == "multilinetext")
        {
            return 1;
        }
        else if (right.Type.ToLower() == "multilinetext")
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    /// <summary>
    /// Уникальные поля-1, примитивные-2, Многострочные и файловые-4, остальные-3
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private int SortOrder(InputFormField field)
    {

        if (field.Type == "MultilineText" || field.Type == "File" || field.Type == "Image" || field.Type == "Foto") return 3;
        return 2;
    }

    public global::TableView ForPrimitiveCollection(dynamic items, Type type, string caption)
    {
        global::TableView tm = new global::TableView();
        tm.type = type;
        tm.Columns.Clear();
        tm.Columns.Add(caption);
        tm.Cells = new List<List<object>>();
        foreach (var item in items)
        {
            tm.Cells.Add(new List<object> { item });
        }
        tm.Editable = true;
        return tm;
    }
    public global::TableView ForCollection(dynamic items, Type type)
    {
        global::TableView tm = new global::TableView();
        tm.type = type;
        tm.Columns.Clear();
        ((from p in ReflectionService.GetOwnPropertyNames(type) select p).ToList()).ForEach(tm.Columns.Add);
        foreach (var item in items)
        {
            try
            {
                tm.Cells.Add(ReflectionService.Values(item, tm.Columns.ToList()));
            }
            catch (Exception ex)
            {
                tm.Cells.Add(new List<object>() { $"Ошибка при получении значения: {ex.Message}" });
            }

        }
        return tm;
    }

    public global::TableView ForCollectionProperty(object target, string property)
    {
        string typeName =
            Typing.ParseCollectionType(target.GetType().GetProperty(property).PropertyType);
        if (Typing.IsPrimitive(typeName))
        {
            return ForPrimitiveCollection(
                        target.GetType().GetProperty(property).GetValue(target),
                        ReflectionService.TypeForName(typeName),
                        AttrsUtil.DescriptionFor(target.GetType(), property));
        }
        else
        {
            return ForCollection(
                        target.GetType().GetProperty(property).GetValue(target),
                        ReflectionService.TypeForName(typeName));
        }

    }


    /// <summary>
    /// Создание модели табличного представления из свойств объекта
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public global::TableView ForDictionary(object model)
    {

        global::TableView tm = new global::TableView();

        tm.Columns = ReflectionService.GetOwnMethodNames(model.GetType());
        List<List<object>> cells = new List<List<object>>();

        //cells
        List<object> row = new();
        int i = 0;
        foreach (string column in tm.Columns)
        {
            row.Add(GetValue(model, column));
        }
        cells.Add(row);

        tm.Cells = cells;
        
        return tm;
        //return ForDictionary(Formating.ToDictionaryLabels(model), Attrs.LabelFor(model.GetType()));
    }

    public global::TableView ForDictionary(IDictionary<string, object> properties, string title)
    {
        var table = new global::TableView();
        table.Title = title;
        table.Searchable = true;
        table.Columns = new List<string> { "Ключ", "Значение" };
        table.Cells = new List<List<object>>();
        foreach (var p in properties)
        {
            table.Cells.Add(new List<object> { p.Key, p.Value });
        }
        return table;
    }
    public InputFormField CreateFormField(string name, string type, Dictionary<string, string> attributes)
    {
        InputFormField field = null;
       
        var propertyType = type;
        
        var Attributes = attributes;
        if (Attributes.ContainsKey(nameof(NotInputAttribute)))
        {
            throw new Exception();
        }

        field = new InputFormField();
        field.DataType = propertyType.GetTypeName();
        field.Target = Item;
        //field.Property = paramModel.Name;
        field.Attributes = Attributes;

        field.Order = Attributes.ContainsKey(nameof(InputOrderAttribute)) ?
            Attributes[nameof(InputOrderAttribute)].ToInt() : 0;
        var attrs = Attributes;
        field.Name = name;
        field.Label = 
            attrs.ContainsKey(nameof(LabelAttribute)) ? attrs[nameof(LabelAttribute)] :
            attrs.ContainsKey(nameof(DisplayAttribute)) ? attrs[nameof(DisplayAttribute)] : name;
        field.Description = attrs.ContainsKey(nameof(DescriptionAttribute)) ? attrs[nameof(DescriptionAttribute)] :
                attrs.ContainsKey(nameof(DescriptionAttribute)) ? attrs[nameof(DescriptionAttribute)] : "";
        field.Icon = attrs.ContainsKey(nameof(IconAttribute)) ? attrs[nameof(IconAttribute)] :
            attrs.ContainsKey(nameof(InputIconAttribute)) ? attrs[nameof(InputIconAttribute)]: "home";
        field.Help = attrs.ContainsKey(nameof(HelpMessageAttribute)) ? attrs[nameof(HelpMessageAttribute)] : "";
        field.IsPrimitive = ReflectionService.IsPrimitive(propertyType);
         
        field.Type = GetInputType(type.ToType(), Attributes);
        if (Attributes.ContainsKey(nameof(NotInputAttribute)))
        {
            return field;
        }
        if (ReflectionService.IsPrimitive(propertyType) == false)
        {
            if (Typing.IsCollectionType(type.ToType()))
            {
               
                field.IsCollection = true;
                var itemType = type.ToType().GetGenericArguments()[0].GetTypeName();
                field.IsPrimitive = Typing.IsPrimitive(itemType);                                                                           //var table = ForCollectionProperty(Item, property.Name);
                field.Type = "custom";
              
                if (field.Value is null && type.ToType().GetConstructors().Any(c => c.GetParameters().Count() == 0))
                    field.Value = type.ToType().New();
                field.ValueType = type.ToType().GetGenericArguments()[0].GetTypeName();// Typing.ParseCollectionType(property.PropertyType);
            }
            else
            {
                var property = type.ToType().GetProperty(name);
                field.Type = "custom";
                field.DataType = type;
                //field.Value = Typing.IsPrimitive(paramModel.Type)? System.Convert.ChangeType("", paramModel.Type.ToType()) : paramModel.Type.New();
                /*var p = paramModel.Type.ToType().New();
                if (p != null)
                {
                    var table = ForDictionary(p);
                    field.Type = "custom";
                    field.Control = table;
                    field.Value = paramModel.Type.New();
                }
                else
                {
                    field.Value = propertyType.New();
                    field.Type = "custom";
                    field.Value = paramModel.Type.New();
                }*/
            }
        }
        else
        {
            bool isInput = AttrsUtil.IsInput(Attributes);


            field.Type = AttrsUtil.GetControlType(Attributes);
            if (field.Type != null)
            {
                if (field.Type.ToLower().Contains("collect"))
                {
                    field.ValueType = Typing.ParseCollectionType(type.ToType());
                }
                string input =
                    Attributes.ContainsKey(field.Type) ? Attributes[field.Type] :
                    Attributes.ContainsKey(field.Type.Replace("Attribute", "")) ? Attributes[field.Type.Replace("Attribute", "")] :
                    Attributes.ContainsKey(field.Type + "Attribute") ? Attributes[field.Type + "Attribute"] :
                    throw new Exception("Не найден атрибут соответвующий типу элемента управления " + field.Type);
                Type fieldType = ReflectionService.TypeForShortName(field.Type);
                ControlAttribute attribute = ReflectionService.Create<ControlAttribute>(fieldType, new object[] { input });

                field.Control = attribute.CreateControl(field);
                field.Type = field.Type.Replace("Attribute", "");
                field.TextValue = input;
                switch (field.Type)
                {
                    case nameof(InputDictionaryAttribute):
                        {
                            string entity = input.Split(",")[0];
                            string display = input.Split(",")[1];

                            break;
                        }
                }
            }

            else
            {
                field.Type = GetInputType(type.ToType(), Attributes);
                if(field.Type is null)
                {
                    field.Type = "text";
                }
                else
                {
                    field.Type = field.Type.ToLower();
                }                 
            }
        }

        this.Warn(field.Validate().ToJsonOnScreen());
        return field;
    }

    

    private string? GetInputType(Type type, Dictionary<string, string> attrs)
    {
        /*CommonUtils.GetInputType(type, attrs);
        var types = attrs.Keys.Where(key => key.ToType().IsExtends(typeof(BaseInputAttribute))).Select(k => ((BaseInputAttribute)k.ToType().New()).DataType.ToString()).ToHashSet();
        return types.FirstOrDefault();*/

        
        if (attrs.ContainsKey("Key") || attrs.ContainsKey("KeyAttribute"))
        {
            return "hidden";
        }
        string result = null;
        string key = null;
        List<string> keys = new List<string>(attrs.Keys);
        BaseInputAttribute.GetInputTypes().ForEach((string name) =>
        {
            if (keys.Contains(name))
            {
                key = name;
            }
        });
        if (key != null)
        {
            result = key.Replace("Attribute", "").Replace("Input", "");
        }
        else
        {

            result = null;
        }
             
        if (type.IsDateTime())
        {
            return "date";
        }
        else if (type.IsBoolean())
        {
            return "checkbox";
        }
        else if (type.IsNumber())
        {
            return "number";
        }
        else
        {
            return "text";
        } 
    }

    public InputFormField CreateFormField( Type type, string PropertyName)
    {
        InputFormField field = null;
        var property = type.GetProperty(PropertyName);
        
        if (property == null)
        {
            throw new Exception("Ошибка при создании поля " + PropertyName            );
        }
        var propertyType = Typing.ParsePropertyType(property.PropertyType);
        if(propertyType is null)
        {
            propertyType = property.PropertyType.GetTypeName();
        }
        var Attributes = AttrsUtil.ForProperty2(type, property.Name);
        if(Attributes.ContainsKey(nameof(NotInputAttribute)))
        {
            throw new Exception($"Свойство: {PropertyName} не требует элемента управления");
        }

        field = new InputFormField();
        field.Parent = this;
        field.DataType = propertyType;
        field.Target = Item;
        field.Attributes = Attributes;

        field.Order = Attributes.ContainsKey(nameof(InputOrderAttribute)) ?
            Attributes[nameof(InputOrderAttribute)].ToInt() : 0;

        field.Name = property.Name;
        field.Label = AttrsUtil.LabelFor(type, property.Name);
        field.Description = AttrsUtil.DescriptionFor(type, property.Name);
        field.Icon = AttrsUtil.IconFor(type, property.Name);
        field.Help = AttrsUtil.HelpFor(type, property.Name);
        field.IsPrimitive = ReflectionService.IsPrimitive(propertyType);
           
        if (Attributes.ContainsKey(nameof(NotInputAttribute)))
        {
            return field;
        }
        if (ReflectionService.IsPrimitive(propertyType) == false)
        {
            if (Typing.IsCollectionType(property.PropertyType))
            {
                if (property.PropertyType == null)
                    throw new Exception("property.PropertyType == null");
                field.IsCollection = true;
                var itemType = property.PropertyType.GetGenericArguments()[0].GetTypeName();
                field.IsPrimitive = Typing.IsPrimitive(itemType);                                                                           //var table = ForCollectionProperty(Item, property.Name);
                field.Type = "custom";
                field.Value = Item.GetValue(PropertyName);
                if (field.Value is null && property.PropertyType.GetConstructors().Any(c => c.GetParameters().Count() == 0))
                    field.Value = property.PropertyType.New();
                field.ValueType = property.PropertyType.GetGenericArguments()[0].GetTypeName();// Typing.ParseCollectionType(property.PropertyType);
            }
            else
            {

                var p = property.GetValue(Item);
                if (p != null)
                {
                    var table = ForDictionary(p);
                    field.Type = "custom";
                    field.Control = table;
                    field.Value = property.PropertyType.New();
                }
                else
                {
                    field.Value = propertyType.New();
                    field.Type = "custom";
                    field.Value = property.PropertyType.New();
                }


            }
        }
        else
        {
            bool isInput = AttrsUtil.IsInput(type, property.Name);

            if(property.Name == "Email")
            {

            }
            field.Type = GetInputType(type, property.Name);
            //AttrsUtil.GetControlType(type, property.Name);
            if (field.Type == null)
            {
                if (field.Type.ToLower().Contains("collect"))
                {
                    field.ValueType = Typing.ParseCollectionType(property.PropertyType);
                }
                var pa = Attributes.FirstOrDefault(a => a.Key.Replace("Attribute", "").Replace("Input", "").ToLower() == field.Type);
               
                //Attributes.ContainsKey(field.Type) ? Attributes[field.Type] :
                //Attributes.ContainsKey(field.Type.Replace("Attribute", "")) ? Attributes[field.Type.Replace("Attribute", "")] :
                //Attributes.ContainsKey(field.Type + "Attribute") ? Attributes[field.Type + "Attribute"] :
                //throw new Exception("Не найден атрибут соответcвующий типу элемента управления " + field.Type);
                Type fieldType = pa.Key.ToType();// ReflectionService.TypeForShortName(field.Type);
                ControlAttribute attribute = ReflectionService.Create<ControlAttribute>(fieldType, new object[] { pa.Value });
                
                field.Control = attribute.CreateControl(field);
                field.Type = field.Type.Replace("Attribute", "");
                //field.TextValue = input;
                switch (field.Type)
                {
                    case nameof(InputDictionaryAttribute):
                        {
                            //string entity = input.Split(",")[0];
                            //string display = input.Split(",")[1];

                            break;
                        }
                }
            }
           
            else
            {
                field.Type = GetInputType(type, property.Name);
                field.Type = field.Type.ToLower();
            }
        }

        this.Warn(field.Name + "  " + field.Type + "  " + field.Validate().ToJsonOnScreen());
        return field;
    }

    /// <summary>
    /// Создание полей формы для отображения и ввода информации
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<InputFormField> CreateFormFields(Type type, IEnumerable<string> propertyNames, object target = null)
    {
        List<InputFormField> fields = new List<InputFormField>();
        foreach (var propertyName in propertyNames)
        {

            try
            {
                var field = CreateFormField(type, propertyName);
                if (target != null)
                {
                    var val = GetValue(target, propertyName);
                    if (val is not null)
                    {
                        field.Value = val;
                    }

                }
                fields.Add(field);
            }
            catch(Exception ex)
            {
                this.Error($"Ошибка при создании поля ввода для свойства: {propertyName} {ex.Message}");
            }
        }
        return fields;
    }

    private ViewItem CreateControl(InputFormField field)
    {
        return null;
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

    private bool IsEnumerable(Type type)
    {
        return type.Name.StartsWith("List") || type.Name.StartsWith("HashSet");
    }


    /// <summary>
    /// Извлекает значение свойства из объекта по наименованию
    /// </summary>
    /// <param name="model"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private object GetValue(object model, string property)
    {
        PropertyInfo propertyInfo = model.GetType().GetProperty(property);
        FieldInfo fieldInfo = model.GetType().GetField(property);
        return
            fieldInfo != null ? fieldInfo.GetValue(model) :
            propertyInfo != null ? propertyInfo.GetValue(model) :
            null;
    }

    private List<string> Validate(InputFormField field)
    {

        List<string> errors = new List<string>();
        foreach (var validationInfo in field.CustomValidators)
        {
            try
            {
                MyValidation validation = ReflectionService.Create<MyValidation>(validationInfo.Key, validationInfo.Value.ToArray());
                string error = validation.Validate(Item, field.Name, field.Value);
                if (string.IsNullOrEmpty(error) == false)
                {
                    errors.Add(error);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Элемент ввода: {field.Name}" + "Ошибка при выподлнении валидации. Забавно! " + ex.Message);
            }

        }
        return errors;

    }


    public Dictionary<string, List<string>> Validate()
    {

        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        if (Item is BaseEntity)
        {
            result = ((BaseEntity)Item).Validate();
        }
        else
        {
            if (Typing.ReferenceIsDictionary(Item))
            {
                foreach (var field in FormFields)
                {
                    List<string> errors = Validate((InputFormField)field);
                    if (errors != null && errors.Count() > 0)
                    {
                        result[((InputFormField)field).Name] = errors;
                    }
                }
            }
            else
            {
                return Item.Validate();
            }
        }
        return result;
    }


    public Dictionary<string, object> GetValues()
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        this.FormFields.ForEach((field) =>
        {
            values[((InputFormField)field).Name] = ((InputFormField)field).Value;
        });
        return values;
    }

    public InputFormField FindFieldByName(string propertyName)
    {
        return (InputFormField)(from p in FormFields where ReflectionService.GetValueFor(p, "Name").ToString() == propertyName select p).SingleOrDefault();
    }


    /// <summary>
    /// Регистрация действия по завершению ввода (отмена, либо успешный ввод)
    /// </summary>
    public InputFormModel WithOnCompleted(Action<Dictionary<string, object>> onsubmitted)
    {
        this.OnComplete += (target) =>
        {
            this.Info($"OnCompleted: {target}");
            if(onsubmitted is not null)
            {
                var data = target.ToDictionary();                
                onsubmitted(data);
            }            
        };
        return this;
    }
}
