using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.Views.AppViews.InputModule;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Связывает <see cref="InputNaturalView"/> с <see cref="InputNaturalControl"/>.
    /// </summary>
    public class InputNaturalComponent : ComponentBase<InputNaturalView, InputNaturalControl>
    {
        public InputNaturalComponent(InputNaturalView viewModel, InputNaturalControl control) : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            var initial = string.IsNullOrWhiteSpace(view.Text)
                ? InputNaturalControl.FormatNatural("1")
                : InputNaturalControl.FormatNatural(view.Text);
            ctrl.SetValueWithoutNotify(initial);
            if (view.Text != ctrl.Value)
                view.Text = ctrl.Value;

            ctrl.textbox.TextChanged += (_, _) =>
            {
                if (view.Text != ctrl.Value)
                    view.Text = ctrl.Value;
            };

            view.AddPropertyChangeListener(nameof(InputNaturalView.Text), evt =>
            {
                var next = InputNaturalControl.FormatNatural(evt.After?.ToString() ?? "1");
                if (ctrl.Value != next)
                    ctrl.SetValueWithoutNotify(next);
            });
        }
    }
}
