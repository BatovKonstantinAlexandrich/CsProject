using MaterialSkin.Controls;

namespace WinFormsApp.AppComponents.AppControls
{
    public class TextControl : ControlBase
    {
        public TextControl()
        {
            InitializeComponent();
            AutoSize = true;
            Height = label.Height;
        }

        public override Font Font { get => label.Font; set => label.Font = value; }

        public string GetLabelText() => label.Text;

        public void SetLabelText(string text) => label.Text = text ?? string.Empty;

        public void ApplyTextAlign(string align)
        {
            var normalized = (align ?? "left").Trim().ToLowerInvariant();
            label.TextAlign = normalized switch
            {
                "center" => ContentAlignment.MiddleCenter,
                "right" => ContentAlignment.MiddleRight,
                _ => ContentAlignment.MiddleLeft,
            };

            var fillWidth = normalized is "center" or "right";
            label.AutoSize = !fillWidth;
            label.Dock = fillWidth ? DockStyle.Fill : DockStyle.None;
            if (fillWidth && ClientSize.Width > 0)
                label.Width = ClientSize.Width;
        }

        public void ApplyForeColor(string colorHtml)
        {
            if (string.IsNullOrWhiteSpace(colorHtml))
                return;
            try
            {
                label.ForeColor = ColorTranslator.FromHtml(colorHtml);
            }
            catch
            {
                // ignore invalid color
            }
        }

        public void ApplyFontSize(float fontSize)
        {
            if (fontSize <= 0)
                return;
            label.Font = new Font(label.Font.FontFamily, fontSize, label.Font.Style, label.Font.Unit);
        }

        public void ApplyTextIndent(int indent)
        {
            label.Padding = new Padding(Math.Max(0, indent), label.Padding.Top, label.Padding.Right, label.Padding.Bottom);
        }

        public void SyncLabelBounds(int viewWidth, int viewHeight)
        {
            if (viewWidth > 0)
                Width = viewWidth;
            if (viewHeight > 0)
                Height = viewHeight;
            if (label.Dock == DockStyle.Fill && ClientSize.Width > 0)
                label.Width = ClientSize.Width;
        }

        private void InitializeComponent()
        {
            label = new Label();
            SuspendLayout();
            label.AutoSize = true;
            label.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            label.Location = new Point(0, 0);
            label.Name = "label";
            label.Size = new Size(62, 19);
            label.TabIndex = 0;
            label.Text = "Text title";
            label.Click += (s, e) => viewModel?.OnClick(s, e);
            AutoSize = true;
            Controls.Add(label);
            Name = "TextControl";
            Size = new Size(66, 33);
            ResumeLayout(false);
            PerformLayout();
        }

        public Label label = new();
    }
}
