using System.Windows.Forms;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// WinForms <see cref="SplitContainer"/> для <see cref="SplitView"/>.
    /// </summary>
    public class SplitControl : ControlBase
    {
        private readonly SplitContainer _split;

        public SplitControl()
        {
            _split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 6, 
                Panel1MinSize = 25,
                Panel2MinSize = 25,
            };
            Controls.Add(_split);
        }

        public SplitContainer SplitContainer => _split;

        public void SetPanel1(Control? panel) => SetPanel(_split.Panel1, panel);

        public void SetPanel2(Control? panel) => SetPanel(_split.Panel2, panel);

        static void SetPanel(Panel host, Control? panel)
        {
            host.Controls.Clear();
            if (panel is null)
                return;

            panel.Dock = DockStyle.Fill;
            host.Controls.Add(panel);
        }

        /// <param name="horizontal">true — панели сверху и снизу; false — слева и справа.</param>
        public void SetOrientation(bool horizontal) =>
            _split.Orientation = horizontal ? Orientation.Horizontal : Orientation.Vertical;

        public void SetSplitterDistance(int distance)
        {
            if (distance <= 0)
                return;

            var max = _split.Orientation == Orientation.Vertical
                ? _split.Width - _split.Panel2MinSize - _split.SplitterWidth
                : _split.Height - _split.Panel2MinSize - _split.SplitterWidth;

            if (max < _split.Panel1MinSize)
                return;

            _split.SplitterDistance = Math.Max(_split.Panel1MinSize, Math.Min(distance, max));
        }

        public void SetSplitterWidth(int width) =>
            _split.SplitterWidth = Math.Max(1, width);

        public int GetSplitterDistance() => _split.SplitterDistance;
    }
}
