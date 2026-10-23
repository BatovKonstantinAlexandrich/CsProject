using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using MaterialSkin.Controls;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Поле ввода IPv4: текст с фильтрацией символов и лёгкой валидацией.
    /// </summary>
    public class InputTcpIpAddressControl : ControlBase
    {
        private bool _suppressEvents;
        private readonly Color _normalFore;

        public readonly MaterialTextBox2 AddressTextBox;

        public Action<string> OnAddressChanged = _ => { };

        public InputTcpIpAddressControl()
        {
            Height = 40;
            MinimumSize = new Size(140, 36);
            MaximumSize = new Size(2000, 48);

            AddressTextBox = new MaterialTextBox2
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                TabIndex = 0,
                AccessibleName = "TCP/IP адрес",
                Hint = "0.0.0.0",
            };
            _normalFore = AddressTextBox.ForeColor;
            AddressTextBox.KeyPress += AddressTextBox_KeyPress;
            AddressTextBox.TextChanged += AddressTextBox_TextChanged;

            this.LayoutFill(AddressTextBox);
        }

        public string AddressValue
        {
            get => AddressTextBox.Text?.Trim() ?? string.Empty;
            set => SetAddressValueInternal(value, raiseChanged: true);
        }

        public event EventHandler AddressValueChanged;

        public void SetAddressValueWithoutNotify(string value)
            => SetAddressValueInternal(value, raiseChanged: false);

        private void SetAddressValueInternal(string value, bool raiseChanged)
        {
            var v = value?.Trim() ?? string.Empty;
            _suppressEvents = true;
            try
            {
                if (AddressTextBox.Text != v)
                    AddressTextBox.Text = v;
                ApplyValidationStyle(v);
            }
            finally
            {
                _suppressEvents = false;
            }

            if (raiseChanged)
            {
                Changed = true;
                AddressValueChanged?.Invoke(this, EventArgs.Empty);
                OnAddressChanged(v);
            }
        }

        private void AddressTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;
            if (char.IsDigit(e.KeyChar) || e.KeyChar == '.')
                return;
            e.Handled = true;
        }

        private void AddressTextBox_TextChanged(object sender, EventArgs e)
        {
            var raw = AddressTextBox.Text ?? string.Empty;
            var filtered = new string(raw.Where(c => char.IsDigit(c) || c == '.').ToArray());
            if (filtered != raw)
            {
                var sel = AddressTextBox.SelectionStart;
                _suppressEvents = true;
                try
                {
                    AddressTextBox.Text = filtered;
                    AddressTextBox.SelectionStart = Math.Min(sel, filtered.Length);
                }
                finally
                {
                    _suppressEvents = false;
                }
                raw = filtered;
            }

            ApplyValidationStyle(raw);

            if (_suppressEvents)
                return;

            Changed = true;
            AddressValueChanged?.Invoke(this, EventArgs.Empty);
            OnAddressChanged(raw.Trim());
        }

        private void ApplyValidationStyle(string text)
        {
            if (InputTcpIpAddressAttribute.TryValidateIpv4(text, out _))
                AddressTextBox.ForeColor = _normalFore;
            else
                AddressTextBox.ForeColor = Color.Firebrick;
        }
    }
}
