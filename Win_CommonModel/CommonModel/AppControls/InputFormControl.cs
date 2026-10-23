using MaterialSkin.Controls;

using System.Collections.Concurrent;

using System.Data;
using System.Text.Json.Serialization;

using WinFormsApp.AppComponents.DataComponents;
using WinFormsApp.AppViews.BaseComponent;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppComponents.AppControls;
using WinFormsDesignerApp.Console_DataModule.InputFormsAPI;
using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

using WinFormsApp.AppViews.InputModule;
using WinFormsApp.AppViews.FormModule;

using WinFormsDesignerApp.Win_MaterialModule;

namespace WinFormsDesignerApp.WinModule.InputModule
{
    public partial class InputFormControl : UserControl
    {
        

        //отступ от верхнего края
        public int offsetTop = 0;
        public object target = new FormViewModel();

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<FieldView> fields;

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public FlowLayoutPanel content = new() { AutoScroll = true };
        public string ErrorMessage
        {
            get
            {
                return ErrorMessageLabel is null ? "" : ErrorMessageLabel?.Text;
            }
            set
            {
                if (ErrorMessageLabel is not null)
                    ErrorMessageLabel.Text = value;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private Label ErrorMessageLabel;

        public static InputFormControl Create(Type ptype, Action<bool, object> oncompleted)
        {
            var form = new InputFormControl();
            form.InfoTime("Конструктор InputPanel", () => {

                form.AutoScroll = true;
                form.SetVerticalScroll();
                form.AutoScroll = true;
                form.target = ptype.New();
                form.oncompleted = oncompleted;
                form.ptype = ptype;
                form.properties = ptype.GetInputPropertiesSorted();
                form.FormatMarginsAndPaddings();
                form.content.SetVerticalScroll();
                var errors = form.target.Validate();
                var valid = errors.Count(kv => kv.Value.Where(err => !String.IsNullOrWhiteSpace(err)).Count() > 0) == 0;
                //this.Info(errors);
                form.ok.Enabled = valid;
                form.UpdateErrorMessages(errors);
            });
            return form;
        }
        private bool useStrictValidation = false;

        public void SetStrictValidation()
        {
            this.useStrictValidation = true;
        }

        public static InputFormControl Create(object target, Type ptype, Action<bool, object> oncompleted, List<string> properties = null)
        {
            var form = new InputFormControl();

            form.InfoTime("Конструктор InputPanel", () => {

                form.AutoScroll = true;
                form.SetVerticalScroll();
                form.AutoScroll = true;
                form.target = target;
                form.oncompleted = oncompleted;
                form.ptype = ptype;
                form.properties = properties is null ? ptype.GetInputProperties() : properties;
                form.FormatMarginsAndPaddings();
                form.content.SetVerticalScroll();
                var errors = target.Validate();
                var valid = errors.Count(kv => kv.Value.Where(err => !String.IsNullOrWhiteSpace(err)).Count() > 0) == 0;
                //this.Info(errors);
                form.ok.Enabled = valid;
                form.UpdateErrorMessages(errors);
            });
            return form;
        }
        public static InputFormControl Create(object target, List<string> properties, Type ptype, Action<bool, object> oncompleted)
        {
            var form = new InputFormControl();
            form.InfoTime("Конструктор InputPanel", () => {

                form.AutoScroll = true;
                form.SetVerticalScroll();
                form.AutoScroll = true;
                form.target = target;
                form.oncompleted = oncompleted;
                form.ptype = ptype;
                form.properties = properties;
                form.FormatMarginsAndPaddings();
                form.content.SetVerticalScroll();
                var errors = target.Validate();
                var valid = errors.Count(kv => kv.Value.Where(err => !String.IsNullOrWhiteSpace(err)).Count() > 0) == 0;
                //this.Info(errors);
                form.ok.Enabled = valid;
                form.UpdateErrorMessages(errors);
            });
            return form;
        }
        public InputFormControl()
        {
            InitializeComponent();
            this.AutoScroll = true;
            this.content.SetVerticalScroll();
        }
     

        public void Init()
        {
            try
            {
                /*if(ptype.Assembly != typeof(WinDesigner.Program).Assembly)
            {
                throw new Exception($"Используйте типы из основной сборки программы {ptype.GetTypeName()} {ptype.Assembly}");
            }*/
                
                if (target is null)
                    throw new NullReferenceException(nameof(target));
                content.Controls.Clear();
                this.Info(nameof(Init) + "" + target.GetType().GetTypeName());

                this.offsetTop = 0;

                
                this.target = target;
                Completed += oncompleted;

                /*var card = new MaterialCard();
                this.AddFill(card);
                card.Controls.Add(content);
                content.Height = card.Height - 40;
                content.Width = card.Width - 40;
                card.Resize += (s, e) => {
                    content.Left = 20;
                    content.Left = 20;
                    content.Height = card.Height - 40;
                    content.Width = card.Width - 40;
                };*/

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
                UpdateFormFields();

                OnInput += (k, v) =>
                {

                    //System.Threading.Tasks.Task.Run(() => {
                    Update(k, v);
                    //});
                };

                this.AutoScroll = true;
                this.content.AutoScroll = true;
                AddControls(Completed);
                try
                {
                    var errors = target.Validate();
                    var valid = errors.Count(kv => kv.Value.Count() > 0) == 0;
                    if (ok is not null)

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
                catch (Exception ex)
                {
                    this.Error($"Ошибка при валидации {ex}");
                }

                /*List<Control> ctrls = new();
                foreach(Control control in this.content.Controls)
                {
                    ctrls.Add(control);
                }
                ctrls.Reverse();
                ctrls.ForEach(item=> AddPanel(item));*/
            }
            catch (Exception ex)
            {
                this.Error(ex);
                throw;
            }

        }

        public void UpdateFormFields()
        {
            InfoTime("Обработка свойств объекта", () =>
            {

                foreach (FieldView field in fields)
                {
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

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        protected global::ControlsFactory factory = new global::ControlsFactory();

        public void AddConsoleMessage(string debugMessage)
        {
            var label = factory.CreateLabel(fields.ToJsonOnScreen());
            this.LayoutTop(label);
        }

        public Control AddPanel(Control control)
        {
            this.Add(control);
            /*content.BackColor = control.BackColor;
            SetDefaults(control);
            content.Controls.Add(control);
            /*control.Location = new(0, offsetTop);
            Controls.Add(SetDefaults(control));* /
            offsetTop = control.Top + control.Height;
            control.Top = offsetTop;
            this.Height = content.GetControls().Max(ctrl => (ctrl.Top + ctrl.Height)) + 150;*/
            return control;
        }

        private void Add(Control control)
        {
            this.columnsPanel.Add(control);
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<bool, object> Completed = (arg1, arg2) =>
        {
        };

        private void AddErrorMessage(string errorMessage)
        {
            AddPanel(ErrorMessageLabel = CreateBaseLabel(errorMessage));
        }

        public void UpdateErrorMessages(Dictionary<string, List<string>> errors)
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

            /*Task.Run(() =>
            {*/
            try
            {
                string message = "";
                TextDataSetter.SetValue(target, key, value);
                var errorsMessages = target.Validate(key);
                if (this.useStrictValidation)
                {
                    //TODO
                }
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
            this.Error(ex);
            var dialog = new DialogService();
            
            var text = ex.ToTextDocument();
            var ctrl = factory.CreateMultilineTextBox(text, changed => { });
            var form = dialog.CreateSubmitDialog("Перехвачено исключение", ctrl, submited => {
            });
            form.ShowDialog();
        }

        public void LayoutHeight()
        {
            int h = 0;
            foreach (Control control in Controls)
            {
                control.Location = new Point(0, h += control.Height);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<string, string> OnInput = (k, v) =>
        {
            System.Console.WriteLine($"{k}={v}");
        };

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public MaterialButton ok;
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public MaterialButton cancel;
        private List<string> _properties = new();

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<string> properties
        {
            get => _properties;
            set
            {
                _properties = value is null ? null : value.ToList();
                if (properties is not null && properties.Count()>0 && target is not null)
                {
                    try
                    {
                        ptype = target.GetType();
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
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        protected Action<bool, object> oncompleted;
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private Type ptype;
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
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
            var flow = new FlowLayoutPanel();
            flow.FlowDirection = FlowDirection.LeftToRight;
            controlsPanel = flow;
            ok = CreateButton("Подтвердить", () => { if (oncompleted is not null) oncompleted(true, target); });
            ok.Height = 22;
            cancel = CreateButton("Отмена", () => { if (oncompleted is not null) oncompleted(false, target); });
            cancel.Height = 22;
            ok.Anchor = AnchorStyles.Left;
            cancel.Anchor = AnchorStyles.Left;
            controlsPanel.Controls.Add(ok);
            controlsPanel.Controls.Add(cancel);

            this.LayoutBottom(controlsPanel);
            /*controlsPanel = new Panel();
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
            this.LayoutBottom(controlsPanel);*/
        }

        public MaterialButton CreateButton(string text, Action onclick)
        {
            var button = new MaterialButton() { Text = text, Width = 100 };
            button.Click += (s, e) => { onclick(); };
            return button;
        }

        public void AddFormField(FieldView field)
        {

            InfoTime($"Добавление поля {field.Name}", () => {
                field.label = CreateLabel(field.Label);

                if (!new List<string>() { "bool", "boolean", "logic", "checkbox" }.Contains(field.Type))
                    AddPanel(field.label);
                //AddPanel(CreateLabel(field.Type));
                field.label.Text = "    " + field.label.Text;
                if (false)//field.CtrlType is not null)
                {
                    /*try
                    {
                        AddPanel((field.input = this.CreateWinFormControl(field)));
                    }
                    catch (Exception e)
                    {
                        this.Error(e);
                    }*/
                }
                else
                {
                    if (field.Name == nameof(ViewItem.DatasetId))
                    {
                        int x = 0;
                        System.Console.WriteLine(1);
                    }
                    if (field.Type.StartsWith("constructor"))
                    {
                        var gentype = field.Type.Substring(field.Type.IndexOf("<") + 1, (field.Type.IndexOf(">") - field.Type.IndexOf("<") - 1)).ToType();
                        var sf = ServiceFactory.Get();
                        var exttypes = sf.GetTypesExtended(gentype);
                        var extnames = exttypes.Select(p => p.GetTypeName()).ToList();
                        
                        Control list = null;
                        Action update = () => {

                        };
                        Action<string> onTypeChanged = typeName => {
                            var target = typeName.New();
                            field.Value = target;
                        };
                        list = factory.CreateSelectbox(extnames, selected => onTypeChanged(selected));
                        var panel = list.ToPanel();
                        if (field.Value is not null)
                        {
                            panel.LayoutRight(factory.CreateButton("Редактировать", () => {
                                Control editPanel = null;
                                panel.LayoutBottom(editPanel = factory.Edit(field.Value, (s, p) => {
                                    if (s)
                                    {
                                        field.Value = p;
                                    }
                                    editPanel.Parent.Controls.Remove(editPanel);
                                }));
                            }));
                        }
                        /*cf.CreateSelectList(extnames, 
                        {
                            this.Controls.Remove(list);                      
                        });*/
                        AddPanel(list);

                    }
                    else
                    {
                        switch (field.Type)
                        {

                            case "interfacecollection<viewbase>":
                                {

                                    Control view = null;
                                    var attrs = field.GetAttribute<InputInterfaceCollectionAttribute<AppViewBase>>();
                                    var types = attrs.GetOptions();

                                    MaterialCheckedListBox checkList = null;
                                    var tp = new ColumnsPanel();

                                    var panel = new ColumnsPanel();
                                    var buttons = new ColumnsPanel() { Mode = true };
                              
                                    Button deleteButton = null;
                                    List<int> checkedList = new();
                                    Control labelRef = null;
                                    tp.Add(labelRef = factory.CreateLabel("Тип элемента"));


                                    buttons.Height = 25;
                                    tp.Height = 0;
                                    Action<int, bool> onchecked = (s, e) => {
                                        if (e)
                                            checkedList.Add(s);
                                        else checkedList.Remove(s);
                                        deleteButton.Enabled = checkedList.Count > 0;
                                    };
                                    buttons.Add(factory.CreateButton("добавить", () => {
                                        try
                                        {
                                            Dictionary<string, string> map = new(types.Select(type => new KeyValuePair<string, string>(type, type.ToType().GetLabel())));
                                            string typeOfNewService = null;
                                            string p = null;


                                            var flow = new Panel() { Dock = DockStyle.Top };
                                            var selectbox = CreateMaterialSelectbox("Класс", map.Values.ToList(), selected => {
                                                p = selected;
                                            });

                                            flow.Height = selectbox.Height + selectbox.Margin.Top + selectbox.Margin.Bottom + 20;
                                            flow.LayoutFill(selectbox);
                                            flow.LayoutRight(factory.CreateButton("ок", () =>
                                            {
                                                tp.Height = 0;
                                                tp.Remove(flow);
                                                if (p is not null)
                                                {
                                                    typeOfNewService = map.FirstOrDefault(pp => pp.Value == p).Key;
                                                    var created = typeOfNewService.ToType().New<ViewItem>();
                                                    if (field.Value is null)
                                                    {
                                                        field.Value = field.DataType.New();
                                                        if (field.Target is not null)
                                                        {
                                                            field.Target.SetValue(field.Name, field.Value);
                                                        }
                                                    }
                                                    field.Value.Invoke("Add", new object[] { created });
                                                    var index = checkList.Items.Count();
                                                    var menuItem = new MaterialCheckbox();
                                                    menuItem.Text = created.GetTypeName();
                                                    menuItem.CheckedChanged += (s, e) => {
                                                        onchecked(index, menuItem.Checked);
                                                    };
                                                    checkList.Items.Add(menuItem);
                                                }
                                            }));
                                            tp.Height = 75;
                                            tp.Add(flow);
                                        }
                                        catch (Exception ex)
                                        {
                                            this.OnError(ex);
                                        }
                                    }));
                                    buttons.Add(deleteButton = factory.CreateButton("удалить", () => {
                                        checkedList = checkedList.OrderDescending().ToList();
                                        foreach (var next in checkedList)
                                        {
                                            checkList.Items.Remove(checkList.Items[next]);

                                            this.Info(field.Value.GetValue("Count"));
                                            this.Info(field.Value);
                                            field.Value.Invoke("RemoveAt", new object[] { next });

                                        }

                                        checkedList.Clear();
                                    }));

                                    deleteButton.Enabled = false;
                                    panel.Add(buttons);
                                    panel.Add(tp);

                                    var list = new List<string>();
                                    list.Clear();
                                    foreach (var item in (dynamic)field.Value)
                                    {
                                        string s = null;
                                        if (attrs.Expression is not null)
                                        {
                                            try
                                            {
                                                System.Console.WriteLine(item.GetType());
                                                s = ($"{Expressions.Compile(attrs.Expression, item)}");
                                            }
                                            catch (Exception ex)
                                            {
                                                this.Error(ex);
                                            }
                                        }
                                        else s = (item.GetType().GetLabel());
                                        if (s is null)
                                            s = (item.GetType().GetLabel());
                                    }


                                    checkList = factory.CreateCheckedListBox(new List<string>(list), (item, success) => {
                                        onchecked(item, success);
                                    });
                                    //checkList.ToBaseForm().ShowDialog();
                                    panel.Add(checkList);
                                    panel.AutoSize = true;
                                    this.AddPanel(panel);

                                    /*



                                    string selected = null;
                                    ColumnsPanel list = null;
                                    Func<string, Control> createButton = text => factory.CreateButton(text, () =>
                                    {
                                        list.Parent.Controls.Remove(list);
                                    });
                                    Action<object> add = p => {
                                    };
                                    list = factory.CreateListPanel(strings,
                                        createButton,
                                        text =>
                                        {
                                            selected = text;
                                        }
                                    );

                                    Action update = () => {
                                        try
                                        {
                                            list.Clear();
                                            foreach (var item in (dynamic)field.Value)
                                            {
                                                string s = null;
                                                if (attrs.Expression is not null)
                                                {
                                                    try
                                                    {
                                                        System.Console.WriteLine(item.GetType());
                                                        s = ($"{Expressions.Compile(attrs.Expression, item)}");
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        this.Error(ex);
                                                    }
                                                }
                                                else s = (item.GetType().GetLabel());
                                                if (s is null)
                                                    s = (item.GetType().GetLabel());
                                                list.Add(createButton(s));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            this.OnError(ex);
                                        }
                                    };
                                    var panel = new ColumnsPanel();
                                    panel.Fixed = false;
                                    Control buttons = null;
                                    buttons = factory.CreateButtons("добавить,удалить".Split(","), command =>
                                    {
                                        switch (command)
                                        {
                                            case "добавить":

                                                try
                                                {
                                                    Dictionary<string, string> map = new(types.Select(type => new KeyValuePair<string, string>(type, type.ToType().GetLabel())));
                                                    string typeOfNewService = null;
                                                    string p = null;


                                                    var flow = new FlowLayoutPanel() { Dock = DockStyle.Top };
                                                    flow.FlowDirection = FlowDirection.LeftToRight;
                                                    var selectbox = CreateMaterialSelectbox("Класс", map.Values.ToList(), selected => {
                                                        p = selected;
                                                    });
                                                    selectbox.Dock = DockStyle.Fill;
                                                    selectbox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                                                    flow.Controls.Add(selectbox);
                                                    flow.Controls.Add(factory.CreateButton("ок", () =>
                                                    {
                                                        list.Controls.Remove(flow);
                                                        if (p is not null)
                                                        {
                                                            typeOfNewService = map.FirstOrDefault(pp => pp.Value == p).Key;
                                                            var created = typeOfNewService.ToType().New<ViewItem>();
                                                            if (field.Value is null)
                                                            {
                                                                field.Value = field.Property.PropertyType.New();
                                                                if (field.Target is not null)
                                                                {
                                                                    field.Target.SetValue(field.Name, field.Value);
                                                                }
                                                            }
                                                            field.Value.Invoke("Add", new object[] { created });
                                                            update();
                                                        }
                                                    }));

                                                    list.LayoutTop(flow);
                                                }
                                                catch(Exception ex)
                                                {
                                                    this.OnError(ex);
                                                }

                                                break;
                                            case "удалить":
                                                break;
                                        }
                                    });
                                    //list.LayoutTop(buttons);
                                    //panel.LayoutBottom(list.ToPanel());                          
                                    panel.Add(buttons);
                                    panel.Add(list);
                                    panel.Add(new Button() { Text = "2" });
                                    this.AddPanel(panel);
                                    /*
                                    var factory = new ComponentsFactory();

                                    var panel = new Panel();
                                    Button deleteButton = null;
                                    panel.LayoutLeft(deleteButton = factory.CreateButton("удалить", () => { }));
                                    panel.LayoutLeft(factory.CreateButton("добавить", () => {
                                        var temp = view;
                                        if (view is not null) side.Controls.Remove(view);
                                        Dictionary<string, string> map = new(types.Select(type => new KeyValuePair<string, string>(type, type.ToType().GetLabel())));

                                        side.LayoutFill(view = (factory.CreateSelectList(map.Values.ToList(), selected =>
                                        {
                                            var element = selected.ToType().New<ViewItem>();
                                            field.Value.Invoke("Add", new object[] { element });
                                            if (view is not null) side.Controls.Remove(view);
                                            side.LayoutFill(temp);
                                        })));
                                    }));




                                    if(field.Value is null)
                                    {
                                        field.Value = field.Type.New();
                                    }






                                    var form = new SubmitDialogPanel(
                                        "Выберите тип",
                                        list,
                                        (submitted) => {
                                            if (submitted)
                                            {
                                                var p = selected.ToType().New<ViewItem>();
                                                var phone = $"{Expressions.Compile(attrs.Expression, p)}";
                                                var item = new MaterialListBoxItem();
                                                item.Tag = phone;
                                                item.Text = phone;                               
                                                list.Items.Add(item);
                                                field.Value.Invoke("Add", new object[] { p });
                                            }
                                        });

                                    //this.AddPanel(side);


                                    side.LayoutFill(view=form);

                                    panel.Height = 40;

                                    side.LayoutTop(panel);
                                    this.AddPanel(side);*/
                                    break;
                                }
                            case "primitivecollection":
                                {
                                    AddPrimitiveCollectionField(field);

                                    break;
                                }
                            case "structurecollection":
                            case "structuredcollection":
                                {
                                    AddStructuredCollectionField(field);
                                    break;

                                    return;
                                }
                            case "color": //AddPanel(field.input = CreateInputColor(field.Value, field.OnInput)); break;
                            case "url":
                                AddInputUrl(field); break;
                            case "image":
                                {
                                    AddInputImage(field);
                                    break;
                                }

                            case "icon": AddInputIcon(field); break;

                            case "int": AddInputNumberInt(field); break;
                            case "percent": AddInputPercent(field); break;
                            case "money": AddInputMoney(field); break;
                            case "natural": AddInputNumberInt(field); break;
                            case "number": AddInputNumberInt(field); break;
                            case "bool":
                            case "boolean":
                            case "logic":
                            case "checkbox":
                                //((ColumnsPanel)field.label.Parent).Remove(field.label);
                                AddCheckBoxControl(field);
                                break;
                            case "dictionary":
                                AddDictionaryControl(field); break;
                            case "entitytype":
                                AddEntityTypeInput(field);
                                break;
                            case "select": AddSelectControl(field); break;
                            case "selectexpression":
                                {
                                    AddSelectExpressionControl(field);

                                    break;
                                }
                            case "selectdictionary":
                                var attribute = field.GetAttribute<SelectDictionaryAttribute>();
                                AddPanel(field.input = attribute.CreateWinFormsControl(field));

                                break;
                            case "combobox":
                                {
                                    field.input = this.CreateWinFormControl(field);
                                    field.AddPropertyChangeListener(nameof(field.Value), evt => {

                                    });
                                    var lpanel = factory.CreateFlexRow(
                                        new Panel() { Width = 0, Height = 0 },
                                        field.input,
                                        factory.CreateButton("создать", () => {
                                            var entityType = field.GetAttribute<ComboboxAttribute>().GetEntityType();
                                            var entityRef = entityType.New();
                                            entityRef.ToEditDialog((s, p) => {
                                                if (s)
                                                {
                                                    if (p is BaseEntity)
                                                    {
                                                        ((BaseEntity)p).Create();
                                                    }
                                                    var combobox = (ComboBox)this.CreateWinFormControl(field);
                                                    var current = (ComboBox)field.input;
                                                    current.Items.Clear();
                                                    foreach (var item in combobox.Items)
                                                    {
                                                        current.Items.Add(item);
                                                    }
                                                    current.SelectedIndex = current.Items.Count - 1;
                                                }
                                            });
                                        }));
                                    lpanel.Height = 25;

                                    AddPanel(lpanel);
                                    break;
                                }
                            case "engword":
                            case "engtext":
                            case "rustext":
                            case "rusword":
                            case "phone":
                                AddInputPhone(field); break;
                            case "email": AddInputEmailField(field); break;
                            case "date": AddInputDateField(field); break;
                            case "datetime": AddInputDateTimeField(field); break;
                            case "multilinetext": AddInputMultiTextField(field); break;
                            case "text": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
                            case "password": AddPanel(field.input = CreateTextbox(field.Value, field.OnInput)); break;
                            case "custom":
                                {
                                    AddPanel(factory.CreateLabel(field.Label));
                                    try
                                    {
                                        /*if (field.Value is null)
                                        {

                                            AddPanel(factory.CreateInputForm(field.Property.PropertyType));
                                        }
                                        else
                                        {
                                            AddPanel(factory.CreateInputForm(field.Value));
                                        }*/
                                    }
                                    catch (Exception ex)
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
                    }


                }

                field.AddPropertyChangeListener(nameof(Visible), evt => {
                    field.errors.Visible = evt.After.ToString().ToLower() == "true";
                    field.label.Visible = evt.After.ToString().ToLower() == "true";
                    field.input.Visible = evt.After.ToString().ToLower() == "true";
                });
                /*var label = CreateBaseLabel("test");
                label.Text = "123123";
                field.SetErrors(label);
                AddPanel(label);8*/
            });
        }

        private void AddInputMultiTextField(FieldView field)
        {
            AddPanel(field.input = factory.CreateMultilineTextBox($"{field.Value}", field.OnInput));
        }

        private void AddInputDateTimeField(FieldView field)
        {
            AddPanel(field.input = CreateInputDateTime(field.Value, field.OnInput));
        }

        private void AddInputDateField(FieldView field)
        {
            AddPanel(field.input = CreateInputDate(field, field.Value, field.OnInput));
        }

        private void AddInputEmailField(FieldView field)
        {
            AddPanel(field.input = CreateTextbox(field.Value, field.OnInput));
        }

        private void AddInputPhone(FieldView field)
        {
            AddPanel(field.input = CreateInputPhone(field.Value, field.OnInput));
        }

        private void AddSelectDictionatyControl(FieldView field)
        {
            //AddPanel(field.input = CreateMaterialSelectbox($"{field.Value}", field.GetSelectOptions(), field.OnInput));
            AddPanel(field.input = CreateMaterialSelectbox($"{field.Value}", field.GetSelectOptions(), field.OnInput));

        }

        private void AddSelectExpressionControl(FieldView field)
        {
            var ctrl = typeof(InputSelectExpressionAttribute).New<InputSelectExpressionAttribute>(new object[] { field.Attributes.First(kv => kv.Key == nameof(InputSelectExpressionAttribute)).Value });
            var value = field.Target.GetValue(ctrl.GetExpression());
            var selectOptions = (ChangingList<string>)value;
            AddPanel(field.input = CreateMaterialSelectbox($"{field.Value}", selectOptions, field.OnInput));
            field.AddPropertyChangeListener(nameof(field.Value), evt => {
                MaterialComboBox combobox = (MaterialComboBox)field.input;

                if (evt.After?.ToString() != combobox.SelectedItem?.ToString())
                {
                    combobox.SelectedIndex = selectOptions.IndexOf(evt.After?.ToString());
                }
                /*
                 * 
                 *            MaterialComboBox combobox = (MaterialComboBox)field.input;
                selectoptions = ctrl.GetOptions(field.Target);
                combobox.Items.Clear();
                var created = CreateMaterialSelectbox($"{evt.After?.ToString()}", selectoptions, field.OnInput);
                foreach(var item in created.Items)
                {
                    combobox.Items.Add(item);
                }
                */
            });
        }

        private void AddSelectControl(FieldView field)
        {
            AddPanel(field.input = CreateMaterialSelectbox($"{field.Value}", field.GetSelectOptions(), field.OnInput));
        }

        private void AddEntityTypeInput(FieldView field)
        {
            var options = field.Attributes.FirstOrDefault(kv => kv.Key.ToType().IsExtends(typeof(InputSelectAttribute))).Key.ToType().New<InputSelectAttribute>(new object[] { }).Options;
            AddPanel(field.input = CreateMaterialSelectbox($"{field.Value}", options, field.OnInput));
        }

        private void AddDictionaryControl(FieldView field)
        {
            AddPanel(field.input = CreateMaterialSelectbox($"{field.Value}", field.GetSelectOptions(), field.OnInput));
        }

        private void AddCheckBoxControl(FieldView field)
        {
            field.input = CreateCheckbox(field.Value is not null && field.Value is bool ? (bool)field.Value : false, field.OnInput);
            field.input.Text = field.Label;
            AddPanel(field.input);
            /*var panel = new ColumnsPanel() { Mode = true };
            panel.Add(field.label);
            panel.Add(field.input);

            AddPanel(panel);*/
        }


        private void AddInputMoney(FieldView field)
        {
            AddPanel(field.input = CreateTextbox(field.Value, field.OnInput));
        }

        private void AddInputPercent(FieldView field)
        {
            AddPanel(field.input = CreateTextbox(field.Value, field.OnInput));
        }

        private void AddInputNumberInt(FieldView field)
        {
            AddPanel(field.input = CreateTextbox(field.Value, field.OnInput));
        }

        private void AddInputIcon(FieldView field)
        {
            AddPanel(field.input = CreateSelectIcon(field.Value, field.OnInput));
        }

        private void AddInputImage(FieldView field)
        {
            var panel = new ColumnsPanel();

            var data = (field.Value is not null && field.Value is byte[] && ((byte[])field.Value).Length > 0) ? ((byte[])field.Value) : new byte[0];
            var image = this.factory.CreateImage(data);
            panel.Add(image);
            var fds = new FileDialogService();
            Action<string> showImage = null;

            showImage = (file) =>
            {
                var data = System.IO.File.ReadAllBytes(file);
                field.Value = data;
                target.SetValue(field.Name, field.Value);
                panel.Remove(image);
                image = this.factory.CreateImage(data);
                image.Click += (s, e) =>
                {
                    fds.OpenFile("Выберите изображение", "Image | *.png", System.IO.Directory.GetCurrentDirectory(), showImage);
                };
                image.Size = new(280, 320);

                panel.Add(image);
            };
            image.Click += (s, e) => {
                fds.OpenFile("Выберите изображение", "Image | *.png", System.IO.Directory.GetCurrentDirectory(), showImage);
            };

            AddPanel(field.input = panel);
        }

        private void AddInputUrl(FieldView field)
        {
            AddPanel(field.input = CreateInputUrl(field.Value, field.OnInput));
        }

        private void AddStructuredCollectionField(FieldView field)
        {
            this.Info(field.Name);
            var itemType = field.ValueType.ToType();
            if (itemType is null)
                throw new Exception("structuredcollection требует указаание типа field.ValueType");
            if (itemType is BaseEntity)
            {
                dynamic target = field.Value is not null ? field.Value : typeof(List<>).MakeGenericType(itemType).New();
                List<object> items = new();
                foreach (var item in target)
                    items.Add(item);
                var grid = factory.CreateGridView<object>(items, new List<string>() { });
                //установка идентифкатора внешенего ключа
                Action<object> onadd = item =>
                {
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
                content.Controls.Add(p = this.factory.CreateCheckBox(field.Label, checkedState =>
                {
                    if (!checkedState)
                    {
                        content.Controls.Remove(p);
                    }
                    else
                    {
                        field.Value = field.Value is null ? field.DataType.New() : field.Value;
                        Control ctrl = typeof(CrudInMemoryController<>).MakeGenericType(itemType).New<Control>(new object[] {
                                field.Value, onadd, null, null
                            });
                        var typeOfEntity = field.ValueType.ToType();
                        var dataset = new List<object>();

                        System.Console.WriteLine(dataset.Count());
                        var buttons = factory.CreateButtons("".Split(), clicked =>
                        {
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
                Panel panel = new Panel();
                Control p = null;
                var checkbox = this.factory.CreateCheckBox(field.Label, checkedState =>
                {
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
                        var buttons = factory.CreateButtons("создать,удалить".Split(","), clicked =>
                        {
                            switch (clicked)
                            {
                                case "создать":
                                    panel.Controls.Remove(editor);
                                    Control temp = null;
                                    panel.Controls.Add(temp = factory.CreateInputFormForType((success, data) =>
                                    {
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
                panel.LayoutFill(checkbox.ToPanel());
                content.LayoutBottom(panel);
                content.Resize += (s, e) =>
                {
                    panel.Width = content.Width;
                };
                var pctrl = panel.Parent;
                while (pctrl is not null)
                {
                    System.Console.WriteLine($"{pctrl.GetTypeName()}: {pctrl.Width}");
                    pctrl = pctrl.Parent;
                }


                panel.Width = 240;
                panel.Height = 120;
            }
        }

        public void AddPrimitiveCollectionField(FieldView field)
        {
            Panel p = null;
            Control control = null;
            if (field.Value is null)
                field.Value = new List<object>();
            MaterialListBox list = null;
            var slider = new Panel();
            field.input = factory.CreateCheckBox(field.Label, val =>
            {
                int temp = 100;
                if (val)
                {
                    slider.Controls.Clear();
                    slider.LayoutTop(field.input);
                    var values = new List<string>();
                    foreach (var item in (dynamic)field.Value)
                    {
                        values.Add(item.ToString());
                    }
                    slider.LayoutTop(factory.CreateButton("создать", () =>
                    {

                       
                        factory.CreateTextbox("", changed =>
                        {
                        }).ToSubmit("Новый элемент", (success, ctrl) =>
                        {
                            if (success)
                            {
                                values.Add(ctrl.Text);
                                slider.Controls.Remove(list);
                                slider.AddFill(list = factory.CreateMaterialList(values, selected => {

                                }));
                            }

                        }).ToBaseForm().ShowDialog();
                    }));
                    Button del = null;
                    slider.LayoutTop(del = factory.CreateButton("удалить", () => {
                        list.Items.RemoveAt(list.SelectedIndex);
                        del.Enabled = false;
                    }));
                    slider.AddFill(list = factory.CreateMaterialList(values, selected => {
                        del.Enabled = (selected is not null);
                    }));

                    p.LayoutFill(slider);
                    temp = p.Height;
                    p.Height = 320;
                }
                else
                {
                    slider.Controls.Remove(field.input);
                    p.Controls.Remove(slider);
                    p.LayoutFill(field.input);
                    p.Height = temp;
                }
            });
            AddPanel(p = field.input.ToPanel());

            p.Height = 100;
        }

        private Control CreateWinFormControl(FieldView field)
        {
            try
            {
                if (field.CtrlType is not null && field.CtrlType.GetTypeName() != nameof(ControlAttribute))
                {
                    field.Control = field.CtrlType.CreateControl(field);
                }

                Control controller = field.CtrlType.CreateWinFormsControl(field);
                return controller;
            }
            catch (Exception ex)
            {
                return new Button() { Text = "Сообщение" };
            }
        }

        private Control CreateInputColor(object value, Action<string> onInput)
        {
            List<string> colorNames = new();
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                colorNames.Add(color.Name);
            }
            return this.CreateMaterialSelectbox("black", colorNames, onInput);
        }

        private Control CreateInputUrl(object value, Action<string> onInput)
        {
            return CreateTextbox(value, onInput, errors => { }, (text) => {
                return text is not null && text.IsUrl() == false ? "На является URL даресом" : null;
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
                    foreach (var error in errors)
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
            return factory.CreateMaterialList(icons, onInput);
        }

        private Control CreateInputDate(FieldView field, object value, Action<string> onInput)
        {
            DateTime? dateVal = (value is not null && value is DateTime) ? ((DateTime)value) :
                $"{value}".IsDate() ? $"{value}".ToDate() : null;
            var textbox = this.factory.CreateInputDate(dateVal, changed => {
                if (onInput is not null)
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
            var checkbox = new MaterialCheckbox() { };
            checkbox.Checked = @checked;
            checkbox.CheckedChanged += (s, e) =>
            {
                onInput(checkbox.Checked.ToString());
            };
            return checkbox;
        }

        public MaterialComboBox CreateMaterialSelectbox(string value, IEnumerable<string> options, Action<string> onInput)
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

        public MaterialComboBox CreateMaterialSelectboxWithCreateAction(Type tmodel, string value, IEnumerable<string> options, Action<string> onInput)
        {
            var control = new MaterialComboBox();


            int index = value is null ? 0 : options.ToList().IndexOf(value);
            //if (index == -1)
            //    throw new ArgumentException(nameof(value), "Значение не содержиться в списке допустимых");
            foreach (var option in options)
            {
                control.Items.Add(option);
            }
            control.Items.Add("Создать");
            control.SelectedIndex = index;
            control.SelectedIndexChanged += (s, e) =>
            {
                if (control.SelectedItem?.ToString() == "Создать")
                {
                    Form form = null;
                    form = factory.Create(tmodel, (s, p) => {
                        form.Close();
                        if (s)
                        {

                        }
                    }).ToBaseForm();
                    form.ShowDialog();
                }
                else
                {
                    onInput(control.SelectedItem?.ToString());
                }

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
                if (filters is not null)
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
                    catch (Exception ex)
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


        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private static ConcurrentDictionary<string, Dictionary<string, FieldView>> Fields { get; set; } = new();
        public IEnumerable<FieldView> GetFormFields(Type ptype, IEnumerable<string> properties = null)
        {
            if (properties is null)
                properties = ptype.GetInputPropertiesSorted();
            var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var key = ptype.GetTypeName();
            if (!Fields.ContainsKey(key))
                Fields[key] = new();

            var ctrl = this;
            FieldView pfield = null;
            fields = new List<FieldView>();

            foreach (var property in properties)
            {
                try
                {
                    this.Info(property);
                    string pname = property;
                    pfield = GetFormField(ptype, property, val =>
                    {
                        try
                        {
                            if (this.target is not null)
                                TextDataSetter.SetValue(this.target, pname, val);
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


        public FieldView GetFormField(Type ptype, string property, Action<string> onchange)
        {
            FieldView pfield;
            FieldView p = null;
            fields.Add(p = pfield = new FieldView() { });
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
                if ((target is not null) && $"{target.GetValue(p.Name)}" != text)
                {
                    TextDataSetter.SetValue(target, p.Name, text);
                    onchange(text);
                }
            };
            pfield.Type = GetInputType(ptype, property);
            pfield.ValueType = GetValueType(ptype, property);
            pfield.Value = target.GetValue(property);
            if (target is ChangeSupport)
            {

                ((ChangeSupport)target).OnChange += (PropertyChangedMessage evt) => {
                    var field = this.fields.FirstOrDefault(f => f.Name == evt.Property);
                    if (field is not null && field.Value != evt.After)
                    {
                        field.Value = evt.After;
                    }
                };
            }
            pfield.CtrlType = ptype.GetPropertyControl(property);
            if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
                p.Control = pfield.CtrlType.CreateControl(p);
            var orederStr = ptype.GetPropertyAttributeValue<InputOrderAttribute>(property, "0");
            if (property == "UserId")
            {
                int x = 0;
                System.Console.WriteLine(1);
            }
            var binding = ptype.GetPropertyAttribute<WinFormsDesignerApp.Console_InputModule.AttributesInput.InputBindingAttribute>(property);
            if (binding is not null)
            {
                if (target is not ChangeSupport)
                    throw new Exception("Атрибут InputBinding можно использовать только в моделях типа ChangeSupport");
                ChangeSupport support = (ChangeSupport)target;
                support.AddPropertyChangeListener(binding.ModelPropertyName, evt => {
                    //this.Info($"{binding.FieldPropertyName}={evt.After}");
                    pfield.SetValue(binding.FieldPropertyName, evt.After);
                });
                pfield.AddPropertyChangeListener(binding.FieldPropertyName, evt => {
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
            label.Font = new System.Drawing.Font("Arial", 18);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Height = 20;
            return label;
        }
        public Label CreateBaseLabel(string text)
        {
            var label = new Label();
            label.Text = text;
            label.ForeColor = Color.Red;
            return label;
        }

        public void InfoTime(string text, Action todo)
        {
            var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            todo();
            var ended = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.Info($"{text}: {ended - started} мс.");
        }
    }
}
