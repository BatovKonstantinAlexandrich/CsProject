using System.Text;
using QRCoder;

namespace BlazorErp.Services;

public static class AuthenticatorQrHelper
{
    public static string GenerateOtpAuthUri(string email, string secretKey, string issuer = "BlazorErp")
    {
        return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" +
               $"?secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}&digits=6";
    }

    public static string FormatSecretKey(string secretKey)
    {
        var key = secretKey.Replace(" ", "");
        var result = new StringBuilder();
        for (var i = 0; i < key.Length; i += 4)
        {
            if (i > 0)
                result.Append(' ');
            result.Append(key.AsSpan(i, Math.Min(4, key.Length - i)));
        }
        return result.ToString();
    }

    public static string GenerateQrCodeDataUri(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data);
        var bytes = png.GetGraphic(pixelsPerModule: 8);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
}
