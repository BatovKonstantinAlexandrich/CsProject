using ActionModels;

using Console_CommonModel.ActionModels;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Microsoft.AspNetCore.Components;

using System.Collections.Concurrent;
using System.Drawing;

namespace Blazor_CommonModel.Components.Shared
{
    public class WebComponentBase: ComponentBase, IControlBase
    {
        public AppViewBase viewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Console_CommonModel.CommonLib.Others.ComponentBase component { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Capture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsHandleCreated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool UseModernStyle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool InvokeRequired { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsDisposed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Size Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool KeyPreview { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ShowSnakBar(string title)
        {
            throw new NotImplementedException();
        }

        public void Invoke(Action action)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void SetLeft(IControlBase value)
        {
            throw new NotImplementedException();
        }

        public void SetTop(IControlBase value)
        {
            throw new NotImplementedException();
        }

        public void SetRight(IControlBase value)
        {
            throw new NotImplementedException();
        }

        public void SetBottom(IControlBase value)
        {
            throw new NotImplementedException();
        }

        public IControlBase ToBaseForm(string v = "")
        {
            throw new NotImplementedException();
        }

        public IControlBase RunAtForm(string ttile = "")
        {
            throw new NotImplementedException();
        }

        public IControlBase GetAppForm()
        {
            throw new NotImplementedException();
        }

        public Point PointToClient(Point point)
        {
            throw new NotImplementedException();
        }

        public ChangingList<IControlBase> Controls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void BeforeSendEvent(object messageEvent)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void FireEvent(CommonEventMessage<string> message)
        {
            throw new NotImplementedException();
        }

        public void FireEvent(PropertyChangedMessage propertyChangedMessage)
        {
            throw new NotImplementedException();
        }

        public void FireEvent<T>(CommonEventMessage<T> message)
        {
            throw new NotImplementedException();
        }

        public void FireEvent<T>(T message)
        {
            throw new NotImplementedException();
        }

        public List<string> GetEventListeners()
        {
            throw new NotImplementedException();
        }

        public void SendChanges()
        {
            throw new NotImplementedException();
        }

        public void SendEvent(object messageEvent)
        {
            throw new NotImplementedException();
        }

        public bool Changed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ConcurrentDictionary<string, object> DataState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ValidateOnInput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ApplyPropertyChangedEvent(PropertyChangedMessage evt)
        {
            throw new NotImplementedException();
        }

        public void Bind(string propertyName, IChangeSupport target, string relativePropertyName)
        {
            throw new NotImplementedException();
        }

        public Action<PropertyChangedMessage> OnChange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            throw new NotImplementedException();
        }

        public void AddPropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            throw new NotImplementedException();
        }

        public void AddPropertyChangeListener<T>(string property, Action<T> changed)
        {
            throw new NotImplementedException();
        }

        public void AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void OnGet<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public bool WasChanged()
        {
            throw new NotImplementedException();
        }

        public void SubmitDialog(string title, object control, Action<object> value)
        {
            throw new NotImplementedException();
        }

        public bool InfoDialog(string Title, string Text, string Button)
        {
            throw new NotImplementedException();
        }

        public void ShowHelp(string Text)
        {
            throw new NotImplementedException();
        }

        public bool RemoteDialog(string Title, string Url)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmDialog(string Title, string Text)
        {
            throw new NotImplementedException();
        }

        public bool CreateEntityDialog(string Title, string Entity)
        {
            throw new NotImplementedException();
        }

        public object InputDialog(string Title, object Properties)
        {
            throw new NotImplementedException();
        }

        public string Eval(string js)
        {
            throw new NotImplementedException();
        }

        public string HandleEvalResult(Func<object, object> handle, string js)
        {
            throw new NotImplementedException();
        }

        public string Callback(string action, params string[] args)
        {
            throw new NotImplementedException();
        }

        public bool AddEventListener(string id, string type, string js)
        {
            throw new NotImplementedException();
        }

        public bool DispatchEvent(string id, string type, object message)
        {
            throw new NotImplementedException();
        }

        public string OnConnected(string token)
        {
            throw new NotImplementedException();
        }

        public bool AnchorBottom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AnchorLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AnchorRight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AnchorTop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BackgroundColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BorderColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float BorderWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Cursor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Dock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float FontSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Height { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Margin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MinHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MinWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Padding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TextAlign { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TextIndent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Width { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int X { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Y { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string GetCssText()
        {
            throw new NotImplementedException();
        }

        public StyledItem GetStyle()
        {
            throw new NotImplementedException();
        }

        public StyledItem MaxSize()
        {
            throw new NotImplementedException();
        }

        public void SetBaseTextStyle()
        {
            throw new NotImplementedException();
        }

        public IControlBase Left { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IControlBase Right { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IControlBase Top { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IControlBase Bottom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IControlBase ShowDialog(string title = "")
        {
            throw new NotImplementedException();
        }

        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnClick { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, DragEventArgs> OnDragEnter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnDragged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnDragLeave { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, DragEventArgs> OnDragOver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnDropped { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnKeyClicked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, KeyEventArgs> OnKeyDown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, KeyPressEventArgs> OnKeyPressed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, KeyEventArgs> OnKeyUp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnLoad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnLocationChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, MouseEventArgs> OnMouseDoubleClick { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, MouseEventArgs> OnMouseDown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnMouseEnter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnMouseLeave { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, MouseEventArgs> OnMouseMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, MouseEventArgs> OnMouseUp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnResized { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, ScrollEventArgs> OnScrollChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnValidated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddOnClick(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyClicked(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyPressed(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnLoad(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseDoubleClick(Action<object, MouseEventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseEnter(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseLeave(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseMove(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnResized(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public List<string> GetBaseEventProperties()
        {
            throw new NotImplementedException();
        }

        public void RemoveOnClick(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyClicked(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyPressed(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnLoad(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseDoubleClick(Action<object, MouseEventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseEnter(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseLeave(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseMove(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnResized(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public IControlBase Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Point GetMousePosition()
        {
            throw new NotImplementedException();
        }

        public string Name => throw new NotImplementedException();

        string INamedObject.Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
