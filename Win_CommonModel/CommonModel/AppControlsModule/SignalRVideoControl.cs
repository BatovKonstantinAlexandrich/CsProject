using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.AspNetCore.SignalR.Client;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.MediaModule;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Воспроизведение видеопотока из SignalR: подписка на hub-метод с кадрами и отображение в <see cref="PictureBox"/>.
/// </summary>
public class SignalRVideoControl : ControlBase
{
    private readonly PictureBox _preview = new()
    {
        Dock = DockStyle.Fill,
        BackColor = Color.Black,
        SizeMode = PictureBoxSizeMode.Zoom,
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
        Height = 52,
        Padding = new Padding(8, 6, 8, 6),
        TextAlign = ContentAlignment.TopLeft,
        ForeColor = Color.FromArgb(210, 210, 210),
        BackColor = Color.FromArgb(200, 0, 0, 0),
        Font = new Font("Segoe UI", 8.5f),
        Visible = false,
    };

    private HubConnection _connection;
    private bool _ownsConnection = true;
    private bool _handlersRegistered;
    private bool _reentryGuard;
    private SignalRVideoView _videoView;
    private long _framesReceived;

    public SignalRVideoControl()
    {
        Dock = DockStyle.Fill;
        MinimumSize = new Size(320, 240);
        Controls.Add(_preview);
        Controls.Add(_fallback);
        Controls.Add(_diagnostic);
        Load += OnSignalRVideoControlLoad;
        VisibleChanged += (_, _) =>
        {
            if (Visible && IsHandleCreated)
                BeginInvoke(ApplyFromView);
        };
        HandleDestroyed += OnSignalRVideoControlHandleDestroyed;
    }

    /// <summary>Подключить уже существующее соединение (например из <c>ActionsConsumer</c>).</summary>
    public void AttachHubConnection(HubConnection connection)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        EnsureVideoView();
        _ = DisconnectAsync().ContinueWith(_ =>
        {
            RunOnUiThreadSafe(() =>
            {
                _connection = connection;
                _ownsConnection = false;
                RegisterFrameHandlers();
                ApplyFromView();
            });
        });
    }

    private void OnSignalRVideoControlLoad(object? sender, EventArgs e)
    {
        EnsureVideoView();
        ApplyFromView();
    }

    private void EnsureVideoView()
    {
        if (_videoView is not null)
            return;

        if (viewModel is SignalRVideoView vv)
            BindVideoView(vv);
        else
        {
            var view = new SignalRVideoView();
            view.control = this;
            viewModel = view;
            BindVideoView(view);
        }
    }

    private void BindVideoView(SignalRVideoView vv)
    {
        if (_videoView == vv)
            return;

        _videoView = vv;
        vv.AddPropertyChangeListener(nameof(SignalRVideoView.HubUrl), _ => ApplyFromView());
        vv.AddPropertyChangeListener(nameof(SignalRVideoView.ReceiveMethod), _ => ApplyFromView());
        vv.AddPropertyChangeListener(nameof(SignalRVideoView.SubscribeMethod), _ => ApplyFromView());
        vv.AddPropertyChangeListener(nameof(SignalRVideoView.UnsubscribeMethod), _ => ApplyFromView());
        vv.AddPropertyChangeListener(nameof(SignalRVideoView.StreamId), _ => ApplyFromView());
        vv.AddPropertyChangeListener(nameof(SignalRVideoView.AccessToken), _ => ApplyFromView());
        vv.AddPropertyChangeListener(nameof(SignalRVideoView.AutoConnect), _ => ApplyFromView());
    }

    private void ApplyFromView()
    {
        EnsureVideoView();
        if (_videoView is null)
        {
            ShowFatal("SignalRVideoView не привязан к контролу.");
            return;
        }

        if (!_videoView.AutoConnect)
        {
            _ = DisconnectAsync();
            ShowFatal("Автоподключение выключено (AutoConnect = false).");
            return;
        }

        if (_ownsConnection || _connection is null)
            _ = ConnectAsync();
        else if (_connection.State == HubConnectionState.Connected)
            _ = SubscribeAsync();
    }

    private async Task ConnectAsync()
    {
        EnsureVideoView();
        var url = _videoView!.HubUrl?.Trim();
        if (string.IsNullOrWhiteSpace(url))
        {
            ShowFatal("Не указан HubUrl.");
            return;
        }

        var receiveMethod = string.IsNullOrWhiteSpace(_videoView.ReceiveMethod)
            ? "VideoFrame"
            : _videoView.ReceiveMethod.Trim();

        await DisconnectAsync();

        SetConnectionState("Connecting");
        SetStatus($"Подключение к {url}…");
        ClearPreview();
        _fallback.Visible = false;
        _preview.Visible = true;

        try
        {
            var token = _videoView.AccessToken?.Trim();
            var builder = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    if (!string.IsNullOrEmpty(token))
                        options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect();

            _connection = builder.Build();
            _ownsConnection = true;

            _connection.Closed += OnConnectionClosed;
            _connection.Reconnecting += OnConnectionReconnecting;
            _connection.Reconnected += OnConnectionReconnected;

            RegisterFrameHandlers(receiveMethod);

            await _connection.StartAsync();

            SetConnectionState("Connected");
            ClearError();
            SetStatus($"Соединение установлено. Ожидание кадров ({receiveMethod})…");

            await SubscribeAsync();
        }
        catch (Exception ex)
        {
            SetConnectionState("Disconnected");
            ReportError(BuildConnectError(ex, url, receiveMethod));
        }
    }

    private void RegisterFrameHandlers(string receiveMethod = null)
    {
        if (_connection is null)
            return;

        receiveMethod ??= string.IsNullOrWhiteSpace(_videoView?.ReceiveMethod)
            ? "VideoFrame"
            : _videoView.ReceiveMethod.Trim();

        if (_handlersRegistered)
        {
            try { _connection.Remove(receiveMethod); } catch { /* ignore */ }
            try { _connection.Remove(receiveMethod + "Bytes"); } catch { /* ignore */ }
        }

        _connection.On<byte[]>(receiveMethod, OnFrameReceived);
        _connection.On<string>(receiveMethod, OnFrameReceived);
        _connection.On<SignalRVideoFrameMessage>(receiveMethod, OnFrameReceived);
        _connection.On<byte[]>(receiveMethod + "Bytes", OnFrameReceived);

        _handlersRegistered = true;
    }

    private async Task SubscribeAsync()
    {
        if (_connection is null || _videoView is null)
            return;
        if (_connection.State != HubConnectionState.Connected)
            return;

        var method = _videoView.SubscribeMethod?.Trim();
        var streamId = _videoView.StreamId?.Trim();
        if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(streamId))
            return;

        try
        {
            await _connection.InvokeAsync(method, streamId);
            SetStatus($"Подписка на поток «{streamId}» ({method}) выполнена. Ожидание кадров…");
        }
        catch (Exception ex)
        {
            ReportError($"Не удалось подписаться на поток «{streamId}»:\r\n{ex.Message}");
        }
    }

    private async Task UnsubscribeAsync()
    {
        if (_connection is null || _videoView is null)
            return;
        if (_connection.State != HubConnectionState.Connected)
            return;

        var method = _videoView.UnsubscribeMethod?.Trim();
        var streamId = _videoView.StreamId?.Trim();
        if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(streamId))
            return;

        try
        {
            await _connection.InvokeAsync(method, streamId);
        }
        catch
        {
            /* при закрытии */
        }
    }

    private void OnFrameReceived(byte[] payload) => ProcessFrame(payload);
    private void OnFrameReceived(string payload) => ProcessFrame(payload);
    private void OnFrameReceived(SignalRVideoFrameMessage payload) => ProcessFrame(payload);

    private void ProcessFrame(object payload)
    {
        if (IsDisposed || _videoView is null)
            return;

        void Apply()
        {
            if (!SignalRVideoFrameDecoder.TryDecode(payload, out var bitmap, out var error))
            {
                if (!string.IsNullOrEmpty(error))
                    SetStatus("Ошибка кадра: " + error);
                return;
            }

            _framesReceived++;
            _reentryGuard = true;
            try
            {
                _videoView.FramesReceived = _framesReceived;
            }
            finally
            {
                _reentryGuard = false;
            }

            var old = _preview.Image;
            _preview.Image = bitmap;
            old?.Dispose();

            _fallback.Visible = false;
            _preview.Visible = true;

            if (_framesReceived == 1)
            {
                ClearError();
                SetStatus($"Первый кадр получен (#{_framesReceived}).");
            }
            else if (_framesReceived % 60 == 0)
            {
                SetStatus($"Поток активен — {_framesReceived} кадров.");
            }
        }

        if (InvokeRequired)
            BeginInvoke(Apply);
        else
            Apply();
    }

    private Task OnConnectionClosed(Exception ex)
    {
        RunOnUiThreadSafe(() =>
        {
            SetConnectionState("Disconnected");
            if (ex is not null)
                ReportError("Соединение закрыто:\r\n" + ex.Message, fatal: false);
            else
                SetStatus("Соединение закрыто.");
        });
        return Task.CompletedTask;
    }

    private Task OnConnectionReconnecting(Exception ex)
    {
        RunOnUiThreadSafe(() =>
        {
            SetConnectionState("Reconnecting");
            SetStatus("Переподключение SignalR…" +
                      (ex is not null ? "\r\n" + ex.Message : ""));
        });
        return Task.CompletedTask;
    }

    private Task OnConnectionReconnected(string connectionId)
    {
        RunOnUiThreadSafe(() =>
        {
            SetConnectionState("Connected");
            SetStatus("Соединение восстановлено. Ожидание кадров…");
            _ = SubscribeAsync();
        });
        return Task.CompletedTask;
    }

    private async Task DisconnectAsync()
    {
        _handlersRegistered = false;
        ClearPreview();

        if (_connection is null)
            return;

        try
        {
            if (_connection.State == HubConnectionState.Connected)
                await UnsubscribeAsync();
        }
        catch
        {
            /* ignore */
        }

        try
        {
            _connection.Closed -= OnConnectionClosed;
            _connection.Reconnecting -= OnConnectionReconnecting;
            _connection.Reconnected -= OnConnectionReconnected;
        }
        catch
        {
            /* ignore */
        }

        if (_ownsConnection)
        {
            try
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
            catch
            {
                /* ignore */
            }
        }

        _connection = null;
        SetConnectionState("Disconnected");
    }

    private static string BuildConnectError(Exception ex, string url, string method)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Не удалось подключиться к SignalR.");
        sb.AppendLine($"URL: {url}");
        sb.AppendLine($"Метод кадра: {method}");
        sb.AppendLine();
        sb.AppendLine(ex.Message);
        sb.AppendLine();
        sb.AppendLine("Проверьте: hub запущен, URL верный, CORS/SSL, токен доступа, имя метода на сервере.");
        return sb.ToString().TrimEnd();
    }

    private void ClearPreview()
    {
        var old = _preview.Image;
        _preview.Image = null;
        old?.Dispose();
    }

    private void ShowFatal(string text)
    {
        _ = DisconnectAsync();
        _preview.Visible = false;
        _fallback.Text = text;
        _fallback.Visible = true;
        _fallback.BringToFront();
        UpdateDiagnostic(text, isError: true);
        SetViewError(text);
    }

    private void ReportError(string message, bool fatal = true)
    {
        SetViewError(message);
        if (fatal)
            ShowFatal(message);
        else
            UpdateDiagnostic(message, isError: true);
    }

    private void SetViewError(string message)
    {
        if (_videoView is null)
            return;
        _videoView.LastError = message;
        _videoView.StatusMessage = "Ошибка";
    }

    private void ClearError()
    {
        if (_videoView is not null)
            _videoView.LastError = "";
    }

    private void SetStatus(string message)
    {
        if (_videoView is not null)
            _videoView.StatusMessage = message;
        UpdateDiagnostic(message, isError: false);
    }

    private void SetConnectionState(string state)
    {
        if (_videoView is null || _reentryGuard)
            return;
        _reentryGuard = true;
        try
        {
            _videoView.ConnectionState = state;
        }
        finally
        {
            _reentryGuard = false;
        }
    }

    private void UpdateDiagnostic(string text, bool isError)
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

    private void OnSignalRVideoControlHandleDestroyed(object? sender, EventArgs e)
    {
        _ = DisconnectAsync();
    }

    private void RunOnUiThreadSafe(Action action)
    {
        try
        {
            if (!IsDisposed && IsHandleCreated)
                BeginInvoke(action);
            else
                action();
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
