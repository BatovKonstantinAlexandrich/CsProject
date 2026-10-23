using System.Globalization;
using MaterialSkin.Controls;
using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Поле ввода натурального целого числа (1, 2, 3, …).
    /// </summary>
    public class InputNaturalControl : ControlBase
    {
        public readonly MaterialTextBox textbox;
        private string _lastValid = "1";
        private bool _suppress;

        public InputNaturalControl()
        {
            AutoSize = true;
            MinimumSize = new Size(20, 20);
            this.LayoutFill(textbox = new MaterialTextBox());
            Height = textbox.Height;
            _lastValid = FormatNatural("1");
            textbox.Text = _lastValid;
            textbox.TextChanged += OnTextChanged;
        }

        public string Value
        {
            get => textbox.Text ?? string.Empty;
            set => SetValueInternal(FormatNatural(value), raiseChanged: false);
        }

        public void SetValueWithoutNotify(string value)
            => SetValueInternal(FormatNatural(value), raiseChanged: false);

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (_suppress)
                return;

            var raw = textbox.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                Revert();
                return;
            }

            if (!IsPartialNaturalInput(raw))
            {
                Revert();
                return;
            }

            if (TryParseNatural(raw, out _))
            {
                _lastValid = raw.Trim();
                Changed = true;
                return;
            }

            // Промежуточный ввод (например, первая цифра многозначного числа).
            _lastValid = raw.Trim();
        }

        private void Revert() => SetValueInternal(_lastValid, raiseChanged: false);

        private void SetValueInternal(string value, bool raiseChanged)
        {
            _suppress = true;
            try
            {
                if (textbox.Text != value)
                    textbox.Text = value;
            }
            finally
            {
                _suppress = false;
            }

            _lastValid = value;
            if (raiseChanged)
                Changed = true;
        }

        public static string FormatNatural(string text)
        {
            if (TryParseNatural(text, out var value))
                return value.ToString(CultureInfo.InvariantCulture);
            return "1";
        }

        private static bool TryParseNatural(string text, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;
            if (!int.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out value))
                return false;
            return value >= 1;
        }

        private static bool IsPartialNaturalInput(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            text = text.Trim();
            if (text.Length > 1 && text[0] == '0')
                return false;

            foreach (var ch in text)
            {
                if (!char.IsDigit(ch))
                    return false;
            }

            return true;
        }
    }
}
