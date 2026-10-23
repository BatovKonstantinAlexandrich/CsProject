using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

/*
/// <summary>
/// 
/// </summary>
public class InputPanel : MaterialCard
{
    public void InfoTime(string text, Action todo)
    {
        var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        todo();
        var ended = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        this.Info($"{text}: {ended - started} мс.");
    }


    //отступт от верхнего края
    public int offsetTop = 0;
    public object target;
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
    public List<InputField> fields;

         [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
    public FlowLayoutPanel content = new() { AutoScroll = true };
    public string ErrorMessage
    {
        get
        {
            return ErrorMessageLabel is null? "": ErrorMessageLabel?.Text;
        }
        set
        {
            if (ErrorMessageLabel is not null)
                ErrorMessageLabel.Text = value;
        }
    }

    [JsonIgnore]
    private Label ErrorMessageLabel;

    public InputPanel(Type ptype, Action<bool, object> oncompleted)
    {
       
        this.InfoTime("Конструктор InputPanel", () => {

            this.AutoScroll = true;
            this.SetVerticalScroll();
            this.AutoScroll = true;
            this.target = ptype.New();
            this.oncompleted = oncompleted;
            this.ptype = ptype;
            this.properties = ptype.GetInputPropertiesSorted();
            this.FormatMarginsAndPaddings();
            this.content.SetVerticalScroll();
            var errors = target.Validate();
            var valid = errors.Count(kv => kv.Value.Where(err => !String.IsNullOrWhiteSpace(err)).Count() > 0) == 0;
            //this.Info(errors);
            ok.Enabled = valid;
            UpdateErrorMessages(errors);
        });
    }
    public InputPanel(object target, Type ptype, Action<bool, object> oncompleted)
    {
        this.InfoTime("Конструктор InputPanel", () => {

            this.AutoScroll = true;
            this.SetVerticalScroll();
            this.AutoScroll = true;
            this.target = target;
            this.oncompleted = oncompleted;
            this.ptype = ptype;
            this.properties = ptype.GetInputProperties();
            this.FormatMarginsAndPaddings();
            this.content.SetVerticalScroll();
            var errors = target.Validate();
            var valid = errors.Count(kv => kv.Value.Where(err => !String.IsNullOrWhiteSpace(err)).Count() > 0) == 0;
            //this.Info(errors);
            ok.Enabled = valid;
            UpdateErrorMessages(errors);
        });

    }
    public InputPanel(object target, List<string> properties, Type ptype, Action<bool, object> oncompleted)
    {
        this.InfoTime("Конструктор InputPanel", () => {

            this.AutoScroll = true;
            this.SetVerticalScroll();
            this.AutoScroll = true;
            this.target = target;
            this.oncompleted = oncompleted;
            this.ptype = ptype;
            this.properties = properties;
            this.FormatMarginsAndPaddings();
            this.content.SetVerticalScroll();
            var errors = target.Validate();
            var valid = errors.Count(kv => kv.Value.Where(err => !String.IsNullOrWhiteSpace(err)).Count() > 0) == 0;
            //this.Info(errors);
            ok.Enabled = valid;
            UpdateErrorMessages(errors);
        });

    }

    public InputPanel()
    {
        this.AutoScroll = true;
        this.content.SetVerticalScroll();
    }

         
    public void Init( )
    {
   

        if (ServiceFactory.Get().Assemblies.Count() < 3)
            ServiceFactory.Load();
        if (target is null)
            throw new NullReferenceException(nameof(target));
        this.offsetTop = 0;
            
        content.Controls.Clear();
        this.target = target;
        Completed += oncompleted;

        var card = new MaterialCard();
        this.AddFill(card);
        card.Controls.Add(content);
        content.Height = card.Height - 40;
        content.Width = card.Width - 40;
        card.Resize += (s, e) => {
            content.Left = 20;
            content.Left = 20;
            content.Height = card.Height - 40;
            content.Width = card.Width - 40;
        };

        SetDefaults(this);
        Size = new(800, 600);
        content.Padding = new(20);

        AddTitle(GetTitle(ptype));
        AddErrorMessage("");
        if (!String.IsNullOrWhiteSpace(ErrorMessage))
        {
            AddErrorMessage(ErrorMessage);
        }
        string title = ptype.GetTypeName();
        this.Info(title);
        this.fields = GetFormFields(ptype, properties).ToList();
        this.fields.ToList().ForEach(field => field.Target = target);
        InfoTime("Обработка свойств объекта", () => {
            foreach (InputField field in fields)                                    
                AddFormField(field);                
        });

        OnInput += (k,v)=> {

            //System.Threading.Tasks.Task.Run(() => {
                Update(k, v);
            //});
        };
        AddControls(Completed);

        try{
            var errors = target.Validate();
            var valid = errors.Count(kv => kv.Value.Count() > 0) == 0;
            ok.Enabled = valid;
            foreach (var field in fields)
            {
                string message = "";
                if (errors.ContainsKey(field.Name))
                    foreach (string error in errors[field.Name])
                        message += $"{error}; ";
              
                field.SetErrors(message);
            }
        }
        catch(Exception ex)
        {
            this.Error($"Ошибка при валидации {ex}");
        }
        this.AutoScroll = true;
        this.content.AutoScroll = true;

        /*List<Control> ctrls = new();
        foreach(Control control in this.content.Controls)
        {
            ctrls.Add(control);
        }
        ctrls.Reverse();
        ctrls.ForEach(item=> AddPanel(item));* /

    }

    [JsonIgnore]
    protected ComponentsFactory factory = new ComponentsFactory();

    private void AddConsoleMessage(string debugMessage)
    {
        var label = factory.CreateLabel(fields.ToJsonOnScreen());
        this.LayoutTop(label);
    }

    public Control AddPanel(Control control)
    {
        content.BackColor = control.BackColor;
        SetDefaults(control);
        content.Controls.Add(control);
        
        offsetTop = control.Top + control.Height;
        control.Top = offsetTop;
        this.Height = content.GetControls().Max(ctrl => (ctrl.Top + ctrl.Height)) + 150;
        return control;
    }

    [JsonIgnore]
    public Action<bool, object> Completed = (arg1, arg2) =>
    {
    };

    private void AddErrorMessage(string errorMessage)
    {
        AddPanel(ErrorMessageLabel = CreateBaseLabel(errorMessage));
    }

    public void UpdateErrorMessages (Dictionary<string, List<string>> errors)
    {
        foreach (var field in fields)
        {
            string message = "";
            if (errors.ContainsKey(field.Name))
                foreach (string error in errors[field.Name])
                    message += $"{error}; ";
            field.SetErrors(message);
        }
        if (errors.ContainsKey("this"))
        {
            this.ErrorMessage = "";
            foreach (string error in errors["this"])
                this.ErrorMessage += $"{error}; ";
        }
    }


    public void Update(string key, string value)
    {
            
         
        try
        {
            string message = "";
            TextDataSetter.SetValue(target, key, value);
            var errorsMessages = target.Validate(key);
            foreach (string error in errorsMessages)
                message += $"{error}; ";
            var field = fields.FirstOrDefault(f => f.Name == key);
            field.SetErrors(message);
            var errors = target.Validate();
            var valid = errors.Count(kv => kv.Value.Where(err => !String.IsNullOrWhiteSpace(err)).Count() > 0) == 0;
            //this.Info(errors);
            ok.Enabled = valid;
            UpdateErrorMessages(errors);
        }
        catch (Exception ex)
        {
            OnError(ex);
        }

        //});
    }

    private void OnError(Exception ex)
    {

    }

    public void LayoutHeight()
    {
        int h = 0;
        foreach (Control control in Controls)
        {
            control.Location = new Point(0, h += control.Height);
        }
    }

    [JsonIgnore]
    public Action<string, string> OnInput = (k, v) =>
    {
        System.Console.WriteLine($"{k}={v}");
    };

    [JsonIgnore]
    public MaterialButton ok;
    [JsonIgnore]
    public MaterialButton cancel;
    private List<string> _properties = new();

    [JsonIgnore]
    public IEnumerable<string> properties
    {
        get => _properties;
        set
        {
            _properties = value is null? null: value.ToList();
            if(properties is not null)
            {
                try
                {
                    this.Init();
                }
                catch (Exception ex)
                {
                    this.Error($"Ошибка при инициализации свойств: {ex.Message}");
                    MessageBox.Show($"Ошибка при инициализации свойств типа {ptype.GetTypeName()}: {ex.Message}\n {ex.StackTrace}");
                    throw;
                }
            }
                
        }
    }
    [JsonIgnore]
    protected Action<bool, object> oncompleted;
    [JsonIgnore]
    private Type ptype;
    [JsonIgnore]
    public Panel controlsPanel;

    public Control SetDefaults(Control control)
    {
        control.Margin = control.Padding = new(0);
        control.Width = Width;
        Resize += (s, e) => control.Width = Width;
        return control;
    }


    public void AddControls(Action<bool, object> oncompleted)
    {
        controlsPanel = new Panel();
        SetDefaults(controlsPanel);
        ok = CreateButton("Подтвердить", () => { if (oncompleted is not null) oncompleted(true, target); });
        ok.Margin = ok.Padding = new(5);
        ok.Enabled = false;
        ok.Width = 100;
        cancel = CreateButton("Отмена", () => { if (oncompleted is not null) oncompleted(false, target); });
        cancel.Margin = cancel.Padding = new(5);
        cancel.Location = new(150, 0);
        cancel.Width = 100;
        controlsPanel.LayoutLeft(ok);
        ok.Dock = DockStyle.Left;
        controlsPanel.AddFill(new Panel() { Height = 20, Width = 20 });
        controlsPanel.LayoutRight(cancel);
        cancel.Dock = DockStyle.Right;
        controlsPanel.Height = 35;
        //AddPanel(panel);
        this.LayoutBottom(controlsPanel);
    }

    public MaterialButton CreateButton(string text, Action onclick)
    {
        var button = new MaterialButton() { Text = text, Width = 100 };
        button.Click += (s, e) => { onclick(); };
        return button;
    }

    public void AddFormField(InputField field)
    {
        AddPanel(field.label = CreateLabel(field.Label));
        field.label.Text = "    " + field.label.Text;
        if (field.CtrlType is not null)
        {
            try
            {
                AddPanel((field.input = this.CreateWinFormControl(field)));
            }
            catch(Exception e)
            {
                this.Error(e);
            }
        }
        else
        switch (field.Type)
        {
            case "primitivecollection":
                {
                    Panel p = null;
                    Control control = null;
                    if (field.Value is null)
                        field.Value = new List<object>();
                    Control list = null;
                    var slider = new Panel();
                    field.input = factory.CreateCheckBox(field.Label, val =>
                    {
                        if (val)
                        {
                            slider.Controls.Clear();
                            slider.LayoutTop(field.input);
                            var values = new List<string>();
                            foreach (var item in (dynamic)field.Value)
                            {
                                values.Add(item.ToString());
                            }                                
                            slider.LayoutTop(factory.CreateButton("создать", () => {

                                var dialog = new ComponentsFactory();
                                dialog.CreateTextbox("", changed => { 
                                }).ToSubmit("Новый элемент", ctrl => {
                                    values.Add(ctrl.Text);
                                    slider.Controls.Remove(list);
                                    slider.AddFill(list = factory.CreateSelectList(values, selected => { }));
                                }).ToBaseForm().ShowDialog();
                            }));
                            slider.AddFill(list = factory.CreateSelectList(values, selected => { }));
                            p.LayoutFill(slider);
                        }
                        else
                        {
                            slider.Controls.Remove(field.input);
                            p.Controls.Remove(slider);
                            p.LayoutFill(field.input);
                        }
                    });
                    AddPanel( p = field.input.ToPanel());

                    p.Height = 100;


                    break;
                }
            case "structurecollection":
            case "structuredcollection":
                {
                    var itemType = field.ValueType.ToType();
                    if (itemType is null)
                        throw new Exception("structuredcollection требует указаание типа field.ValueType");
                    if(itemType is BaseEntity)
                    {
                        dynamic target = field.Value is not null ? field.Value : typeof(List<>).MakeGenericType(itemType).New();
                        List<object> items = new();
                        foreach (var item in target)
                            items.Add(item);
                        var grid = factory.CreateGridView<object>(items, new List<string>() { });
                        //установка идентифкатора внешенего ключа
                        Action<object> onadd = item => {
                            var prop = item.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == this.target.GetType());
                            if (prop != null)
                            {
                                prop.SetValue(item, this.target);
                                var pid = item.GetType().GetProperties().FirstOrDefault(p => p.Name == $"{prop.Name}Id");
                                if (pid is not null && this.target is BaseEntity)
                                {
                                    pid.SetValue(item, ((BaseEntity)this.target).Id);
                                }
                            }
                        };
                        Panel p = null;
                        content.Controls.Add(p = this.factory.CreateCheckBox(field.Label, checkedState => {
                            if (!checkedState)
                            {
                                content.Controls.Remove(p);
                            }
                            else
                            {
                                field.Value = field.Value is null ? field.Property.PropertyType.New() : field.Value;
                                Control ctrl = typeof(CrudInMemoryController<>).MakeGenericType(itemType).New<Control>(new object[] {
                                    field.Value, onadd, null, null
                                });
                                var typeOfEntity = field.ValueType.ToType();
                                var dataset = new List<object>();

                                System.Console.WriteLine(dataset.Count());
                                var buttons = factory.CreateButtons("".Split(), clicked => {
                                    int x = 0;
                                });
                                var table = factory.CreateTable(
                                    field.Label,
                                    typeOfEntity.GetInputPropertiesSorted(),
                                    dataset, (grid, r, c, val, target, prop) =>
                                    {




                                    }
                                );
                                //active = true;
                                //table.Enabled = false;

                                var tableController = typeof(DataGridViewController<>).MakeGenericType(typeOfEntity).New<dynamic>(table);
                                object selected = null;
                                table.SelectionChanged += (s, e) =>
                                {
                                    int selectedIndex = -1;
                                    try
                                    {
                                        if (table.CurrentRow is not null && table.CurrentRow is not null)
                                        {
                                            selectedIndex = table.Rows.IndexOf(table.CurrentRow);

                                            selected = selectedIndex >= 0 && selectedIndex < dataset.Count ? dataset[selectedIndex] : null;
                                        }
                                        else
                                        {
                                            selected = null;
                                        }
                                    }
                                    catch
                                    {
                                        MessageBox.Show($"Длина массива {dataset.Count} индекс {selectedIndex}");
                                    }
                                };

                                content.LayoutBottom(buttons);
                                content.LayoutBottom(table);

                            }

                        }).ToPanel());
                    }
                    else
                    {
                        Panel panel = null;
                        Control p = null;
                        var checkbox = this.factory.CreateCheckBox(field.Label, checkedState => {
                            if (!checkedState)
                            {
                                panel.Controls.RemoveAt(0);
                            }
                            else
                            {
                                var editor = new MaterialCard();
                                editor.SetVerticalScroll();
                                var dataset = typeof(List<>).MakeGenericType(itemType).New();
                                System.Console.WriteLine(itemType.GetTypeName());
                                var ctrl = (Control)typeof(CollectionController<>).MakeGenericType(itemType).New
                                (
                                    new object[] {
                                    dataset,
                                    (object item)=>{ },
                                    (string item)=>{ }
                                    }
                                );
                                var buttons = factory.CreateButtons("создать,удалить".Split(","), clicked => {
                                    switch (clicked)
                                    {
                                        case "создать":
                                            panel.Controls.Remove(editor);
                                            Control temp = null;
                                            panel.Controls.Add(temp = factory.CreateInputFormForType((success, data) => {
                                                if (success)
                                                {
                                                    dataset.Invoke("Add", new object[] { data });
                                                }
                                                    
                                                panel.Controls.Remove(temp);
                                                panel.Controls.Add(editor);
                                            }, itemType));
                                            break;
                                        case "удалить": 
                                            break;
                                    }
                                });
                                editor.LayoutTop(buttons);
                                editor.LayoutFill(ctrl);
                                panel.LayoutFill(editor);
                                panel.Height = ctrl.Height + buttons.Height + 250;
                            }

                        });
                        content.Controls.Add(panel = checkbox.ToPanel());
                        content.Resize += (s, e) => {
                            panel.Width = content.Width;
                        };
                        var pctrl = panel.Parent;
                        while(pctrl is not null)
                        {
                            System.Console.WriteLine($"{pctrl.GetTypeName()}: {pctrl.Width}");
                            pctrl = pctrl.Parent;
                        }

                            
                        panel.Width = 240;
                    }                           
                    break;
                }
            case "color": //AddPanel(field.input = CreateInputColor(field.Value, field.OnInput)); break;
            case "url": AddPanel(field.input = CreateInputUrl(field.Value, field.OnInput)); break;
            case "image":
                {
                    var panel = new FlowLayoutPanel();
                    panel.Controls.Add(new Button() { Text = "ok" });
                        
                    if( field.Value is not null && field.Value is byte[] && ((byte[])field.Value).Length >0 )
                    {
                        var data = ((byte[])field.Value);
                        var image = this.factory.CreateImage(data);
                        image.Click += (s, e) => {
                            var fds = new FileDialogService();
                            fds.OpenFile("Выберите изображение", "Image | *.png", global::System.IO.Directory.GetCurrentDirectory(), (global::System.Action<string>)(selected => {
                                var data = global::System.IO.File.ReadAllBytes(selected);
                                field.Value = data;
                                target.SetValue(field.Name, field.Value);
                                panel.LayoutFill((global::System.Windows.Forms.Control)this.factory.CreateImage(data));
                            }));
                        };
                        image.Size = new(280, 320);
                        panel.Height = image.Height;
                        panel.Controls.Clear();
                        panel.Controls.Add(image);

                        }
                    else
                    {
                        var fds = new FileDialogService();
                        Action<string> showImage = null;
                        showImage = (file) => {
                            var data = System.IO.File.ReadAllBytes(file);
                            field.Value = data;
                            target.SetValue(field.Name, field.Value);
                            var image = this.factory.CreateImage(data);
                            image.Click += (s, e) => {                           
                                fds.OpenFile("Выберите изображение", "Image | *.png", System.IO.Directory.GetCurrentDirectory(), showImage);
                            };
                            image.Size = new(280, 320);
                            panel.Controls.Clear();
                            panel.Controls.Add(image);
                        };
                        var button = this.factory.CreateButton("Загрузить", () => {                                
                            fds.OpenFile("Выберите изображение", "Image | *.png", System.IO.Directory.GetCurrentDirectory(), showImage);
                        });
                        button.Size = new(280, 320);
                        panel.Height = button.Height;
                        panel.Controls.Clear();
                        panel.Controls.Add(button);
                    }
                        
                    AddPanel(field.input = panel);
                    break;
                }

            case "icon": AddPanel(field.input = CreateSelectIcon(field.Value, field.OnInput)); break;

            case "int": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "percent": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "money": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "natural": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "number": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "bool":
            case "boolean":
            case "logic":
            case "checkbox": AddPanel(field.input = CreateCheckbox(field.Value is not null && field.Value is bool ? (bool)field.Value : false, field.OnInput)); break;
            case "dictionary": break;//AddPanel(field.input = CreateSelectbox($"{field.Value}", field.GetSelectOptions(), field.OnInput)); break;
            case "entitytype":
                var options = field.Attributes.FirstOrDefault(kv => kv.Key.ToType().IsExtends(typeof(InputSelectAttribute))).Key.ToType().New<InputSelectAttribute>(new object[] { }).Options;
                AddPanel(field.input = CreateSelectbox($"{field.Value}", options, field.OnInput)); 
                break;
            case "select": AddPanel(field.input = CreateSelectbox($"{field.Value}", field.GetSelectOptions(), field.OnInput)); break;
            case "selectexpression":
                    {
                        var ctrl = typeof(InputSelectExpressionAttribute).New<InputSelectExpressionAttribute>(new object[] { field.Attributes.First(kv => kv.Key == nameof(InputSelectExpressionAttribute)).Value });
                        var selectoptions = ctrl.GetOptions(field.Target);
                        AddPanel(field.input = CreateSelectbox($"{field.Value}", selectoptions, field.OnInput));
                        break;
                    }
            case "selectdictionary": //AddPanel(field.input = CreateSelectbox($"{field.Value}", field.GetSelectOptions(), field.OnInput)); break;
                
                    AddPanel((field.input = this.CreateWinFormControl(field))); break;
            case "engword":
            case "engtext":
            case "rustext":
            case "rusword": 
            case "phone": AddPanel(field.input = CreateInputPhone(field.Value, field.OnInput)); break;                
            case "email": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "date": AddPanel(field.input = CreateInputDate(field, field.Value, field.OnInput)); break;
            case "datetime": AddPanel(field.input = CreateInputDateTime(field.Value, field.OnInput)); break;
            case "multilinetext": AddPanel(field.input = factory.CreateMultilineTextBox($"{field.Value}", field.OnInput)); break;
            case "text": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "password": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
            case "custom":
                {
                    AddPanel(factory.CreateLabel(field.Label));
                        try
                        {
                            if (field.Value is null)
                            {

                                AddPanel(factory.CreateInputForm(field.Property.PropertyType));
                            }
                            else
                            {
                                AddPanel(factory.CreateInputForm(field.Value));
                            }
                        }catch(Exception ex)
                        {
                            this.Error("Ошибка при создании элемента управления custom" +
                                "");
                        }
                }
                break;
            default: //throw new NotImplementedException($"Не поддерживается тип ввода {field.Type}");
                    AddPanel(factory.CreateLabel($"Не поддерживается тип ввода {field.Type}"));
                    break;
        }

        var label = CreateBaseLabel("test");
        label.Text = "123123";
        field.SetErrors(label);
        AddPanel(label);       
    }

    private Control CreateWinFormControl(InputField field)
    {
        try
        {                                 
            if (field.CtrlType is not null && field.CtrlType.GetTypeName() != nameof(ControlAttribute))
            {
                field.Control = field.CtrlType.CreateControl(field);
            }
                
            Control controller = field.CtrlType.CreateWinFormsControl(field);                
            return controller;
        }catch(Exception ex)
        {
            return new Button() { Text = "Сообщение" };
        }
    }

    private Control CreateInputColor(object value, Action<string> onInput)
    {
        List<string> colorNames = new();
        foreach(Color color in Enum.GetValues(typeof(Color)))
        {
            colorNames.Add(color.Name);
        }
        return this.CreateSelectbox("black", colorNames, onInput);
    }

    private Control CreateInputUrl(object value, Action<string> onInput)
    {
        return CreateTextbox(value, onInput, errors => { }, (text) => {
            return text is not null && text.IsUrl() == false? "На является URL даресом": null;
        });
    }

    private Control CreateInputDateTime(object value, Action<string> onInput)
    {
        return this.CreateInputDate(null, value, onInput);
    }

    private Control CreateInputPhone(object value, Action<string> onInput)
    {
        MaterialTextBox2 box = null;
        box = CreateTextbox(value, onInput, 
            errors => {
                foreach(var error in errors)
                    System.Console.WriteLine(error);
            }, 
            (text) => {                             
                if (onInput is not null)
                {
                    onInput($"{value}");
                }                                      
                return null;
        });
            
        return box;
    }

    private Control CreateSelectIcon(object value, Action<string> onInput)
    {
        List<string> icons = new()
        {
            "home","login","logout","back","messages","help","edit","wait","error" 
        };
        return factory.CreateSelectList(icons, onInput);
    }

    private Control CreateInputDate(InputField field, object value, Action<string> onInput)
    {
        DateTime? dateVal = (value is not null && value is DateTime) ? ((DateTime)value) :
            $"{value}".IsDate() ? $"{value}".ToDate() : null;
        var textbox = this.factory.CreateInputDate(dateVal, changed => {
            if(onInput is not null)
            {
                onInput(changed.ToString("dd.MM.yyyy"));
            }

        });
        return textbox;
    }

    public Control CreateInputPasswordControl(object value, Action<string> oninput)
    {
        var textBox = new InputPasswordTextBox();
        textBox.Text = $"{value}";

        string text = "";
        textBox.TextChanged += (sender, evt) =>
        {
            value = textBox.Text;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "";
            }
            else
            {
                string s = "";
                for (int i = 0; i < textBox.Text.Length; i++)
                {
                    s += "*";
                }
                text = s;
                textBox.Text = s;
            }
            oninput(textBox.GetPassword());
        };
        return textBox;
    }

    private Control CreateSelectbox2(string value, IEnumerable<string> options, Action<string> onInput)
    {
        var box = new MaterialComboBox();
        foreach (var option in options)
        {
            box.Items.Add(option);
        }
        box.SelectedItem = value;
        box.SelectedIndexChanged += (s, e) => { onInput($"{box.SelectedItem}"); };
        return box;
    }

    public MaterialCheckbox CreateCheckbox(bool @checked, Action<string> onInput)
    {
        var checkbox = new MaterialCheckbox();
        checkbox.Checked = @checked;
        checkbox.CheckedChanged += (s, e) =>
        {
            onInput(checkbox.Checked.ToString());
        };
        return checkbox;
    }

    public MaterialComboBox CreateSelectbox(string value, IEnumerable<string> options, Action<string> onInput)
    {
        var control = new MaterialComboBox();

        
        int index = options.ToList().IndexOf(value);
        //if (index == -1)
        //    throw new ArgumentException(nameof(value), "Значение не содержиться в списке допустимых");
        foreach (var option in options)
        {
            control.Items.Add(option);
        }
        control.SelectedIndex = index;
        control.SelectedIndexChanged += (s, e) =>
        {
            onInput(control.SelectedItem?.ToString());
        };
        return control;
    }

    public MaterialTextBox2 CreateTextbox(object value, Action<string> oninput)
    {
        var textbox = new MaterialTextBox2();
        textbox.Text = $"{value}";
        textbox.TextChanged += (s, e) => { oninput(textbox.Text); };
        return textbox;
    }

    public MaterialTextBox2 CreateTextbox(object value, Action<string> oninput, Action<List<string>> onerrors, params Func<string, string>[] filters)
    {
        var textbox = new MaterialTextBox2();
        textbox.Text = $"{value}";
        string temp = null;
        textbox.TextChanged += (s, e) => {
            //this.Info(textbox.Text);
            if(filters is not null)
            {
                try
                {
                    var errors = filters.Select(filter => filter(textbox.Text)).Where(err => !String.IsNullOrWhiteSpace(err)).ToList();
                    if (errors.Count() > 0 && onerrors is not null)
                    {
                        onerrors(errors);
                    }
                    else
                    {
                        if (oninput is not null)
                            oninput(textbox.Text);
                    }
                }    
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                if (oninput is not null)
                    oninput(textbox.Text);
            }                                    
        };
        return textbox;
    }


    public MaterialTextBox2 CreateInputNumber(object value, Action<string> oninput)
    {
        var textbox = new MaterialTextBox2();
        textbox.Text = $"{value}";
        string temp = "";
        textbox.TextChanged += (s, e) =>
        {
            if (!textbox.Text.IsNumber())
            {
                textbox.Text = temp;
            }
            else
            {
                temp = textbox.Text;
            }
            oninput(textbox.Text);
        };
        return textbox;
    }


    [JsonIgnore]
    private static ConcurrentDictionary<string, Dictionary<string,InputField>> Fields { get; set; } = new();
    public IEnumerable<InputField> GetFormFields(Type ptype, IEnumerable<string> properties )
    {
        var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var key = ptype.GetTypeName();
        if (!Fields.ContainsKey(key))            
            Fields[key] = new();
            
                
        var ctrl = this;
        InputField pfield = null;
        fields = new List<InputField>();
          
        foreach (var property in properties)
        {
            /*if(Fields[key].ContainsKey(property) && !fields.Any(f => f.Name== property))
            {
                fields.Add(Fields[key][property]);
            }
            else
            {  * /                 
                string pname = property;
                pfield = GetFormField(ptype, property, val =>
                {
                    if (this.target is not null)
                        TextDataSetter.SetValue(this.target, pname, val);
                    ctrl.OnInput(property, val);
                });
                //fields.Add(pfield);                    
            //}                
        }           
        long ms = DateTimeOffset.Now.ToUnixTimeMilliseconds() - started;
        fields.Sort((p1, p2) =>
        {
            return
            p1.Order != p2.Order ? p1.Order - p2.Order :
            p1.Order == p2.Order ?
            p1.IsCollection && !p2.IsCollection ? -1 :
            !p1.IsCollection && p2.IsCollection ? 1 :
                0 : 0;
        });                      
        return this.fields;
    }


    public InputField GetFormField(Type ptype,  string property, Action<string> onchange)
    {
        InputField pfield;
        InputField p = null;
        fields.Add(p = pfield = new InputField() { });
        pfield.Name = property;
        pfield.Help = ptype.GetPropertyHelp(property);
        pfield.Property = ptype.GetProperty(property);
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
            pfield.SetErrors("asdfdsf");
            if ((target is not null) && $"{target.GetValue(p.Name)}" != text)
                {
                    TextDataSetter.SetValue(target, p.Name, text);
                    onchange(text);
                }                               
        };
        pfield.Type = GetInputType(ptype, property);
        pfield.ValueType = GetValueType(ptype, property);
        pfield.Value = target.GetValue(property);                        
        pfield.CtrlType = ptype.GetPropertyControl(property);
        if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
            p.Control = pfield.CtrlType.CreateControl(p);            
        var orederStr = ptype.GetPropertyAttribute<InputOrderAttribute>(property, "0");
        pfield.Order = orederStr.ToInt();
        pfield.OnInput += (text) => { };
        if (pfield.IsCollection && CommonUtils.IsPrimitive(pfield.ValueType.ToType()))
        {
            pfield.Type = "primitivecollection";
        }

        return pfield;
    }

    private string GetValueType(Type ptype, string property)
    {
        return ptype.GetProperty(property).PropertyType.IsCollectionType() ?
            CommonUtils.ParseCollectionType(ptype.GetProperty(property).PropertyType) :
            ptype.GetProperty(property).PropertyType.GetTypeName();
    }
        
        
    public string GetInputType(Type type, string property)
    {
        var attribute = type.GetPropertyAttributes(property).Where(kv => kv.Key.ToType().IsExtends(typeof(BaseInputAttribute))).Select(kv => kv.Key.Replace("Input", "").Replace("Attribute", "").ToLower()).FirstOrDefault();
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

    public void AddTitle(string title)
    {
        content.LayoutTop(CreateTitle(title));
    }

    public string GetTitle(Type ptype)
    {
        var label = ptype.GetLabel();
        return label is null ? ptype.GetTypeName() : label;
    }

       

    public MaterialLabel CreateLabel(string text)
    {
        var label = new MaterialLabel();
        label.Text = text;
        return label;
    }
    public Label CreateTitle(string text)
    {
        var label = new Label();
        label.Text = text;
        label.Font = new Font("Arial", 16);
        label.TextAlign = ContentAlignment.MiddleCenter;
        label.Height = 25;
        return label;
    }
    public Label CreateBaseLabel(string text)
    {
        var label = new Label();
        label.Text = text;
        label.ForeColor = Color.Red;
        return label;
    }
}
*/















