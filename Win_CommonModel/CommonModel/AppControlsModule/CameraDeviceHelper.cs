using System.Collections.Generic;
using System.Text;

using OpenCvSharp;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Поиск и открытие камер (DirectShow / Media Foundation / авто).
/// </summary>
public static class CameraDeviceHelper
{
    public sealed class CameraDeviceInfo
    {
        public int Index { get; init; }
        public string Name { get; init; } = "";
        public string Backend { get; init; } = "";
    }

    static readonly VideoCaptureAPIs[] PreferredApis =
    [
        VideoCaptureAPIs.DSHOW,
        VideoCaptureAPIs.MSMF,
        VideoCaptureAPIs.ANY,
    ];

    /// <summary>Первая камера, с которой удаётся прочитать кадр.</summary>
    public static int ResolveDefaultDeviceIndex(int maxProbe = 10)
    {
        for (var i = 0; i < maxProbe; i++)
        {
            if (TryOpen(i, out var capture, out _))
            {
                capture.Dispose();
                return i;
            }
        }
        return 0;
    }

    public static IReadOnlyList<CameraDeviceInfo> EnumerateDevices(int maxProbe = 10)
    {
        var list = new List<CameraDeviceInfo>();
        for (var i = 0; i < maxProbe; i++)
        {
            if (!TryOpen(i, out var capture, out var backend))
                continue;

            capture.Dispose();
            list.Add(new CameraDeviceInfo
            {
                Index = i,
                Backend = backend,
                Name = $"Камера {i} ({backend})" + (i == 0 ? " — по умолчанию" : "")
            });
        }
        return list;
    }

    /// <summary>
    /// Открывает камеру, перебирая бэкенды; после открытия сбрасывает буфер и проверяет, что кадр не пустой.
    /// </summary>
    public static VideoCapture OpenCamera(int index, out string backendUsed, out string openLog)
    {
        var log = new StringBuilder();
        log.AppendLine($"Поиск рабочего бэкенда для камеры с индексом {index}:");

        foreach (var api in PreferredApis)
        {
            log.AppendLine();
            log.Append($"  {api}: ");

            if (!TryOpen(index, out var capture, out var backend, api))
            {
                log.AppendLine("не удалось получить кадр.");
                continue;
            }

            backendUsed = backend;
            openLog = log.AppendLine("OK.").ToString();
            return capture;
        }

        backendUsed = "";
        openLog = log.AppendLine().AppendLine("Все бэкенды не дали изображение.").ToString();
        throw new InvalidOperationException(
            $"Не удалось открыть камеру с индексом {index}. Подробности:\r\n{openLog}");
    }

    public static bool TryOpen(
        int index,
        out VideoCapture capture,
        out string backendUsed,
        VideoCaptureAPIs? onlyApi = null)
    {
        capture = null!;
        backendUsed = "";

        var apis = onlyApi.HasValue ? new[] { onlyApi.Value } : PreferredApis;
        foreach (var api in apis)
        {
            VideoCapture? cap = null;
            try
            {
                cap = new VideoCapture(index, api);
                if (!cap.IsOpened())
                {
                    cap.Dispose();
                    continue;
                }

                TrySetDefaultResolution(cap);

                if (!TryReadValidFrame(cap, warmupFrames: 12))
                {
                    cap.Dispose();
                    continue;
                }

                capture = cap;
                backendUsed = api.ToString();
                return true;
            }
            catch
            {
                cap?.Dispose();
            }
        }

        return false;
    }

    static void TrySetDefaultResolution(VideoCapture capture)
    {
        try
        {
            capture.Set(VideoCaptureProperties.FrameWidth, 640);
            capture.Set(VideoCaptureProperties.FrameHeight, 480);
            capture.Set(VideoCaptureProperties.Fps, 30);
        }
        catch
        {
            /* драйвер может не поддерживать */
        }
    }

    /// <summary>Сброс буфера и проверка, что приходит непустой кадр.</summary>
    static bool TryReadValidFrame(VideoCapture capture, int warmupFrames = 8)
    {
        using var frame = new Mat();
        for (var i = 0; i < warmupFrames; i++)
            capture.Read(frame);

        if (!capture.Read(frame) || frame.Empty())
            return false;

        return frame.Width > 0 && frame.Height > 0;
    }
}
