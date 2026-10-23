using MaterialSkin.Controls;
using MaterialSkin;
using WinFormsApp.Console.Win_MaterialModule;

/// <summary>
/// Создаёт компоненты MaterialSkin
/// </summary>
public static class ComponentsExtensions
{

    /// <summary>
    /// Поиск родительского элемента управления реализующего заданный тип
    /// </summary>
    public static TControl GetParent<TControl>(this Control current) 
        where TControl : Control
    {
        Control p = current.Parent;
        while (p is not null && (p.GetType().IsExtends(typeof(TControl))))
        {
            p = p.Parent;
        }
        if (p is null)
            throw new ArgumentException("Generic type parameter for method incorrect");
        return (TControl)p;
    }
    

    /// <summary>
    /// Поиск элемента управления вверх по иерархии
    /// </summary>
    public static TControl GetControl<TControl>(this Control control) where TControl : Control  
    {
        List<Control> res = new();
        var p = control;
        while (p is not null)
        {
            p = p.Parent;
            if (p is TControl)
                return (TControl)p;
        }
        return null;
    }


    /// <summary>
    /// Получение всех родителей
    /// </summary>
    public static List<Control> GetParents( this Control control )
     {
        List<Control> res = new();
        var p = control;        
        while(p is not null)
        {
            p = p.Parent;
            if (p is not null)
                res.Add(p);
        }
        return res;
     }

   


    public static void CheckList(this Control app, Action<IEnumerable<string>> oncompleted, string title, IEnumerable<string> options)
     {
        var factory = new ControlsFactory();
        var label = factory.CreateLabel(title);
        var checkList = factory.CreateCheckedListBox(options);
        var panel = new Panel();
        panel.LayoutTop(label);
        //panel.LayoutRow(checkList, true);             
        panel.Controls.Add(checkList);
        app.LayoutFill(panel);
     }

    public static void CheckList(this Control app, Action<IEnumerable<string>> oncompleted, string title, Dictionary<string,bool> options)
    {
        var factory = new ControlsFactory();
        var label = factory.CreateLabel(title);
        var checkList = factory.CreateCheckedListBox(options);
        var panel = factory.CreateSubmitPanel(title, checkList, succes => {
            if (succes)
            {
                var selected = checkList.Items.Where(item => item.Checked).Select(item => item.Text).ToList();
                if(oncompleted is not null)
                    oncompleted(selected);
            }
        });
        
        app.LayoutFill(panel);
    }
    //public static Task<TModel> Create<TModel>(this Control app) where TModel : class
    // {
    //    TModel model = null;
    //    bool completed = false;
    //    var contentPane = new ControlsFactory().Create<TModel>();
    //    contentPane.ControlRemoved += (s, e) => {
    //        try
    //        {
    //            model = (TModel)contentPane.target;
    //        }
    //        finally
    //        {
    //            completed = true;
    //        }
    //    };
    //    app.LayoutFill(contentPane);
    //    return Task.Run(() => {
    //        long time = 0;
    //        while (!completed)
    //        {
    //            Task.Delay(100);
    //            time += 100;
    //        }
    //        return model;
           
    //    });
    // }


    public static void SingleSelectSubmit(this Control app, Action<string> oncompleted, IEnumerable<string> options)
    {
        var factory = new ControlsFactory();
        var list = factory.CreateMaterialList(options.ToList(), oncompleted);

        MaterialCard card = factory.LayoutIntoCard(list);

        var temp = app.GetControlList().ToPanel();
        bool completed = false;
        var panel = factory.CreateSubmitPanel("Выберите", card, (success) =>
        {
            app.LayoutFill(temp);
            var result = list.SelectedItem is null ? null : list.SelectedItem.ToString();
            completed = true;
            if (success)
                oncompleted(result);
        });
        app.LayoutFill(panel);
    }

    public static void SingleSelect(this Control app, Action<string> oncompleted, IEnumerable<string> options)
    {
        var factory = new ControlsFactory();
        var list = factory.CreateMaterialList(options.ToList(), oncompleted);
        MaterialCard card = factory.LayoutIntoCard(list);
        app.LayoutFill(card);
    }

    public static Form GetAppForm(this Control control)
    {
        List<Control> res = new();
        var p = control;
        while (p is not null)
        {
            p = p.Parent;
            if (p is Form)
                return (Form)p;
        }        
        return control.ToBaseForm();
    }
    public static TModel UpdateDialog<TModel>(this Form app, TModel target) where TModel : class
        => new DialogService().Update<TModel>(app, target);

    public static void CheckList(this Form app, string title, IEnumerable<string> options, Action<bool, IEnumerable<string>> oncomplete)
    {
        app.LayoutFill(new ControlsFactory().CreateCheckedListBox(new List<string>() { "1", "2" }));
    }

    /// <summary>
    /// Меню для сервиса
    /// </summary>
    public static MaterialToolStripMenuItem CreateMenuForServiceObject(this Form app, object service)
    {
        var executer = service;
        var name = service.GetType().Name;
        var menu = new MaterialToolStripMenuItem()
        {
            Name = name,
            Text = name
        };
        var items = service.GetType().GetOwnMethodNames().Select(item =>
        {
            var p = new MaterialToolStripMenuItem()
            {
                Name = item,
                Text = item
            };
            p.Click += (sender, e) =>
            {
                var method = executer.GetType().GetMethod(item);
                Task.Run(() => {
                    method.Invoke(executer, new object[] {

                    });
                });
            };
            return p;
        });
        foreach (var item in items)
        {
            menu.DropDownItems.Add(item);
        }
        return menu;
    }

    public static void LayoutAbs(this System.Windows.Forms.Control panel, System.Windows.Forms.Control control, Point location, Size size)
    {
        control.Dock = DockStyle.Top;
        panel.Controls.Add(control);
        control.Location = location;
        control.Size = size;
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

 



    /// <summary>
    /// Создаёт визуальный компонент со списком выбора
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="items"></param>
    /// <param name="convert"></param>
    /// <param name="onselect"></param>
    /// <returns></returns>
    public static MaterialListBox CreateSelectList<TModel>(
        this IEnumerable<TModel> items,
        Func<TModel, MaterialListBoxItem> convert,
        Action<TModel> onselect)
    where TModel : class
    {
        return new ControlsFactory().CreateMaterialList<TModel>(items.ToList(), convert,onselect);
    }

    public static MaterialCard CreateEditFormView<TModel>(this TModel model, Action<TModel> oncompleted) where TModel : class
    {
        return new ControlsFactory().CreateEditFormView<TModel>(model, oncompleted);
    }


}
