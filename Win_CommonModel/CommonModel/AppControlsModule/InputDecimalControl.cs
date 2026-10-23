using System.Globalization;
using MaterialSkin.Controls;
using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Поле ввода десятичного числа с ограничением знаков после запятой.
    /// </summary>
    public class InputDecimalControl : ControlBase
    {
        public readonly MaterialTextBox textbox;
        private string _lastValid = "0.00";
        private bool _suppress;

        public int DecimalPlaces { get; set; } = 2;

        public InputDecimalControl()
        {
            AutoSize = true;
            MinimumSize = new Size(20, 20);
            this.LayoutFill(textbox = new MaterialTextBox());
            Height = textbox.Height;
            _lastValid = FormatDecimal("0", DecimalPlaces);
            textbox.Text = _lastValid;
            textbox.TextChanged += OnTextChanged;
        }

        public string Value
        {
            get => textbox.Text ?? string.Empty;
            set => SetValueInternal(NormalizeComplete(value), raiseChanged: false);
        }

        public void SetValueWithoutNotify(string value)
            => SetValueInternal(NormalizeComplete(value), raiseChanged: false);

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

            if (raw.ValidateIsFloat() == null)
            {
                var normalized = NormalizeComplete(raw);
                if (normalized != raw)
                {
                    SetValueInternal(normalized, raiseChanged: true);
                    return;
                }

                _lastValid = normalized;
                Changed = true;
                return;
            }

            if (IsPartialDecimalInput(raw))
            {
                _lastValid = raw.Trim().Replace(',', '.');
                return;
            }

            Revert();
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

        public static string FormatDecimal(string text, int decimalPlaces)
        {
            if (string.IsNullOrWhiteSpace(text))
                return decimalPlaces > 0
                    ? (0m).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)
                    : "0";

            var normalized = text.Trim().Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return normalized;

            return decimalPlaces > 0
                ? value.ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)
                : value.ToString("G", CultureInfo.InvariantCulture);
        }

        private string NormalizeComplete(string text) => FormatDecimal(text, DecimalPlaces);

        private static bool IsPartialDecimalInput(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            s = s.Trim();
            if (s == "-")
                return true;

            var normalized = s.Replace(',', '.');
            var start = normalized.StartsWith('-') ? 1 : 0;
            if (start >= normalized.Length)
                return false;

            var hasDot = false;
            for (var i = start; i < normalized.Length; i++)
            {
                var ch = normalized[i];
                if (ch == '.')
                {
                    if (hasDot)
                        return false;
                    hasDot = true;
                    continue;
                }

                if (!char.IsDigit(ch))
                    return false;
            }

            return true;
        }
    }
}
