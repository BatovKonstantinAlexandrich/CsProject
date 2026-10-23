using WinFormsApp.AppViews.InputModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Связывает <see cref="InputColorView"/> с <see cref="InputColorControl"/>.
    /// </summary>
    public class InputColorComponent : ComponentBase<InputColorView, InputColorControl>
    {
        public InputColorComponent(InputColorView viewModel, InputColorControl control) : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();
            var initial = string.IsNullOrWhiteSpace(view.Color) ? "#000000" : InputColorControl.NormalizeHex(view.Color);
            ctrl.SetHexValueWithoutNotify(initial);

            ctrl.HexValueChanged += (_, _) =>
            {
                var h = ctrl.HexValue;
                if (view.Color != h)
                    view.Color = h;
            };

            view.AddPropertyChangeListener(nameof(InputColorView.Color), evt =>
            {
                var next = InputColorControl.NormalizeHex(evt.After?.ToString() ?? "#000000");
                if (ctrl.HexValue != next)
                    ctrl.SetHexValueWithoutNotify(next);
            });
        }
    }
}
