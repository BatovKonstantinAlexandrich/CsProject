namespace WinFormsApp.AppViewModules.MediaModule;

/// <summary>
/// Сообщение кадра от hub SignalR (JSON).
/// Поддерживаются поля <see cref="Data"/> (байты) и/или <see cref="Base64"/> (строка).
/// </summary>
public sealed class SignalRVideoFrameMessage
{
    public byte[] Data { get; set; }
    public string Base64 { get; set; }
    public string Format { get; set; } = "jpeg";
    public int Width { get; set; }
    public int Height { get; set; }
    public string StreamId { get; set; }
}
