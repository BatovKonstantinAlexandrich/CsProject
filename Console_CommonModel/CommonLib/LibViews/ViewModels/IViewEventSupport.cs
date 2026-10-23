using Console_CommonModel.ActionModels;

namespace ActionModels
{
    public interface IViewEventSupport
    {
        Action<object, EventArgs> OnClick { get; set; }
        Action<object, DragEventArgs> OnDragEnter { get; set; }
        Action<object, EventArgs> OnDragged { get; set; }
        Action<object, EventArgs> OnDragLeave { get; set; }
        Action<object, DragEventArgs> OnDragOver { get; set; }
        Action<object, EventArgs> OnDropped { get; set; }
        Action<object, EventArgs> OnKeyClicked { get; set; }
        Action<object, KeyEventArgs> OnKeyDown { get; set; }
        Action<object, KeyPressEventArgs> OnKeyPressed { get; set; }
        Action<object, KeyEventArgs> OnKeyUp { get; set; }
        Action<object, EventArgs> OnLoad { get; set; }
        Action<object, EventArgs> OnLocationChanged { get; set; }
        Action<object, MouseEventArgs> OnMouseDoubleClick { get; set; }
        Action<object, MouseEventArgs> OnMouseDown { get; set; }
        Action<object, EventArgs> OnMouseEnter { get; set; }
        Action<object, EventArgs> OnMouseLeave { get; set; }
        Action<object, MouseEventArgs> OnMouseMove { get; set; }
        Action<object, MouseEventArgs> OnMouseUp { get; set; }
        Action<object, EventArgs> OnResized { get; set; }
        Action<object, ScrollEventArgs> OnScrollChanged { get; set; }
        Action<object, EventArgs> OnValidated { get; set; }

        void AddOnClick(Action<object, EventArgs> todo);
        void AddOnKeyClicked(Action<object, EventArgs> todo);
        void AddOnKeyDown(Action<object, EventArgs> todo);
        void AddOnKeyPressed(Action<object, KeyPressEventArgs> todo);
        void AddOnKeyUp(Action<object, EventArgs> todo);
        void AddOnLoad(Action<object, EventArgs> todo);
        void AddOnMouseDoubleClick(Action<object, MouseEventArgs> todo);
        void AddOnMouseDown(Action<object, EventArgs> todo);
        void AddOnMouseEnter(Action<object, EventArgs> todo);
        void AddOnMouseLeave(Action<object, EventArgs> todo);
        void AddOnMouseMove(Action<object, EventArgs> todo);
        void AddOnMouseUp(Action<object, EventArgs> todo);
        void AddOnResized(Action<object, EventArgs> todo);
        void Dispose();
        void FireEvent(CommonEventMessage<string> message);
        void FireEvent(PropertyChangedMessage propertyChangedMessage);
        void FireEvent<T>(CommonEventMessage<T> message);
        void FireEvent<T>(T message);
        List<string> GetBaseEventProperties();
        List<string> GetEventListeners();
         

        void RemoveOnClick(Action<object, EventArgs> todo);
        void RemoveOnKeyClicked(Action<object, EventArgs> todo);
        void RemoveOnKeyDown(Action<object, EventArgs> todo);
        void RemoveOnKeyPressed(Action<object, KeyPressEventArgs> todo);
        void RemoveOnKeyUp(Action<object, EventArgs> todo);
        void RemoveOnLoad(Action<object, EventArgs> todo);
        void RemoveOnMouseDoubleClick(Action<object, MouseEventArgs> todo);
        void RemoveOnMouseDown(Action<object, EventArgs> todo);
        void RemoveOnMouseEnter(Action<object, EventArgs> todo);
        void RemoveOnMouseLeave(Action<object, EventArgs> todo);
        void RemoveOnMouseMove(Action<object, EventArgs> todo);
        void RemoveOnMouseUp(Action<object, EventArgs> todo);
        void RemoveOnResized(Action<object, EventArgs> todo);
    }
}