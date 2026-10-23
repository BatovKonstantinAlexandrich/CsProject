using System;

public interface ConnectionEvents
{
    string OnConnected(string token);
}


public interface IModalDialogAPI
{
    bool InfoDialog(string Title, string Text, string Button);
    void ShowHelp(string Text);
    bool RemoteDialog(string Title, string Url);
    bool ConfirmDialog(string Title, string Text);
    bool CreateEntityDialog(string Title, string Entity);
    object InputDialog(string Title, object Properties);

}



public interface IDomAPI
{
    string Eval(string js);
    string HandleEvalResult(Func<object, object> handle, string js);
    string Callback(string action, params string[] args);
    bool AddEventListener(string id, string type, string js);
    bool DispatchEvent(string id, string type, object message);
}


public interface IAngularAPI
{
    
}



public interface IClientAPI : IModalDialogAPI, IDomAPI, IAngularAPI, ConnectionEvents
{
    public Point GetMousePosition();
    public void SubmitDialog(string title, object control, Action<object> value);
    public Point PointToClient(Point point);
    public void ShowSnakBar(string title);
    Task<bool> ShowView<TAppView>(string title, TAppView model, Action<bool, TAppView> oncompleted) where TAppView : AppViewBase;

}