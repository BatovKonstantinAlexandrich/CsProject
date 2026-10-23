public static class SplitExtensions
{
    public static SplitContainer Split(this Control panel, Control left, Control right)
    {
        var split = new SplitContainer();
        right.Anchor = left.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        right.Dock = left.Dock = DockStyle.Fill;
        split.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        split.Location = new System.Drawing.Point(0, 0);
        split.Dock = DockStyle.Fill;
        split.Panel1.Controls.Add(left);
        split.Panel2.Controls.Add(right);
        split.Panel2.AutoScroll = true;
        panel.Controls.Clear();
        panel.Controls.Add(split);
        return split;
    }

    public static SplitContainer Split(this Form panel, Control left, Control right)
    {
        var split = new SplitContainer();
        right.Anchor = left.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        right.Dock = left.Dock = DockStyle.Fill;
        split.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        split.Location = new System.Drawing.Point(0, 0);
        split.Dock = DockStyle.Fill;
        split.Panel1.Controls.Add(left);
        split.Panel2.Controls.Add(right);
        panel.Controls.Clear();
        panel.Controls.Add(split);
        return split;
    }

    public static SplitContainer SplitVertical(this Panel panel, Control left, Control right)
    {       
        var split = new SplitContainer();
        split.Orientation = Orientation.Vertical;
        right.Anchor = left.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        right.Dock = left.Dock = DockStyle.Fill;
        split.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        split.Location = new System.Drawing.Point(0, 0);
        split.Dock = DockStyle.Fill;
        split.Panel1.Controls.Add(left);
        split.Panel2.Controls.Add(right);
        panel.Controls.Clear();
        panel.Controls.Add(split);
        return split;
    }
}