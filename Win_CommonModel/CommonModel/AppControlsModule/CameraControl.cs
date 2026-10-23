using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using OpenCvSharp;
using OpenCvSharp.Extensions;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.MediaModule;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Предпросмотр с системной камеры: нативный захват (OpenCV) или WebView2 (<c>getUserMedia</c>).
/// Ошибки и статус отображаются на панели и в <see cref="CameraView.StatusMessage"/> / <see cref="CameraView.LastError"/>.
/// </summary>
public class CameraControl : ControlBase
{
    private const int MaxConsecutiveReadFailures = 45;
    private const int NoFrameWatchdogMs = 4000;

    private readonly PictureBox _preview = new()
    {
        Dock = DockStyle.Fill,
        BackColor = Color.Black,
        SizeMode = PictureBoxSizeMode.Zoom,
    };

    private readonly WebView2 _webView = new()
    {
        Dock = DockStyle.Fill,
        Visible = false,
    };

    private readonly Label _fallback = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter,
        ForeColor = Color.FromArgb(255, 180, 120),
        BackColor = Color.FromArgb(24, 24, 28),
        Font = new Font("Segoe UI", 9.5f),
        Visible = false,
    };

    private readonly Label _diagnostic = new()
    {
        Dock = DockStyle.Bottom,
        AutoSize = false,
        Height = 56,
        Padding = new Padding(8, 6, 8, 6),
        TextAlign = ContentAlignment.TopLeft,
        ForeColor = Color.FromArgb(220, 220, 220),
        BackColor = Color.FromArgb(200, 0, 0, 0),
        Font = new Font("Segoe UI", 8.5f),
        Visible = false,
    };

    private readonly System.Windows.Forms.Timer _frameTimer = new() { Interval = 33 };
    private readonly System.Windows.Forms.Timer _watchdogTimer = new() { Interval = 500 };

    private VideoCapture _capture;
    private CameraView _cameraView;
    private bool _webViewReady;
    private bool _useNativeCapture = true;
    private bool _reentryGuard;
    private int _activeDeviceIndex = -1;
    private int _consecutiveReadFailures;
    private int _framesDecoded;
    private DateTime _captureStartedUtc;
    private string _nativeBackend = "";
    private readonly CameraSignalRStreamer _signalR = new();
    private int _initGeneration;

    /// <summary>Привязка модели сразу после <see cref="ComponentsFactory.CreateCameraPanel"/>.</summary>
    public void BindViewModel(CameraView view)
    {
        if (view is null)
            return;
        viewModel = view;
        view.control = this;
        BindCameraView(view);
    }

    /// <summary>Подключить существующее соединение SignalR (например из <c>ActionsConsumer</c>).</summary>
    public void AttachSignalRHubConnection(HubConnection connection)
    {
        EnsureCameraView();
        _signalR.Configure(_cameraView!, OnSignalRStatus, OnSignalRError, OnSignalRConnectionState);
        _signalR.AttachHubConnection(connection);
    }

    public CameraControl()
    {
        Dock = DockStyle.Fill;
        MinimumSize = new System.Drawing.Size(320, 240);
        Controls.Add(_preview);
        Controls.Add(_webView);
        Controls.Add(_fallback);
        Controls.Add(_diagnostic);
        Load += OnCameraControlLoad;
        VisibleChanged += (_, _) =>
        {
            if (Visible && IsHandleCreated)
                ScheduleCameraInitialization();
        };
        HandleDestroyed += OnCameraControlHandleDestroyed;
        _frameTimer.Tick += OnFrameTimerTick;
        _watchdogTimer.Tick += OnWatchdogTick;
    }

    private void OnCameraControlLoad(object sender, EventArgs e)
    {
        EnsureCameraView();
        ScheduleCameraInitialization();
    }

    private void EnsureCameraView()
    {
        if (_cameraView is not null)
            return;

        if (viewModel is CameraView cv)
            BindCameraView(cv);
        else
        {
            var view = new CameraView();
            view.control = this;
            viewModel = view;
            BindCameraView(view);
        }
    }

    private void BindCameraView(CameraView cv)
    {
        if (_cameraView == cv)
            return;

        _cameraView = cv;
        cv.AddPropertyChangeListener(nameof(CameraView.DeviceIndex), _ =>
        {
            if (!_reentryGuard)
                ApplyCameraFromView();
        });
        cv.AddPropertyChangeListener(nameof(CameraView.DeviceId), _ => ApplyCameraFromView());
        cv.AddPropertyChangeListener(nameof(CameraView.AutoStart), _ => ApplyCameraFromView());
        cv.AddPropertyChangeListener(nameof(CameraView.UseNativeCapture), _ => ApplyCameraFromView());
        cv.AddPropertyChangeListener(nameof(CameraView.MirrorPreview), _ => { /* зеркало в OnFrameTimerTick */ });
        cv.AddPropertyChangeListener(nameof(CameraView.SignalREnabled), _ => ApplySignalRFromView());
        cv.AddPropertyChangeListener(nameof(CameraView.SignalRHubUrl), _ => ApplySignalRFromView());
        cv.AddPropertyChangeListener(nameof(CameraView.SignalRSendMethod), _ => { });
        cv.AddPropertyChangeListener(nameof(CameraView.SignalRStreamId), _ => ApplySignalRFromView());
        cv.AddPropertyChangeListener(nameof(CameraView.SignalRAccessToken), _ => ApplySignalRFromView());
        cv.AddPropertyChangeListener(nameof(CameraView.SignalRAutoConnect), _ => ApplySignalRFromView());

        _signalR.Configure(cv, OnSignalRStatus, OnSignalRError, OnSignalRConnectionState);
        ApplySignalRFromView();
    }

    private void ApplySignalRFromView()
    {
        if (_cameraView is null)
            return;

        if (!_cameraView.SignalREnabled)
        {
            _ = _signalR.DisconnectAsync();
            return;
        }

        if (!_cameraView.UseNativeCapture)
        {
            SetStatus("SignalR: для передачи кадров включите нативный захват (UseNativeCapture).");
        }

        if (_cameraView.SignalRAutoConnect && !_signalR.IsConnected)
            _ = _signalR.ConnectAsync();
    }

    private void ApplyCameraFromView()
        => ScheduleCameraInitialization();

    private void ScheduleCameraInitialization()
    {
        EnsureCameraView();
        if (_cameraView is null || IsDisposed)
            return;

        var generation = Interlocked.Increment(ref _initGeneration);

        _cameraView.RunOnUiThread(() =>
        {
            if (!IsInitializationCurrent(generation))
                return;
            if (!_cameraView.AutoStart)
            {
                StopAllCapture();
                ShowFatal("Автозапуск выключен (AutoStart = false).");
                return;
            }

            _useNativeCapture = _cameraView.UseNativeCapture;
            if (_cameraView.SignalREnabled && !_useNativeCapture)
                _useNativeCapture = true;

            StopWebViewStream();
            _fallback.Visible = false;
            _preview.Visible = true;
            _webView.Visible = !_useNativeCapture;
            _diagnostic.BringToFront();
            SetCaptureMode(_useNativeCapture ? "OpenCV (нативный)" : "WebView2 (getUserMedia)");
            SetStatus("Инициализация камеры…");
        });

        PutBackgroundWork("Инициализация камеры", () => RunInitializationWorker(generation));
    }

    private void PutBackgroundWork(string name, Action work)
    {
        if (_cameraView is null)
            return;

        if (_cameraView.GetAppRootView() is not null)
            _cameraView.PutAction(name, work);
        else
            Task.Run(work);
    }

    private bool IsInitializationCurrent(int generation)
        => generation == Volatile.Read(ref _initGeneration) && !IsDisposed;

    private void RunInitializationWorker(int generation)
    {
        if (!IsInitializationCurrent(generation) || _cameraView is null)
            return;

        if (!_cameraView.AutoStart)
            return;

        var useNative = _cameraView.UseNativeCapture || _cameraView.SignalREnabled;
        if (useNative)
            RunNativeInitializationWorker(generation);
        else
        {
            _cameraView.RunOnUiThread(() =>
            {
                if (!IsInitializationCurrent(generation))
                    return;
                _useNativeCapture = false;
                StartWebViewCapture();
            });
        }
    }

    private void RunNativeInitializationWorker(int generation)
    {
        if (_cameraView is null)
            return;

        var requestedIndex = _cameraView.DeviceIndex;
        var index = requestedIndex >= 0
            ? requestedIndex
            : CameraDeviceHelper.ResolveDefaultDeviceIndex();

        if (!IsInitializationCurrent(generation))
            return;

        try
        {
            var devices = CameraDeviceHelper.EnumerateDevices();
            if (devices.Count == 0)
                throw new InvalidOperationException(
                    "Не найдено ни одной камеры (индексы 0–9).\r\n" +
                    "Проверьте: устройство подключено, драйвер установлен, доступ не занят другим приложением, " +
                    "в параметрах Windows разрешён доступ к камере для классических приложений.");

            if (devices.All(d => d.Index != index))
            {
                var list = string.Join("\r\n", devices.Select(d => $"  [{d.Index}] {d.Name}"));
                throw new InvalidOperationException(
                    $"Камера с индексом {index} недоступна.\r\nДоступные устройства:\r\n{list}");
            }

            var capture = CameraDeviceHelper.OpenCamera(index, out var backend, out var openLog);
            var deviceLabel = devices.FirstOrDefault(d => d.Index == index)?.Name ?? $"Камера {index}";

            if (!IsInitializationCurrent(generation))
            {
                capture.Release();
                capture.Dispose();
                return;
            }

            _cameraView.RunOnUiThread(() =>
            {
                if (!IsInitializationCurrent(generation))
                {
                    capture.Release();
                    capture.Dispose();
                    return;
                }

                CommitNativeCapture(capture, index, backend, openLog.Trim(), deviceLabel);
            });
        }
        catch (Exception ex)
        {
            var details = BuildNativeFailureMessage(ex, index);
            _cameraView.RunOnUiThread(() =>
            {
                if (!IsInitializationCurrent(generation))
                    return;
                ReportError(details, tryWebViewFallback: true);
            });
        }
    }

    private void CommitNativeCapture(
        VideoCapture capture,
        int index,
        string backend,
        string openLog,
        string deviceLabel)
    {
        StopNativeCapture();
        StopWebViewStream();

        _useNativeCapture = true;
        _capture = capture;
        _nativeBackend = backend;
        _activeDeviceIndex = index;
        _framesDecoded = 0;
        _consecutiveReadFailures = 0;
        _captureStartedUtc = DateTime.UtcNow;

        _webView.Visible = false;
        _fallback.Visible = false;
        _preview.Visible = true;
        _diagnostic.BringToFront();

        _reentryGuard = true;
        try
        {
            _cameraView!.DeviceIndex = index;
            _cameraView.DeviceLabel = deviceLabel;
        }
        finally
        {
            _reentryGuard = false;
        }

        ClearError();
        SetCaptureMode("OpenCV (нативный)");
        SetStatus($"Камера #{index} ({_nativeBackend}). Ожидание кадра…\r\n{openLog}");
        ApplySignalRFromView();

        _frameTimer.Start();
        _watchdogTimer.Start();
    }

    private static string BuildNativeFailureMessage(Exception ex, int index)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Нативный захват (OpenCV) не удался.");
        sb.AppendLine($"Индекс камеры: {index}");
        sb.AppendLine();
        sb.AppendLine(ex.Message);
        if (ex.InnerException is not null)
        {
            sb.AppendLine();
            sb.AppendLine("Внутренняя ошибка:");
            sb.AppendLine(ex.InnerException.Message);
        }
        return sb.ToString().TrimEnd();
    }

    private void OnFrameTimerTick(object? sender, EventArgs e)
    {
        if (_capture is null || !_capture.IsOpened() || _cameraView is null)
            return;

        try
        {
            using var frame = new Mat();
            if (!_capture.Read(frame) || frame.Empty())
            {
                _consecutiveReadFailures++;
                if (_consecutiveReadFailures >= MaxConsecutiveReadFailures)
                {
                    ReportError(
                        $"Камера #{_activeDeviceIndex} открыта ({_nativeBackend}), но кадры не приходят " +
                        $"({_consecutiveReadFailures} неудачных чтений подряд).\r\n" +
                        "Закройте другие программы, использующие камеру, или смените DeviceIndex.",
                        tryWebViewFallback: true);
                }
                else if (_consecutiveReadFailures % 15 == 0)
                {
                    SetStatus(
                        $"Камера #{_activeDeviceIndex}: кадры не читаются ({_consecutiveReadFailures})…");
                }
                return;
            }

            _consecutiveReadFailures = 0;
            _framesDecoded++;

            if (_cameraView.MirrorPreview)
                Cv2.Flip(frame, frame, FlipMode.Y);

            var old = _preview.Image;
            _preview.Image = BitmapConverter.ToBitmap(frame);
            old?.Dispose();

            if (_cameraView.SignalREnabled)
                _signalR.TrySendFrame(frame);

            if (_framesDecoded == 1)
            {
                ClearError();
                var sr = _cameraView.SignalREnabled ? " | SignalR: передача включена" : "";
                SetStatus($"Видео: камера #{_activeDeviceIndex}, {_nativeBackend}, {frame.Width}×{frame.Height}{sr}");
            }
            else if (_framesDecoded % 90 == 0)
            {
                var sent = _cameraView.SignalREnabled ? $", SignalR отправлено: {_cameraView.SignalRFramesSent}" : "";
                SetStatus($"Видео активно — {_framesDecoded} кадров, {frame.Width}×{frame.Height}{sent}");
            }
        }
        catch (Exception ex)
        {
            ReportError("Ошибка при обработке кадра:\r\n" + ex.Message, tryWebViewFallback: true);
        }
    }

    private void OnWatchdogTick(object? sender, EventArgs e)
    {
        if (_capture is null || _cameraView is null || !_frameTimer.Enabled)
            return;

        if (_framesDecoded > 0)
        {
            _watchdogTimer.Stop();
            return;
        }

        var elapsedMs = (DateTime.UtcNow - _captureStartedUtc).TotalMilliseconds;
        if (elapsedMs < NoFrameWatchdogMs)
            return;

        _watchdogTimer.Stop();
        ReportError(
            $"Камера #{_activeDeviceIndex} открыта ({_nativeBackend}), но за {(int)elapsedMs} мс " +
            "не получено ни одного кадра.\r\n" +
            "Возможные причины: камера занята другим приложением, виртуальная камера без сигнала, " +
            "устаревший драйвер, блокировка доступа в Windows (Параметры → Конфиденциальность → Камера).",
            tryWebViewFallback: true);
    }

    private void StopNativeCapture()
    {
        _frameTimer.Stop();
        _watchdogTimer.Stop();
        _activeDeviceIndex = -1;
        _framesDecoded = 0;
        _consecutiveReadFailures = 0;
        _nativeBackend = "";
        _capture?.Release();
        _capture?.Dispose();
        _capture = null;
        var old = _preview.Image;
        _preview.Image = null;
        old?.Dispose();
    }

    private async void StartWebViewCapture()
    {
        StopNativeCapture();
        _preview.Visible = false;
        _fallback.Visible = false;
        _webView.Visible = true;
        _diagnostic.BringToFront();

        SetCaptureMode("WebView2 (getUserMedia)");
        SetStatus("Инициализация WebView2 и запрос доступа к камере…");

        if (!_webViewReady)
            await EnsureWebViewAsync();
        if (_webViewReady)
            ApplyWebViewHtml();
        else if (string.IsNullOrWhiteSpace(_fallback.Text))
            ShowFatal("WebView2 не готов. Установите Microsoft Edge WebView2 Runtime.");
    }

    private async System.Threading.Tasks.Task EnsureWebViewAsync()
    {
        if (_webViewReady || IsDisposed || !IsHandleCreated)
            return;

        try
        {
            await _webView.EnsureCoreWebView2Async();
            _webView.CoreWebView2.PermissionRequested += OnPermissionRequested;
            _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            _webViewReady = true;

            if (!_useNativeCapture && _cameraView is not null && _cameraView.AutoStart)
                ApplyWebViewHtml();
        }
        catch (Exception ex)
        {
            ReportError(
                "Не удалось инициализировать WebView2.\r\n" +
                "Установите Microsoft Edge WebView2 Runtime.\r\n\r\n" + ex.Message,
                tryWebViewFallback: false);
        }
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            var json = e.TryGetWebMessageAsString();
            if (string.IsNullOrWhiteSpace(json) || _cameraView is null)
                return;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("type", out var typeEl))
                return;

            var type = typeEl.GetString() ?? "";
            switch (type)
            {
                case "device":
                    if (root.TryGetProperty("deviceId", out var idEl))
                        _cameraView.DeviceId = idEl.GetString() ?? "";
                    if (root.TryGetProperty("label", out var labelEl))
                        _cameraView.DeviceLabel = labelEl.GetString() ?? "";
                    ClearError();
                    SetStatus("WebView2: поток с камеры активен" +
                              (string.IsNullOrWhiteSpace(_cameraView.DeviceLabel)
                                  ? ""
                                  : $" — {_cameraView.DeviceLabel}"));
                    break;

                case "status":
                    SetStatus(root.TryGetProperty("message", out var msgEl)
                        ? msgEl.GetString() ?? ""
                        : "WebView2: ожидание…");
                    break;

                case "error":
                    var err = root.TryGetProperty("message", out var errEl)
                        ? errEl.GetString() ?? "Неизвестная ошибка"
                        : "Неизвестная ошибка";
                    var name = root.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;
                    var cameras = root.TryGetProperty("cameraCount", out var ccEl) ? ccEl.GetInt32() : -1;

                    var sb = new StringBuilder();
                    sb.AppendLine("WebView2 / getUserMedia:");
                    if (!string.IsNullOrWhiteSpace(name))
                        sb.AppendLine($"[{name}]");
                    sb.AppendLine(err);
                    if (cameras >= 0)
                        sb.AppendLine($"Найдено videoinput: {cameras}");
                    sb.AppendLine();
                    sb.AppendLine("Проверьте разрешение камеры для приложения в Windows и что камера не занята.");

                    ReportError(sb.ToString().TrimEnd(), tryWebViewFallback: false);
                    break;
            }
        }
        catch (Exception ex)
        {
            SetStatus("Ошибка разбора сообщения WebView2: " + ex.Message);
        }
    }

    private static void OnPermissionRequested(object? sender, CoreWebView2PermissionRequestedEventArgs e)
    {
        if (e.PermissionKind is CoreWebView2PermissionKind.Camera
            or CoreWebView2PermissionKind.Microphone)
            e.State = CoreWebView2PermissionState.Allow;
    }

    private void ShowFatal(string text)
    {
        StopAllCapture();
        _webView.Visible = false;
        _preview.Visible = false;
        _fallback.Text = text;
        _fallback.Visible = true;
        _fallback.BringToFront();
        UpdateDiagnosticOverlay(text, isError: true);
        SetViewError(text);
    }

    private void ReportError(string message, bool tryWebViewFallback)
    {
        SetViewError(message);
        SetStatus("Ошибка — см. LastError");

        if (tryWebViewFallback && _useNativeCapture)
        {
            _useNativeCapture = false;
            if (_cameraView is not null)
            {
                _reentryGuard = true;
                try { _cameraView.UseNativeCapture = false; }
                finally { _reentryGuard = false; }
            }

            UpdateDiagnosticOverlay(message + "\r\n\r\nПробуем WebView2…", isError: true);
            StartWebViewCapture();
            return;
        }

        ShowFatal(message);
    }

    private void StopAllCapture()
    {
        StopNativeCapture();
        StopWebViewStream();
        _ = _signalR.DisconnectAsync();
    }

    private void OnSignalRStatus(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || _cameraView is null)
            return;
        _cameraView.RunOnUiThread(() =>
        {
            var baseStatus = _cameraView.StatusMessage ?? "";
            if (!baseStatus.Contains("SignalR:", StringComparison.Ordinal))
                SetStatus(baseStatus + "\r\n" + message);
            else
                SetStatus(message);
        });
    }

    private void OnSignalRError(string message)
    {
        if (_cameraView is null)
            return;
        _cameraView.RunOnUiThread(() =>
        {
            _cameraView.SignalRLastError = message;
            UpdateDiagnosticOverlay(message, isError: true);
        });
    }

    private void OnSignalRConnectionState(string state)
    {
        if (_cameraView is null)
            return;
        _cameraView.RunOnUiThread(() =>
        {
            if (_reentryGuard)
                return;
            _reentryGuard = true;
            try { _cameraView.SignalRConnectionState = state; }
            finally { _reentryGuard = false; }
        });
    }

    private void SetViewError(string message)
    {
        if (_cameraView is null)
            return;
        _cameraView.LastError = message;
        _cameraView.StatusMessage = "Ошибка";
        UpdateDiagnosticOverlay(message, isError: true);
    }

    private void ClearError()
    {
        if (_cameraView is not null)
            _cameraView.LastError = "";
    }

    private void SetStatus(string message)
    {
        if (_cameraView is not null)
            _cameraView.StatusMessage = message;
        UpdateDiagnosticOverlay(message, isError: false);
    }

    private void SetCaptureMode(string mode)
    {
        if (_cameraView is not null)
            _cameraView.CaptureMode = mode;
    }

    private void UpdateDiagnosticOverlay(string text, bool isError)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _diagnostic.Visible = false;
            return;
        }

        _diagnostic.Text = text;
        _diagnostic.ForeColor = isError ? Color.FromArgb(255, 160, 120) : Color.FromArgb(210, 210, 210);
        _diagnostic.Visible = true;
        _diagnostic.BringToFront();
    }

    private void ApplyWebViewHtml()
    {
        if (!_webViewReady || _webView.CoreWebView2 is null || _cameraView is null)
            return;

        try
        {
            _webView.NavigateToString(BuildCameraHtml(_cameraView.DeviceId, _cameraView.MirrorPreview));
            SetStatus("WebView2: запрос getUserMedia…");
        }
        catch (Exception ex)
        {
            ReportError("WebView2 NavigateToString:\r\n" + ex.Message, tryWebViewFallback: false);
        }
    }

    private static string BuildCameraHtml(string deviceId, bool mirror)
    {
        var mirrorCss = mirror
            ? "width:100%;height:100%;object-fit:contain;background:#000;transform:scaleX(-1)"
            : "width:100%;height:100%;object-fit:contain;background:#000";

        var preferredId = string.IsNullOrWhiteSpace(deviceId)
            ? "null"
            : JsonSerializer.Serialize(deviceId);

        return "<!DOCTYPE html><html><head><meta charset=\"utf-8\"/></head>" +
               "<body style=\"margin:0;overflow:hidden;background:#111\">" +
               "<video id=\"cam\" autoplay playsinline muted style=\"" + mirrorCss + "\"></video>" +
               "<div id=\"err\" style=\"display:none;padding:12px;color:#e88;font:13px sans-serif;white-space:pre-wrap\"></div>" +
               "<script>(async function(){" +
               "var v=document.getElementById('cam');var err=document.getElementById('err');var preferred=" + preferredId + ";" +
               "function post(t,p){if(window.chrome&&chrome.webview&&chrome.webview.postMessage)chrome.webview.postMessage(JSON.stringify(Object.assign({type:t},p||{})));}" +
               "function showError(x){var m=(x&&x.message)||String(x);var n=(x&&x.name)||'';err.style.display='block';err.textContent=m;v.style.display='none';post('error',{message:m,name:n});}" +
               "function pickDeviceId(cameras){" +
               "if(preferred&&cameras.some(function(c){return c.deviceId===preferred;}))return preferred;" +
               "var def=cameras.find(function(c){return /default/i.test(c.label||'');});" +
               "if(def)return def.deviceId;return cameras.length?cameras[0].deviceId:'';}" +
               "try{" +
               "post('status',{message:'Запрос доступа к камере (getUserMedia)…'});" +
               "if(!navigator.mediaDevices||!navigator.mediaDevices.getUserMedia)throw new Error('getUserMedia недоступен в WebView2');" +
               "var stream=await navigator.mediaDevices.getUserMedia({video:true,audio:false});" +
               "v.srcObject=stream;await v.play();" +
               "post('status',{message:'Поток получен, перечисление устройств…'});" +
               "var devices=await navigator.mediaDevices.enumerateDevices();" +
               "var cameras=devices.filter(function(d){return d.kind==='videoinput';});" +
               "if(!cameras.length)throw new Error('Список videoinput пуст. Разрешите камеру в Windows.');" +
               "var id=pickDeviceId(cameras);" +
               "if(id&&(!preferred||id!==preferred)){" +
               "stream.getTracks().forEach(function(t){t.stop();});" +
               "stream=await navigator.mediaDevices.getUserMedia({video:{deviceId:{exact:id}},audio:false});" +
               "v.srcObject=stream;await v.play();}" +
               "var label=(cameras.find(function(c){return c.deviceId===id;})||{}).label||'(без имени)';" +
               "post('device',{deviceId:id||'',label:label,cameraCount:cameras.length});" +
               "}catch(x){var c=0;try{var d=await navigator.mediaDevices.enumerateDevices();c=d.filter(function(x){return x.kind==='videoinput';}).length;}catch(e){}" +
               "post('error',{message:(x&&x.message)||String(x),name:(x&&x.name)||'',cameraCount:c});showError(x);}" +
               "})();</script></body></html>";
    }

    private void StopWebViewStream()
    {
        if (!_webViewReady || _webView.CoreWebView2 is null)
            return;
        try
        {
            _ = _webView.ExecuteScriptAsync(
                "(function(){var v=document.getElementById('cam');if(v&&v.srcObject){v.srcObject.getTracks().forEach(function(t){t.stop();});v.srcObject=null;}})();");
        }
        catch
        {
            /* ignore */
        }
    }

    private void OnCameraControlHandleDestroyed(object? sender, EventArgs e)
    {
        StopAllCapture();
        _ = _signalR.DisposeAsync();
        try
        {
            if (_webViewReady && _webView.CoreWebView2 is not null)
            {
                _webView.CoreWebView2.PermissionRequested -= OnPermissionRequested;
                _webView.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
                StopWebViewStream();
            }
        }
        catch
        {
            /* при закрытии */
        }
    }
}
