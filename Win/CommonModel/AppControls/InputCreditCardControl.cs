using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using MaterialSkin.Controls;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Поле ввода номера карты: цифры с автоформатированием XXXX-XXXX-XXXX-XXXX.
    /// </summary>
    public class InputCreditCardControl : ControlBase
    {
        private bool _suppressEvents;
        private readonly Color _normalFore;

        public readonly MaterialTextBox2 NumberTextBox;

        public Action<string> OnNumberChanged = _ => { };

        public InputCreditCardControl()
        {
            Height = 40;
            MinimumSize = new Size(180, 36);
            MaximumSize = new Size(2000, 48);

            NumberTextBox = new MaterialTextBox2
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                TabIndex = 0,
                AccessibleName = "Номер кредитной карты",
                Hint = "0000-0000-0000-0000",
                MaxLength = 19,
            };
            _normalFore = NumberTextBox.ForeColor;
            NumberTextBox.KeyPress += NumberTextBox_KeyPress;
            NumberTextBox.TextChanged += NumberTextBox_TextChanged;

            this.LayoutFill(NumberTextBox);
        }

        public string NumberValue
        {
            get => NumberTextBox.Text?.Trim() ?? string.Empty;
            set => SetNumberValueInternal(value, raiseChanged: true);
        }

        public event EventHandler NumberValueChanged;

        public void SetNumberValueWithoutNotify(string value)
            => SetNumberValueInternal(value, raiseChanged: false);

        private void SetNumberValueInternal(string value, bool raiseChanged)
        {
            var v = InputCreditCardAttribute.FormatCardNumber(value);
            _suppressEvents = true;
            try
            {
                if (NumberTextBox.Text != v)
                    NumberTextBox.Text = v;
                ApplyValidationStyle(v);
            }
            finally
            {
                _suppressEvents = false;
            }

            if (raiseChanged)
            {
                Changed = true;
                NumberValueChanged?.Invoke(this, EventArgs.Empty);
                OnNumberChanged(v);
            }
        }

        private void NumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;
            if (char.IsDigit(e.KeyChar) || e.KeyChar == '-')
                return;
            e.Handled = true;
        }

        private void NumberTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents)
                return;

            var formatted = InputCreditCardAttribute.FormatCardNumber(NumberTextBox.Text);
            if (formatted != NumberTextBox.Text)
            {
                var sel = NumberTextBox.SelectionStart;
                _suppressEvents = true;
                try
                {
                    NumberTextBox.Text = formatted;
                    NumberTextBox.SelectionStart = Math.Min(sel, formatted.Length);
                }
                finally
                {
                    _suppressEvents = false;
                }
            }

            ApplyValidationStyle(formatted);

            Changed = true;
            NumberValueChanged?.Invoke(this, EventArgs.Empty);
            OnNumberChanged(formatted);
        }

        private void ApplyValidationStyle(string text)
        {
            if (InputCreditCardAttribute.TryValidate(text, out _))
                NumberTextBox.ForeColor = _normalFore;
            else
                NumberTextBox.ForeColor = Color.Firebrick;
        }
    }
}
