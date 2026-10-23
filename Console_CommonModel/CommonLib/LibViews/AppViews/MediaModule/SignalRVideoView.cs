using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;

/// <summary>
/// Воспроизведение видеопотока, получаемого по SignalR (кадры JPEG/PNG в hub-сообщениях).
/// </summary>
[ViewUsageExample("var view = new SignalRVideoView { Text = \"Видео SignalR\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<signal-r-video-view Text=\"Видео SignalR\" Dock=\"Top\" />")]
[Label("Видео SignalR")]
public class SignalRVideoView : AppViewBase
{

    [Label("Надпись")]
    public virtual string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }


    [Label("URL хаба SignalR")]
    public string HubUrl { get => Get<string>(nameof(HubUrl)); set => Set(nameof(HubUrl), value); }

    /// <summary>Имя метода hub → клиент с кадром (например <c>VideoFrame</c>).</summary>

    [Label("Метод приёма кадра")]
    public string ReceiveMethod { get => Get<string>(nameof(ReceiveMethod)); set => Set(nameof(ReceiveMethod), value); }

    /// <summary>Метод клиент → hub для подписки на поток (необязательно).</summary>

    [Label("Метод подписки")]
    public string SubscribeMethod { get => Get<string>(nameof(SubscribeMethod)); set => Set(nameof(SubscribeMethod), value); }

    /// <summary>Метод клиент → hub для отписки (необязательно).</summary>

    [Label("Метод отписки")]
    public string UnsubscribeMethod { get => Get<string>(nameof(UnsubscribeMethod)); set => Set(nameof(UnsubscribeMethod), value); }


    [Label("Идентификатор потока / группы")]
    public string StreamId { get => Get<string>(nameof(StreamId)); set => Set(nameof(StreamId), value); }


    [Label("Токен доступа (Bearer)")]
    public string AccessToken { get => Get<string>(nameof(AccessToken)); set => Set(nameof(AccessToken), value); }


    [Label("Подключаться при загрузке")]
    public bool AutoConnect { get => Get<bool>(nameof(AutoConnect)); set => Set(nameof(AutoConnect), value); }

    [NotInput]

    [Label("Состояние соединения")]
    public string ConnectionState { get => Get<string>(nameof(ConnectionState)); set => Set(nameof(ConnectionState), value); }

    [NotInput]

    [Label("Статус")]
    public string StatusMessage { get => Get<string>(nameof(StatusMessage)); set => Set(nameof(StatusMessage), value); }

    [NotInput]

    [Label("Последняя ошибка")]
    public string LastError { get => Get<string>(nameof(LastError)); set => Set(nameof(LastError), value); }

    [NotInput]

    [Label("Получено кадров")]
    public long FramesReceived { get => Get<long>(nameof(FramesReceived)); set => Set(nameof(FramesReceived), value); }

    public SignalRVideoView()
    {
        Text = "";
        HubUrl = "https://localhost:8000/hubs/VideoHub";
        ReceiveMethod = "VideoFrame";
        SubscribeMethod = "SubscribeVideo";
        UnsubscribeMethod = "UnsubscribeVideo";
        StreamId = "";
        AccessToken = "";
        ConnectionState = "Disconnected";
        StatusMessage = "";
        LastError = "";
        AutoConnect = true;
        MinHeight = 240;
    }
}
