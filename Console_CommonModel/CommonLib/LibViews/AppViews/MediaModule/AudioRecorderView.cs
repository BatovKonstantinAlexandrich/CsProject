using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;

/// <summary>
/// Представление диктофона: запись с микрофона в WAV-файл.
/// </summary>
[ViewUsageExample(
    "var view = new AudioRecorderView { Text = \"Диктофон\" };\nview.Dock = \"Top\";\nparent.Add(view);",
    "<audio-recorder-view Text=\"Диктофон\" Dock=\"Top\" />")]
[Label("Диктофон")]
public class AudioRecorderView : AppViewBase
{

    [Label("Надпись")]
    public virtual string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }


    [Label("Путь к записанному файлу")]
    public string OutputPath { get => Get<string>(nameof(OutputPath)); set => Set(nameof(OutputPath), value); }

    [InputBool]
    [Label("Воспроизводить после записи")]
    public bool AutoPlayAfterRecord { get => Get<bool>(nameof(AutoPlayAfterRecord)); set => Set(nameof(AutoPlayAfterRecord), value); }

    [NotInput]
    [Label("Идёт запись")]
    public bool IsRecording { get => Get<bool>(nameof(IsRecording)); set => Set(nameof(IsRecording), value); }

    [NotInput]
    [Label("Длительность записи (с)")]
    public double ElapsedSeconds { get => Get<double>(nameof(ElapsedSeconds)); set => Set(nameof(ElapsedSeconds), value); }

    [NotInput]
    [Label("Статус")]
    public string StatusMessage { get => Get<string>(nameof(StatusMessage)); set => Set(nameof(StatusMessage), value); }

    [NotInput]
    [Label("Последняя ошибка")]
    public string LastError { get => Get<string>(nameof(LastError)); set => Set(nameof(LastError), value); }

    public AudioRecorderView()
    {
        Text = "";
        OutputPath = "";
        AutoPlayAfterRecord = false;
        IsRecording = false;
        ElapsedSeconds = 0;
        StatusMessage = "Готов к записи";
        LastError = "";
        MinHeight = 140;
        Height = 140;
        MinWidth = 480;
        Width = 480;
    }
}
