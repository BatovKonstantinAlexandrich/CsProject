using System.Reflection;
using Console_CommonModel.CommonLib.LibViews;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Org.BouncyCastle.Asn1.X509;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ViewServices;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using MySqlX.XDevAPI;


[Label("Формирование контекстного меню")]
public class ViewContextMenuBuilder : ViewBuilder
{

    protected ContextFactory contextFactory = new ContextFactory();

    [Label("Контекстное меню")]
    [NotInput()]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ContextView ContextMenu { get; } = new();

    [Label("Контекстное меню")]
    [NotInput()]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ContextView ContextMenuPublic { get; } = new();

    [NotInput()]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ContextViewBuilder ContextMenuBuilder { get; set; }
    public Action AddServiceMethods(object service) => ContextMenuBuilder.AddServiceMethods(service);

    public ViewContextMenuBuilder(): base()
    {
        ContextMenuBuilder = new(this.ContextMenu);
        this.ContextMenuPublic.Items.OnAdded((i, p) => { this.ContextMenu.Items.Add((ContextView)p); });
        this.ContextMenuPublic.Items.OnRemoved((i, p) => { this.ContextMenu.Items.Remove((ContextView)p); });
    }


    /// <summary>
    /// Добавление в контекстное меню кнопок для выполнения методов заданного сервиса.
    /// </summary>
    /// <typeparam name="TService">тип сервиса выполнения</typeparam>
    /// <param name="name">наименование пункта меню</param>
    /// <param name="service">ссылка на сервис</param>
    /// <param name="parametersProvider">функция поставщик значений для параметров</param>
    public Action AddContextServiceMethods<TService>([NotNullNotEmpty] string name, [NotNullNotEmpty] TService service, [NotNullNotEmpty] Func<MethodInfo, object[]> parametersProvider)
        => ContextMenuBuilder.AddServiceMethodsType<TService>(name, service, parametersProvider);

    public Action AddContextServiceMethods<TService>([NotNullNotEmpty] string name, [NotNullNotEmpty] TService service)
        => ContextMenuBuilder.AddServiceMethodsType<TService>(name, service, GetUserInputParameters);


    /// <summary>
    /// Получение аргументов выполнения метода из
    /// формы ввода пользоватлелем
    /// </summary>    
    public object[] GetUserInputParameters(MethodInfo m)
    {
        List<object> args = new();
        var parameterNames = m.GetParameters().Select(p => p.Name);
        var inputFormModel = m.DeclaringType.GetInputFormModel(m.Name);
        var editView = new EditView();
        editView.FormFields.AddRange(inputFormModel.FormFields);
        foreach (var p in editView.FormFields)
        {
            if (p.Name == "Icon")
            {
                p.Height += 400;
                p.AutoSize = false;
            }
            editView.scrollView.LayoutTop(p);
        } 
        var list = editView.scrollView.Controls.ToList();
        list.Reverse();
        editView.scrollView.Controls.Clear();
        editView.scrollView.Controls.AddRange(list);
         
        var form = editView.ToControl().RunAtForm();
        editView.OnEditCompleted += (s, e) => {
            if(s)
            {
                foreach (var name in parameterNames)
                {
                    args.Add(inputFormModel.FormFields.FirstOrDefault(f => f.Name == name).Value);
                }
                
            }
            form.Close();
        };
        
        return args.ToArray();

        /*var controlFormModel = inputFormModel.ToControl();
        List<object> args = new();
        if (inputFormModel.FormFields.Count() > 0)
        {
            inputFormModel.ToSubmit((s, m) =>
            {
                if (s)
                {
                    var inputedParameters = new Dictionary<string, object>(inputFormModel.FormFields.Select(f => new KeyValuePair<string, object>(f.Name, f.Value)));
                    m.control.GetForm().Close();
                }
                else
                {
                    throw new Exception();
                }
            }).ShowDialog();
        }
        foreach (var name in m.GetParameters().Select(p => p.Name))
        {
            args.Add(inputFormModel.FormFields.FirstOrDefault(f => f.Name == name).Value);
        }


        return args.ToArray();*/
    }

    /// <summary>
    /// Получение аргументов выполнения метода из
    /// формы ввода пользоватлелем
    /// </summary>    
    public object[] GetUserInputParametersSide(MethodInfo m)
    {
        List<object> args = new();

        var inputFormModel = m.DeclaringType.GetInputFormModel(m.Name);
        var editView = new EditView();
        editView.FormFields.AddRange(inputFormModel.FormFields);
        foreach (var p in editView.FormFields)
        {
            if (p.Name == "Icon")
            {
                p.Height += 400;
                p.AutoSize = false;
            }
            editView.scrollView.LayoutTop(p);
        }
        var list = editView.scrollView.Controls.ToList();
        list.Reverse();
        editView.scrollView.Controls.Clear();
        editView.scrollView.Controls.AddRange(list);

        var temp = ((AppViewBase)this).Right;
        editView.OnEditCompleted += (s, e) => {
            if (s)
            {
                foreach (var name in m.GetParameters().Select(p => p.Name))
                {
                    args.Add(inputFormModel.FormFields.FirstOrDefault(f => f.Name == name).Value);
                }

            }
            ((AppViewBase)this).Right = temp;
        };
        ((AppViewBase)this).Right = editView;
        return args.ToArray();

        /*var controlFormModel = inputFormModel.ToControl();
        List<object> args = new();
        if (inputFormModel.FormFields.Count() > 0)
        {
            inputFormModel.ToSubmit((s, m) =>
            {
                if (s)
                {
                    var inputedParameters = new Dictionary<string, object>(inputFormModel.FormFields.Select(f => new KeyValuePair<string, object>(f.Name, f.Value)));
                    m.control.GetForm().Close();
                }
                else
                {
                    throw new Exception();
                }
            }).ShowDialog();
        }
        foreach (var name in m.GetParameters().Select(p => p.Name))
        {
            args.Add(inputFormModel.FormFields.FirstOrDefault(f => f.Name == name).Value);
        }


        return args.ToArray();*/
    }

    public Action AddContexxtMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] IDictionary<string, Action> todos)
    {
        var contextView = ContextMenuBuilder.CreateServiceMethods(name, todos);
        this.ContextMenu.Items.Add(contextView);
        return () =>
        {
            this.ContextMenu.Items.Remove(contextView);
        };
    }
  
    public Action AddContextMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] Dictionary<string, Action> todos)
    {
        var contextView = ContextMenuBuilder.CreateServiceMethods(name, todos);
        this.ContextMenu.Items.Add(contextView);
        return () =>
        {
            this.ContextMenu.Items.Remove(contextView);
        };
    }

    public Action AddContextViewControls<TService>()
        where TService : class
    {
        return this.AddContextMethods(typeof(TService).GetLabel(), new(this.GetActionMap(this.Cast<AppViewBase>().Get<TService>(), typeof(TService).GetOwnMethodNames())));
    }
    
    public Action AddContextViewControls<TService>(TService service)
        where TService : class
    {
        return this.AddContextMethods(typeof(TService).GetLabel(), new(this.GetActionMap(service, typeof(TService).GetOwnMethodNames())));
    }
    public Action AddContextServiceMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] object service, [NotNullNotEmpty] Func<MethodInfo, object[]> parametersProvider)
        => ContextMenuBuilder.AddServiceMethods(name, service, parametersProvider);

    public Action AddContextServiceMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] object service, List<string> methods, [NotNullNotEmpty] Func<MethodInfo, object[]> parametersProvider)
            => ContextMenuBuilder.AddServiceMethods(name, service, methods, parametersProvider);


    public Action AddContextServiceOptions<TServiceOptions>([NotNullNotEmpty] object service)
        => ContextMenuBuilder.AddContextServiceOptions<TServiceOptions>(service);


    public Action AddContextRadioGroup([NotNullNotEmpty] string name, [NotNullNotEmpty] ChangeSupport service, Action<string> onchanged = null)
    {
        var attrs = service.GetType().GetPropertyAttributes(name);
        var valuesstr = attrs.FirstOrDefault(kv => kv.Key == nameof(InputSelectAttribute)).Value;
        var options = valuesstr.Split(",");
        var label = service.GetType().GetPropertyLabel(name);
        var value = service.GetValue<string>(name);
        ContextView res = new()
        {
            Label = label
        };
        bool enabled = true;
        bool isInitializing = true;
        service.AddPropertyChangeListener(name, evt =>
        {
            var value = evt.After.ToString();
            foreach (var next in res.Items)
            {
                if (next.Label != value)
                {
                    next.EnableChangeSupport = false;
                    ((ContextViewCheckbox)next).IsChecked = false;
                    next.EnableChangeSupport = true;
                }
            }
        });
        foreach (var item in options)
        {
            var pitem = item;
            ContextViewCheckbox pp = null;
            pp = new()
            {
                IsChecked = item == value,
                Label = item
            };
            pp.OnCheckedChanged += (bool p) =>
            {
                if (isInitializing)
                    return;

                if (p && service.GetValue<string>(name) != pitem)
                {
                    service.SetValue(name, value = pitem);
                }

                if (onchanged is not null)
                    onchanged(pitem);

                if (p)
                {
                    foreach (var resItem in res.Items)
                    {
                        if (resItem != pp)
                        {
                            ((ContextViewCheckbox)resItem).IsChecked = false;
                        }
                    }
                }
                this.Info(res.Items.Where(resItem => ((ContextViewCheckbox)resItem).IsChecked).Select(resItem => ((ContextViewCheckbox)resItem).Label));
            };
            res.Items.Add(pp);
        }
        isInitializing = false;
        this.ContextMenu.Items.Add(res);
        return () =>
        {
            this.ContextMenu.Items.Remove(res);
        };
        /*ContextMenuBuilder.AddContextRadioGroup(name, service, options.ToList() , val => {
            if (onchanged is not null)
                onchanged(val);
            service.SetValue(name, val);
        });*/
    }



    /// <summary>
    /// Создание меню со списком выбора
    /// </summary>
    /// <param name="name">наименование меню</param>
    /// <param name="value">текущее значение</param>
    /// <param name="options">варианты выбора</param>
    /// <param name="onchanged">дейтсвие при изменении</param>    
    public Action AddContextSelectOption([NotNullNotEmpty] string name, string value, ChangingList<string> options, Action<string> onchanged = null)
    {
        ContextView contextMenu = new()
        {
            Label = name
        };
        foreach (string option in options)
        {
            ContextViewCheckbox contextMenuCheckBox = null;
            contextMenuCheckBox = new()
            {
                IsChecked = option == value,
                Label = option
            };
            contextMenuCheckBox.OnCheckedChanged += (bool p) =>
            {

                foreach (var resItem in contextMenu.Items)
                {
                    if (resItem != contextMenuCheckBox && ((ContextViewCheckbox)resItem).IsChecked)
                    {
                        ((ContextViewCheckbox)resItem).IsChecked = false;
                    }

                }
                if (onchanged is not null)
                    onchanged(option);
            };
            contextMenu.Items.Add(contextMenuCheckBox);
        }
        this.ContextMenu.Items.Add(contextMenu);
        return () =>
        {
            this.ContextMenu.Items.Remove(contextMenu);
        };
    }


    /// <summary>
    /// Управлениме свойсвам и
    /// </summary>
    /// <param name="target"></param>
    /// <param name="delaringType"></param>
    /// <returns></returns>
    public ContextView CreateContextOptionsView(object target, System.Type delaringType)
    {
        return AddContextMenu(contextFactory.CreateContextOptionsView(target, delaringType));
    }

    public ContextView CreateContextView(string text, Dictionary<string, Action> operations)
    {
        return AddContextMenu(contextFactory.CreateContextView(text, operations));
    }

    public ContextView CreateContextView(string text, object service)
    {
        return AddContextMenu(contextFactory.CreateContextView(text, service));
    }

    private ContextView AddContextMenu(ContextView contextView)
    {
        this.ContextMenu.Items.Add(contextView);
        return contextView;
    }

    private Action AddContextMenuItem(ContextView contextView)
    {
        this.ContextMenu.Items.Add(contextView);
        return () => {
            this.ContextMenu.Items.Remove(contextView);
        };
        
    }

    public IDictionary<string, Action> GetServicesActionMap(List<object> services, Action<string, object> onresult = null)
    {
        return contextFactory.GetActionMap(services, onresult);
    }

    public virtual Dictionary<string, Action> GetActionMap(object service, Action<string, object> onresult = null)
    {
        return GetActionNamesMap(new List<object>() { service  }, onresult);
    }
    public IDictionary<string, Action> GetActionNamesMap(object services, Action<string,object> onresult = null)
    {
        return contextFactory.GetActionNamesMap(new List<object>() { services }, onresult);
    }

    

    public Dictionary<string, Action> GetActionMap(object service, List<string> methods)
    {
        return contextFactory.GetActionMap(service, methods);
    }

   

    public ContextView AddContextView(string menu, string text, Action onclick)
    {
        return AddContextMenu(menu, contextFactory.AddContextView(menu, text, onclick));
    }

    private ContextView AddContextMenu(string menu, ContextView contextView)
    {
        ContextMenu.Items.FirstOrDefault(p => p.Label == menu).Items.Add(contextView);
        return contextView;
    }

    public ContextView CreateContextAction(string name, Action todo)
    {
        throw new Exception();
    }

    public ContextView CreateContextMenuView(IEnumerable<object> services, Func<object, string> getTextForService, Func<object, List<string>> getActionsForService, Func<object, string, string> getTextForAction, Func<object, string, object> invokeServiceMethod)
    {
        throw new Exception();
    }

    public Dictionary<string, Action> GetActionNamesMap(List<object> services, Action<string, object> onresult = null)
    {
        return contextFactory.GetActionNamesMap(services, onresult);
    }
    
}

