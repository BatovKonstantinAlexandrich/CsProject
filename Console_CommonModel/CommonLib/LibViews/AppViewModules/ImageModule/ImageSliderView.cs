using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ImageModule;

/// <summary>
/// Слайдер изображений: при наведении курсора автоматически переключает кадры из коллекции.
/// </summary>
[ViewUsageExample("var view = new ImageSliderView { Text = \"Слайдер изображений\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<image-slider-view Text=\"Слайдер изображений\" Dock=\"Top\" />")]
[Label("Слайдер изображений")]
public class ImageSliderView : AppViewBase
{

    [Label("Изображения")]
    public ChangingList<byte[]> Images { get; set; } = new();

    [Label("Текущее изображение")]
    public byte[] Data { get => Get<byte[]>(nameof(Data)); set => Set(nameof(Data), value); }
    [InputNumber]
    [Label("Интервал слайдшоу (мс)")]
    public int Interval { get => Get<int>(nameof(Interval)); set => Set(nameof(Interval), value); }

    /// <summary>
    /// Растянуть, замостить, разместить в исходном размере.
    /// </summary>
    [Label("Растягивание")]
    [InputSelect("cover,repeat,corner")]
    public string Position { get => Get<string>(nameof(Position)); set => Set(nameof(Position), value); }

    private int _imageIndex = -1;
    private bool _mouseOver;

    public ImageSliderView()
    {
        Interval = 1000;
        Position = "cover";
        MinHeight = 180;
        MinWidth = 180;

        OnMouseEnter += OnSliderMouseEnter;
        OnMouseLeave += OnSliderMouseLeave;

        Images.OnAdded((_, _) =>
        {
            if (Images.Count == 1)
                NextImage();
        });
        Images.OnRemoved((_, _) =>
        {
            if (Images.Count == 0)
            {
                _imageIndex = -1;
                Data = null;
            }
            else
                NextImage();
        });
    }

    public void NextImage()
    {
        if (Images is null || Images.Count == 0)
        {
            Data = null;
            return;
        }

        _imageIndex++;
        if (_imageIndex >= Images.Count)
            _imageIndex = 0;
        Data = Images[_imageIndex];
    }

    public void ShowFirst()
    {
        _imageIndex = -1;
        NextImage();
    }

    private void OnSliderMouseEnter(object sender, EventArgs e)
    {
        _mouseOver = true;
        ScheduleNextSlide();
    }

    private void OnSliderMouseLeave(object sender, EventArgs e)
    {
        _mouseOver = false;
    }

    private void ScheduleNextSlide()
    {
        if (Images is null || Images.Count <= 1)
            return;

        Action todo = null;
        todo = () =>
        {
            if (!_mouseOver)
                return;
            NextImage();
            Thread.Sleep(Math.Max(100, Interval));
            if (_mouseOver)
                PutAction("След. слайд", todo);
        };
        PutAction("След. слайд", todo);
    }
}
