using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Данные сегмента для отрисовки в <see cref="BreadcrumbControl"/>.
    /// </summary>
    public sealed class BreadcrumbSegment
    {
        public string Label { get; init; } = string.Empty;
        public string Href { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }

    public sealed class BreadcrumbClickEventArgs : EventArgs
    {
        public BreadcrumbClickEventArgs(int index, string label, string href)
        {
            Index = index;
            Label = label;
            Href = href;
        }

        public int Index { get; }
        public string Label { get; }
        public string Href { get; }
    }

    /// <summary>
    /// Desktop-контрол цепочки навигации в стиле веб-компонента breadcrumbs.
    /// </summary>
    public class BreadcrumbControl : ControlBase
    {
        private static readonly Color BackgroundColor = Color.FromArgb(248, 249, 251);
        private static readonly Color BorderLineColor = Color.FromArgb(214, 219, 226);
        private static readonly Color LinkColor = Color.FromArgb(37, 99, 235);
        private static readonly Color LinkHoverBackColor = Color.FromArgb(239, 246, 255);
        private static readonly Color LinkHoverColor = Color.FromArgb(29, 78, 216);
        private static readonly Color ActiveTextColor = Color.FromArgb(31, 41, 55);
        private static readonly Color ActiveBadgeBackColor = Color.FromArgb(237, 242, 247);
        private static readonly Color ActiveBadgeBorderColor = Color.FromArgb(203, 213, 225);
        private static readonly Color SeparatorColor = Color.FromArgb(156, 163, 175);

        private static readonly Font LinkFont = new("Segoe UI", 9.75f, FontStyle.Regular);
        private static readonly Font ActiveFont = new("Segoe UI", 9.75f, FontStyle.Bold);
        private static readonly Font SeparatorFont = new("Segoe UI", 9f, FontStyle.Regular);

        private readonly FlowLayoutPanel _flow;

        public event EventHandler<BreadcrumbClickEventArgs>? ItemClick;

        public BreadcrumbControl()
        {
            AutoSize = true;
            MinimumSize = new Size(160, 40);
            Height = 40;

            UseModernStyle = true;
            BorderRadius = 8;
            BorderWidth = 1;
            BorderColor = BorderLineColor;
            ShowShadow = false;
            BackColor = BackgroundColor;
            Padding = new Padding(10, 6, 10, 6);

            _flow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(2, 0, 2, 0),
                BackColor = BackgroundColor
            };

            Controls.Add(_flow);
        }

        public void Render(IReadOnlyList<BreadcrumbSegment> segments, string separator)
        {
            _flow.SuspendLayout();
            _flow.Controls.Clear();

            if (segments is null || segments.Count == 0)
            {
                _flow.ResumeLayout(true);
                return;
            }

            var sep = string.IsNullOrEmpty(separator) ? "›" : separator;

            for (var i = 0; i < segments.Count; i++)
            {
                if (i > 0)
                    _flow.Controls.Add(CreateSeparator(sep));

                var segment = segments[i];
                _flow.Controls.Add(segment.IsActive
                    ? CreateActiveBadge(segment.Label)
                    : CreateLinkBadge(segment, i));
            }

            _flow.ResumeLayout(true);
            Invalidate();
        }

        private static Control CreateSeparator(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                ForeColor = SeparatorColor,
                BackColor = BackgroundColor,
                Font = SeparatorFont,
                Margin = new Padding(4, 6, 4, 6),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private static Control CreateActiveBadge(string text)
        {
            var badge = CreateBadgePanel(ActiveBadgeBackColor, ActiveBadgeBorderColor, Cursors.Default);
            var label = new Label
            {
                AutoSize = true,
                Text = text,
                ForeColor = ActiveTextColor,
                BackColor = ActiveBadgeBackColor,
                Font = ActiveFont,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                TextAlign = ContentAlignment.MiddleLeft
            };
            badge.Controls.Add(label);
            badge.Size = MeasureBadge(badge, label);
            return badge;
        }

        private Control CreateLinkBadge(BreadcrumbSegment segment, int index)
        {
            var badge = CreateBadgePanel(Color.Transparent, Color.Transparent, Cursors.Hand);
            var link = new LinkLabel
            {
                AutoSize = true,
                Text = segment.Label,
                LinkColor = LinkColor,
                ActiveLinkColor = LinkHoverColor,
                VisitedLinkColor = LinkColor,
                LinkBehavior = LinkBehavior.HoverUnderline,
                BackColor = Color.Transparent,
                Font = LinkFont,
                Margin = Padding.Empty,
                Cursor = Cursors.Hand,
                Tag = index
            };

            link.LinkClicked += (_, _) =>
            {
                ItemClick?.Invoke(this, new BreadcrumbClickEventArgs(index, segment.Label, segment.Href));
            };

            badge.Controls.Add(link);
            badge.Size = MeasureBadge(badge, link);

            void SetHover(bool hovered)
            {
                var back = hovered ? LinkHoverBackColor : Color.Transparent;
                var border = hovered ? Color.FromArgb(191, 219, 254) : Color.Transparent;
                badge.BackColor = back;
                link.BackColor = back;
                badge.Invalidate();
                ApplyBadgeBorder(badge, border);
            }

            badge.MouseEnter += (_, _) => SetHover(true);
            badge.MouseLeave += (_, _) => SetHover(false);
            link.MouseEnter += (_, _) => SetHover(true);
            link.MouseLeave += (_, _) => SetHover(false);

            return badge;
        }

        private static Panel CreateBadgePanel(Color backColor, Color borderColor, Cursor cursor)
        {
            var badge = new Panel
            {
                AutoSize = false,
                BackColor = backColor,
                Margin = new Padding(0, 2, 0, 2),
                Padding = new Padding(8, 4, 8, 4),
                Cursor = cursor
            };
            ApplyBadgeBorder(badge, borderColor);
            return badge;
        }

        private static void ApplyBadgeBorder(Panel badge, Color borderColor)
        {
            badge.Paint -= BadgePaint;
            badge.Tag = borderColor;
            if (borderColor.A > 0)
                badge.Paint += BadgePaint;
            badge.Invalidate();
        }

        private static void BadgePaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel badge || badge.Tag is not Color borderColor || borderColor.A == 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, badge.Width - 1, badge.Height - 1);
            using var path = CreateRoundedRect(rect, 6);
            using var pen = new Pen(borderColor, 1f);
            e.Graphics.DrawPath(pen, path);
        }

        private static Size MeasureBadge(Panel badge, Control inner)
        {
            var w = inner.PreferredSize.Width + badge.Padding.Horizontal + 2;
            var h = Math.Max(inner.PreferredSize.Height + badge.Padding.Vertical + 2, 24);
            inner.Location = new Point(badge.Padding.Left, (h - inner.Height) / 2);
            return new Size(w, h);
        }

        private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
