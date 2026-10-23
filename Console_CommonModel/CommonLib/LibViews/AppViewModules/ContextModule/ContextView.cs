


using Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ViewServices;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Microsoft.EntityFrameworkCore.Internal;

using System;
using System.Reflection;

using Console_CommonModel.CommonLib.LibViews;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

public interface IContextViewFactory
{
    ContextView CreateContextAction(string name, Action todo);
    ContextView CreateContextMenuView(
        IEnumerable<object> services,
        Func<object, string> getTextForService,
        Func<object, List<string>> getActionsForService,
        Func<object, string, string> getTextForAction,
        Func<object, string, object> invokeServiceMethod);
}
[Label("Контекстное представление")]
[Description("Базовое представление для элементов контекстного меню с поддержкой иконок, меток и вложенных элементов, обеспечивающее структурированное отображение меню")]
public class ContextView : ViewNode, IContextViewFactory, IContextView
{

    public string Icon { get => Get<string>(nameof(Icon)); set => Set<string>(nameof(Icon), value); }
    public string Label { get => Get<string>(nameof(Label)); set => Set<string>(nameof(Label), value); }

    public string Text { get => Label; }

    private ContextFactory contextFactory = new ContextFactory();
    public ContextViewBuilder ContextMenuBuilder;
    public ChangingList<ContextView> Items { get; set; } = new ChangingList<ContextView>();
    public bool Disabled { get; set; } = false;


    public ContextView()
    {
        this.Label = "";
        this.Icon = "";

        this.ContextMenuBuilder = new ContextViewBuilder(this);
        this.Items.OnAdded((i, p) => { ((ViewNode)p).Parent = ((ViewNode)this); });
        this.Items.OnRemoved((i, p) => { ((ViewNode)p).Parent = ((ViewNode)this); });
    }

    public ContextView CreateContextMenuItems(string text, IEnumerable<string> items, Action<string> onclick)
    {
        var menu = new ContextView() { Label = text };
        var dropDownItems = items.Select(text =>
        {
            var command = text;
            var item = new ContextViewButton() { Label = text };
            item.OnClick += (cmd, s) => { if (onclick is not null) onclick(command); };
            return item;
        });
        menu.Items.AddRange(dropDownItems.ToArray());
        return menu;
    }

    public ContextView CreateContextAction(string name, Action todo)
    {
        var p = new ContextView();
        p.Label = name;
        p.OnClick += (s, e) => { todo(); };
        return p;
    }
    public ContextView Find(List<string> list)
    {
        if (list is null || list.Count() == 0)
        {
            return this;
        }
        else
        {
            var pchild = Items.First(p => p.Label == list.First());
            list.RemoveAt(0);
            return pchild.Find(list);
        }
    }

    public void Trace()
    {
        Console.WriteLine(this.Label);
        foreach (var item in this.Items)
        {
            item.Trace();
        }
    }

    public override IEnumerable<DesignerBase> GetLayoutDesignServices()
    {
        return base.GetLayoutDesignServices();
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
        var inputFormModel = m.DeclaringType.GetInputFormModel(m.Name);
        var controlFormModel = inputFormModel.ToControl();
        List<object> args = new();
        if (inputFormModel.FormFields.Count() > 0)
        {
            inputFormModel.ToSubmit((s, m) =>
            {
                if (s)
                {
                    var inputedParameters = new Dictionary<string, object>(inputFormModel.FormFields.Select(f => new KeyValuePair<string, object>(f.Name, f.Value)));
                    m.control.GetAppForm().Close();
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


        return args.ToArray();
    }

    public Action AddContexxtMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] Dictionary<string, Action> todos)
    {
        var contextView = ContextMenuBuilder.CreateServiceMethods(name, todos);
        this.Items.Add(contextView);
        return () =>
        {
            this.Items.Remove(contextView);
        };
    }
    public Action AddContextMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] Dictionary<string, Action> todos)
    {
        var contextView = ContextMenuBuilder.CreateServiceMethods(name, todos);
        this.Items.Add(contextView);
        return () =>
        {
            this.Items.Remove(contextView);
        };
    }
    public Action AddContextServiceMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] object service, [NotNullNotEmpty] Func<MethodInfo, object[]> parametersProvider)
        => ContextMenuBuilder.AddServiceMethods(name, service, parametersProvider);


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
        /*bool isInitializing = true;
         //service.AddPropertyChangeListener(name, evt =>
         //{
         //    var value = evt.After.ToString();
         //    foreach (var next in res.Items)
         //    {
         //        if (next.Label != value)
         //        {
         //            next.EnableChangeSupport = false;
         //            ((ContextViewCheckbox)next).IsChecked = false;
         //            next.EnableChangeSupport = true;
         //        }
         //    }
         //});
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

                 if (p && onchanged is not null)
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
         }*/
        this.Items.Add(res);
        Action<string> set = option =>
        {
            foreach (var item in res.Items)
            {
                if (item.Label != option)
                {
                    ((ContextViewCheckbox)item).IsChecked = false;
                }
                else
                {
                    if (onchanged is not null)
                    {
                        onchanged(item.Label);
                    }
                }
            }
        };
        res.Items.AddRange(options.Select(option => new ContextViewCheckbox() { Label = option, IsChecked = value.ToString().ToLower() == option.ToLower(), OnCheckedChanged = (p) => { set(option); } }));
        return () =>
        {
            this.Items.Remove(res);
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
    public Action c([NotNullNotEmpty] string name, string value, ChangingList<string> options, Action<string> onchanged = null)
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
        this.Items.Add(contextMenu);
        return () =>
        {
            this.Items.Remove(contextMenu);
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
        this.Items.Add(contextView);
        return contextView;
    }

    public IDictionary<string, Action> GetActionMap(List<object> services)
    {
        return contextFactory.GetActionMap(services);
    }

    public IDictionary<string, Action> GetActionMap(object service, System.Type ptype)
    {
        return contextFactory.GetActionMap(service, ptype);
    }



    public ContextView AddContextView(string menu, string text, Action onclick)
    {
        var ctxMenu = new ContextView() { 
            Label = text,
            OnClick = (p, e) => {
                if(onclick is not null)
                {
                    onclick();
                }
            }
        };

        return AddContextMenu(menu, ctxMenu);
    }

    private ContextView AddContextMenu(string menu, ContextView contextView)
    {
        var ctxmenu = this.Items.FirstOrDefault(p => p.Label == menu);
        if(ctxmenu is null)
        {
            ctxmenu = new ContextView()
            {
                Label = menu
            };
            this.Items.Add(ctxmenu);
        }
        ctxmenu.Items.Add(contextView);
        return ctxmenu;
    }


    public IDictionary<string, Action> GetActionNamesMap(List<object> services, Action<string, object> onresult = null)
    {
        return ((IContextFactory)contextFactory).GetActionNamesMap(services, onresult);
    }

    public void CreateContextMenuEdit(Type ptype, AppViewBase view)
    {
        var label = ptype.GetLabel();
        ChangingList<ContextView> items = new();
        var names = ptype.GetOwnInputPropertyNames();
        this.Info($"{ptype.GetTypeName()}=>{names.ToJson()}");

        foreach (var name in names)
        {
            this.Info(Console.Title = name);
            var property = view.GetProperties().FirstOrDefault(p => p.Name == name);
            var propertyType = property.PropertyType;
            var attributes = view.GetType().GetPropertyAttributes(name);
            var formField = view.GetInputFormField(name, text =>
            {
            });
            items.Add(this.CreateContextViewFromFormField(formField));
        }


        this.Items.Add(new()
        {
            Label = label,
            Items = items
        });
    }


    public ContextView CreateContextMenuView(IEnumerable<object> services, Func<object, string> getTextForService, Func<object, List<string>> getActionsForService, Func<object, string, string> getTextForAction, Func<object, string, object> invokeServiceMethod)
    {
        throw new Exception();
    }
    private ContextView CreateContextViewFromFormField(InputFormField formField)
    {
        switch (formField.Type)
        {
            case "bool":
            case "boolean":
            case "checkbox":
                var checkbox = new ContextViewCheckbox();
                checkbox.Label = formField.Label;
                checkbox.OnCheckedChanged += (checkedState) =>
                {
                    formField.OnInput(checkedState.ToString());
                };
                return checkbox;
            case "select":
                {
                    var radioGroup = new ContextViewRadioGroup();
                    var skv = formField.Attributes.FirstOrDefault(kv => kv.Key == nameof(InputSelectAttribute));
                    var selectAttribute = skv.Key.ToType().New<InputSelectAttribute>(new object[] { skv.Value });
                    foreach (var option in selectAttribute.Options)
                    {
                        radioGroup.options.Add(option);
                    }
                    return radioGroup; ;
                }
            case "selectdictionary":
                {
                    var radioGroup = new ContextViewRadioGroup();
                    var skv = formField.Attributes.FirstOrDefault(kv => kv.Key == nameof(SelectDictionaryAttribute));
                    var selectAttribute = skv.Key.ToType().New<InputSelectAttribute>(new object[] { skv.Value });
                    foreach (var option in selectAttribute.Options)
                    {
                        radioGroup.options.Add(option);

                    }
                    return radioGroup;
                }
            case "selectexpression":
                {
                    var radioGroup = new ContextViewRadioGroup();
                    var skv = formField.Attributes.FirstOrDefault(kv => kv.Key == nameof(InputSelectExpressionAttribute));
                    var selectAttribute = skv.Key.ToType().New<InputSelectAttribute>(new object[] { skv.Value });
                    foreach (var option in selectAttribute.Options)
                    {
                        radioGroup.options.Add(option);

                    }
                    return radioGroup;
                }
            default:
                return new ContextView()
                {
                    Label = formField.Label,
                    OnClick = (s, e) =>
                    {
                        var editView = new EditView();
                        var oninputTemp = formField.oninput;
                        formField.oninput = text => { };
                        editView.FormFields.Add(formField);
                        editView.UpdateEditView();
                        var submitView = editView.ToSubmit((success, ptarget) =>
                        {
                            if (success)
                            {
                                formField.Target.SetValue(formField.Name, formField.Value);
                            }
                            editView.ToControl().GetAppForm().Close();
                        });
                        submitView.ShowDialog();
                    }
                };

        }

    }
}
