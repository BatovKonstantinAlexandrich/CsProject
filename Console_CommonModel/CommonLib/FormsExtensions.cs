
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;



public static class ElementFactoryExtensions
{
    public static void InvokeMethodWithInput<TResult>(this object target, string action, Action<TResult> onsuccess, Action<Exception> onerror)
        where TResult : class
    {
        Action<MethodResult<TResult>> OnActionResult = result => { };

        if (target is Type)
            throw new ArgumentException("Метод стоит использовать со ссылкой на экземпляр типа");
        var methodInfo = target.GetType().GetMethods().FirstOrDefault(m => m.Name == action);
        var formModel = target.GetType().GetInputFormModel(action).WithOnCompleted(arguments => {
            try
            {

                var argv = new List<object>();
                foreach (var key in methodInfo.GetParameters().Select(p => p.Name).ToList())
                {
                    argv.Add(arguments[key]);
                }

                var result = methodInfo.Invoke(target, argv.ToArray());

                if (result is MethodResult<TResult> mr)
                {
                    OnActionResult(mr);
                }
                else if (OnActionResult is not null)
                {
                    OnActionResult(MethodResult<TResult>.OnResult((TResult)result)); // result
                }
            }
            catch (Exception ex)
            {
                if (OnActionResult is not null)
                {
                    OnActionResult(MethodResult<TResult>.OnError(ex)); // result
                }
                throw;
            }
        });
        formModel.Trace();
        var title = target.GetType().GetMethodLabel(action);

        var ctrl = formModel.ToControl();
        ctrl.Dock = "Fill";
        var submit = formModel.ToSubmit(title, (success, data) => {
            //if (success)
            //{
            //    var args = ((FormControl)data).viewModel.Cast<InputFormModel>().Item.ToDictionary();
            //    formModel.OnComplete(args);
            //}
        }, true);
        formModel.OnValidationStateChanged += state => {
            //submit.ForEach(p =>
            //{
            //    if (p is MaterialButton && p.Text.IndexOf("подтвердит") != -1)
            //    {
            //        p.Enabled = state;
            //    }
            //});
        };


        var form = submit.ToControl().RunAtForm();

        form.ShowDialog();

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

    //public static FormInputField GetFormInputField(this object target, string property, Action<string> onchange)
    //{

    //    Type ptype = target is Type ? (Type)target : target.GetType();
    //    target = target is Type ? ptype.New() : target;
    //    FormInputField pfield;
    //    FormInputField p = null;
    //    p = pfield = new FormInputField() { };
    //    pfield.Target = target;
    //    pfield.Name = property;
    //    pfield.Help = ptype.GetPropertyHelp(property);
    //    pfield.ValueType = ptype.GetProperty(property).PropertyType.GetTypeName();
    //    pfield.Description = ptype.GetPropertyDescription(property);
    //    var pp = ptype.GetProperty(property);
    //    if (pp is null)
    //        throw new Exception($"Не найдено свойство {property} в типе {ptype.GetTypeName()}");
    //    pfield.IsCollection = pp.PropertyType.IsCollectionType();
    //    pfield.Order = 0;
    //    pfield.Icon = ptype.GetPropertyIcon(property);
    //    pfield.Label = ptype.GetPropertyLabel(property);
    //    pfield.Attributes = ptype.GetPropertyAttributes(property);
    //    pfield.OnInput = text =>
    //    {
    //        pfield.SetErrors("");
    //        if ((target is not null) && $"{target.GetValue(p.Name)}" != text)
    //        {
    //            TextDataSetter.SetValue(target, p.Name, text);
    //            onchange(text);
    //        }
    //    };

    //    pfield.Type = ptype.GetPropertyInputType(property);
    //    if (pfield.Type.IndexOf("collection") != -1)
    //    {
    //        pfield.ValueType = ptype.GetPropertyValueType(property);
    //    }

    //    pfield.Value = target.GetValue(property);
    //    if (target is ChangeSupport)
    //    {
    //        ((ChangeSupport)target).OnChange += (PropertyChangedMessage evt) => {
    //            var field = pfield;
    //            if (field is not null && field.Value != evt.After)
    //            {
    //                field.Value = evt.After;
    //            }
    //        };
    //    }

    //    pfield.CtrlType = target.GetType().GetPropertyControl(property);
    //    if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
    //        p.Control = pfield.CtrlType.CreateControl(p);
    //    var orederStr = ptype.GetPropertyAttributeValue<InputOrderAttribute>(property, "0");

    //    var binding = ptype.GetPropertyAttribute<Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputBindingAttribute>(property);
    //    if (binding is not null)
    //    {
    //        if (target is not ChangeSupport)
    //            throw new Exception("Атрибут InputBinding можно использовать только в моделях типа ChangeSupport");
    //        ChangeSupport support = (ChangeSupport)target;
    //        support.AddPropertyChangeListener(binding.ModelPropertyName, evt => {
    //            support.Info($"{binding.FieldPropertyName}={evt.After}");
    //            pfield.SetValue(binding.FieldPropertyName, evt.After);
    //        });
    //        pfield.AddPropertyChangeListener(binding.FieldPropertyName, evt => {
    //            //UpdateFormFields();
    //        });
    //    }
    //    pfield.Order = orederStr.ToInt();
    //    pfield.OnInput += (text) => { };
    //    return pfield;
    //}

    public static ChangingList<InputFormField> GetOwnInputFormFields(this object target, Action<object, string, string> OnPropertyChanged = null)
        => target.GetOwnInputFormFields(target.GetType().GetOwnInputPropertyNames(), OnPropertyChanged);
    public static InputFormField GetInputFormField(this object target, string property, Action<object, string, string> OnPropertyChanged)
    {
        var formFieldsFactory = new FormFieldFactory();
        return formFieldsFactory.GetInputFormField(target, property, val =>
        {
            if (OnPropertyChanged is not null)
                OnPropertyChanged(target, property, val);
        });
    }

    public static InputFormField GetInputFormField(this object target, string property, Action<string> OnPropertyChanged)
    {
        var formFieldsFactory = new FormFieldFactory();
        return formFieldsFactory.GetInputFormField(target, property, val =>
        {
            if (OnPropertyChanged is not null)
                OnPropertyChanged(val);
        });
    }

    public static ChangingList<InputFormField> GetOwnInputFormFields(this object target, List<string> properties, Action<object,string,string> OnPropertyChanged=null)
    {
        var formFieldsFactory = new FormFieldFactory();
        var formFields = new List<InputFormField>(properties.Count);
        var formFieldsMap = new ConcurrentDictionary<string,InputFormField>();

        var tasks = new List<Task>();
        foreach (var P in properties.ToList())
        {
            string property = P;
            tasks.Add(Task.Run(() => {
                var formField = formFieldsFactory.GetInputFormField(target, property, val =>
                {
                    if (OnPropertyChanged is not null)
                        OnPropertyChanged(target, property, val);
                });
                if (formField is null)
                    throw new Exception($"Не удалось создать поле ввода для свойства {property}");
                formFieldsMap[property] = formField;
            }));
            
        }
        Task.WaitAll(tasks.ToArray());
        formFields.AddRange(formFieldsMap.Values);
        //foreach (var property in properties.ToList())
        //{ 
        //    var formField = formFieldsFactory.GetInputFormField(target, property, val =>
        //    {
        //        if (OnPropertyChanged is not null)
        //            OnPropertyChanged(target, property, val);
        //    });
        //    if (formField is null)
        //        throw new Exception($"Не удалось создать поле ввода для свойства {property}");
        //    formFields.Add(formField);
        //}

        var orderMap = properties
            .Select((name, index) => new { name, index })
            .ToDictionary(x => x.name, x => x.index);
        formFields.Sort((f1, f2) =>
        {
            var byOrder = f1.Order.CompareTo(f2.Order);
            if (byOrder != 0)
                return byOrder;
            var i1 = orderMap.TryGetValue(f1.Name, out var a) ? a : int.MaxValue;
            var i2 = orderMap.TryGetValue(f2.Name, out var b) ? b : int.MaxValue;
            return i1.CompareTo(i2);
        });
        return new ChangingList<InputFormField>(formFields);
    }
    //public static FieldView GetFormField(this object target, string property, Action<string> onchange = null)
    //{
    //    Type ptype = target is Type? (Type)target: target.GetType();
    //    target = target is Type ? ptype.New() : target;
    //    FieldView pfield;
    //    FieldView p = null;
    //    p = pfield = new FieldView() { };
    //    pfield.Name = property;
    //    pfield.Help = ptype.GetPropertyHelp(property);
    //    pfield.Description = ptype.GetPropertyDescription(property);
    //    var pp = ptype.GetProperty(property);
    //    if (pp is null)
    //        throw new Exception($"Не найдено свойство {property} в типе {ptype.GetTypeName()}");
    //    pfield.IsCollection = pp.PropertyType.IsCollectionType();
    //    pfield.Order = 0;
    //    pfield.Icon = ptype.GetPropertyIcon(property);
    //    pfield.Label = ptype.GetPropertyLabel(property);
    //    pfield.Attributes = ptype.GetPropertyAttributes(property);
    //    pfield.OnInput = text =>
    //    {
    //        pfield.SetErrors("");
    //        if ((target is not null) && $"{target.GetValue(p.Name)}" != text)
    //        {
    //            TextDataSetter.SetValue(target, p.Name, text);
    //            onchange(text);
    //        }
    //    };
        
    //    pfield.Type = ptype.GetPropertyInputType(property);
    //    if (pfield.Type.IndexOf("collection") != -1)
    //    {
    //        pfield.ValueType = ptype.GetPropertyValueType(property);
    //    }

    //    pfield.Value = target.GetValue(property);
    //    if (target is ChangeSupport)
    //    {
    //        ((ChangeSupport)target).OnChange += (PropertyChangedMessage evt) => {
    //            var field = pfield;
    //            if (field is not null && field.Value != evt.After)
    //            {
    //                field.Value = evt.After;
    //            }
    //        };
    //    }

    //    pfield.CtrlType = target.GetType().GetPropertyControl(property);
    //    if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
    //        p.Control = pfield.CtrlType.CreateControl(p);
    //    var orederStr = ptype.GetPropertyAttributeValue<InputOrderAttribute>(property, "0");

    //    var binding = ptype.GetPropertyAttribute<Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputBindingAttribute>(property);
    //    if (binding is not null)
    //    {
    //        if (target is not ChangeSupport)
    //            throw new Exception("Атрибут InputBinding можно использовать только в моделях типа ChangeSupport");
    //        ChangeSupport support = (ChangeSupport)target;
    //        support.AddPropertyChangeListener(binding.ModelPropertyName, evt => {
    //            support.Info($"{binding.FieldPropertyName}={evt.After}");
    //            pfield.SetValue(binding.FieldPropertyName, evt.After);
    //        });
    //        pfield.AddPropertyChangeListener(binding.FieldPropertyName, evt => {
    //            //UpdateFormFields();
    //        });
    //    }
    //    pfield.Order = orederStr.ToInt();
    //    pfield.OnInput += (text) => { };
    //    return pfield;
    //}

    //public static void FormatMarginsAndPaddings(this Control control )
    //{
    //    control.Padding = control.Margin = new(0);
    //    foreach (Control child in control.Controls)
    //    {
    //        child.FormatMarginsAndPaddings();
    //    }        
    //}
    /*
    public static List<string> ControlSelectMany(this Form form, string title, IEnumerable<string> options)
    {
        var dialogService = new DialogService();
        return form.SelectMany(form, title, options.ToList());
    }
    public static List<string> ControlSelectMany(this Form form, IEnumerable<string> options)
    {
        var dialogService = new DialogService();
        return dialogService.SelectMany(form, options.ToList());
    }
    public static void ControlSelectMany(this Form form, IEnumerable<string> options ,Action<bool, List<string>> oncompleted)
    {
        var dialogService = new DialogService();
        List<string> results = dialogService.SelectMany(form, options.ToList(), oncompleted);
        oncompleted(true, results);
    }*/
    
    //public static bool ControlShowDialog(this Form form, string title, Control control)
    //{
      
    //    return dialogService.Dialog(form, title, control);
    //}
    //public static List<Control> GetControls(this Form form)
    //{
    //    List<Control> controls = new List<Control>();
    //    foreach (Control control in form.Controls)
    //    {
    //        controls.Add(control);
    //    }
    //    return controls;
    //}


    //public static void AddMenu(this Form form, ToolStripDropDownItem menu)
    //{
    //    MenuStrip menuStrip = null;
    //    var pmenu = form.GetControls().FirstOrDefault(c => c is MenuStrip);
    //    if (pmenu == null)
    //    {
    //        menuStrip = new MenuStrip();
    //        form.LayoutTop(menuStrip);
    //    }
    //    else
    //    {
    //        menuStrip = (MenuStrip)pmenu;
    //    }
    //    menuStrip.Items.Add(menu);
    //}

    //public static Control WithContextMenu(this Control ctrl, List<string> buttons, Action<string> clicked )
    //{
    //    var factory = new ControlsFactory();
    //    ctrl.ContextMenuStrip = factory.CreateContextMenu(buttons.Select(button => factory.CreateContextMenuItem(button, () => {
    //        clicked(button);
    //    })).ToArray());

        
    //    // добавляем элементы в меню
    //    var context = new ContextMenuStrip();
    //    foreach (var button in buttons)
    //    {
    //        // создаем элементы меню
    //        string command = button;
    //        var menuItem = new ToolStripMenuItem(command);
    //        menuItem.Click += (s, e) => {
    //            if(clicked is not null)
    //                clicked(command);
    //        };
    //        context.Items.Add(menuItem);
    //    }
        
    //    // ассоциируем контекстное меню с текстовым полем
    //    ctrl.ContextMenuStrip = context;
    //    return ctrl;
    //}

    //public static IEnumerable<Control> GetControls(this Control ctrl)
    //{
    //    List<Control> list = new();
    //    foreach (Control p in ctrl.Controls)
    //    {
    //        list.Add(p);
    //    }
    //    return list;
    //}

    //public static Form WithControls(this Form form, params Control[] controls)
    //{
    //    var free = new Panel();
    //    var card = new MaterialCard();
    //    card.AddFill(free);
    //    form.LayoutFill(card);
    //    controls.ToList().ForEach(card.LayoutTop);
    //    return form;
    //}

    public static InputFormField GetFormFieldModel(this PropertyInfo property, string PropertyName)
    {
        InputFormField field = null;

        var type = property.DeclaringType;
        if (property == null)
        {
            throw new Exception("Ошибка при создании поля " + PropertyName);
        }
        var propertyType = Typing.ParsePropertyType(property.PropertyType);
        if (propertyType is null)
        {
            propertyType = property.PropertyType.GetTypeName();
        }
        var Attributes = AttrsUtil.ForProperty2(type, property.Name);
        if (Attributes.ContainsKey(nameof(NotInputAttribute)))
        {
            throw new Exception($"Свойство: {PropertyName} не требует элемента управления");
        }

        field = new InputFormField();
        //field.Target = Item;
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
                field.Value = "";// Item.GetValue(PropertyName);
                if (field.Value is null && property.PropertyType.GetConstructors().Any(c => c.GetParameters().Count() == 0))
                    field.Value = property.PropertyType.New();
                field.ValueType = property.PropertyType.GetGenericArguments()[0].GetTypeName();// Typing.ParseCollectionType(property.PropertyType);
            }
            else
            {

                /*var p = property.GetValue(Item);
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
                }*/


            }
        }
        else
        {
            bool isInput = AttrsUtil.IsInput(type, property.Name);


            field.Type = AttrsUtil.GetControlType(type, property.Name);
            if (field.Type != null)
            {
                if (field.Type.ToLower().Contains("collect"))
                {
                    field.ValueType = Typing.ParseCollectionType(property.PropertyType);
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
                field.Type = CommonUtils.GetInputType(type, property.Name);
                field.Type = field.Type.ToLower();
            }
        }

        //this.Warn(field.Validate().ToJsonOnScreen());
        return field;
    }

    //public static MaterialLabel CreateTitleText(this Control ctrl, string text)
    //{
    //    return new MaterialLabel()
    //    {
    //        Text = text,
    //        Font = new Font("Roboto", 16, FontStyle.Regular)
    //    };
    //}

    //public static System.Windows.Forms.ComboBox CreateSelectBox(this Control form, List<string> Options)
    //{
    //    var select = new System.Windows.Forms.ComboBox();
    //    select.Items.AddRange(Options.Select(o => (object)o).ToArray());
    //    return select;
    //}

    //public static System.Windows.Forms.ComboBox CreateSelectBox(this Control form,string title, List<string> Options, Action<string> onchanged)
    //{
    //    var select = new MaterialComboBox();
    //    select.Items.AddRange(Options.Select(o => (object)o).ToArray());
    //    select.SelectedIndexChanged += (s, e) => {
    //        if(onchanged is not null)
    //        {
    //            onchanged(Options[select.SelectedIndex]);
    //        }            
    //    };
    //    return select;
    //}

    //public static System.Windows.Forms.TreeView CreateTreeSelectView(this Control form, NodeModel root, Action<string> selected)
    //{
    //    var view = root.CreateTreeView();
    //    view.AfterSelect += (sender, e) => { 
    //        if(e?.Node is not null)
    //        {
    //            selected(e.Node.Text);
    //        }            
    //    };
    //    return view;
    //}

    //public static System.Windows.Forms.Panel CreateDropabblePanel(this Control form )
    //{
    //    var panel = new Panel();
    //    panel.AllowDrop = true;
        
    //    panel.DragDrop += (sender, evt) =>
    //    {
    //        string item = (string)evt.Data.GetData(typeof(string));
    //        Control p = panel.Parent;
    //        while(p is not null && p is Form == false)
    //        {
    //            p = p.Parent;
    //        }
    //        Form form = (Form)p;
    //        panel.LayoutTop(new Button() { Text = item });
    //        //panel.Controls.Add(new Button() { Text = item, Location = new Point(evt.X - form.Location.X - panel.Location.X - 10, evt.Y - panel.Location.Y - 45) });
    //    };
    //    panel.DragEnter += (sender, evt) => {
    //        if (evt.Data.GetDataPresent(typeof(string)))
    //        {
    //            evt.Effect = DragDropEffects.Move;
    //        }
    //        else
    //        {
    //            evt.Effect = DragDropEffects.None; 
    //        }
    //    };
    //    return panel;
    //}

    //public static void ConfigureDrop(this Control panel, Action<string> ondrop)
    //{
        
    //    panel.AllowDrop = true;

    //    panel.DragDrop += (sender, evt) =>
    //    {
    //        string item = (string)evt.Data.GetData(typeof(string));
    //        ondrop(item);
    //        /*Control p = panel.Parent;
    //        while (p is not null && p is Form == false)
    //        {
    //            p = p.Parent;
    //        }
    //        Form form = (Form)p;
    //        panel.LayoutTop(new Button() { Text = item });
    //        //panel.Controls.Add(new Button() { Text = item, Location = new Point(evt.X - form.Location.X - panel.Location.X - 10, evt.Y - panel.Location.Y - 45) });*/
    //    };
    //    panel.DragEnter += (sender, evt) => {
    //        if (evt.Data.GetDataPresent(typeof(string)))
    //        {
    //            evt.Effect = DragDropEffects.Move;
    //        }
    //        else
    //        {
    //            evt.Effect = DragDropEffects.None;
    //        }
    //    };
    //}

    //public static System.Windows.Forms.TreeView CreateTreeDraggableView(this Control form, NodeModel root)
    //{
    //    var view = root.CreateTreeDraggableView();
    //    view.MouseDown += (sender, evt) => { 
    //        if(view.SelectedNode is not null)
    //            view.DoDragDrop(view.SelectedNode.Text, DragDropEffects.Move);    
    //    };
    //    /*view.AfterSelect += (sender, e) => {
    //        Clipboard.SetText(e.Node.Text, TextDataFormat.UnicodeText);
    //    };*/
    //    return view;
    //}
}
