using WinFormsApp.AppViews.InputModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Связывает <see cref="InputCreditCardView"/> с <see cref="InputCreditCardControl"/>.
    /// </summary>
    public class InputCreditCardComponent : ComponentBase<InputCreditCardView, InputCreditCardControl>
    {
        public InputCreditCardComponent(InputCreditCardView viewModel, InputCreditCardControl control)
            : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            ctrl.SetNumberValueWithoutNotify(view.Number ?? "");

            ctrl.NumberValueChanged += (_, _) =>
            {
                var number = ctrl.NumberValue;
                if (view.Number != number)
                    view.Number = number;
            };

            view.AddPropertyChangeListener(nameof(InputCreditCardView.Number), evt =>
            {
                var next = evt.After?.ToString() ?? "";
                if (ctrl.NumberValue != next)
                    ctrl.SetNumberValueWithoutNotify(next);
            });
        }
    }
}
