
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Microsoft.JSInterop;

using System.Collections.Concurrent;
using System.Drawing;
using System.Text.Json;

namespace Blazor_CommonModel.App
{

    public sealed class BlazorClientAPI : IClientAPI
    {
        public Point GetMousePosition()
        {
            throw new NotImplementedException();
        }

        public void SubmitDialog(string title, object control, Action<object> value)
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
    }

}
