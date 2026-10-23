using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    /// <summary>
    /// Представление ввода IPv4-адреса.
    /// </summary>
    [Label("TCP/IP адрес")]
    public class InputTcpIpAddressView : InputView
    {
        [Label("Адрес")]
        public string Address
        {
            get => Get<string>(nameof(Address));
            set => Set(nameof(Address), value);
        }

        [Label("Маска")]
        public string Mask
        {
            get => Get<string>(nameof(Mask));
            set => Set(nameof(Mask), value);
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public Action<string> OnAddressChanged { get; set; } = _ => { };

        public InputTcpIpAddressView()
        {
            Address = "";
            Mask = "255.255.255.0";
            AddPropertyChangeListener(nameof(Address), evt =>
            {
                OnAddressChanged(evt.After?.ToString() ?? "");
            });
        }

        /// <summary>Проверка формата IPv4: 4 сегмента 0–254 через точку.</summary>
        public static bool TryValidateIpv4(string text, out string error)
            => InputTcpIpAddressAttribute.TryValidateIpv4(text, out error);
    }
}
