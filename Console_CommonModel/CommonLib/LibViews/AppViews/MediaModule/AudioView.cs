using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;

/// <summary>
/// Представление воспроизведения аудио по пути к файлу или URL.
/// </summary>
[ViewUsageExample("var view = new AudioView { Text = \"Аудио\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<audio-view Text=\"Аудио\" Dock=\"Top\" />")]
[Label("Аудио")]
public class AudioView : AppViewBase
{

    [Label("Надпись")]
    public virtual string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }

    [Label("Путь к файлу или URL")]
    public string Source { get => Get<string>(nameof(Source)); set => Set(nameof(Source), value); }

    [InputBool]
    [Label("Автовоспроизведение при смене источника")]
    public bool AutoPlay { get => Get<bool>(nameof(AutoPlay)); set => Set(nameof(AutoPlay), value); }

    public AudioView()
    {
        Text = "";
        AutoPlay = false;
        Source = "";
        MinHeight = 140;
        Height = 140;
        MinWidth = 480;
        Width = 480;
    }
}
