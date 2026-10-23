using Console_CommonModel.CommonLib.LibViews.AppViews;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using MaterialSkin.Controls;

using MaterialSurface;

using Win_BusinessApp.CommonModel;

using WinFormsApp.AppViews;

public static class LayoutExtensions
{

    public static Action BindHeightToParent(this Control ctrl)
    {
        var temp = ctrl.Height;
        var p = ctrl.Parent;
        if (p is null)
            throw new Exception("Элемент управления нужно разместить в контейнёр");
        ctrl.Height = p.Height - p.Padding.Top - p.Padding.Bottom;
        EventHandler onresize = (s, e) => ctrl.Height = p.Height - p.Padding.Top - p.Padding.Bottom;
        p.Resize += onresize;
        return () => { p.Resize -= onresize; ctrl.Height = temp; };
    }

    public static Action FixLeftRight(this Control ctrl, Control control)
    {
        var temp = ctrl.Width;
        ctrl.Left = control.Left + control.Width;
        EventHandler onlocationChanged = (s, e) => ctrl.Left = control.Left + control.Width;
        control.LocationChanged += onlocationChanged;
        control.Resize += onlocationChanged;
        return () => {
            control.Resize -= onlocationChanged;
            control.LocationChanged -= onlocationChanged;
            ctrl.Width = temp;
        };
    }

    public static Action FixTopBottom(this Control ctrl, Control control)
    {
        var temp = ctrl.Height;
        ctrl.Top = control.Top + control.Height;
        EventHandler onlocationChanged = (s, e) => ctrl.Top = control.Location.Y + control.Height; ;
        control.LocationChanged += onlocationChanged;
        control.Resize += onlocationChanged;
        return () => {
            control.LocationChanged -= onlocationChanged;
            control.Resize -= onlocationChanged;
            ctrl.Height = temp;
        };
    }


    public static Action BindWidthToParent(this Control ctrl)
    {
        var temp = ctrl.Width;
        var p = ctrl.Parent;
        ctrl.Width = p.Width - p.Padding.Left - p.Padding.Right;
        EventHandler onresize = (s, e) => ctrl.Width = p.Width - p.Padding.Left - p.Padding.Right;
        p.Resize += onresize;
        return () => {
            p.Resize -= onresize;
            ctrl.Width = temp;
        };
    }
    
    public static Action FillWithPadding(this Control ctrl)
    {
        var temp = ctrl.Width;
        var p = ctrl.Parent;
        
        ctrl.Width = p.Width - p.Padding.Left - p.Padding.Right;
        EventHandler onresize1 = (s, e) => ctrl.Width = p.Width - p.Padding.Left - p.Padding.Right;

        ctrl.Height = p.Height - p.Padding.Top - p.Padding.Bottom;
        EventHandler onresize2 = (s, e) => ctrl.Height = p.Height - p.Padding.Top - p.Padding.Bottom;
        EventHandler onresize = (s, e) => { 
            onresize1(s,e); 
            onresize2(s, e); 
            ctrl.Left = p.Padding.Left;
            ctrl.Top = p.Padding.Top;
        };
        p.Resize += onresize;
         
        return () => {
            p.Resize -= onresize;
        };
    }

    public static Action BindWidthToParent(this Control ctrl, Control p)
    {
        var temp = ctrl.Width;

        ctrl.Width = p.Width - p.Padding.Left - p.Padding.Right;
        EventHandler onresize = (s, e) => ctrl.Width = p.Width - p.Padding.Left - p.Padding.Right;
        p.Resize += onresize;
        return () => {
            p.Resize -= onresize;
            ctrl.Width = temp;
        };
    }

    public static Action BindWidthToParent(this AppViewBase ctrl, AppViewBase p)
    {
        var temp = ctrl.Width;

        ctrl.Width = p.Width - p.Padding - p.Padding;
        EventHandler onresize = (s, e) => ctrl.Width = p.Width - p.Padding - p.Padding;
        Action<object,EventArgs> pp = (s, e) => { onresize(s, e); };
        p.OnResized += pp;
        return () => {
            p.OnResized -= pp;
            ctrl.Width = temp;
        };
    }
    public static void SetVerticalScroll(this Panel flowLayoutPanel)
    {
        flowLayoutPanel.AutoScroll = false;
        flowLayoutPanel.HorizontalScroll.Enabled = false;
        flowLayoutPanel.HorizontalScroll.Visible = false;
        flowLayoutPanel.HorizontalScroll.Maximum = 0;
        flowLayoutPanel.AutoScroll = true;
    }
    public static void SetHorizontalScroll(this Panel flowLayoutPanel)
    {
        flowLayoutPanel.AutoScroll = false;
        flowLayoutPanel.VerticalScroll.Enabled = false;
        flowLayoutPanel.VerticalScroll.Visible = false;
        flowLayoutPanel.VerticalScroll.Maximum = 0;
        flowLayoutPanel.AutoScroll = true;
    }
    public static void SetVerticalScroll(this UserControl flowLayoutPanel)
    {
        flowLayoutPanel.AutoScroll = false;
        flowLayoutPanel.HorizontalScroll.Enabled = false;
        flowLayoutPanel.HorizontalScroll.Visible = false;
        flowLayoutPanel.HorizontalScroll.Maximum = 0;
        flowLayoutPanel.AutoScroll = true;
    }

    public static void SetHorizontalScroll(this UserControl flowLayoutPanel)
    {
        flowLayoutPanel.AutoScroll = false;
        flowLayoutPanel.VerticalScroll.Enabled = false;
        flowLayoutPanel.VerticalScroll.Visible = false;
        flowLayoutPanel.VerticalScroll.Maximum = 0;
        flowLayoutPanel.AutoScroll = true;
    }

    public static MaterialExpansionPanel ToExpansionPanel(this Control control, string text, Action<bool, object> onsubmit)
    {
        var panel = new MaterialExpansionPanel();
        panel.Title = text;
        return panel;
    }
    public static Control WithRight(this Control control, Control slider)
    {
        control.LayoutRight(slider);
        slider.Width = 420;
        return control;
    }

    public static void SetWidthInner(this Control control, Control panel)
    {
        control.Width = panel.Width - panel.Padding.Left - panel.Padding.Right;
    }
    public static void SetWidth(this Control control, Control panel)
    {
        control.Width = panel.Width;
    }


    public static Panel ToScrollPanel(this Control ctrl)
    {
        var panel = new Panel() { Margin = new(0), Padding = new(0) };
        panel.SetVerticalScroll();
        panel.HorizontalScroll.Visible = false;
        panel.VerticalScroll.Visible = true;
        ctrl.Width = panel.Width;
        panel.Resize += (s, e) => ctrl.Width = panel.Width - panel.Padding.Left - panel.Padding.Right;
        panel.Controls.Add(ctrl);
        return panel;
    }

    public static Panel ToPanel(this Control ctrl)
    {
        var panel = new Panel();
        ctrl.Dock = DockStyle.Fill;
        panel.Controls.Add(ctrl);
        return panel;
    }

    /*public static SidePanel ToSidePanel(this Control ctrl)
    {
        var panel = new SidePanel();
        var service = new SideService(panel);
        service.ToggleBottom();
        service.ToggleLeft();
        service.ToggleRight();
        service.ToggleTop();
        ctrl.Dock = DockStyle.Fill;
        service.SetView(ctrl);
        return panel;
    }*/
    public static void PutTopInPanel(this Control ctrl, Control panel)
    {
        panel.LayoutTop(ctrl);
    }
    public static Panel ToPanel(this Control ctrl, Size size)
    {
        var panel = new Panel();
        ctrl.Dock = DockStyle.Fill;
        panel.Controls.Add(ctrl);
        panel.Size = size;
        return panel;
    }

    public static Panel ToSubmit(this Control ctrl, string title = "", Action<bool, Control> oncompleted = null, bool closeForm = false)
    {
        var factory = new ControlsFactory();
        var panel = factory.CreateSubmitPanel(title, ctrl, success => {
            if (oncompleted is null)
            {
                oncompleted(success, ctrl);
            }
            if (closeForm)
            {
                ctrl.Cast<IControlBase>().GetAppForm().Close();
            }
        });
        return panel;
    }

    public static Form ToForm(this Control ctrl, int width = 800, int height = 600)
    {
        Form form = new Form();
        form.Width = 1024;
        form.LayoutFill(ctrl);

        return form;
    }

    public static void LayoutFill(this System.Windows.Forms.Control panel, [NotNullNotEmpty] System.Windows.Forms.Control control)
    {
        if (panel is null)
            throw new ArgumentNullException(nameof(panel));
        if (control is null)
            throw new ArgumentNullException(nameof(control));
        /*if(panel is ControlBase cb)
        {
            cb.BindMouseEvents(control);
        }*/
        control.Anchor = AnchorStyles.Right;
        control.Location = new Point(0, 0);
        control.Dock = DockStyle.Fill;

        panel.Controls.Add(control);


    }





    public static Form ToMaterialForm(this Control ctrl, int width = 800, int height = 600)
    {
        MaterialForm materialForm = new MaterialForm();
        materialForm.Width = 650;        
        materialForm.LayoutFill(ctrl);
        materialForm.Size = new(ctrl.Size.Width + 123, ctrl.Size.Height + 170);
        return materialForm;
    }

    public static Form ToBaseForm(this Control ctrl, string title = "Главная форма")
    {
        Form form = new Form();
        form.KeyPreview = true;
        form.Text = title;
        form.Width = 1024;
        form.Visible = false;
        form.Size = new(800, 600);
        form.Location = new(0, 0);

        ctrl.Dock = DockStyle.Fill;
        Control p = ctrl;
        while (p.Parent is not null)
            p = p.Parent;
        form.LayoutFill(ctrl);
        form.Load += (s, e) => {
            if(ctrl is ControlBase ctrlBase && (AppViewBase)ctrlBase.viewModel is not null)
            {
                ((AppViewBase)ctrlBase.viewModel).OnLoad(s,e);
            }
        };

        /*ctrl.MouseMove += (s, e) => {
            //System.Console.WriteLine($"({e.X},{e.Y})");  
            if (ctrl is ControlBase cb)
            {
                if (cb.viewModel is not null)
                {
                    cb.viewModel.OnMouseMove(s, e);
                }
                else
                {
                    ctrl.Warn($"viewModel is null {cb.GetTypeName()}");
                    //throw new Exception($"viewModel is null {cb.GetTypeName()}");
                }
            }
        };*/
        form.KeyPress += (s, e) => {
            if (ctrl is ControlBase cb && cb.viewModel is not null)
                cb.viewModel.OnKeyPressed(s, new() {
                    KeyChar = ""+ e.KeyChar

                });
        };
        form.KeyDown += (s, e) => {
            if (ctrl is ControlBase cb && cb.viewModel is not null)
                cb.viewModel.OnKeyDown(s, new()
                {
                    Control = e.Control,
                    Handled = e.Handled,
                    //KeyCode = Enum.GetValues(typeof(Keys)).Enum.GetName(e.KeyCode),
                    KeyValue = e.KeyValue,
                    Alt = e.Alt,
                });
        };
        /*ctrl.MouseMove += (s, e) => {
            System.Console.WriteLine($"{e.X},{e.Y}");
        };*/

        return form;
    }

    public static Form ToBaseForm(this ControlBase ctrl, string title = "Главная форма")
    {
        MaterialForm form = new MaterialForm();
        form.Text = title;
        form.KeyPreview = true;
        form.Width = 1024;
        form.Visible = false;
        form.Size = new(800, 600);
        form.Location = new(0, 0);
        ctrl.Dock = DockStyle.Fill;
        form.Controls.Add(ctrl);
        form.MouseMove += (s, e) => {
            //System.Console.WriteLine($"({e.X},{e.Y})");
            if (ctrl is ControlBase cb)
            {
                if (cb.viewModel is not null)
                {
                    cb.viewModel.OnMouseMove(s, new()
                    {
                        X = e.X,
                        Y = e.Y,
                        Button = Enum.GetName(e.Button).CreateEnumValue<Console_CommonModel.API.MouseButtons>()
                    });
                }
            }
        };
        form.KeyPress += (s, e) => {
            if (ctrl.viewModel is not null)
                ctrl.viewModel.OnKeyPressed(s, new()
                {
                    KeyChar = e.KeyChar+""
                });
        };
        form.KeyDown += (s, e) => {
            if (ctrl.viewModel is not null)
                ctrl.viewModel.OnKeyDown(s, new()
                {
                    Control = e.Control,
                    Handled = e.Handled,
                    //KeyCode = Enum.GetValues(typeof(Keys)).Enum.GetName(e.KeyCode),
                    KeyValue = e.KeyValue,
                    Alt = e.Alt,

                });
        };
        form.AutoSize = true;
        return form;
    }

    public static Form ShowDialog(this Control ctrl, string title = "")
    {
        var form = ctrl.ToBaseForm();
        form.Text = title;
        form.ShowDialog();
        return form;
    }

    public static void ShowSubmitDialog(this Control ctrl, string title, Action<bool, Control> submitted)
    {
        var submit = ctrl.ToSubmit(title, submitted, true);


        var form = submit.ToBaseForm();

        form.ShowDialog();
    }

    public static Form ToAppForm(this Control ctrl, int width = 800, int height = 600)
    {
       Form form = new Form();
        form.Width = 1024;
        form.LayoutFill(ctrl);

        return form;
    }
    public static MaterialSkin.Controls.MaterialCard ToCard(this Control ctrl)
    {

        var panel = new MaterialSkin.Controls.MaterialCard();
        panel.Margin = new(0);
        panel.Padding = new(0);
        panel.Dock = DockStyle.Fill;
        ctrl.Dock = DockStyle.Fill;
        panel.Controls.Add(ctrl);
        panel.AutoScroll = true;
        panel.AutoScroll = false;
        panel.HorizontalScroll.Enabled = false;
        panel.HorizontalScroll.Visible = false;
        panel.HorizontalScroll.Maximum = 0;
        panel.AutoScroll = true;
        return panel;
    }

    public static Panel ToTop(this Control ctrl, Control control)
    {

        var panel = new Panel();
        panel.LayoutTop(control);
        panel.AddFill(ctrl);
        return panel;
    }

    /// <summary>
    /// Выравнивание слева на право
    /// </summary>    
    public static void ToRowLayout(this Panel panel)
    {
        List<Control> controls = new();
        foreach (Control control in panel.Controls)
        {
            controls.Add(control);
        }
        panel.Controls.Clear();
        foreach (var control in controls)
        {
            panel.LayoutLeft(control);
        }
    }

    /// <summary>
    /// Выравнивание сверху вниз
    /// </summary>
    public static void ToColumnLayout(this Panel panel)
    {
        List<Control> controls = new();
        foreach (Control control in panel.Controls)
        {
            controls.Add(control);
        }
        panel.Controls.Clear();
        foreach (var control in controls)
        {
            panel.LayoutTop(control);
        }
    }

 

    public static void LayoutRight(this Control panel, Control ctrl)
    {
        var dx = 0;
        foreach (Control control in panel.Controls)
        {
            var border = (control.Location.X + control.Size.Width);
            dx = border > dx ? border : dx;
        }
        panel.Controls.Add(ctrl);
        ctrl.Dock = DockStyle.Right;
        /*ctrl.Location = new Point(dx, 0);
        ctrl.Size = new Size(ctrl.Width, panel.Height);
        panel.Resize += (sender, evt) => {
            
            ctrl.Size = new Size(ctrl.Width, panel.Height);
        };*/
    }

    public static void LayoutLeft(this Control panel, Control ctrl)
    {
        var dx = 0;
        foreach (Control control in panel.Controls)
        {
            var border = (control.Location.X + control.Size.Width);
            dx = border > dx ? border : dx;
        }
        panel.Controls.Add(ctrl);
        ctrl.Dock = DockStyle.Left;
        ctrl.Location = new Point(0, 0);
        ctrl.Size = new Size(ctrl.Width, panel.Height);
        panel.Resize += (sender, evt) => {
            ctrl.Size = new Size(ctrl.Width, panel.Height);
        };
    }

    public static void AddFill(this Control panel, Control ctrl)
    {
        var dx = 0;
        foreach (Control control in panel.Controls)
        {
            var border = (control.Location.X + control.Size.Width);
            dx = border > dx ? border : dx;
        }
        panel.Controls.Add(ctrl);
        ctrl.Dock = DockStyle.Fill;
    }


    /// <summary>
    /// Выравнивание сверху-вниз
    /// </summary>
    public static void LayoutTop(this Control panel, Control top)
    {
        var dy = 0;

        foreach (Control control in panel.Controls)
        {
            var border = (control.Location.Y + control.Size.Height);
            dy = border > dy ? border : dy;
        }
        top.Margin = top.Padding = new(0);
        top.Dock = DockStyle.Top;
        top.Location = new Point(0, dy);
        top.Size = new Size(panel.Size.Width, top.Height);
        panel.Resize += (sender, evt) => {
            top.Size = new Size(panel.Size.Width, top.Height);
        };
        panel.Controls.Add(top);
    }

    /// <summary>
    /// Выравнивание сверху-вниз
    /// </summary>
    public static void LayoutBottom(this Control panel, Control top)
    {
        var dy = 0;

        foreach (Control control in panel.Controls)
        {
            var border = (control.Location.Y + control.Size.Height);
            dy = border > dy ? border : dy;
        }
        top.Margin = top.Padding = new(0);
        top.Dock = DockStyle.Bottom;
        top.Location = new Point(0, dy);
        top.Size = new Size(panel.Size.Width, top.Height);
        panel.Resize += (sender, evt) => {
            top.Size = new Size(panel.Size.Width, top.Height);
        };
        panel.Controls.Add(top);
    }

    /// <summary>
    /// Привязывает модель AppViewBaseModel к MaterialExpansionPanel
    /// </summary>
    /// <param name="panel">Панель расширения Material Design</param>
    /// <param name="model">Модель представления</param>
    /// <param name="saveButtonText">Текст кнопки сохранения (по умолчанию "Сохранить")</param>
    /// <param name="cancelButtonText">Текст кнопки отмены (по умолчанию "Отменить")</param>
    /// <returns>Панель расширения с привязанной моделью</returns>
    public static MaterialExpansionPanel BindToAppViewBaseModel(
        this MaterialExpansionPanel panel,
        AppViewBaseModel model,
        string saveButtonText = "Сохранить",
        string cancelButtonText = "Отменить")
    {
        if (panel == null)
            throw new ArgumentNullException(nameof(panel));
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        // Привязка свойств Title и Description
        panel.DataBindings.Clear();
        panel.DataBindings.Add(nameof(panel.Title), model, nameof(model.Title),
            false, DataSourceUpdateMode.OnPropertyChanged);
        panel.DataBindings.Add(nameof(panel.Description), model, nameof(model.Description),
            false, DataSourceUpdateMode.OnPropertyChanged);

        // Настройка кнопок
        panel.ShowValidationButtons = true;
        panel.ValidationButtonEnable = true;
        panel.ValidationButtonText = saveButtonText;
        panel.CancelButtonText = cancelButtonText;

        // Привязка событий
        panel.SaveClick += (sender, e) => model.RaiseSave();
        panel.CancelClick += (sender, e) => model.RaiseCancel();

        // Инициализация значений из модели
        panel.Title = model.Title;
        panel.Description = model.Description;

        return panel;
    }

    /// <summary>
    /// Создает MaterialExpansionPanel и привязывает к ней модель AppViewBaseModel
    /// </summary>
    /// <param name="model">Модель представления</param>
    /// <param name="saveButtonText">Текст кнопки сохранения (по умолчанию "Сохранить")</param>
    /// <param name="cancelButtonText">Текст кнопки отмены (по умолчанию "Отменить")</param>
    /// <returns>Панель расширения с привязанной моделью</returns>
    public static MaterialExpansionPanel ToExpansionPanel(
        this AppViewBaseModel model,
        string saveButtonText = "Сохранить",
        string cancelButtonText = "Отменить")
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var panel = new MaterialExpansionPanel();
        return panel.BindToAppViewBaseModel(model, saveButtonText, cancelButtonText);
    }

    


}

public static class LayoutViewsExtensions
{
    public static Action BindHeightToParent(this AppViewBase ctrl)
    {
        var temp = ctrl.Height;
        var p = (AppViewBase)ctrl.Parent;
        ctrl.Height = (int)(p.Height - (p.Padding * 2));
        Action<object,EventArgs> onresize = (s, e) => ctrl.Height = (int)(p.Height - (p.Padding * 2)); 
        p.OnResized += onresize;
        return () => { p.OnResized -= onresize; ctrl.Height = temp; };
    }

    public static Action FixLeftRight(this AppViewBase ctrl, AppViewBase control)
    {
        var temp = ctrl.Width;
        ctrl.X = control.X + control.Width;
        Action<object, EventArgs> onlocationChanged = (s, e) => ctrl.X = control.X + control.Width;
        control.OnLocationChanged += onlocationChanged;
        control.OnResized += onlocationChanged;
        return () => { control.OnResized -= onlocationChanged; ctrl.Width = temp; };

        /*var p = ctrl.Parent;
        ctrl.Top = control.Top + control.Height;
        EventHandler onlocationChanged = (s, e) => ctrl.Top = control.Location.Y + control.Height; ;
        p.LocationChanged += onlocationChanged;
        return () => { p.Resize -= onlocationChanged; };*/
    }
    public static Action FixTopBottom(this AppViewBase ctrl, AppViewBase control)
    {
        var temp = ctrl.Height;
        ctrl.Y = control.Y + control.Height;
        Action<object, EventArgs> onlocationChanged = (s, e) => ctrl.Y = control.Y + control.Height; ;
        control.OnLocationChanged += onlocationChanged;
        control.OnResized += onlocationChanged;
        return () => { 
            control.OnLocationChanged -= onlocationChanged; 
            control.OnResized -= onlocationChanged;
            ctrl.Height = temp;
        };

        /*var p = ctrl.Parent;
        ctrl.Top = control.Top + control.Height;
        EventHandler onlocationChanged = (s, e) => ctrl.Top = control.Location.Y + control.Height; ;
        p.LocationChanged += onlocationChanged;
        return () => { p.Resize -= onlocationChanged; };*/
    }
    public static Action BindWidthToParent(this AppViewBase ctrl)
    {
        var temp = ctrl.Width;
        var p = (AppViewBase)ctrl.Parent;
        ctrl.Width = (int)(p.Width - (p.Padding * 2));
        Action<object, EventArgs> onresize = (s, e) => ctrl.Width = (int)(p.Width - (p.Padding*2));
        p.OnResized += onresize;
        return () => { p.OnResized -= onresize; ctrl.Width = temp; };
    }
    public static Action BindWidthToParent(this AppViewBase ctrl, AppViewBase p)
    {
        var temp = ctrl.Width;

        ctrl.Width = (int)(p.Width - (p.Padding * 2));
        Action<object, EventArgs> onresize = (s, e) => ctrl.Width = (int)(p.Width - (p.Padding * 2));
        p.OnResized += onresize;
        return () => { p.OnResized -= onresize; ctrl.Width = temp; };
    }
}
