using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
[ViewUsageExample("var view = new ImageView { Text = \"Изображение\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<image-view Text=\"Изображение\" Dock=\"Top\" />")]
[Label("Изображение")]
public class ImageView: AppViewBase
{
    public string Text { get; set; }
    public ImageView()
    {
       
        Position = "cover";
        KeepAspectRatio = true;
        MinWidth = 200;
        MinHeight = 160;
        Width = MinWidth;
        Height = MinHeight;
    }

    /// <summary>
    /// Растянуть,замостить,разместить в исходном размере
    /// </summary>
    [InputSelect("cover,repeat,corner")]
    [Label("Position")]
    public string Position { get; set; }

    /// <summary>
    /// Если true — при отображении сохраняются пропорции изображения (без растягивания).
    /// </summary>
    [InputBool]
[Label("Сохранять пропорции")]
    public bool KeepAspectRatio { get => Get<bool>(nameof(KeepAspectRatio)); set => Set(nameof(KeepAspectRatio), value); }

    
    [Label("Data")]
    public byte[] Data { get; set; }


}