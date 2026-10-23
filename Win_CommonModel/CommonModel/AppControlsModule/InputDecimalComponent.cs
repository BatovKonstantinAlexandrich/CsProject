using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.Views.AppViews.InputModule;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Связывает <see cref="InputDecimalView"/> с <see cref="InputDecimalControl"/>.
    /// </summary>
    public class InputDecimalComponent : ComponentBase<InputDecimalView, InputDecimalControl>
    {
        public InputDecimalComponent(InputDecimalView viewModel, InputDecimalControl control) : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            ctrl.DecimalPlaces = view.DecimalPlaces;
            var initial = string.IsNullOrWhiteSpace(view.Text)
                ? InputDecimalControl.FormatDecimal("0", view.DecimalPlaces)
                : InputDecimalControl.FormatDecimal(view.Text, view.DecimalPlaces);
            ctrl.SetValueWithoutNotify(initial);
            if (view.Text != ctrl.Value)
                view.Text = ctrl.Value;

            ctrl.textbox.TextChanged += (_, _) =>
            {
                if (view.Text != ctrl.Value)
                    view.Text = ctrl.Value;
            };

            view.AddPropertyChangeListener(nameof(InputDecimalView.Text), evt =>
            {
                var next = InputDecimalControl.FormatDecimal(evt.After?.ToString() ?? "0", view.DecimalPlaces);
                if (ctrl.Value != next)
                    ctrl.SetValueWithoutNotify(next);
            });

            view.AddPropertyChangeListener(nameof(InputDecimalView.DecimalPlaces), evt =>
            {
                var places = Convert.ToInt32(evt.After ?? 2);
                if (ctrl.DecimalPlaces != places)
                    ctrl.DecimalPlaces = places;
                var formatted = InputDecimalControl.FormatDecimal(ctrl.Value, places);
                ctrl.SetValueWithoutNotify(formatted);
                if (view.Text != formatted)
                    view.Text = formatted;
            });
        }
    }
}
