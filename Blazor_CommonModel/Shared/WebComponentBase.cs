using Console_CommonModel.ActionModels;
using System.Collections.Concurrent;
using System.ComponentModel;

using static WinFormsApp.API.DesignerResizeTool;

namespace Blazor_CommonModel.Components.Shared
{

    public class WebComponentBase : WebEventSupport, IControlRoot, IControlBase
    {
        public virtual string ErrorMessage { get; set; } = "";
        public virtual bool Initiallized { get; set; } = false;


        public override IClientAPI clientAPI { get => ViewModel; }

        [Parameter]
        public override AppViewBase ViewModel { get; set; }
        public override Console_CommonModel.CommonLib.Others.ComponentBase component { get; set; }

        [Parameter]
        public virtual bool UseModernStyle { get; set; }

        [Parameter]
        public virtual string Id { get; set; }

        [Parameter]
        public virtual IControlBase Parent { get; set; }

        [Parameter]
        public virtual string Name { get; set; }
        public ChangingList<IControlBase> Controls { get; set; } = new();

        public virtual Size Size { get;  set;  }
        [Parameter]
        public virtual bool KeyPreview { get;  set;  }

        [Parameter]
        public virtual string Text { get;  set;  }


        /// <summary>
        /// Создаёт связанный элемент управления типа TControl, в случае если
        /// существует IComponentBase<TAppViewBase, TControlBase>
        /// </summary>
        public System.Type GetWebControlType<TControl>( AppViewBase view )
            where TControl: IControlBase
        {
            var viewType = view.GetType();
            var controlType = typeof(TControl);
            var componentType = typeof(WebControlBase<,>).MakeGenericType(new System.Type[] { viewType, controlType });
            return ServiceFactory.Get().GetTypesExtended(componentType).FirstOrDefault();
        }

        public virtual TControl CreateControl<TControl>(AppViewBase view)
             where TControl : WebComponentBase
        {
            var type = GetWebControlType<TControl>(view);
            if(type is not null)
            {
                var ctrl = typeof(TControl).New<TControl>();
                var comp = type.New(new object[] { view, ctrl });
                return ctrl;
            }
            return null;
        }

       



        public IControlRoot GetControlRoot()
        {
            if (this is IControlRoot)
                return (IControlRoot)this;
            if(this.Parent is not null)
            {
                return this.Parent.Cast<WebComponentBase>().GetControlRoot();
            }
            else
            {
                return this;
            }
            
        }


        public void Dispose() { }

        public void Close()
        {
            GetControlRoot().Close();
        }

        public void Invoke(Action todo)
        {
            //GetControlRoot().Invoke(showErr);
            try
            {
                ViewModel.OnProgressStarted(todo.GetHashCode().ToString());
                todo();
                ViewModel.OnProgressCompleted(todo.GetHashCode().ToString());
            }
            catch (Exception ex)
            {

            }
        }

        public void SetSize(int v1, int v2)
        {
            GetControlRoot().SetSize(v1, v2);
        }

        public bool Capture { get; set; }
        public bool IsHandleCreated { get; set; }
        public bool InvokeRequired { get; set; } = true;
        public virtual bool IsDisposed { get; set; }

        public IControlBase ShowDialog(string title = "")
        {
            return GetControlRoot().ShowDialog(title);
        }

        public IControlBase ToBaseForm(string title = "")
        {
            return GetControlRoot().ToBaseForm(title);
        }

        public IControlBase RunAtForm(string title = "")
        {
            return GetControlRoot().RunAtForm(title);
        }

        public IControlBase GetAppForm()
        {
            return (IControlBase)GetControlRoot();
        }

        public ISite Site { get; set; }

        public event EventHandler Disposed;

        public void Add(IComponent component)
        {
            if(component is IControlBase cb)
            {
                Controls.Add(cb);
            }
        }

        public void Add(IComponent component, string name)
        {
            throw new NotImplementedException();
        }

        public void Remove(IComponent component)
        {
            throw new NotImplementedException();
        }

        public ComponentCollection Components => throw new NotImplementedException();

        public bool Enabled { get; set; }
        public object AutoSizeMode { get; set; }
        public bool AllowDrop { get; set; }
        public Size MinimumSize { get; set; }
        public Size PreferredSize { get; set; }
        public Action<object, object> HandleCreated { get; set; }

        public void SetBounds(int x, int y, int w, int h)
        {
            throw new NotImplementedException();
        }
    }





    /// <summary>
    /// 
    /// </summary>
    public class WebPanelsControl : Microsoft.AspNetCore.Components.ComponentBase
    {
        public virtual IControlBase Left { get; set; }
        public virtual IControlBase Right { get; set; }
        public virtual IControlBase Top { get; set; }
        public virtual IControlBase Bottom { get; set; }

        public void SetLeft(IControlBase value) => Left = value;
        public void SetTop(IControlBase value) => Top = value;
        public void SetRight(IControlBase value) => Right = value;
        public void SetBottom(IControlBase value) => Bottom = value;
    }


    public class WebEventsControl : WebPanelsControl, IEventSupport
    {
        /// <summary>
        /// ChangeSupport
        /// </summary>
        public ConcurrentDictionary<string, object> DataState { get => ((IChangeSupport)ChangeSupport).DataState; set => ((IChangeSupport)ChangeSupport).DataState = value; }
        public Action<PropertyChangedMessage> OnChange { get => ((IChangeSupport)ChangeSupport).OnChange; set => ((IChangeSupport)ChangeSupport).OnChange = value; }

        public ChangeSupport ChangeSupport { get; set; } = new();

        public virtual AppViewBase ViewModel { get; set; }

        public virtual void FireEvent(CommonEventMessage<string> message)
        {
            ((IViewEventSupport)ChangeSupport).FireEvent(message);
        }
        public List<string> GetEventListeners()
        {
            return ((IViewEventSupport)ChangeSupport).GetEventListeners();
        }

        public void SendChanges()
        {
            this.ViewModel.SendChanges();
        }

        public void SendEvent(object messageEvent)
        {
            this.ViewModel.SendEvent(messageEvent);
        }
        public virtual void FireEvent(PropertyChangedMessage propertyChangedMessage)
        {
            ((IViewEventSupport)ChangeSupport).FireEvent(propertyChangedMessage);
        }

        public void FireEvent<T>(CommonEventMessage<T> message)
        {
            ((IViewEventSupport)ChangeSupport).FireEvent(message);
        }

        public void FireEvent<T>(T message)
        {
            ((IViewEventSupport)ChangeSupport).FireEvent(message);
        }
        public virtual void AddOnClick(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnClick(todo);
        }
        public virtual void AddOnKeyClicked(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnKeyClicked(todo);
        }
        public virtual void AddOnKeyDown(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnKeyDown(todo);
        }
        public virtual void AddOnKeyPressed(Action<object, KeyPressEventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnKeyPressed(todo);
        }
        public virtual void AddOnKeyUp(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnKeyUp(todo);
        }
        public virtual void AddOnLoad(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnLoad(todo);
        }

        public virtual void AddOnMouseDoubleClick(Action<object, MouseEventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnMouseDoubleClick(todo);
        }

        public virtual void AddOnMouseDown(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnMouseDown(todo);
        }

        public virtual void AddOnMouseEnter(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnMouseEnter(todo);
        }

        public virtual void AddOnMouseLeave(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnMouseLeave(todo);
        }

        public virtual void AddOnMouseMove(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnMouseMove(todo);
        }

        public virtual void AddOnMouseUp(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnMouseUp(todo);
        }

        public virtual void AddOnResized(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).AddOnResized(todo);
        }

        public virtual List<string> GetBaseEventProperties()
        {
            return ((IViewEventSupport)ViewModel).GetBaseEventProperties();
        }

        public virtual void RemoveOnClick(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnClick(todo);
        }

        public virtual void RemoveOnKeyClicked(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnKeyClicked(todo);
        }

        public virtual void RemoveOnKeyDown(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnKeyDown(todo);
        }

        public virtual void RemoveOnKeyPressed(Action<object, KeyPressEventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnKeyPressed(todo);
        }

        public virtual void RemoveOnKeyUp(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnKeyUp(todo);
        }

        public virtual void RemoveOnLoad(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnLoad(todo);
        }

        public virtual void RemoveOnMouseDoubleClick(Action<object, MouseEventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnMouseDoubleClick(todo);
        }

        public virtual void RemoveOnMouseDown(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnMouseDown(todo);
        }

        public virtual void RemoveOnMouseEnter(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnMouseEnter(todo);
        }

        public virtual void RemoveOnMouseLeave(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnMouseLeave(todo);
        }

        public virtual void RemoveOnMouseMove(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnMouseMove(todo);
        }

        public virtual void RemoveOnMouseUp(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnMouseUp(todo);
        }

        public virtual void RemoveOnResized(Action<object, EventArgs> todo)
        {
            ((IViewEventSupport)ViewModel).RemoveOnResized(todo);
        }



        public void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)ChangeSupport).RemovePropertyChangeListener(listener);
        }

        public void AddPropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)ChangeSupport).AddPropertyChangeListener(listener);
        }

        public void AddPropertyChangeListener<T>(string property, Action<T> changed)
        {
            ((IChangeSupport)ChangeSupport).AddPropertyChangeListener(property, changed);
        }

        public void AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)ChangeSupport).AddPropertyChangeListener(property, listener);
        }

        public T Get<T>(string key)
        {
            return ((IChangeSupport)ChangeSupport).Get<T>(key);
        }

        public void OnGet<T>(string key, T value)
        {
            ((IChangeSupport)ChangeSupport).OnGet(key, value);
        }

        public void Set<T>(string key, T value)
        {
            ((IChangeSupport)ChangeSupport).Set(key, value);
        }

        public void BeforeSendEvent(object messageEvent)
        {
            ViewModel.BeforeSendEvent(messageEvent);

        }

        public bool WasChanged()
        {
            return ((IChangeSupport)ChangeSupport).Changed;
        }
    }

    public class WebClientControl : WebEventsControl, IClientAPI
    {
        public IClientAPI clientAPI { get => this.ViewModel; }
        public void ShowSnakBar(string title) => clientAPI.ShowSnakBar(title);
        public Point PointToClient(Point point) => clientAPI.PointToClient(point);
        public Point GetMousePosition()
        {
            return clientAPI.GetMousePosition();
        }

        public void SubmitDialog(string title, object control, Action<object> value)
        {
            clientAPI.SubmitDialog(title, control, value);
        }

        public bool InfoDialog(string Title, string Text, string Button)
        {
            return clientAPI.InfoDialog(Title, Text, Button);
        }

        public void ShowHelp(string Text)
        {
            clientAPI.ShowHelp(Text);
        }

        public bool RemoteDialog(string Title, string Url)
        {
            return clientAPI.RemoteDialog(Title, Url);
        }

        public bool ConfirmDialog(string Title, string Text)
        {
            return clientAPI.ConfirmDialog(Title, Text);
        }

        public bool CreateEntityDialog(string Title, string Entity)
        {
            return clientAPI.CreateEntityDialog(Title, Entity);
        }

        public object InputDialog(string Title, object Properties)
        {
            return clientAPI.InputDialog(Title, Properties);
        }
        public string Eval(string js)
        {
            return clientAPI is null? null: clientAPI.Eval(js);
        }

        public string HandleEvalResult(Func<object, object> handle, string js)
        {
            return clientAPI.HandleEvalResult(handle, js);
        }

        public string Callback(string action, params string[] args)
        {
            return clientAPI.Callback(action, args);
        }

        public bool AddEventListener(string id, string type, string js)
        {
            return clientAPI.AddEventListener(id, type, js);
        }

        public bool DispatchEvent(string id, string type, object message)
        {
            return clientAPI.DispatchEvent(id, type, message);
        }

        public string OnConnected(string token)
        {
            return clientAPI.OnConnected(token);
        }

        public Task<bool> ShowView<TAppView>(string title, TAppView model, Action<bool, TAppView> oncompleted) where TAppView : AppViewBase
        {
            return clientAPI.ShowView<TAppView>(title, model, oncompleted);
        }
    }

    public class WebComponentControl : WebClientControl
    {
        public virtual IClientAPI clientAPI { get => ViewModel; }
        public override AppViewBase ViewModel { get; set; }

        public virtual Console_CommonModel.CommonLib.Others.ComponentBase component { get; set; }
        public Point GetMousePosition() => clientAPI.GetMousePosition();



    }



    public class WebStylesControl : WebComponentControl, IStyledItem
    {
        public bool AnchorBottom { get; set; }
        public bool AnchorLeft { get; set; }
        public bool AnchorRight { get; set; }
        public bool AnchorTop { get; set; }
        public bool AutoSize { get; set; }
        public virtual string BackgroundColor { get; set; }
        public virtual string BorderColor { get; set; }
        public virtual float BorderWidth { get; set; }
        public virtual string Color { get; set; }
        public virtual Cursors Cursor { get; set; }
        public virtual string Dock { get; set; }
        public virtual float FontSize { get; set; }
        public virtual int Height { get; set; }
        public virtual int Margin { get; set; }
        public virtual int MinHeight { get; set; }
        public virtual int MinWidth { get; set; }
        public virtual int Padding { get; set; }
        public virtual string TextAlign { get; set; }
        public virtual int TextIndent { get; set; }
        public virtual int Width { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }

        public string GetCssText()
        {
            string[] options = ReflectionService.GetOwnPropertyNames(typeof(StyledItem)).ToArray();
            Dictionary<string, object> valuesMap = this.GetValues(options);
            string cssText = new CssSerializer().Seriallize(this,valuesMap, new());
            return cssText;
        }


        public void SetBaseTextStyle()
        {
            this.FontSize = 12;
            this.Color = "#000000";
            this.TextAlign = "left";
            this.TextIndent = 0;
        }
    }



    public class WebEventSupport : WebStylesControl, IEventSupport
    {
        public virtual bool Changed { get => ((IChangeSupport)ChangeSupport).Changed; set => ((IChangeSupport)ChangeSupport).Changed = value; }
        public virtual bool ValidateOnInput { get => ((IChangeSupport)ChangeSupport).ValidateOnInput; set => ((IChangeSupport)ChangeSupport).ValidateOnInput = value; }

        [Parameter]
        public virtual Action<object, EventArgs> OnClick { get => ((IViewEventSupport)ViewModel).OnClick; set => ((IViewEventSupport)ViewModel).OnClick = value; }
        [Parameter]
        public virtual Action<object, DragEventArgs> OnDragEnter { get => ((IViewEventSupport)ViewModel).OnDragEnter; set => ((IViewEventSupport)ViewModel).OnDragEnter = value; }
        [Parameter]
        public virtual Action<object, EventArgs> OnDragged { get => ((IViewEventSupport)ViewModel).OnDragged; set => ((IViewEventSupport)ViewModel).OnDragged = value; }
        [Parameter]
        public virtual Action<object, EventArgs> OnDragLeave { get => ((IViewEventSupport)ViewModel).OnDragLeave; set => ((IViewEventSupport)ViewModel).OnDragLeave = value; }
        [Parameter]
        public virtual Action<object, DragEventArgs> OnDragOver { get => ((IViewEventSupport)ViewModel).OnDragOver; set => ((IViewEventSupport)ViewModel).OnDragOver = value; }
        [Parameter]
        public virtual Action<object, EventArgs> OnDropped { get => ((IViewEventSupport)ViewModel).OnDropped; set => ((IViewEventSupport)ViewModel).OnDropped = value; }
        public virtual Action<object, EventArgs> OnKeyClicked { get => ((IViewEventSupport)ViewModel).OnKeyClicked; set => ((IViewEventSupport)ViewModel).OnKeyClicked = value; }
        [Parameter]
        public virtual Action<object, KeyEventArgs> OnKeyDown { get => ((IViewEventSupport)ViewModel).OnKeyDown; set => ((IViewEventSupport)ViewModel).OnKeyDown = value; }
        [Parameter]
        public virtual Action<object, KeyPressEventArgs> OnKeyPressed { get => ((IViewEventSupport)ViewModel).OnKeyPressed; set => ((IViewEventSupport)ViewModel).OnKeyPressed = value; }
        [Parameter]
        public virtual Action<object, KeyEventArgs> OnKeyUp { get => ((IViewEventSupport)ViewModel).OnKeyUp; set => ((IViewEventSupport)ViewModel).OnKeyUp = value; }
        [Parameter]
        public virtual Action<object, EventArgs> OnLoad { get => ((IViewEventSupport)ViewModel).OnLoad; set => ((IViewEventSupport)ViewModel).OnLoad = value; }
        [Parameter]
        public virtual Action<object, EventArgs> OnLocationChanged { get => ((IViewEventSupport)ViewModel).OnLocationChanged; set => ((IViewEventSupport)ViewModel).OnLocationChanged = value; }
        [Parameter]
        public virtual Action<object, MouseEventArgs> OnMouseDoubleClick { get => ((IViewEventSupport)ViewModel).OnMouseDoubleClick; set => ((IViewEventSupport)ViewModel).OnMouseDoubleClick = value; }
        [Parameter]
        public virtual Action<object, MouseEventArgs> OnMouseDown { get => ((IViewEventSupport)ViewModel).OnMouseDown; set => ((IViewEventSupport)ViewModel).OnMouseDown = value; }
        [Parameter] public virtual Action<object, EventArgs> OnMouseEnter { get => ((IViewEventSupport)ViewModel).OnMouseEnter; set => ((IViewEventSupport)ViewModel).OnMouseEnter = value; }
        [Parameter] public virtual Action<object, EventArgs> OnMouseLeave { get => ((IViewEventSupport)ViewModel).OnMouseLeave; set => ((IViewEventSupport)ViewModel).OnMouseLeave = value; }
        [Parameter] public virtual Action<object, MouseEventArgs> OnMouseMove { get => ((IViewEventSupport)ViewModel).OnMouseMove; set => ((IViewEventSupport)ViewModel).OnMouseMove = value; }
        [Parameter] public virtual Action<object, MouseEventArgs> OnMouseUp { get => ((IViewEventSupport)ViewModel).OnMouseUp; set => ((IViewEventSupport)ViewModel).OnMouseUp = value; }
        [Parameter] public virtual Action<object, EventArgs> OnResized { get => ((IViewEventSupport)ViewModel).OnResized; set => ((IViewEventSupport)ViewModel).OnResized = value; }
        [Parameter] public virtual Action<object, ScrollEventArgs> OnScrollChanged { get => ((IViewEventSupport)ViewModel).OnScrollChanged; set => ((IViewEventSupport)ViewModel).OnScrollChanged = value; }
        [Parameter] public virtual Action<object, EventArgs> OnValidated { get => ((IViewEventSupport)ViewModel).OnValidated; set => ((IViewEventSupport)ViewModel).OnValidated = value; }
        
        public virtual void ApplyPropertyChangedEvent(PropertyChangedMessage evt)
        {
            ((IChangeSupport)ChangeSupport).ApplyPropertyChangedEvent(evt);
        }

        public virtual void Bind(string propertyName, IChangeSupport target, string relativePropertyName)
        {
            ((IChangeSupport)ChangeSupport).Bind(propertyName, target, relativePropertyName);
        }
    }

}
