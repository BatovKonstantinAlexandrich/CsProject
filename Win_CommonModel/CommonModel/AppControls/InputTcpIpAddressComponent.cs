using WinFormsApp.AppViews.InputModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Связывает <see cref="InputTcpIpAddressView"/> с <see cref="InputTcpIpAddressControl"/>.
    /// </summary>
    public class InputTcpIpAddressComponent : ComponentBase<InputTcpIpAddressView, InputTcpIpAddressControl>
    {
        public InputTcpIpAddressComponent(InputTcpIpAddressView viewModel, InputTcpIpAddressControl control)
            : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            ctrl.SetAddressValueWithoutNotify(view.Address ?? "");

            ctrl.AddressValueChanged += (_, _) =>
            {
                var address = ctrl.AddressValue;
                if (view.Address != address)
                    view.Address = address;
            };

            view.AddPropertyChangeListener(nameof(InputTcpIpAddressView.Address), evt =>
            {
                var next = evt.After?.ToString() ?? "";
                if (ctrl.AddressValue != next)
                    ctrl.SetAddressValueWithoutNotify(next);
            });
        }
    }
}
