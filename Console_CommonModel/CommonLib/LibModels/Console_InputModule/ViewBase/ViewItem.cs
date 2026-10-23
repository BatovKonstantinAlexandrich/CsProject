 
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewBase;
using Console_CommonModel.CommonLib.LibModels.Console_XmlModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Newtonsoft.Json.Linq;

using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;

public enum DataFormats
{
    Text,
    UnicodeText
}
/// <summary>
/// Базовая модель представления
/// </summary>
[Label("Модель представления")]
public class ViewItem : StyledItem, IClientAPI, IViewDialogFactory
{
    public static string DragAndDropText;
    public static FocusModel focusModel = new();

    public async Task<object> CreateEditDialog(object target, Action<object> onsuccess=null)
    {
        var title = typeof(UserAccount).GetLabel();
        var view = this.CreateEditFormView<object>(target, onsuccess);
        var result = await this.ShowView<EditView>(title, view, (success, target) => {
            if (success)
            {
                if (onsuccess is not null)
                {
                    onsuccess(view.Target);
                }
            }
        });
        return result? view.Target: null;
    }


    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ILayoutViewNode layoutViewControls;

    /// <summary>Обработчики DragOver для SetDroppable подписаны один раз; иначе повторный вызов дублирует превью и ломает drop.</summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _dropSurfaceListenersAttached;

    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private Action<object, int, int> _droppableOnDrop;

    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private Func<string, object> _droppableOnDragEnter;

    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private Action<object> _droppableOnDragLeave;

    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private Action<object, int, int> _droppableOnDragMove;

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private object _droppableTarget;

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private bool _droppableLeaved;

    public virtual string ToString() => ToXmlText();

    public XmlNode<ComponentModel> ToXmlNode()
    {

        return XmlNodeCompiler.CreateXmlNode(this);
    }

    public string ToXmlText()
    {
        var xmlNode = ToXmlNode();
        return XmlNodeCompiler.GetXml(xmlNode);
    }

    public void FromXml(string xml)
    {
        var compilter = new XmlNodeCompiler();
        var root = compilter.Compile(xml, false);
        var view = (ViewItem)root.Item.Target;
        ((AppViewBase)view).Trace();
        Controls.Clear();
        if (this is ILayoutViewNode lv)
        {
            lv.Add(view);
        }
        else
        {

            Controls.Add(view);
        }

        view.ForEach(pnode =>
        {
            if (pnode.Parent is null)
                throw new Exception($"Не установленна ссылка на родителя для {pnode.GetType().GetTypeName()}");
        });
    }

    public class LayoutViewControls(ChangingList<ViewNode> controls) : ILayoutViewNode
    {
        public void Add(ViewNode item) => controls.Add(item);
        public void Remove(ViewNode item) => controls.Remove(item);
    }

    /// <summary>
    /// Связывание свойства по имени с привязкой на изменение данных.
    /// Выполняется обход снизу вверх к родительскому представлению.
    /// </summary>
    /// <param name="name">имя свойства</param>
    /// <param name="update">функция на обновление значения</param>
    public void @scope(string name, Action<object> update)
        => @scope<object>(name, update);
    public void @scope<T>(string name, Action<T> update) where T : class
    {
        if (!DataState.ContainsKey(name))
        {
            if (Parent is not null)
            {
                ((ViewItem)Parent).@scope(name, update);
            }
            else
            {
                throw new ArgumentException(nameof(name));
            }
        }
        else 
        {
            update((T)DataState[name]);
            AddPropertyChangeListener(name, evt => update(evt.After is null ? null : (T)evt.After));
        }
    }


    public virtual void Set(ViewItem item)
    {

        Controls.Clear();
        if(item is not null)
        {
            item.Dock = "Fill";
            Append(item);
        }
    }

    public CardView ToCard()
    {
        var card = new CardView() { Width = 100, Height = 100, WidthUnit = "%", HeightUnit = "%"};
        card.Add(this);
        this.Dock = "Fill";
        return card;
    }
    public AppViewBase ToAppViewBase()
    {
        var card = new AppViewBase();
        card.LayoutFill((AppViewBase)
            this);
        return card;
    }

    public ScrollView ToScrollView(Action<int> onscroll = null)
    {
        var scroll = new ScrollView();
        scroll.OnScrollChanged += (s, e) =>
        {
            int value = e.NewValue;
            if (onscroll is not null)
                onscroll(value);
        };

        scroll.Add(this);
        return scroll;
    }

    public ViewItem() : base()
    {
        layoutViewControls = new LayoutViewControls(Controls);
        SetupDefaults();


        OnEvent += (message) =>
        {
            if (message is PropertyChangedMessage)
            {
                PropertyChangedMessage propertyChangedMessage = (PropertyChangedMessage)message;
                if (propertyChangedMessage.Property == "Selectable")
                {
                    ViewItem viewSource = (ViewItem)propertyChangedMessage.Source;
                    if (viewSource == this)
                    {
                        Changed = true;
                    }

                }
                else if (propertyChangedMessage.Property == "Selected")
                {
                    if (propertyChangedMessage.Source is ViewItem)
                    {
                        ViewItem viewSource = (ViewItem)propertyChangedMessage.Source;
                        if ((bool)propertyChangedMessage.After)
                        {
                            SelectionModel.Add(viewSource);
                        }
                        else
                        {
                            SelectionModel.Remove(viewSource);
                        }
                    }

                }
            }
        };
        OnScrollChanged = (s, e) => { };


        ContextMenu.Parent = this;
        //Changed = false;



        /*OnInitPort += (item) =>
        {
            Emit(item);
        };*/
        /*this.Children.OnChange += (sender, args) =>
        {
            this.Info("Children changed");
        };*/
    }





    public void LayoutFill<TView>([NotNullNotEmpty] AppViewBase control)
        where TView : AppViewBase
    {
        var panel = this;
        if (panel is null)
            throw new ArgumentNullException(nameof(panel));
        if (control is null)
            throw new ArgumentNullException(nameof(control));

        control.Dock = "Fill";
        var parent = ((AppViewBase)panel).GetParent<TView>();
        parent.LayoutFill(control);
    }

    public virtual void LayoutFill(ViewItem node)
    {
        node.Dock = "Fill";
        Controls.Clear();
        Append(node);
    }

    public void LayoutLeft(ViewItem node)
    {
        node.Dock = "Left";
        Append(node);
    }

    public void LayoutRight(ViewItem node)
    {
        node.Dock = "Right";
        Append(node);
    }

    public void LayoutTop(ViewItem node)
    {
        node.Dock = "Top";
        Append(node);
    }

    public void LayoutBottom(ViewItem node)
    {
        node.Dock = "Bottom";
        Append(node);
    }


    /// <summary>
    /// Установка значений по-умолчанию
    /// </summary>
    private void SetupDefaults()
    {
        var ctrl = this;

        OnEvent = (message) =>
        {
            //TODO
            //TODO
        };
        hashcode = GetContentPath();
        var viewItem = this;

        HasRegistered = false;
        Parent = null;

        Focused = false;
        Changed = false;
    }
    public void OnContextMenu()
    {
        this.Info($"{this.GetId()}");
        throw new NotImplementedException();
    }

    [NotInput]
    public int hashcode { get; set; }

    [Label("Функция дсотупа к сеансу")]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]
    public Func<object, object> GetSession { get; set; }




    [NotInput("Устанавливается значение True после вывода контента компонента представления")]
    public bool ViewInitiallized = false;


    [NotInput("Устанавливается значение True после регистрации в сеансе. см. частичное представление Container")]
    public bool HasRegistered { get; set; }


    [Label("Форматы представления")]
    [HelpMessage("Форматы представления представляют из себя классы CSS стилей")]
    [InputPrimitiveCollection(nameof(String))]
    public ChangingList<string> ClassList { get; set; } = new ChangingList<string>();



    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]

    public SelectionModel SelectionModel = new SelectionModel();

    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    private Dictionary<string, Action> Procedures = new Dictionary<string, Action>();




    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<ViewItem> OnUpdate { get; set; }


    public virtual ViewItem IsNotForUpdate()
    {
        Changed = false;
        return this;
    }


    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput("")]
    public string ClassAttr
    {
        get
        {

            string r = "";
            foreach (string s in ClassList)
            {
                r += s + " ";
            }
            return r;
        }
        set
        {

        }
    }



    /**
     * выполняется по событиям элементов контекстного меню
     */
    public void OnContextMenu(int hashcode)
    {
        ContextView menuitem = (from p in ContextMenu.Items where p.GetHashCode() == hashcode select p).FirstOrDefault();
        //if( menuitem != null)
        //    menuitem.OnClick(menuitem);
    }







    public ContextView GetContextMenu()
    {
        return ContextMenu;
    }








    /**
     * Возвращает результат выполнения запроса к базовому источнику данных
     */
    //public abstract object Sql(string sql);
    /* {

         using (var db = new ApplicationDbContext())
         {
             return db.GetDatabaseManager().CleverExecute(sql).ResultSet;
         }

     }*/




    public override void Init()
    {
        base.Init();
        var ctrl = this;
        if (ContextMenu == null)
        {
            ContextMenu.Items.Add(new ContextViewButton()
            {
                Label = "Справка",
                OnClick = (b, s) =>
                {
                    ShowHelp(ctrl.GetHelp());
                }
            });
            ContextMenu.Items.Add(new ContextViewButton()
            {
                Label = "Свойства",
                OnClick = (b, s) =>
                {
                    InputDialog("Test", "Person");
                }
            });
        }
        Interpolate();
    }




    [Label("Управление интерфейсом пользователя")]
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public IClientAPI Client { get; set; } 




    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<object> OnEvent;

    public void AddPropertyChangeListener(string propertyName)
    {

    }



    public override void SendEvent(object messageEvent)
    {
        BeforeSendEvent(messageEvent);
        if (Parent != null)
        {
            Parent.SendEvent(messageEvent);
        }
    }


    public override void BeforeSendEvent(object messageEvent)
    {
        if (messageEvent is PropertyChangedMessage)
        {
            ((PropertyChangedMessage)messageEvent).GetBindedActionName();
        }
        if (OnEvent != null)
        {
            OnEvent(messageEvent);
        }
    }










    /*public string ToJson()
    {
        return JObject.FromObject(this).ToString();
    }
    */

    public string CreateProcedure(string name, Action todo)
    {
        Procedures[name] = todo;
        return name;
    }


    public List<string> GetChanges()
    {
        List<string> changes = new List<string>();
        if (WasChanged())
        {
            changes.Add("" + GetContentPath());
        }
        foreach (var pchild in Controls)
        {
            if (pchild is ViewItem)
            {
                ViewItem childItem = (ViewItem)pchild;
                changes.AddRange(childItem.GetChanges());
            }

        }
        return changes;
    }


    public HashSet<int> GetChangedModels()
    {
        HashSet<int> changes = new HashSet<int>();
        if (ViewInitiallized)
        {
            if (WasChanged())
            {
                changes.Add(GetContentPath());
            }
            foreach (var pchild in Controls)
            {
                if (pchild is ViewItem)
                {
                    ViewItem childItem = (ViewItem)pchild;
                    var changedModels = childItem.GetChangedModels();
                    foreach (int code in changedModels)
                    {
                        changes.Add(code);
                    }

                }
            }
        }
        return changes;
    }


    public ViewItem Find(int hashcode)
    {
        ViewItem result = null;
        Do((p) =>
        {
            if (p is ViewItem)
            {
                if (((ViewItem)p).GetContentPath() == hashcode)
                {
                    result = (ViewItem)p;
                }
            }

        });
        return result;
    }








    public virtual void OnTimeUpdate()
    {

    }












    /*public void OnClick( JObject message )
    {
        if(message.ContainsKey("action") == false)
        {
            throw new Exception("Отсутствует параметер eventArgs.action");
        }
        else
        {
            string action = message["action"].ToString();
            InvokeHelper.Do(this,action);
        }        
    }*/


    /*public int GetLevel()
    {
        int level = 0;
        var p = this;
        while (p != null && (p is Tree))
        {
            level++;
            p = p.Parent;
        }
        return level;
    }*/


    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public string ContentPathExpression = "{{GetHashCode()}}";
    public int GetContentPath()
    {
        return GetHashCode();
    }





    public object Map(string propertyName, JArray dataset, Dictionary<string, string> binndings)
    {
        string itemTypeName = Typing.ParseCollectionType(GetType().GetProperty(propertyName).PropertyType);
        Type itemType = ReflectionService.TypeForName(itemTypeName);
        var list = new List<object>();
        foreach (var jvalue in dataset)
        {
            object newInstance = ReflectionService.Create<object>(itemType, new object[0]);
            foreach (var p in binndings)
            {
                object value = Expressions.Compile(p.Value, jvalue);
                Setter.SetValue(newInstance, p.Key, value);
            }
        }
        return list;
    }


    public string Trace()
    {
        return this.ToJson();
    }






    public bool InfoDialog(string Title, string Text, string Button)
    {
        return Client.InfoDialog(Title, Text, Button);
    }

    public void ShowHelp(string Text)
    {
        Client.ShowHelp(Text);
    }

    public bool RemoteDialog(string Title, string Url)
    {
        return Client.RemoteDialog(Title, Url);
    }

    public bool ConfirmDialog(string Title, string Text)
    {
        return Client.ConfirmDialog(Title, Text);
    }

    public bool CreateEntityDialog(string Title, string Entity)
    {
        return Client.ConfirmDialog(Title, Entity);
    }

    public object InputDialog(string Title, object Properties)
    {
        return Client.InputDialog(Title, Properties);
    }

    public string Eval(string js)
    {
        return Client is null? null: Client.Eval(js);
    }

    public string HandleEvalResult(Func<object, object> handle, string js)
    {
        return Client.HandleEvalResult(handle, js);
    }

    public string Callback(string action, params string[] args)
    {
        return Client.Callback(action, args);
    }

    public bool AddEventListener(string id, string type, string js)
    {
        return Client.AddEventListener(id, type, js);
    }

    public bool DispatchEvent(string id, string type, object message)
    {
        return Client.DispatchEvent(id, type, message);
    }

    public string OnConnected(string token)
    {
        return Client.OnConnected(token);
    }

    public void SetDroppable1(Action<object, int, int> value)
    {
        Dropable = true;
        OnDropped = (s, e) =>
        {

        };
        DoDrop = value;
    }

    public void SetDroppable(
        Action<object, int, int> OnDrop,

        Func<string, object> OnDragEnter,
        Action<object> OnDragLeave,
        Action<object, int, int> OnDragMove)
    {
        //this.Info("SetDroppable");
        _droppableOnDrop = OnDrop;
        _droppableOnDragEnter = OnDragEnter;
        _droppableOnDragLeave = OnDragLeave;
        _droppableOnDragMove = OnDragMove;
        Dropable = true;

        if (_dropSurfaceListenersAttached)
            return;

        this.OnDragLeave = (s, e) =>
        {
            _droppableLeaved = true;
            _droppableOnDragLeave?.Invoke(_droppableTarget);
        };
        this.OnDragEnter = (s, e) =>
        {
            if (e is DragEventArgs de)
                de.Effect = DragDropEffects.Copy;
        };
        OnDragOver = (s, e) =>
        {
            if (e is not DragEventArgs de)
                return;
            de.Effect = Dropable ? DragDropEffects.Copy : DragDropEffects.None;
            if (!Dropable)
                return;

            var text = de.GetData(DataFormats.UnicodeText, false) as string
                ?? de.GetData(DataFormats.Text, true) as string;
            if (string.IsNullOrWhiteSpace(text))
                return;

            var client = s is IControlBase host
                ? host.PointToClient(new Point(de.X, de.Y))
                : new Point(de.X, de.Y);

            if (_droppableLeaved)
            {
                _droppableLeaved = false;
                if (_droppableTarget is not null)
                {
                    _droppableOnDragLeave?.Invoke(_droppableTarget);
                    _droppableTarget = null;
                }
            }

            if (_droppableTarget is not null)
                _droppableOnDragMove?.Invoke(_droppableTarget, client.X, client.Y);
            else
                _droppableTarget = _droppableOnDragEnter?.Invoke(text);
        };
        DoDrop = (s, x, y) =>
        {
            var preview = _droppableTarget;
            _droppableTarget = null;
            _droppableLeaved = false;
            _droppableOnDrop?.Invoke(preview, x, y);
        };
        _dropSurfaceListenersAttached = true;
    }

    public void SubmitDialog(string title, object control, Action<object> value)
    {
        throw new NotImplementedException();
    }

    public Point GetMousePosition()
    {
        throw new NotImplementedException();
    }

    public Point PointToClient(Point point)
    {
        throw new NotImplementedException();
    }

    public void ShowSnakBar(string title)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ShowView<TAppView>(string title, TAppView model, Action<bool, TAppView> oncompleted) where TAppView : AppViewBase
    {
        try
        {
            if(model.Parent is null)
            {
                model.Parent = this.Cast<AppViewBase>().GetAppRootView();
            }

            var rootView = this.Cast<AppViewBase>().GetAppRootView();
            var rootCtrl = (IClientAPI)(rootView.control);
            this.Info(rootView.GetTypeName());
            this.Info(rootCtrl.GetTypeName());
            if (rootCtrl is null)
            {
                throw new Exception();
            }
            return rootCtrl.ShowView<TAppView>(title,model, oncompleted);
        }
        catch(Exception ex)
        {
            this.Error(ex);
            throw;
        }
    }
}

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewBase
{
    enum DragDropEffects
    {
        Copy,
        None
    }
}