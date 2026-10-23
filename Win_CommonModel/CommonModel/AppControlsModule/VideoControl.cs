using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.AppViews.MediaModule;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Воспроизведение видео через WebView2 (Chromium): локальные файлы и HTTP(S)-URL.
/// </summary>
public class VideoControl : ControlBase
{
    private const string LocalVideoHost = "local.appvideo";

    private readonly WebView2 _webView = new()
    {
        Dock = DockStyle.Fill,
    };

    private readonly Label _fallback = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Color.FromArgb(220, 220, 220),
        BackColor = Color.FromArgb(24, 24, 28),
        Font = new Font("Segoe UI", 9.5f),
        Visible = false,
    };

    /// <summary>
    /// WebView2 не пробрасывает WinForms Mouse* — в DesignMode поверх него слой для перетаскивания в конструкторе.
    /// </summary>
    private readonly Label _designSurface = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Color.FromArgb(220, 220, 220),
        BackColor = Color.FromArgb(32, 36, 44),
        Font = new Font("Segoe UI", 9.5f),
        Visible = false,
    };

    private bool _webViewReady;
    private VideoView _model;
    private bool _handlersAttached;
    private bool _designModeHandlerAttached;
    private bool _contextMenuHandlerAttached;
    private string _pendingPath;
    private bool _pendingAutoPlay = true;

    public VideoControl()
    {
        Dock = DockStyle.Fill;
        MinimumSize = new Size(320, 240);
        Controls.Add(_webView);
        Controls.Add(_fallback);
        Controls.Add(_designSurface);
        Load += OnVideoControlLoaded;
        HandleCreated += (_, _) => _ = EnsureWebViewAsync();
        HandleDestroyed += OnHandleDestroyed;
    }

    /// <summary>Привязка модели сразу после <see cref="ComponentsFactory.CreateVideoPanel"/>.</summary>
    public void BindViewModel(VideoView view)
    {
        if (view is null)
            return;
        viewModel = view;
        Bind(view);
        ApplySource(view.Source, view.AutoPlay);
        ApplyDesignModeSurface(view.DesignMode);
    }

    private void OnHandleDestroyed(object sender, EventArgs e)
    {
        try
        {
            if (_webView.CoreWebView2 is not null)
                _webView.CoreWebView2.ContextMenuRequested -= OnWebViewContextMenuRequested;
        }
        catch
        {
            /* ignore */
        }
        _contextMenuHandlerAttached = false;
        _webViewReady = false;
    }

    private void OnVideoControlLoaded(object sender, EventArgs e)
    {
        if (viewModel is VideoView vv)
            BindViewModel(vv);
        else
            _ = EnsureWebViewAsync();
    }

    private void Bind(VideoView vv)
    {
        _model = vv;
        if (!_handlersAttached)
        {
            _handlersAttached = true;
            vv.AddPropertyChangeListener(nameof(VideoView.Source), evt =>
                ApplySource(evt.After?.ToString() ?? "", vv.AutoPlay));
            vv.AddPropertyChangeListener(nameof(VideoView.AutoPlay), evt =>
                ApplySource(vv.Source, evt.After.ToString().ToBool()));
            vv.AddPropertyChangeListener(nameof(VideoView.Text), _ =>
                UpdateDesignSurfaceText());
        }

        if (!_designModeHandlerAttached)
        {
            _designModeHandlerAttached = true;
            vv.AddPropertyChangeListener(nameof(AppViewBase.DesignMode), evt =>
                ApplyDesignModeSurface(evt.After.ToString().ToBool()));
        }
    }

    private void ApplyDesignModeSurface(bool designMode)
    {
        if (designMode)
        {
            UpdateDesignSurfaceText();
            _webView.Visible = false;
            _fallback.Visible = false;
            _designSurface.Visible = true;
            _designSurface.BringToFront();
            BindMouseEvents(_designSurface);
            ApplyViewContextMenu();
            return;
        }

        _designSurface.Visible = false;
        _webView.Visible = true;
        if (!string.IsNullOrEmpty(_fallback.Text) && _fallback.Visible)
            _fallback.BringToFront();
    }

    private void UpdateDesignSurfaceText()
    {
        var caption = string.IsNullOrWhiteSpace(_model?.Text) ? "Видео" : _model.Text.Trim();
        var source = string.IsNullOrWhiteSpace(_model?.Source) ? "источник не задан" : _model.Source.Trim();
        _designSurface.Text = $"{caption}\r\n\r\n{source}\r\n\r\n(режим конструктора — перетаскивание)";
    }

    private async Task EnsureWebViewAsync()
    {
        if (_webViewReady || IsDisposed || !IsHandleCreated)
            return;

        try
        {
            await _webView.EnsureCoreWebView2Async();
            _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            _webViewReady = true;
            _fallback.Visible = false;
            ApplyViewContextMenu();

            if (_model is null && viewModel is VideoView vv)
                Bind(vv);

            if (!string.IsNullOrWhiteSpace(_pendingPath) || _model is not null)
            {
                var path = _model?.Source ?? _pendingPath ?? "";
                var autoPlay = _model?.AutoPlay ?? _pendingAutoPlay;
                ApplySource(path, autoPlay);
            }
        }
        catch (Exception ex)
        {
            ShowFallback(
                "Не удалось инициализировать проигрыватель (WebView2).\r\n" +
                "Установите Microsoft Edge WebView2 Runtime.\r\n\r\n" + ex.Message);
        }
    }

    private void ApplySource(string path, bool autoPlay)
    {
        _pendingPath = path;
        _pendingAutoPlay = autoPlay;

        if (!_webViewReady || _webView.CoreWebView2 is null)
        {
            _ = EnsureWebViewAsync();
            return;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                ShowHtmlMessage("Нет источника видео");
                return;
            }

            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                ShowHtmlVideo(path.Trim(), autoPlay);
                return;
            }

            var fullPath = Path.GetFullPath(path.Trim());
            if (!File.Exists(fullPath))
            {
                ShowHtmlMessage($"Файл не найден:\r\n{fullPath}");
                return;
            }

            var folder = Path.GetDirectoryName(fullPath) ?? "";
            var fileName = Path.GetFileName(fullPath);
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                LocalVideoHost,
                folder,
                CoreWebView2HostResourceAccessKind.Allow);

            var mediaUrl = $"https://{LocalVideoHost}/{Uri.EscapeDataString(fileName)}";
            ShowHtmlVideo(mediaUrl, autoPlay);
        }
        catch (Exception ex)
        {
            ShowFallback(ex.Message);
        }
    }

    private void ShowHtmlVideo(string mediaUrl, bool autoPlay)
    {
        var ap = autoPlay ? "autoplay muted playsinline" : "";
        var esc = WebUtility.HtmlEncode(mediaUrl);
        _webView.NavigateToString(
            $"<!DOCTYPE html><html><head><meta charset=\"utf-8\"/></head>" +
            $"<body style=\"margin:0;overflow:hidden;background:#000;height:100vh\">" +
            $"<video controls style=\"width:100%;height:100%;object-fit:contain\" {ap}>" +
            $"<source src=\"{esc}\" type=\"video/mp4\">" +
            $"<source src=\"{esc}\" type=\"video/quicktime\">" +
            $"</video></body></html>");
        _fallback.Visible = false;
    }

    private void ShowHtmlMessage(string message)
    {
        var esc = WebUtility.HtmlEncode(message);
        _webView.NavigateToString(
            $"<!DOCTYPE html><html><body style='margin:0;background:#222;color:#ccc;" +
            $"font:12px Segoe UI,sans-serif;padding:12px;white-space:pre-wrap'>{esc}</body></html>");
        _fallback.Visible = false;
    }

    private void ShowFallback(string message)
    {
        _fallback.Text = message;
        _fallback.Visible = true;
        _fallback.BringToFront();
        ApplyViewContextMenu();
    }

    /// <summary>
    /// Пробрасывает <see cref="ContextMenuStrip"/> модели (<see cref="ComponentsFactory.BindContextMenu"/> / <see cref="ContextView"/>)
    /// на область видео. У WebView2 <c>ContextMenuStrip</c> только для чтения — используется <see cref="CoreWebView2.ContextMenuRequested"/>.
    /// </summary>
    public void ApplyViewContextMenu()
    {
        var menu = ContextMenuStrip;
        _fallback.ContextMenuStrip = menu;
        _designSurface.ContextMenuStrip = menu;
        if (_webViewReady && _webView.CoreWebView2 is not null)
            WireWebViewContextMenu();
    }

    private void WireWebViewContextMenu()
    {
        if (_contextMenuHandlerAttached || _webView.CoreWebView2 is null)
            return;
        _contextMenuHandlerAttached = true;
        _webView.CoreWebView2.ContextMenuRequested += OnWebViewContextMenuRequested;
    }

    private void OnWebViewContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs e)
    {
        var menu = ContextMenuStrip;
        if (menu is null || menu.Items.Count == 0)
            return;

        e.Handled = true;
        var deferral = e.GetDeferral();
        void OnClosed(object s, ToolStripDropDownClosedEventArgs args)
        {
            menu.Closed -= OnClosed;
            deferral.Complete();
        }
        menu.Closed += OnClosed;
        menu.Show(_webView, e.Location);
    }
}
