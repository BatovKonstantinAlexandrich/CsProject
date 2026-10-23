using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.LineModule;

using WinFormsApp.AppComponents.AppControls;

using static WinFormsApp.AppViews.LineModule.LineView;
using System.Windows.Forms;

namespace WinFormsApp.AppComponents.AppControls
{
    public class LineViewControl : ControlBase
    {

        private readonly LineView View;
        private TableLayoutPanel tableLayout;

        public LineViewControl()
        {
            View = new();
            this.LayoutFill(tableLayout = new TableLayoutPanel()
            {
                 
            });
             
            UpdateLineView();
        }
        public LineViewControl(LineView view)
        {
            View = view;
            tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
                Margin = Padding.Empty,
            };
            Controls.Add(tableLayout);
            tableLayout.Dock = DockStyle.Fill;

            void RefreshLayout()
            {
                if (View.InLine)
                    UpdateLineView();
                else
                    UpdateColumnView();
            }

            RefreshLayout();
            View.lineViewLayouts.OnChanged += (_, _) => RefreshLayout();
            View.AddPropertyChangeListener(nameof(View.InLine), _ => RefreshLayout());
            Resize += (_, _) => RefreshLayout();
        }

        private static void ConfigureCellControl(Control ctrl, bool fill)
        {
            ctrl.Margin = Padding.Empty;
            ctrl.Dock = fill ? DockStyle.Fill : DockStyle.None;
            if (fill && ctrl is ControlBase cb)
                cb.AutoSize = false;
        }

        public void UpdateLineView()
        {
            tableLayout.SuspendLayout();
            tableLayout.Controls.Clear();
            tableLayout.ColumnStyles.Clear();
            tableLayout.RowStyles.Clear();

            var items = View.lineViewLayouts.ToList();
            tableLayout.ColumnCount = Math.Max(items.Count, 1);
            tableLayout.RowCount = 1;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var fillCount = items.Count(i => i.fill);
            foreach (var layout in items)
            {
                if (layout.fill)
                    tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, fillCount > 0 ? 100F / fillCount : 100F));
                else
                    tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            for (var x = 0; x < items.Count; x++)
            {
                var layout = items[x];
                var ctrl = layout.view.ToControl();
                ConfigureCellControl(ctrl, layout.fill);
                tableLayout.Controls.Add(ctrl, x, 0);
            }

            tableLayout.ResumeLayout(true);
        }

        public void UpdateColumnView()
        {
            tableLayout.SuspendLayout();
            tableLayout.Controls.Clear();
            tableLayout.ColumnStyles.Clear();
            tableLayout.RowStyles.Clear();

            var items = View.lineViewLayouts.ToList();
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = Math.Max(items.Count, 1);
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var fillCount = items.Count(i => i.fill);
            foreach (var layout in items)
            {
                if (layout.fill)
                    tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, fillCount > 0 ? 100F / fillCount : 100F));
                else
                    tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (var y = 0; y < items.Count; y++)
            {
                var layout = items[y];
                var ctrl = layout.view.ToControl();
                ConfigureCellControl(ctrl, layout.fill);
                tableLayout.Controls.Add(ctrl, 0, y);
            }

            tableLayout.ResumeLayout(true);
        }

    }
}
