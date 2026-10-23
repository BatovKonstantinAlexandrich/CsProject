using System;
using System.Drawing;
using System.IO;
using System.Text;

using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>Декодирование кадров из hub (байты, Base64, DTO).</summary>
public static class SignalRVideoFrameDecoder
{
    public static bool TryDecode(object payload, out Bitmap bitmap, out string error)
    {
        bitmap = null!;
        error = "";

        try
        {
            byte[] bytes = payload switch
            {
                null => null,
                byte[] raw => raw,
                string s => DecodeBase64(s),
                SignalRVideoFrameMessage msg => ExtractBytes(msg),
                _ => null
            };

            if (bytes is null || bytes.Length == 0)
            {
                error = $"Неподдерживаемый формат кадра: {payload?.GetType().Name ?? "null"}";
                return false;
            }

            using var mat = Cv2.ImDecode(bytes, ImreadModes.Color);
            if (mat.Empty())
            {
                using var ms = new MemoryStream(bytes);
                bitmap = new Bitmap(System.Drawing.Image.FromStream(ms));
            }
            else
            {
                bitmap = BitmapConverter.ToBitmap(mat);
            }

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            bitmap = null!;
            return false;
        }
    }

    static byte[] ExtractBytes(SignalRVideoFrameMessage msg)
    {
        if (msg.Data is { Length: > 0 })
            return msg.Data;
        if (!string.IsNullOrWhiteSpace(msg.Base64))
            return DecodeBase64(msg.Base64);
        return null;
    }

    static byte[] DecodeBase64(string value)
    {
        var s = value.Trim();
        var comma = s.IndexOf(',');
        if (comma >= 0 && s[..comma].Contains("base64", StringComparison.OrdinalIgnoreCase))
            s = s[(comma + 1)..];
        return Convert.FromBase64String(s);
    }
}
