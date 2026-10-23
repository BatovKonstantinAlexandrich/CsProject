using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;

using OpenCvSharp;

using WinFormsApp.AppViews.MediaModule;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Отправка кадров с камеры на hub SignalR (клиент → сервер).
/// </summary>
public sealed class CameraSignalRStreamer : IAsyncDisposable
{
    private HubConnection _connection;
    private bool _ownsConnection;
    private DateTime _lastSendUtc = DateTime.MinValue;
    private int _sendInProgress;
    private long _framesSent;
    private CameraView _view;
    private Action<string> _onStatus;
    private Action<string> _onError;
    private Action<string> _onConnectionState;

    public bool IsConnected =>
        _connection?.State == HubConnectionState.Connected;

    public void Configure(
        CameraView view,
        Action<string> onStatus,
        Action<string> onError,
        Action<string> onConnectionState)
    {
        _view = view;
        _onStatus = onStatus;
        _onError = onError;
        _onConnectionState = onConnectionState;
    }

    public void AttachHubConnection(HubConnection connection)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        _ = DisconnectAsync().ContinueWith(_ =>
        {
            _connection = connection;
            _ownsConnection = false;
            _onConnectionState?.Invoke(connection.State.ToString());
            _onStatus?.Invoke("SignalR: используется внешнее соединение.");
        });
    }

    public async Task ConnectAsync()
    {
        if (_view is null)
            return;

        var url = _view.SignalRHubUrl?.Trim();
        if (string.IsNullOrWhiteSpace(url))
        {
            ReportError("Не указан SignalRHubUrl.");
            return;
        }

        await DisconnectAsync();

        _onConnectionState?.Invoke("Connecting");
        _onStatus?.Invoke($"SignalR: подключение к {url}…");

        try
        {
            var token = _view.SignalRAccessToken?.Trim();
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
            _connection.Reconnecting += OnReconnecting;
            _connection.Reconnected += OnReconnected;

            await _connection.StartAsync();

            _onConnectionState?.Invoke("Connected");
            _onStatus?.Invoke("SignalR: соединение установлено.");

            await InvokeStartPublishingAsync();
        }
        catch (Exception ex)
        {
            _onConnectionState?.Invoke("Disconnected");
            ReportError("SignalR: не удалось подключиться.\r\n" + ex.Message);
        }
    }

    private Task OnReconnecting(Exception ex)
    {
        _onConnectionState?.Invoke("Reconnecting");
        return Task.CompletedTask;
    }

    private Task OnReconnected(string connectionId)
    {
        _onConnectionState?.Invoke("Connected");
        _onStatus?.Invoke("SignalR: соединение восстановлено.");
        return InvokeStartPublishingAsync();
    }

    private Task OnConnectionClosed(Exception ex)
    {
        _onConnectionState?.Invoke("Disconnected");
        if (ex is not null)
            ReportError("SignalR: соединение закрыто.\r\n" + ex.Message);
        return Task.CompletedTask;
    }

    private async Task InvokeStartPublishingAsync()
    {
        if (_connection is null || _view is null)
            return;
        if (_connection.State != HubConnectionState.Connected)
            return;

        var method = _view.SignalRStartMethod?.Trim();
        var streamId = _view.SignalRStreamId?.Trim();
        if (string.IsNullOrEmpty(method))
            return;

        try
        {
            if (string.IsNullOrEmpty(streamId))
                await _connection.InvokeAsync(method);
            else
                await _connection.InvokeAsync(method, streamId);
        }
        catch (Exception ex)
        {
            ReportError($"SignalR: {method} не выполнен.\r\n{ex.Message}");
        }
    }

    private async Task InvokeStopPublishingAsync()
    {
        if (_connection is null || _view is null)
            return;
        if (_connection.State != HubConnectionState.Connected)
            return;

        var method = _view.SignalRStopMethod?.Trim();
        if (string.IsNullOrEmpty(method))
            return;

        var streamId = _view.SignalRStreamId?.Trim();
        try
        {
            if (string.IsNullOrEmpty(streamId))
                await _connection.InvokeAsync(method);
            else
                await _connection.InvokeAsync(method, streamId);
        }
        catch
        {
            /* при остановке */
        }
    }

    /// <summary>Кодирует кадр в JPEG и отправляет на hub (с ограничением частоты).</summary>
    public void TrySendFrame(Mat frame)
    {
        if (_view is null || !_view.SignalREnabled || frame.Empty())
            return;
        if (_connection is null || _connection.State != HubConnectionState.Connected)
            return;

        var intervalMs = Math.Max(33, _view.SignalRSendIntervalMs);
        var elapsed = (DateTime.UtcNow - _lastSendUtc).TotalMilliseconds;
        if (elapsed < intervalMs)
            return;

        if (Interlocked.CompareExchange(ref _sendInProgress, 1, 0) != 0)
            return;

        var quality = Math.Clamp(_view.SignalRJpegQuality, 10, 100);
        var streamId = _view.SignalRStreamId?.Trim() ?? "";
        var method = string.IsNullOrWhiteSpace(_view.SignalRSendMethod)
            ? "SendVideoFrame"
            : _view.SignalRSendMethod.Trim();
        var sendAsDto = _view.SignalRSendAsDto;

        Mat clone = null;
        try
        {
            clone = frame.Clone();
            _lastSendUtc = DateTime.UtcNow;

            Task.Run(async () =>
            {
                try
                {
                    if (!Cv2.ImEncode(".jpg", clone, out var jpeg, new[] { (int)ImwriteFlags.JpegQuality, quality }))
                        return;

                    if (sendAsDto)
                    {
                        var msg = new SignalRVideoFrameMessage
                        {
                            Data = jpeg,
                            Format = "jpeg",
                            Width = clone.Width,
                            Height = clone.Height,
                            StreamId = streamId,
                        };
                        await SendPayloadAsync(method, streamId, msg);
                    }
                    else if (string.IsNullOrEmpty(streamId))
                        await _connection.InvokeAsync(method, jpeg);
                    else
                        await _connection.InvokeAsync(method, streamId, jpeg);

                    IncrementFramesSent();
                }
                catch (Exception ex)
                {
                    ReportError("SignalR: ошибка отправки кадра.\r\n" + ex.Message);
                }
                finally
                {
                    clone?.Dispose();
                    Interlocked.Exchange(ref _sendInProgress, 0);
                }
            });
        }
        catch
        {
            clone?.Dispose();
            Interlocked.Exchange(ref _sendInProgress, 0);
        }
    }

    private async Task SendPayloadAsync(string method, string streamId, SignalRVideoFrameMessage msg)
    {
        if (string.IsNullOrEmpty(streamId))
            await _connection.InvokeAsync(method, msg);
        else
            await _connection.InvokeAsync(method, streamId, msg);
    }

    public async Task DisconnectAsync()
    {
        Interlocked.Exchange(ref _sendInProgress, 0);

        if (_connection is null)
            return;

        try
        {
            if (_connection.State == HubConnectionState.Connected)
                await InvokeStopPublishingAsync();
        }
        catch
        {
            /* ignore */
        }

        try
        {
            _connection.Closed -= OnConnectionClosed;
            _connection.Reconnecting -= OnReconnecting;
            _connection.Reconnected -= OnReconnected;
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
        _onConnectionState?.Invoke("Disconnected");
    }

    private void ReportError(string message)
    {
        _onError?.Invoke(message);
    }

    public void IncrementFramesSent()
    {
        var n = Interlocked.Increment(ref _framesSent);
        if (_view is not null)
            _view.SignalRFramesSent = n;
    }

    public async ValueTask DisposeAsync() => await DisconnectAsync();
}
