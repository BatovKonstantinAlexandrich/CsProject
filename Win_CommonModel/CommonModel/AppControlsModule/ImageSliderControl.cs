using System.Drawing;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.ImageModule;

namespace WinFormsApp.AppViewModules.ImageModule;

/// <summary>
/// WinForms-контрол слайдера изображений для <see cref="ImageSliderView"/>.
/// </summary>
public class ImageSliderControl : ControlBase
{
    private readonly ImageControl _image = new();
    private ImageSliderView _sliderView;

    public ImageSliderControl()
    {
        Dock = DockStyle.Fill;
        MinimumSize = new Size(180, 180);
        Controls.Add(_image);
        Load += OnImageSliderControlLoad;
    }

    private void OnImageSliderControlLoad(object sender, EventArgs e)
    {
        if (viewModel is ImageSliderView v)
            BindSliderView(v);
    }

    private void BindSliderView(ImageSliderView v)
    {
        _sliderView = v;
        ApplyData(v.Data);

        v.AddPropertyChangeListener(nameof(ImageSliderView.Data), evt =>
            ApplyData(evt.After as byte[]));

        v.Images.OnAdded((_, _) =>
        {
            if (v.Images.Count == 1 && v.Data is null)
                v.ShowFirst();
        });

        WireMouseToView(_image.pictureBox, v);
    }

    private static void WireMouseToView(Control control, ImageSliderView view)
    {
        control.MouseEnter += (_, e) => view.OnMouseEnter(control, e);
        control.MouseLeave += (_, e) => view.OnMouseLeave(control, e);
    }

    private void ApplyData(byte[] data) => _image.SetData(data);
}
