using Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;
using Console_CommonModel.CommonLib.LibViews.LayoutVIews;


namespace Console_CommonModel.CommonLib.LibViews.Others
{
    /// <summary>
    /// При размещении <see cref="CameraView"/> внутри <see cref="CameraStreamView"/>
    /// включает предпросмотр и передачу кадров на URL (SignalR hub).
    /// </summary>
    public class CameraStreamLayoutService : LayoutViewService<CameraStreamView, CameraView>
    {
        public override Action Activate(CameraView camera)
        {
            var snapshot = Snapshot(camera);
            ApplyStreamSettings(camera);

            var unbindUrl = Control.AddPropertyChangeListener(nameof(CameraStreamView.StreamUrl), _ =>
                camera.SignalRHubUrl = Control.StreamUrl ?? "");
            var unbindStreamId = Control.AddPropertyChangeListener(nameof(CameraStreamView.StreamId), _ =>
                camera.SignalRStreamId = Control.StreamId ?? "");
            var unbindToken = Control.AddPropertyChangeListener(nameof(CameraStreamView.AccessToken), _ =>
                camera.SignalRAccessToken = Control.AccessToken ?? "");
            var unbindEnabled = Control.AddPropertyChangeListener(nameof(CameraStreamView.StreamingEnabled), _ =>
            {
                camera.SignalREnabled = Control.StreamingEnabled;
                if (Control.StreamingEnabled)
                    camera.UseNativeCapture = true;
            });
            var unbindQuality = Control.AddPropertyChangeListener(nameof(CameraStreamView.JpegQuality), _ =>
                camera.SignalRJpegQuality = Control.JpegQuality);
            var unbindInterval = Control.AddPropertyChangeListener(nameof(CameraStreamView.SendIntervalMs), _ =>
                camera.SignalRSendIntervalMs = Control.SendIntervalMs);
            var unbindSendMethod = Control.AddPropertyChangeListener(nameof(CameraStreamView.SendMethod), _ =>
                camera.SignalRSendMethod = Control.SendMethod ?? "SendVideoFrame");
            var unbindStartMethod = Control.AddPropertyChangeListener(nameof(CameraStreamView.StartMethod), _ =>
                camera.SignalRStartMethod = Control.StartMethod ?? "StartPublishing");
            var unbindStopMethod = Control.AddPropertyChangeListener(nameof(CameraStreamView.StopMethod), _ =>
                camera.SignalRStopMethod = Control.StopMethod ?? "StopPublishing");

            return () =>
            {
                unbindUrl();
                unbindStreamId();
                unbindToken();
                unbindEnabled();
                unbindQuality();
                unbindInterval();
                unbindSendMethod();
                unbindStartMethod();
                unbindStopMethod();
                Restore(camera, snapshot);
            };
        }

        private void ApplyStreamSettings(CameraView camera)
        {
            camera.UseNativeCapture = true;
            camera.SignalRHubUrl = Control.StreamUrl ?? "";
            camera.SignalRStreamId = Control.StreamId ?? "";
            camera.SignalRAccessToken = Control.AccessToken ?? "";
            camera.SignalRSendMethod = string.IsNullOrWhiteSpace(Control.SendMethod)
                ? "SendVideoFrame"
                : Control.SendMethod;
            camera.SignalRStartMethod = string.IsNullOrWhiteSpace(Control.StartMethod)
                ? "StartPublishing"
                : Control.StartMethod;
            camera.SignalRStopMethod = string.IsNullOrWhiteSpace(Control.StopMethod)
                ? "StopPublishing"
                : Control.StopMethod;
            camera.SignalRJpegQuality = Control.JpegQuality;
            camera.SignalRSendIntervalMs = Control.SendIntervalMs;
            camera.SignalRAutoConnect = true;
            camera.SignalREnabled = Control.StreamingEnabled;
        }

        private static CameraStreamSnapshot Snapshot(CameraView camera) => new()
        {
            SignalREnabled = camera.SignalREnabled,
            UseNativeCapture = camera.UseNativeCapture,
            SignalRHubUrl = camera.SignalRHubUrl,
            SignalRStreamId = camera.SignalRStreamId,
            SignalRAccessToken = camera.SignalRAccessToken,
            SignalRSendMethod = camera.SignalRSendMethod,
            SignalRStartMethod = camera.SignalRStartMethod,
            SignalRStopMethod = camera.SignalRStopMethod,
            SignalRJpegQuality = camera.SignalRJpegQuality,
            SignalRSendIntervalMs = camera.SignalRSendIntervalMs,
            SignalRAutoConnect = camera.SignalRAutoConnect,
        };

        private static void Restore(CameraView camera, CameraStreamSnapshot s)
        {
            camera.SignalREnabled = s.SignalREnabled;
            camera.UseNativeCapture = s.UseNativeCapture;
            camera.SignalRHubUrl = s.SignalRHubUrl;
            camera.SignalRStreamId = s.SignalRStreamId;
            camera.SignalRAccessToken = s.SignalRAccessToken;
            camera.SignalRSendMethod = s.SignalRSendMethod;
            camera.SignalRStartMethod = s.SignalRStartMethod;
            camera.SignalRStopMethod = s.SignalRStopMethod;
            camera.SignalRJpegQuality = s.SignalRJpegQuality;
            camera.SignalRSendIntervalMs = s.SignalRSendIntervalMs;
            camera.SignalRAutoConnect = s.SignalRAutoConnect;
        }

        private sealed class CameraStreamSnapshot
        {
            public bool SignalREnabled { get; init; }
            public bool UseNativeCapture { get; init; }
            public string SignalRHubUrl { get; init; }
            public string SignalRStreamId { get; init; }
            public string SignalRAccessToken { get; init; }
            public string SignalRSendMethod { get; init; }
            public string SignalRStartMethod { get; init; }
            public string SignalRStopMethod { get; init; }
            public int SignalRJpegQuality { get; init; }
            public int SignalRSendIntervalMs { get; init; }
            public bool SignalRAutoConnect { get; init; }
        }
    }
}
