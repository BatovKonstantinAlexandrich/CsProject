using WinFormsApp.AppViewModules.CheckboxModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class CheckboxComponent : ComponentBase<CheckboxView, CheckboxControl>
    {
        public CheckboxComponent([NotNullNotEmpty] CheckboxView ViewModel, [NotNullNotEmpty] CheckboxControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();
            ctrl.checkbox.Checked = view.Checked;
            view.Height = ctrl.Height;
            ctrl.Enabled = view.Enabled;
            view.AddPropertyChangeListener<bool>(nameof(view.Enabled), text => ctrl.Enabled = text);
            ctrl.Text = view.Text;
            view.AddPropertyChangeListener<string>(nameof(view.Text), text => ctrl.Text = text);
            var before = ctrl.IsChecked;
            view.OnCheckedChanged += (s, e) =>
            {
                var checkedValue = e.After.ToString().ToBool();
                if (view.Checked != checkedValue)
                {
                    view.Checked = checkedValue;
                }
            };


            ctrl.OnCheckedChanged += (val) =>
            {
                view.OnCheckedChanged(this, new PropertyChangedMessage<bool>()
                {
                    Source = ctrl,
                    Property = nameof(ctrl.IsChecked),
                    Before = before,
                    After = val
                });
                before = val;
            };
        }
    }
}
