using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;

/// <summary>
/// Предпросмотр видеопотока с камеры устройства.
/// По умолчанию используется нативный захват (первая доступная камера / <see cref="DeviceIndex"/>).
/// </summary>
[ViewUsageExample("var view = new CameraView { Text = \"Камера\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<camera-view Text=\"Камера\" Dock=\"Top\" />")]
[Label("Камера")]
public class CameraView : AppViewBase
{

    [Label("Надпись")]
    public virtual string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }


    /// <summary>Индекс камеры DirectShow (0 — обычно устройство по умолчанию). -1 — авто.</summary>
    [Label("Индекс камеры")]
    public int DeviceIndex { get => Get<int>(nameof(DeviceIndex)); set => Set(nameof(DeviceIndex), value); }


    /// <summary>Идентификатор для WebView2 (<c>deviceId</c>). Используется при <see cref="UseNativeCapture"/> = false.</summary>
    [Label("Идентификатор камеры (deviceId)")]
    public string DeviceId { get => Get<string>(nameof(DeviceId)); set => Set(nameof(DeviceId), value); }


    [NotInput]
    [Label("Имя устройства")]
    public string DeviceLabel { get => Get<string>(nameof(DeviceLabel)); set => Set(nameof(DeviceLabel), value); }

    [NotInput]
    [Label("Статус")]
    public string StatusMessage { get => Get<string>(nameof(StatusMessage)); set => Set(nameof(StatusMessage), value); }

    [NotInput]
    [Label("Последняя ошибка")]
    public string LastError { get => Get<string>(nameof(LastError)); set => Set(nameof(LastError), value); }

    [NotInput]
    [Label("Режим захвата")]
    public string CaptureMode { get => Get<string>(nameof(CaptureMode)); set => Set(nameof(CaptureMode), value); }
    [Label("Нативный захват (OpenCV, рекомендуется)")]
    public bool UseNativeCapture { get => Get<bool>(nameof(UseNativeCapture)); set => Set(nameof(UseNativeCapture), value); }
    [Label("Запускать предпросмотр при загрузке")]
    public bool AutoStart { get => Get<bool>(nameof(AutoStart)); set => Set(nameof(AutoStart), value); }
    [Label("Зеркально (удобно для фронтальной камеры)")]
    public bool MirrorPreview { get => Get<bool>(nameof(MirrorPreview)); set => Set(nameof(MirrorPreview), value); }

    // --- SignalR: передача видео с камеры на сервер ---
    [Label("Передавать видео в SignalR")]
    public bool SignalREnabled { get => Get<bool>(nameof(SignalREnabled)); set => Set(nameof(SignalREnabled), value); }
    [Label("URL хаба SignalR")]
    public string SignalRHubUrl { get => Get<string>(nameof(SignalRHubUrl)); set => Set(nameof(SignalRHubUrl), value); }

    /// <summary>Метод клиент → hub для отправки кадра (например <c>SendVideoFrame</c>).</summary>
    [Label("Метод отправки кадра")]
    public string SignalRSendMethod { get => Get<string>(nameof(SignalRSendMethod)); set => Set(nameof(SignalRSendMethod), value); }
    [Label("Метод начала публикации")]
    public string SignalRStartMethod { get => Get<string>(nameof(SignalRStartMethod)); set => Set(nameof(SignalRStartMethod), value); }
    [Label("Метод остановки публикации")]
    public string SignalRStopMethod { get => Get<string>(nameof(SignalRStopMethod)); set => Set(nameof(SignalRStopMethod), value); }
    [Label("Идентификатор потока")]
    public string SignalRStreamId { get => Get<string>(nameof(SignalRStreamId)); set => Set(nameof(SignalRStreamId), value); }
    [Label("Токен доступа (Bearer)")]
    public string SignalRAccessToken { get => Get<string>(nameof(SignalRAccessToken)); set => Set(nameof(SignalRAccessToken), value); }
    [Label("Подключать SignalR при загрузке")]
    public bool SignalRAutoConnect { get => Get<bool>(nameof(SignalRAutoConnect)); set => Set(nameof(SignalRAutoConnect), value); }
    [Label("Отправлять кадр как DTO (JSON)")]
    public bool SignalRSendAsDto { get => Get<bool>(nameof(SignalRSendAsDto)); set => Set(nameof(SignalRSendAsDto), value); }
    [Label("Качество JPEG (10–100)")]
    public int SignalRJpegQuality { get => Get<int>(nameof(SignalRJpegQuality)); set => Set(nameof(SignalRJpegQuality), value); }
    [Label("Интервал отправки кадров (мс)")]
    public int SignalRSendIntervalMs { get => Get<int>(nameof(SignalRSendIntervalMs)); set => Set(nameof(SignalRSendIntervalMs), value); }

    [NotInput]
    [Label("SignalR: состояние")]
    public string SignalRConnectionState { get => Get<string>(nameof(SignalRConnectionState)); set => Set(nameof(SignalRConnectionState), value); }

    [NotInput]
    [Label("SignalR: отправлено кадров")]
    public long SignalRFramesSent { get => Get<long>(nameof(SignalRFramesSent)); set => Set(nameof(SignalRFramesSent), value); }

    [NotInput]
    [Label("SignalR: последняя ошибка")]
    public string SignalRLastError { get => Get<string>(nameof(SignalRLastError)); set => Set(nameof(SignalRLastError), value); }

    public CameraView()
    {
        Text = "";
        DeviceId = "";
        DeviceLabel = "";
        StatusMessage = "";
        LastError = "";
        CaptureMode = "";
        DeviceIndex = -1;
        UseNativeCapture = true;
        AutoStart = true;
        MirrorPreview = true;
        SignalREnabled = false;
        SignalRHubUrl = "https://localhost:8000/hubs/VideoHub";
        SignalRSendMethod = "SendVideoFrame";
        SignalRStartMethod = "StartPublishing";
        SignalRStopMethod = "StopPublishing";
        SignalRStreamId = "";
        SignalRAccessToken = "";
        SignalRAutoConnect = true;
        SignalRSendAsDto = false;
        SignalRJpegQuality = 75;
        SignalRSendIntervalMs = 100;
        SignalRConnectionState = "Disconnected";
        SignalRLastError = "";
        MinHeight = 240;
    }
}
