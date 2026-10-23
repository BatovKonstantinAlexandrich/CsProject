using System.ComponentModel;
using Console_CommonModel.CommonLib.LibViews.ViewCustomizers;
using Console_CommonModel.CommonLib.LibViews.NavModule;
using Console_CommonModel.CommonLib.LibDesigner.DesignerControls;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.StatusModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{



    /// <summary>
    /// Представление верхнего уровня.
    /// Меню, панель статуса, фокусирование.
    /// 
    /// Внедряет в редактор функции управления элементами иерархии.
    /// </summary>
    [Label("Корневое представление")]
    [TypeConverter(typeof(ExpandableObjectConverter))] // Раскрываемый объект
    [DefaultProperty("Name")] // Свойство по умолчанию
    [NotInput]
    public class AppViewRoot : AppViewBase, IErrorHandler, IStatusView, ILayoutViewNode
    {
        public IServiceProvider serviceProvider { get; set; }
        public MenuView MenuView = new() { ResolveDesignMode = false };
        public readonly ChangingList<ViewItem> FocusedViews = new();
        public AppViewBase contentPane;
        public AppViewBase layoutView = new AppViewBase()
        {
            BackgroundColor = "#f1f1f1",
            DesignMode = true
        };
        public StatusView StatusView = new();


        public MenuView CreateMenuView(string name, Dictionary<string, Action> todos)
        {
            return MenuView.CreateMenuView(name, todos);
        }

        public virtual void PutError(string name, string error)
        {
            StatusView.PutError(name, error);
        }

        /// <summary>
        /// Сервисы управления в режиме разработки
        /// </summary>
        private ViewServicesControlDesigner viewServicesControlDesigner = new();


        [Label("filename")]
        public string filename { get => Get<string>(nameof(filename)); set => Set(nameof(filename), value); }

        public AppViewRoot() : this(null) { }
        public AppViewRoot(string filename = null)
        {
            //this.Name = nameof(AppViewRoot);
            //MinHeight = MinWidth = 222;
            //Height = Width = 222;
            //this.OnKeyDown += (p, evt)=> {
            //    this.Info(evt.KeyCodeStr);
            //    foreach(AppViewBase pchildView in this.FocusedViews)
            //    {
            //        pchildView.OnKeyDown(p, evt);
            //    }
            //};
            ////this.OnMouseMove += OnMouseMoved;
            //Dock = "Fill";

            //Top = MenuView;
            //LayoutFill(contentPane = new AppViewBase());
            //contentPane.LayoutFill(layoutView);
            //Bottom = StatusView;
            ////StatusView.StartAsync();
            //this.filename = filename is null ?
            //            Path.Combine(Directory.GetCurrentDirectory(), "Projects\\app.xml") :
            //            filename;
            //if (!File.Exists(this.filename))
            //{
            //    if (!Directory.Exists("Projects"))
            //        Directory.CreateDirectory("Projects");
            //    System.IO.File.WriteAllText("Projects\\app.xml", this.ToXmlText());
            //}
            this.Bottom = StatusView;

        }

    
        public virtual void LoadXmlFile(string filename)
        {
            var xml = File.ReadAllText(filename);
            this.filename = filename;
            LoadXmlText(xml);
        }

        public virtual AppViewBase LoadXmlText(string xml)
        {
            this.DesignMode = false;
            var view = new AppViewBase() { Dock = "Fill" };
            view.FromXml(xml);
            view.Trace();
            layoutView.Controls.Clear();
            view = (AppViewBase)view.Controls.FirstOrDefault();
            layoutView.LayoutFill(view);
            layoutView.Trace();

            this.DesignMode = true;


            return view;
        }

        protected override void OnDesignModeChanged(PropertyChangedMessage message)
        {
            base.OnDesignModeChanged(message);
        }

        private void OnMouseMoved(object arg1, MouseEventArgs args)
        {
            var position = (args.X, args.Y);

            ForEach(p =>
            {
                var x = ((AppViewBase)p).X;
                var y = ((AppViewBase)p).Y;
                var w = ((AppViewBase)p).Width;
                var h = ((AppViewBase)p).Height;

                bool focused =
                    position.X >= x && position.X <= x + w &&
                    position.Y >= y && position.Y <= y + h;
                if (focused)
                {
                    if (!FocusedViews.Contains((AppViewBase)p))
                    {
                        FocusedViews.Add((AppViewBase)p);
                    }
                }
                else
                {
                    if (FocusedViews.Contains((AppViewBase)p))
                    {
                        FocusedViews.Remove((AppViewBase)p);
                    }

                }
            });
        }




        /// <summary>
        /// Загрузка главного меню из файла
        /// </summary>
        public IEnumerable<MenuView> LoadMainMenuFromFile(Action<string> oncommand, string filename = "mainmenu.json")
        {
            IEnumerable<MenuView> menus = null;
            this.InfoTime("Загрузка главного меню", () =>
            {
                MenuView.MenuItems.Clear();
                Top = MenuView;
                menus = MainMenuService.ReadMenuModelFromJsonFile(filename, oncommand);
                MenuView.MenuItems.AddRange(menus);
            });
            return menus;
        }
        public IEnumerable<MenuView> LoadMainMenuFromFile()
        {
            IEnumerable<MenuView> menus = null;
            this.InfoTime("Загрузка главного меню", () =>
            {
                MenuView.MenuItems.Clear();
                Top = MenuView;
                menus = MainMenuService.ReadMenuModelFromJsonFile("mainmenu.json", OnCommandExecuted);
                MenuView.MenuItems.AddRange(menus);
            });
            return menus;
        }

        public Dictionary<string, string> LoadKeyboardConfig(string filename = "keyboard.json")
        {
            var service = new KeyboardService();
            service.Setup(this, filename);
            return service.KeyboardConfig;
        }

        public void SaveKeyboardConfig(string filename = "keyboard.json")
        {
            File.WriteAllText(filename, keyboardConfig.ToJsonOnScreen());
        }


        /// <summary>
        /// В очередь на выполнение
        /// </summary>
        public override void PutAction(string name, Action todo, int timeout=10000)
        {
            StatusView.PutAction(name, todo, timeout);
        }

        /// <summary>
        /// Вызов из фонового действия <see cref="PutAction"/> для доступа к WinForms с UI-потока.
        /// </summary>
        public void InvokeUi(Action action)
        {
            StatusView.InvokeUi(action);
        }


        public override void HandleError(object sender, string message)
        {
            //TODO: показать
            StatusView.Text = $"{sender.GetTypeName()}: {message}";
        }

        public override IControlBase ToControl()
        {
            this.Name = nameof(AppViewRoot);
            return this.ToControl<AppViewRoot>();
        }




        /// <summary>
        /// Обработка события выполнения комманды
        /// </summary>
        /// <param name="command"></param>
        public void OnCommandExecuted(string command)
        {
            StatusView.PutAction(command, () =>
            {
                this.Info(command);

            }, 100000);
        }


        public void CloseAppForm()
        {
            ToControl().GetAppForm().Close();
        }


    }




    /*public ViewItem SelectedViewItem { get => Get<ViewItem>(nameof(SelectedViewItem)); set => Set(nameof(SelectedViewItem), value); }

    

    public FlexView FlexView = new();

    private KeyboardService keyboardService = new();

    protected ChangingList<IViewTool> tools = new() { };
    protected IViewTool tool;

    public AppViewRoot()
    {
        this.SelectedViewItem = new ButtonView();
        this.Top = MenuView;
        this.LayoutFill( FlexView);

        this.Bottom = StatusView;
        this.BottomHeight = 30;
        this.StatusView.StartAsync();
        this.InitFocusModel();
        this.InitTools();
        this.InitMainMenu();
        this.InitKeyboardConfig();


        this.TopHeight = 30;
    }


    public void ValidateViews()
    {

    }



    private void InitKeyboardConfig()
    {
        keyboardService.Setup(this, new());
    }

    private void InitFocusModel()
    {
        focusModel.NowInFocus.OnChanged += OnFocusChanged;
        FocusedViews.OnChanged += (s, e) =>
        {
            StatusView.Text = $"Выделено ({FocusedViews.Count}) элемента";
        };
        this.AddPropertyChangeListener(nameof(SelectedViewItem), evt =>
        {
            if (evt.Before is not null)
            {
                var p = (AppViewBase)evt.Before;
                var box = new Panel();
                p.control.Parent.ReplaceControl(p.control, box);
            }
            if (evt.After is not null)
            {
                var p = (AppViewBase)evt.After;
            }
        });
    }

   







    


    private void InitTools()
    {
        tools.Add(new SelectViewItemTool(this));
    }

    private void OnControlsChanged(object sender, IChangingListEvent e)
    {


    }



    private void OnViewItemClicked(object sender, EventArgs args)
    {
        this.Info(sender);
    }



    private void OnFocusChanged(object sender, ChangingDictionaryEvent<int, ViewItem> e)
    {
        switch (e.Target.Action)
        {
            case "added": FocusedViews.Add(e.Target.Value); break;
            case "removed": FocusedViews.Remove(e.Target.Value); break;
            case "updated": FocusedViews.Remove(e.Target.Value); FocusedViews.Add(e.Target.Value); break;
            default: throw new Exception();
        }
        StatusView.Text = $"{FocusedViews.Count()}";
    }

    public void Add(ViewNode item)        
        => ((ILayoutView)FlexView).Add(item);        

    public void Remove(ViewNode item)
        => ((ILayoutView)FlexView).Remove(item);

}



/// <summary>
/// Базовый интерфейс инструментов управления элементами.
/// </summary>
public interface IViewTool
{
    Action<object, EventArgs> OnClick(ViewItem view);
}


/// <summary>
/// Инструмент выбора элемента для редактирования.
/// </summary>
public class SelectViewItemTool : IViewTool
{
    private readonly AppViewRoot root;

    public SelectViewItemTool(AppViewRoot root)
    {
        this.root = root;
    }



    public Action<object, EventArgs> OnClick(ViewItem view)
    {
        return (s, e) =>
        {
            root.SelectedViewItem = view;
        };
    }
}*/
}

//using System.ComponentModel;
//using Console_CommonModel.CommonLib.LibViews.ViewCustomizers;
//using Console_CommonModel.CommonLib.LibViews.AppViews.ApiViews;
//using Console_CommonModel.CommonLib.LibDesigner.DesignerControls;
//using Console_CommonModel.CommonLib.LibViews.NavModule;

//namespace Console_CommonModel.CommonLib.LibViews.AppViews.AppRootModule
//{



//    /// <summary>
//    /// Представление верхнего уровня.
//    /// Меню, панель статуса, фокусирование.
//    /// 
//    /// Внедряет в редактор функции управления элементами иерархии.
//    /// </summary>
//    [Label("Корневое представление")]
//    [TypeConverter(typeof(ExpandableObjectConverter))] // Раскрываемый объект
//    [DefaultProperty("Name")] // Свойство по умолчанию
//    [NotInput]
//    public class AppViewRoot : AppViewBase, IErrorHandler, IStatusView, ILayoutView

//    {        
//        public MenuView MenuView = new() { ResolveDesignMode = false };
//        public readonly ChangingList<ViewItem> FocusedViews = new();
//        public AppViewBase contentPane;
//        public AppViewBase layoutView = new AppViewBase()
//        {
//            BackgroundColor = "#f1f1f1",
//            DesignMode = true
//        };
//        public StatusView StatusView = new();


//        public virtual void PutError(string name, string error)
//        {
//            StatusView.PutError(name,error);
//        }

//        /// <summary>
//        /// Сервисы управления в режиме разработки
//        /// </summary>
//        private ViewServicesControlDesigner viewServicesControlDesigner = new();


//        [Label("filename")]
//        public string filename { get => Get<string>(nameof(filename)); set => Set<string>(nameof(filename), value); }

//        public AppViewRoot() : this(null) { }
//        public AppViewRoot(string filename=null)
//        {
//            this.MinHeight = this.MinWidth = 222;
//            this.Height = this.Width = 222;
//            //this.OnMouseMove += OnMouseMoved;
//            this.Dock = "Fill";
//            this.Top = MenuView;            
//            this.LayoutFill(this.contentPane = new AppViewBase());
//            this.contentPane.LayoutFill(this.layoutView);
//            this.Bottom = StatusView;
//            this.StatusView.StartAsync();
//            this.filename = (filename is null) ?
//                        Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Projects\\app.xml") :
//                        filename;
//            if (!System.IO.File.Exists(this.filename))
//            {
//                throw new Exception();
//            }

//        }


//        public virtual void LoadXmlFile(string filename)
//        {
//            var xml = System.IO.File.ReadAllText(filename);
//            this.filename = filename;            
//            LoadXmlText(xml);
//        }

//        public virtual AppViewBase LoadXmlText(string xml )
//        {
//            var view = new AppViewBase() { Dock = "Fill" };
//            view.FromXml(xml);
//            view.Trace();
//            this.layoutView.Controls.Clear();
//            view = (AppViewBase)view.Controls.FirstOrDefault();
//            this.layoutView.LayoutFill(view);
//            this.layoutView.Trace();


//            return view;
//        }

//        protected override void OnDesignModeChanged(PropertyChangedMessage message)
//        {
//            if(DesignMode)
//            {
//                this.TogglePanels();
//                viewServicesControlDesigner.Activates(this);
//            }
//            else
//            {
//                this.TogglePanels();
//                viewServicesControlDesigner.Deactivate(this);
//            }
//        }

//        private void OnMouseMoved(object arg1, MouseEventArgs args)
//        {
//            var position = (args.X, args.Y);

//            this.ForEach(p =>
//            {
//                var x = ((AppViewBase)p).X;
//                var y = ((AppViewBase)p).Y;
//                var w = ((AppViewBase)p).Width;
//                var h = ((AppViewBase)p).Height;

//                bool focused =
//                    (position.X >= x && position.X <= (x + w)) &&
//                    (position.Y >= y && position.Y <= (y + h));
//                if (focused)
//                {
//                    if (!FocusedViews.Contains((AppViewBase)p))
//                    {
//                        FocusedViews.Add((AppViewBase)p);
//                    }
//                }
//                else
//                {
//                    if (FocusedViews.Contains((AppViewBase)p))
//                    {
//                        FocusedViews.Remove((AppViewBase)p);
//                    }

//                }
//            });
//        }




//        /// <summary>
//        /// Загрузка главного меню из файла
//        /// </summary>
//        public IEnumerable<MenuView> LoadMainMenuFromFile(Action<string> oncommand, string filename="mainmenu.json")
//        {
//            IEnumerable<MenuView> menus = null;
//            this.InfoTime("Загрузка главного меню", () => {
//                this.MenuView.MenuItems.Clear();
//                Top = MenuView;
//                menus = MainMenuService.ReadMenuModelFromJsonFile(filename, oncommand);
//                MenuView.MenuItems.AddRange(menus);
//            });
//            return menus;
//        }
//        public IEnumerable<MenuView> LoadMainMenuFromFile()
//        {
//            IEnumerable<MenuView> menus = null;
//            this.InfoTime("Загрузка главного меню", () => {
//                this.MenuView.MenuItems.Clear();
//                Top = MenuView;
//                menus = MainMenuService.ReadMenuModelFromJsonFile("mainmenu.json", this.OnCommandExecuted);
//                MenuView.MenuItems.AddRange(menus);
//            });
//            return menus;
//        }

//        public Dictionary<string, string> LoadKeyboardConfig(string filename = "keyboard.json")
//        {
//            var service = new KeyboardService();
//            service.Setup(this, filename);
//            return service.KeyboardConfig;
//        }

//        public void SaveKeyboardConfig(string filename = "keyboard.json")
//        {            
//            File.WriteAllText(filename, keyboardConfig.ToJsonOnScreen());            
//        }


//        /// <summary>
//        /// В очередь на выполнение
//        /// </summary>
//        public override void PutAction(string name, Action todo)
//        {
//            StatusView.PutAction(name, todo);
//        }

//        /// <summary>
//        /// Вызов из фонового действия <see cref="PutAction"/> для доступа к WinForms с UI-потока.
//        /// </summary>
//        public void InvokeUi(Action action)
//        {
//            StatusView.InvokeUi(action);
//        }


//        public override void HandleError(object sender, string message)
//        {
//            //TODO: показать
//            StatusView.Text = $"{sender.GetTypeName()}: {message}";
//        }

//        public override Control ToControl()
//           => base.ToControl<AppViewBase>();




//        /// <summary>
//        /// Обработка события выполнения комманды
//        /// </summary>
//        /// <param name="command"></param>
//        public void OnCommandExecuted(string command)
//        {
//            this.StatusView.PutAction(command, () => {
//                this.Info(command);

//            });
//        }


//        public void CloseAppForm()
//        {
//            this.ToControl().GetAppForm().Close();
//        }


//    }




//    /*public ViewItem SelectedViewItem { get => Get<ViewItem>(nameof(SelectedViewItem)); set => Set(nameof(SelectedViewItem), value); }



//    public FlexView FlexView = new();

//    private KeyboardService keyboardService = new();

//    protected ChangingList<IViewTool> tools = new() { };
//    protected IViewTool tool;

//    public AppViewRoot()
//    {
//        this.SelectedViewItem = new ButtonView();
//        this.Top = MenuView;
//        this.LayoutFill( FlexView);

//        this.Bottom = StatusView;
//        this.BottomHeight = 30;
//        this.StatusView.StartAsync();
//        this.InitFocusModel();
//        this.InitTools();
//        this.InitMainMenu();
//        this.InitKeyboardConfig();


//        this.TopHeight = 30;
//    }


//    public void ValidateViews()
//    {

//    }



//    private void InitKeyboardConfig()
//    {
//        keyboardService.Setup(this, new());
//    }

//    private void InitFocusModel()
//    {
//        focusModel.NowInFocus.OnChanged += OnFocusChanged;
//        FocusedViews.OnChanged += (s, e) =>
//        {
//            StatusView.Text = $"Выделено ({FocusedViews.Count}) элемента";
//        };
//        this.AddPropertyChangeListener(nameof(SelectedViewItem), evt =>
//        {
//            if (evt.Before is not null)
//            {
//                var p = (AppViewBase)evt.Before;
//                var box = new Panel();
//                p.control.Parent.ReplaceControl(p.control, box);
//            }
//            if (evt.After is not null)
//            {
//                var p = (AppViewBase)evt.After;
//            }
//        });
//    }












//    private void InitTools()
//    {
//        tools.Add(new SelectViewItemTool(this));
//    }

//    private void OnControlsChanged(object sender, IChangingListEvent e)
//    {


//    }



//    private void OnViewItemClicked(object sender, EventArgs args)
//    {
//        this.Info(sender);
//    }



//    private void OnFocusChanged(object sender, ChangingDictionaryEvent<int, ViewItem> e)
//    {
//        switch (e.Target.Action)
//        {
//            case "added": FocusedViews.Add(e.Target.Value); break;
//            case "removed": FocusedViews.Remove(e.Target.Value); break;
//            case "updated": FocusedViews.Remove(e.Target.Value); FocusedViews.Add(e.Target.Value); break;
//            default: throw new Exception();
//        }
//        StatusView.Text = $"{FocusedViews.Count()}";
//    }

//    public void Add(ViewNode item)        
//        => ((ILayoutView)FlexView).Add(item);        

//    public void Remove(ViewNode item)
//        => ((ILayoutView)FlexView).Remove(item);

//}



///// <summary>
///// Базовый интерфейс инструментов управления элементами.
///// </summary>
//public interface IViewTool
//{
//    Action<object, EventArgs> OnClick(ViewItem view);
//}


///// <summary>
///// Инструмент выбора элемента для редактирования.
///// </summary>
//public class SelectViewItemTool : IViewTool
//{
//    private readonly AppViewRoot root;

//    public SelectViewItemTool(AppViewRoot root)
//    {
//        this.root = root;
//    }



//    public Action<object, EventArgs> OnClick(ViewItem view)
//    {
//        return (s, e) =>
//        {
//            root.SelectedViewItem = view;
//        };
//    }
//}*/
//}
