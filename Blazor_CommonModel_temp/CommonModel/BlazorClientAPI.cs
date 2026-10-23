

using Blazor_CommonModel.Components.Shared;

using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Concurrent;
using System.Drawing;
using System.Text.Json;

namespace BlazorErp.CommonModel
{
    public sealed class BlazorClientAPI : IClientAPI
    {
        private readonly IJSRuntime? _js;
        private readonly IDialogService? _dialogs;
        private readonly ConcurrentDictionary<string, Action> _callbacks = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, (string Id, string Type, string Js)> _listeners = new(StringComparer.Ordinal);

        public BlazorClientAPI(IJSRuntime js, IDialogService dialogs)
        {
            _js = js;
            _dialogs = dialogs;
        }

        public bool InfoDialog(string title, string text, string button)
        {
            var message = string.IsNullOrWhiteSpace(title) ? text : $"{title}\n\n{text}";
            if (!string.IsNullOrWhiteSpace(button) && !message.Contains(button, StringComparison.Ordinal))
                message += $"\n\n[{button}]";

            if (InvokeJs<bool>("blazorErpClient.alert", message))
                return true;

            Console.WriteLine($"[Info] {title}: {text}");
            return true;
        }

        public void ShowHelp(string text)
        {
            if (!InvokeJs<bool>("blazorErpClient.alert", text ?? string.Empty))
                Console.WriteLine($"[Help] {text}");
        }

        public bool RemoteDialog(string title, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            if (InvokeJs<bool>("blazorErpClient.openUrl", url))
                return true;

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                });
                return true;
            }
            catch
            {
                InfoDialog(title, url, "OK");
                return false;
            }
        }

        public bool ConfirmDialog(string title, string text)
        {
            var message = string.IsNullOrWhiteSpace(title) ? text : $"{title}\n\n{text}";
            var jsResult = InvokeJs<bool?>("blazorErpClient.confirm", message);
            if (jsResult.HasValue)
                return jsResult.Value;

            Console.WriteLine($"[Confirm] {message} -> true (fallback)");
            return true;
        }

        public bool CreateEntityDialog(string title, string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
                return false;

            InfoDialog(title, $"Создание сущности: {entity}", "OK");
            return true;
        }

        public object InputDialog(string title, object properties)
        {
            if (properties is null)
                return null!;

            InfoDialog(title, $"Ввод свойств: {properties}", "OK");
            return properties;
        }

        public string Eval(string js)
        {
            if (string.IsNullOrWhiteSpace(js))
                return string.Empty;

            var fromHelper = InvokeJs<string>("blazorErpClient.evalJs", js);
            if (fromHelper != null)
                return fromHelper;

            return InvokeEvalFallback(js);
        }

        public string HandleEvalResult(Func<object, object> handle, string js)
        {
            var raw = Eval(js);
            if (handle == null)
                return raw;

            try
            {
                var handled = handle(raw);
                return handled?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Callback(string action, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(action))
                return string.Empty;

            var key = args.Length == 0 ? action : $"{action}:{string.Join('|', args)}";
            if (_callbacks.TryGetValue(key, out var callback))
            {
                callback();
                return "ok";
            }

            return Eval($"blazorErpClient._callbacks && blazorErpClient._callbacks[{JsonSerializer.Serialize(key)}]");
        }

        public bool AddEventListener(string id, string type, string js)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(type))
                return false;

            var handlerId = Guid.NewGuid().ToString("N");
            _listeners[handlerId] = (id, type, js);
            _callbacks[handlerId] = () => Eval(js);

            if (InvokeJs<bool>("blazorErpClient.addEventListener", id, type, handlerId))
                return true;

            return _listeners.ContainsKey(handlerId);
        }

        public bool DispatchEvent(string id, string type, object message)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(type))
                return false;

            if (InvokeJs<bool>("blazorErpClient.dispatchEvent", id, type, message))
                return true;

            return false;
        }

        public string OnConnected(string token) => token ?? string.Empty;

        public void SubmitDialog(string title, object control, Action<object> value)
        {
            if (control == null)
            {
                value?.Invoke(null);
                return;
            }

            if (_dialogs != null)
            {
                _ = ShowSubmitDialogAsync(title, control, value);
                return;
            }

            var accepted = ConfirmDialog(title, "Подтвердить отправку данных?");
            value?.Invoke(accepted ? control : null);
        }

        private async Task ShowSubmitDialogAsync(string title, object control, Action<object>? value)
        {
            try
            {
                var parameters = new DialogParameters
                {
                    [nameof(SubmitDialogMessage.Title)] = title,
                    [nameof(SubmitDialogMessage.ControlDescription)] = control.ToString() ?? control.GetType().Name,
                };

                var dialog = _dialogs!.Show<SubmitDialogMessage>(title, parameters);
                var result = await dialog.Result;
                if (result is { Canceled: false })
                    value?.Invoke(control);
                else
                    value?.Invoke(null);
            }
            catch
            {
                value?.Invoke(null);
            }
        }

        private string InvokeEvalFallback(string js)
        {
            if (_js == null)
                return string.Empty;

            try
            {
                var wrapped = "(function(){try{" + js + "}catch(e){return String(e);}})()";
                return _js.InvokeAsync<string>("eval", wrapped).AsTask().ConfigureAwait(false).GetAwaiter().GetResult() ?? string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private T? InvokeJs<T>(string identifier, params object?[] args)
        {
            if (_js == null)
                return default;

            try
            {
                return _js.InvokeAsync<T>(identifier, args).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (JSException)
            {
                return default;
            }
            catch (InvalidOperationException)
            {
                // JS ещё недоступен (prerender / вне circuit).
                return default;
            }
        }

        public Point GetMousePosition()
        {
            throw new NotImplementedException();
        }
    }

}
