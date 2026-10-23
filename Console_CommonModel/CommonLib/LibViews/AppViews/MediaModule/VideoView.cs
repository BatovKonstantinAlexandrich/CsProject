using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;

/// <summary>
/// Представление встроенного воспроизведения видео по пути к файлу или URL.
/// </summary>
[ViewUsageExample("var view = new VideoView { Text = \"Видео\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<video-view Text=\"Видео\" Dock=\"Top\" />")]
[Label("Видео")]
public class VideoView : AppViewBase
{

    [Label("Надпись")]
    public virtual string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }


    [InputFilePath]
    [Label("Путь к файлу или URL")]
    public string Source { get => Get<string>(nameof(Source)); set => Set(nameof(Source), value); }


    [InputBool]
    [Label("Автовоспроизведение при смене источника")]
    public bool AutoPlay { get => Get<bool>(nameof(AutoPlay)); set => Set(nameof(AutoPlay), value); }

    public VideoView()
    {
        Text = "";
        AutoPlay = true;
        Source = "";
        MinHeight = 240;
    }
}
