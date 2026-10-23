using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using MaterialSkin.Controls;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Поле выбора цвета: образец, ввод HEX (#RRGGBB или #RGB), кнопка системной палитры.
    /// </summary>
    public class InputColorControl : ControlBase
    {
        private static readonly Regex HexRegex = new(
            @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$",
            RegexOptions.Compiled);

        private bool _suppressHexEvents;

        public readonly Panel ColorPreview;
        public readonly MaterialTextBox2 HexTextBox;
        public readonly Button PickButton;

        public Action<string> OnColorChanged = text => { };

        public InputColorControl()
        {
            Height = 40;
            MinimumSize = new Size(160, 36);
            MaximumSize = new Size(2000, 48);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36F));

            ColorPreview = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 8, 2),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black,
                Cursor = Cursors.Hand,
                AccessibleName = "Образец цвета",
            };
            ColorPreview.Click += (_, _) => ShowColorDialog();
            ColorPreview.DoubleClick += (_, _) => ShowColorDialog();

            HexTextBox = new MaterialTextBox2
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 4, 2),
                TabIndex = 0,
                AccessibleName = "HEX цвета",
            };
            HexTextBox.TextChanged += HexTextBox_TextChanged;
            HexTextBox.TextChanged += (s, e) =>
            {
                OnColorChanged(HexTextBox.Text);
            };

            PickButton = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                Text = "…",
                TabIndex = 2,
                AccessibleName = "Открыть палитру",
                UseVisualStyleBackColor = true,
            };
            PickButton.Click += (_, _) => ShowColorDialog();

            layout.Controls.Add(ColorPreview, 0, 0);
            layout.Controls.Add(HexTextBox, 1, 0);
            layout.Controls.Add(PickButton, 2, 0);

            this.LayoutFill(layout);
        }

        /// <summary>Текстовое значение цвета в формате #RRGGBB.</summary>
        public string HexValue
        {
            get => HexTextBox.Text?.Trim() ?? string.Empty;
            set => SetHexValueInternal(value, raiseChanged: true);
        }

        /// <summary>Текущий цвет как <see cref="Color"/>.</summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectedColor
        {
            get => TryGetColor(HexValue, out var c) ? c : Color.Black;
            set => SetHexValueInternal(ColorToHex(value), raiseChanged: true);
        }

        /// <summary>Вызывается после успешного разбора HEX и обновления образца (ввод с клавиатуры или диалог).</summary>
        public event EventHandler HexValueChanged;

        /// <summary>Синхронизация из модели без лишних уведомлений (например, при двустороннем биндинге).</summary>
        public void SetHexValueWithoutNotify(string value)
        {
            SetHexValueInternal(value, raiseChanged: false);
        }

        private void SetHexValueInternal(string value, bool raiseChanged)
        {
            var v = string.IsNullOrWhiteSpace(value) ? "#000000" : NormalizeHex(value);
            _suppressHexEvents = true;
            try
            {
                if (HexTextBox.Text != v)
                    HexTextBox.Text = v;
                else
                    ApplyPreviewFromHex(v);
            }
            finally
            {
                _suppressHexEvents = false;
            }

            if (raiseChanged && TryGetColor(v, out _))
            {
                Changed = true;
                HexValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HexTextBox_TextChanged(object sender, EventArgs e)
        {
            var raw = HexTextBox.Text?.Trim() ?? string.Empty;
            if (TryGetColor(raw, out var c))
                ColorPreview.BackColor = c;

            if (_suppressHexEvents)
                return;

            Changed = true;
            HexValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ShowColorDialog()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            using var dlg = new ColorDialog
            {
                Color = TryGetColor(HexValue, out var cur) ? cur : Color.Black,
                FullOpen = true,
            };

            var owner = FindForm();
            if (dlg.ShowDialog(owner) != DialogResult.OK)
                return;

            SetHexValueInternal(ColorToHex(dlg.Color), raiseChanged: true);
        }

        private void ApplyPreviewFromHex(string hex)
        {
            if (TryGetColor(hex, out var c))
                ColorPreview.BackColor = c;
        }

        public static string ColorToHex(Color c)
            => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        public static string NormalizeHex(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return "#000000";
            s = s.Trim();
            if (!s.StartsWith("#", StringComparison.Ordinal))
                s = "#" + s;
            if (s.Length == 4 && HexRegex.IsMatch(s))
            {
                var r = s[1];
                var g = s[2];
                var b = s[3];
                return $"#{r}{r}{g}{g}{b}{b}";
            }
            return s;
        }

        public static bool TryGetColor(string hex, out Color color)
        {
            color = Color.Black;
            if (string.IsNullOrWhiteSpace(hex))
                return false;
            var n = NormalizeHex(hex);
            if (!HexRegex.IsMatch(n))
                return false;
            try
            {
                color = ColorTranslator.FromHtml(n);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
