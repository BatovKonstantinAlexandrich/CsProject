using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using WinFormsApp.AppViewModules.ButtonModule;
using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.DesignerControls;

namespace WinFormsApp.AppComponents.AppControlsModule;

/// <summary>
/// Элемент панели Visual Studio для <see cref="ButtonView"/>.
/// На форме отображается результат <see cref="AppViewBase.ToControl"/>,
/// свойства модели редактируются в Property Grid.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(Button))]
[DefaultProperty(nameof(ButtonText))]
[Designer(typeof(ViewContainerControlDesigner))]
[Description("Кнопка (ButtonView): preview через ToControl(), свойства модели — в Property Grid")]
public class ButtonViewHost : ViewContainerControl
{
    public ButtonViewHost()
    {
        base.AppViewBase = new ButtonView { Text = "Кнопка" };
        MinimumSize = new Size(80, 32);
        Size = new Size(120, 40);
    }

    private ButtonView Model => (ButtonView)base.AppViewBase;

    /// <summary>Скрываем общий редактор типа — тип зафиксирован как ButtonView.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new AppViewBase AppViewBase
    {
        get => base.AppViewBase;
        set => base.AppViewBase = value;
    }

    [Category("AppView")]
    [DisplayName("Надпись")]
    [Description("Текст кнопки (ButtonView.Text)")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ButtonText
    {
        get => Model?.Text ?? "";
        set
        {
            if (Model is null || Model.Text == value)
                return;
            Model.Text = value;
        }
    }

    [Category("AppView")]
    [DisplayName("Включена")]
    [Description("ButtonView.Enabled")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool ButtonEnabled
    {
        get => Model?.Enabled ?? true;
        set
        {
            if (Model is null || Model.Enabled == value)
                return;
            Model.Enabled = value;
        }
    }

    [Category("AppView")]
    [DisplayName("Иконка")]
    [Description("ButtonView.Icon")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ButtonIcon
    {
        get => Model?.Icon ?? "";
        set
        {
            if (Model is null || Model.Icon == value)
                return;
            Model.Icon = value;
        }
    }

    private bool ShouldSerializeButtonIcon() => ButtonIcon != "home";

    private bool ShouldSerializeButtonText() => ButtonText != "Кнопка";

    private bool ShouldSerializeButtonEnabled() => !ButtonEnabled;
}
