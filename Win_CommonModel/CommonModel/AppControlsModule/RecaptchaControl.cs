using System.Drawing;
using System.Net;
using System.Text.Json;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.MediaModule;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Отображает Google reCAPTCHA v2 через WebView2 и передаёт токен в <see cref="RecaptchaView.ResponseToken"/>.
/// </summary>
public class RecaptchaControl : ControlBase
{
    private readonly WebView2 _webView = new()
    {
        Dock = DockStyle.Fill,
    };

    private readonly Label _fallback = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Color.Gray,
        BackColor = Color.FromArgb(250, 250, 250),
    };

    private bool _webViewReady;
    private RecaptchaView _model;
    private bool _handlersAttached;

    public RecaptchaControl()
    {
        Dock = DockStyle.Fill;
        MinimumSize = new Size(304, 90);
        Controls.Add(_webView);
        Load += OnLoad;
        HandleCreated += (_, _) => _ = EnsureWebViewAsync();
        HandleDestroyed += OnHandleDestroyed;
    }

    private void OnHandleDestroyed(object sender, EventArgs e)
    {
        try
        {
            if (_webViewReady && _webView.CoreWebView2 is not null)
                _webView.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
        }
        catch
        {
            /* ignore */
        }
        _webViewReady = false;
    }

    private void OnLoad(object sender, EventArgs e)
    {
        if (viewModel is RecaptchaView rv)
            Bind(rv);
        _ = EnsureWebViewAsync();
    }

    private void Bind(RecaptchaView rv)
    {
        _model = rv;
        if (_handlersAttached)
            return;
        _handlersAttached = true;
        rv.AddPropertyChangeListener(nameof(RecaptchaView.SiteKey), _ => NavigateRecaptcha(rv));
        rv.AddPropertyChangeListener(nameof(RecaptchaView.Language), _ => NavigateRecaptcha(rv));
        rv.AddPropertyChangeListener(nameof(RecaptchaView.Theme), _ => NavigateRecaptcha(rv));
        rv.AddPropertyChangeListener(nameof(RecaptchaView.Size), _ => NavigateRecaptcha(rv));
    }

    private async System.Threading.Tasks.Task EnsureWebViewAsync()
    {
        if (_webViewReady || IsDisposed || !IsHandleCreated)
            return;

        try
        {
            await _webView.EnsureCoreWebView2Async();
            _webView.CoreWebView2.Settings.IsScriptEnabled = true;
            _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            _webViewReady = true;
            _fallback.Visible = false;

            if (_model is not null)
                NavigateRecaptcha(_model);
            else if (viewModel is RecaptchaView rv)
            {
                Bind(rv);
                NavigateRecaptcha(rv);
            }
        }
        catch (Exception ex)
        {
            ShowFallback(
                "Не удалось открыть reCAPTCHA (WebView2).\r\nУстановите Microsoft Edge WebView2 Runtime.\r\n\r\n" + ex.Message);
        }
    }

    private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        if (_model is null)
            return;

        try
        {
            var json = e.TryGetWebMessageAsString();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var type = root.GetProperty("type").GetString();
            if (type == "token")
            {
                var token = root.TryGetProperty("token", out var t) ? t.GetString() ?? "" : "";
                _model.ResponseToken = token;
            }
            else if (type == "expired")
            {
                _model.ResponseToken = "";
            }
        }
        catch
        {
            /* ignore malformed messages */
        }
    }

    private void ShowFallback(string text)
    {
        _webView.Visible = false;
        _fallback.Text = text;
        if (!Controls.Contains(_fallback))
            Controls.Add(_fallback);
        _fallback.BringToFront();
    }

    private static string BuildHtml(RecaptchaView v)
    {
        if (string.IsNullOrWhiteSpace(v.SiteKey))
        {
            return "<!DOCTYPE html><html><body style=\"margin:0;padding:16px;background:#fafafa;color:#666;font:13px Segoe UI,sans-serif\">Задайте <b>SiteKey</b> в свойствах представления.</body></html>";
        }

        var siteKeyJson = JsonSerializer.Serialize(v.SiteKey.Trim());
        var theme = v.Theme?.ToLowerInvariant() is "dark" ? "dark" : "light";
        var size = v.Size?.ToLowerInvariant() is "compact" ? "compact" : "normal";
        var hl = string.IsNullOrWhiteSpace(v.Language) ? "" : "&hl=" + WebUtility.UrlEncode(v.Language.Trim());
        var scriptSrc = "https://www.google.com/recaptcha/api.js?render=explicit" + hl;

        return $"<!DOCTYPE html><html lang=\"\"><head><meta charset=\"utf-8\"/><style>html,body{{margin:0;padding:12px;background:#fafafa;display:flex;justify-content:center;}}</style>"
            + $"<script src=\"{scriptSrc}\" async defer></script></head><body>"
            + "<script>"
            + "window.onCaptchaCompleted=function(tok){try{if(window.chrome&&window.chrome.webview)window.chrome.webview.postMessage(JSON.stringify({type:'token',token:tok||''}));}catch(e){}}"
            + "window.onCaptchaExpired=function(){try{if(window.chrome&&window.chrome.webview)window.chrome.webview.postMessage(JSON.stringify({type:'expired'}));}catch(e){}}"
            + "document.addEventListener('DOMContentLoaded',function(){"
            + $"var cfg={{sitekey:{siteKeyJson},theme:'{WebUtility.HtmlEncode(theme)}',size:'{WebUtility.HtmlEncode(size)}',callback:onCaptchaCompleted,'expired-callback':onCaptchaExpired}};"
            + "var el=document.createElement('div');"
            + "function tryRender(){if(typeof grecaptcha!=='undefined'&&grecaptcha.render){grecaptcha.render(el,cfg);}else{setTimeout(tryRender,50);}}"
            + "document.body.appendChild(el);tryRender();"
            + "});"
            + "</script></body></html>";
    }

    private void NavigateRecaptcha(RecaptchaView v)
    {
        if (!_webViewReady || _webView.CoreWebView2 is null)
            return;

        v.ResponseToken = "";
        try
        {
            _webView.Visible = true;
            if (Controls.Contains(_fallback))
                Controls.Remove(_fallback);
            _webView.NavigateToString(BuildHtml(v));
        }
        catch (Exception ex)
        {
            _webView.NavigateToString(
                "<!DOCTYPE html><html><body style=\"margin:0;padding:8px;color:#c00;font:12px sans-serif\">"
                + WebUtility.HtmlEncode(ex.Message)
                + "</body></html>");
        }
    }

    /// <summary>Сброс виджета (например после ошибки на сервере).</summary>
    public async System.Threading.Tasks.Task ResetWidgetAsync()
    {
        if (!_webViewReady || _webView.CoreWebView2 is null)
            return;
        try
        {
            await _webView.ExecuteScriptAsync("try{if(window.grecaptcha&&grecaptcha.reset)grecaptcha.reset();}catch(e){}");
        }
        catch
        {
            /* ignore */
        }
    }
}
