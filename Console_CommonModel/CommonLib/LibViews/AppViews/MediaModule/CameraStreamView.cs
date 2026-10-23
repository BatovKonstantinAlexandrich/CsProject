using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Console_CommonModel.CommonLib.LibViews.Others;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule
{
    /// <summary>
    /// Контейнер передачи видео с камеры на URL (SignalR hub).
    /// Разместите внутри <see cref="CameraView"/> — кадры отображаются локально
    /// и одновременно отправляются на <see cref="StreamUrl"/>.
    /// </summary>
    [Label("Трансляция камеры")]
    [Description(
        "Контейнер с LayoutViewService: при помещении CameraView " +
        "видео показывается и передаётся на URL хаба SignalR.")]
    [Icon("videocam")]
    [ViewUsageExample(
        "var stream = new CameraStreamView { StreamUrl = \"https://host/hubs/VideoHub\" };\n" +
        "stream.Add(new CameraView { Text = \"Камера\" });",
        "<camera-stream-view StreamUrl=\"https://host/hubs/VideoHub\" />")]
    public class CameraStreamView : AppLayoutView
    {
        private readonly CameraStreamLayoutService _layoutService = new();

        [Label("URL трансляции (SignalR hub)")]
        public string StreamUrl
        {
            get => Get<string>(nameof(StreamUrl));
            set => Set(nameof(StreamUrl), value);
        }

        [Label("Идентификатор потока")]
        public string StreamId
        {
            get => Get<string>(nameof(StreamId));
            set => Set(nameof(StreamId), value);
        }

        [Label("Токен доступа (Bearer)")]
        public string AccessToken
        {
            get => Get<string>(nameof(AccessToken));
            set => Set(nameof(AccessToken), value);
        }

        [Label("Передавать видео на URL")]
        public bool StreamingEnabled
        {
            get => Get<bool>(nameof(StreamingEnabled));
            set => Set(nameof(StreamingEnabled), value);
        }

        [Label("Метод отправки кадра")]
        public string SendMethod
        {
            get => Get<string>(nameof(SendMethod));
            set => Set(nameof(SendMethod), value);
        }

        [Label("Метод начала публикации")]
        public string StartMethod
        {
            get => Get<string>(nameof(StartMethod));
            set => Set(nameof(StartMethod), value);
        }

        [Label("Метод остановки публикации")]
        public string StopMethod
        {
            get => Get<string>(nameof(StopMethod));
            set => Set(nameof(StopMethod), value);
        }

        [Label("Качество JPEG (10–100)")]
        public int JpegQuality
        {
            get => Get<int>(nameof(JpegQuality));
            set => Set(nameof(JpegQuality), value);
        }

        [Label("Интервал отправки кадров (мс)")]
        public int SendIntervalMs
        {
            get => Get<int>(nameof(SendIntervalMs));
            set => Set(nameof(SendIntervalMs), value);
        }

        public CameraStreamView()
        {
            StreamUrl = "https://localhost:8000/hubs/VideoHub";
            StreamId = "";
            AccessToken = "";
            StreamingEnabled = true;
            SendMethod = "SendVideoFrame";
            StartMethod = "StartPublishing";
            StopMethod = "StopPublishing";
            JpegQuality = 75;
            SendIntervalMs = 100;

            IsVertical = true;
            FillSize = true;
            WrapContent = false;
            MinHeight = 320;
            MinWidth = 320;
            Width = MinWidth;
            Height = MinHeight;
            BorderWidth = 1;
            BackgroundColor = "#F3F6FA";

            _layoutService.SetControl(this);
        }
    }
}
