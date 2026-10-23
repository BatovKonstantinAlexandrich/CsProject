using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

using WinFormsApp.AppComponents.AppControlsModule;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.DesignerControls;

/// <summary>
/// Отображает <see cref="ViewContainerControl.AppViewBase.ToControl"/> в визуальном редакторе Visual Studio.
/// </summary>
public class ViewContainerControlDesigner : ParentControlDesigner
{
    private static readonly HashSet<string> LayoutPropertyNames = new(StringComparer.Ordinal)
    {
        nameof(Control.Location),
        nameof(Control.Size),
        nameof(Control.ClientSize),
        nameof(Control.Bounds),
        nameof(Control.Name),
        nameof(Control.TabIndex),
        nameof(Control.Anchor),
        nameof(Control.Dock),
        nameof(Control.Margin),
        nameof(Control.Padding),
        nameof(Control.Visible),
        nameof(Control.BackColor),
        nameof(Control.ForeColor),
        nameof(Control.Font),
    };

    private ViewContainerControl _host;
    private Control _previewControl;
    private IComponentChangeService _changeService;
    private PropertyDescriptor _appViewBaseProperty;
    private AppViewBase _subscribedModel;

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        _host = component as ViewContainerControl;
        if (_host is null)
            return;

        _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        if (_changeService is not null)
            _changeService.ComponentChanged += OnComponentChanged;

        _appViewBaseProperty = TypeDescriptor.GetProperties(_host)[nameof(ViewContainerControl.AppViewBase)];
        if (_appViewBaseProperty is not null)
            _appViewBaseProperty.AddValueChanged(_host, OnAppViewBaseChanged);

        UpdatePreview();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_changeService is not null)
                _changeService.ComponentChanged -= OnComponentChanged;

            if (_appViewBaseProperty is not null && _host is not null)
                _appViewBaseProperty.RemoveValueChanged(_host, OnAppViewBaseChanged);

            UnsubscribeModelChanges();
            RemovePreview();
        }

        base.Dispose(disposing);
    }

    private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
    {
        if (e.Component != _host)
            return;

        var name = e.Member?.Name;
        if (string.IsNullOrEmpty(name) || LayoutPropertyNames.Contains(name))
            return;

        ScheduleUpdatePreview();
    }

    private void OnAppViewBaseChanged(object sender, EventArgs e) => ScheduleUpdatePreview();

    private void OnModelChanged(PropertyChangedMessage _) => ScheduleUpdatePreview();

    private void ScheduleUpdatePreview()
    {
        if (_host is null)
            return;

        if (_host.IsHandleCreated)
            _host.BeginInvoke(new Action(UpdatePreview));
        else
            UpdatePreview();
    }

    private void SubscribeModelChanges(AppViewBase model)
    {
        UnsubscribeModelChanges();
        _subscribedModel = model;
        if (_subscribedModel is not null)
            _subscribedModel.OnChange += OnModelChanged;
    }

    private void UnsubscribeModelChanges()
    {
        if (_subscribedModel is not null)
        {
            _subscribedModel.OnChange -= OnModelChanged;
            _subscribedModel = null;
        }
    }

    private void UpdatePreview()
    {
        if (_host is null)
            return;

        RemovePreview();

        var model = _host.AppViewBase;
        if (model is null)
            return;

        SubscribeModelChanges(model);

        try
        {
            model.DesignMode = true;
            model.control = null;
            _previewControl = model.ToControl();
            if (_previewControl is null)
                return;

            _previewControl.Dock = DockStyle.Fill;
            _previewControl.Visible = true;
            _previewControl.Enabled = true;
            _previewControl.Parent = _host;
            _host.Controls.Add(_previewControl);
            EnableDesignMode(_previewControl, "AppViewPreview");
            _host.Invalidate(true);
        }
        catch (Exception ex)
        {
            var label = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = System.Drawing.Color.DarkRed,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Text = "Предпросмотр:\r\n" + ex.Message,
            };
            _previewControl = label;
            _host.Controls.Add(label);
        }
    }

    private void RemovePreview()
    {
        if (_previewControl is null || _host is null)
            return;

        if (_host.Controls.Contains(_previewControl))
            _host.Controls.Remove(_previewControl);

        _previewControl = null;
    }

    public override System.Collections.ICollection AssociatedComponents
    {
        get
        {
            var components = base.AssociatedComponents;
            if (_previewControl is null)
                return components;

            var list = new System.Collections.ArrayList(components);
            list.Add(_previewControl);
            return list;
        }
    }

    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        base.OnPaintAdornments(pe);

        if (_host is null)
            return;

        if (_previewControl is null || !_host.Controls.Contains(_previewControl))
            UpdatePreview();
    }
}
