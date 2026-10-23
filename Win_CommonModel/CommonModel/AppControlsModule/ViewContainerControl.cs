using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

using WinFormsApp.AppViewModules.ButtonModule;
using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.DesignerControls;

namespace WinFormsApp.AppComponents.AppControlsModule;

/// <summary>
/// Хост WinForms: принимает <see cref="AppViewBase"/>, отображает результат <see cref="AppViewBase.ToControl"/>.
/// В Visual Studio предпросмотр обеспечивает <see cref="ViewContainerControlDesigner"/>.
/// </summary>
[ToolboxItem(false)]
[ToolboxBitmap(typeof(UserControl))]
[TypeConverter(typeof(ExpandableObjectConverter))]
[DefaultProperty(nameof(AppViewBase))]
[Designer(typeof(ViewContainerControlDesigner))]
[Description("Отображает AppViewBase через ToControl() в runtime и в дизайнере Visual Studio")]
public class ViewContainerControl : UserControl
{
    private AppViewBase _appViewBase = new ButtonView { Text = "Кнопка" };
    private Control _hostedControl;

    public ViewContainerControl()
    {
        DoubleBuffered = true;
        MinimumSize = new Size(120, 80);
        BackColor = SystemColors.Control;
    }

    /// <summary>
    /// Модель представления, которая визуализируется в контроле.
    /// </summary>
    [Editor(typeof(AppViewBaseTypeSelectorEditor), typeof(UITypeEditor))]
    [Category("AppView")]
    [Description("Модель представления (результат ToControl отображается внутри контрола)")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public AppViewBase AppViewBase
    {
        get => _appViewBase;
        set
        {
            if (ReferenceEquals(_appViewBase, value))
                return;
            _appViewBase = value;
            RefreshHostedView();
        }
    }

    /// <summary>
    /// Привязать модель и обновить содержимое.
    /// </summary>
    public void BindView(AppViewBase view)
    {
        AppViewBase = view;
    }

    /// <summary>
    /// Пересоздаёт дочерний control из <see cref="AppViewBase.ToControl"/>.
    /// В режиме дизайна VS обновление выполняет <see cref="ViewContainerControlDesigner"/>.
    /// </summary>
    public void RefreshHostedView()
    {
        if (IsVisualStudioDesignHost)
            return;

        RemoveHostedControl();
        if (_appViewBase is null)
            return;

        try
        {
            ResetViewControlCache();
            _hostedControl = _appViewBase.ToControl();
            if (_hostedControl is null)
                return;

            _hostedControl.Dock = DockStyle.Fill;
            Controls.Add(_hostedControl);
            _hostedControl.BringToFront();
        }
        catch (Exception ex)
        {
            Controls.Add(CreateErrorLabel(ex.Message));
        }
    }

    private static Label CreateErrorLabel(string message) =>
        new()
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.DarkRed,
            BackColor = Color.FromArgb(255, 245, 245),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Ошибка ToControl():\r\n" + message,
            AutoEllipsis = true,
        };

    private void RemoveHostedControl()
    {
        if (_hostedControl is null)
        {
            Controls.Clear();
            return;
        }

        if (Controls.Contains(_hostedControl))
            Controls.Remove(_hostedControl);
        else
            Controls.Clear();

        _hostedControl = null;
    }

    /// <summary>Сбрасывает кэш WinForms-контрола модели перед повторным ToControl().</summary>
    protected void ResetViewControlCache()
    {
        if (_appViewBase is null)
            return;
        _appViewBase.control = null;
    }

    /// <summary>
    /// Дизайн-поверхность Visual Studio (не путать с <see cref="AppViewBase.DesignMode"/>).
    /// </summary>
    protected bool IsVisualStudioDesignHost =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshHostedView();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!IsVisualStudioDesignHost)
            RefreshHostedView();
    }
}
