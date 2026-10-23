using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MaterialSkin.Controls;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Поле пути к файлу: текстовый ввод и кнопка выбора через <see cref="OpenFileDialog"/>.
    /// </summary>
    public class InputFilePathControl : ControlBase
    {
        private bool _suppressPathEvents;

        public readonly MaterialTextBox2 PathTextBox;
        public readonly Button BrowseButton;

        public string Filter { get; set; } = "All files (*.*)|*.*";
        public string DialogTitle { get; set; } = "Выберите файл";

        public Action<string> OnPathChanged = _ => { };

        public InputFilePathControl()
        {
            Height = 40;
            MinimumSize = new Size(180, 36);
            MaximumSize = new Size(2000, 48);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));

            PathTextBox = new MaterialTextBox2
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 4, 2),
                TabIndex = 0,
                AccessibleName = "Путь к файлу",
            };
            PathTextBox.TextChanged += PathTextBox_TextChanged;

            BrowseButton = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                Text = "…",
                TabIndex = 1,
                AccessibleName = "Выбрать файл",
                UseVisualStyleBackColor = true,
            };
            BrowseButton.Click += (_, _) => ShowOpenFileDialog();

            layout.Controls.Add(PathTextBox, 0, 0);
            layout.Controls.Add(BrowseButton, 1, 0);
            this.LayoutFill(layout);
        }

        /// <summary>Текущий путь к файлу.</summary>
        public string PathValue
        {
            get => PathTextBox.Text?.Trim() ?? string.Empty;
            set => SetPathValueInternal(value, raiseChanged: true);
        }

        public event EventHandler PathValueChanged;

        public void SetPathValueWithoutNotify(string value)
            => SetPathValueInternal(value, raiseChanged: false);

        private void SetPathValueInternal(string value, bool raiseChanged)
        {
            var v = value?.Trim() ?? string.Empty;
            _suppressPathEvents = true;
            try
            {
                if (PathTextBox.Text != v)
                    PathTextBox.Text = v;
            }
            finally
            {
                _suppressPathEvents = false;
            }

            if (raiseChanged)
            {
                Changed = true;
                PathValueChanged?.Invoke(this, EventArgs.Empty);
                OnPathChanged(v);
            }
        }

        private void PathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suppressPathEvents)
                return;

            Changed = true;
            var path = PathValue;
            PathValueChanged?.Invoke(this, EventArgs.Empty);
            OnPathChanged(path);
        }

        private void ShowOpenFileDialog()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            var initialDir = Directory.GetCurrentDirectory();
            var current = PathValue;
            if (!string.IsNullOrWhiteSpace(current))
            {
                try
                {
                    var full = System.IO.Path.GetFullPath(current);
                    var dir = System.IO.Path.GetDirectoryName(full);
                    if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                        initialDir = dir;
                }
                catch
                {
                    /* ignore */
                }
            }

            new FileDialogService().OpenFile(
                string.IsNullOrWhiteSpace(DialogTitle) ? "Выберите файл" : DialogTitle,
                string.IsNullOrWhiteSpace(Filter) ? "All files (*.*)|*.*" : Filter,
                initialDir,
                file => SetPathValueInternal(file, raiseChanged: true));
        }
    }
}
