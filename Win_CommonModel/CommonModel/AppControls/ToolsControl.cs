using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ToolsControl : ControlBase
    {
        private readonly ToolStripContainer toolStripContainer1;
        private readonly ToolStrip toolStrip1;

        public ToolsControl()
        {
            Height = 30;
            AutoSize = true;

            toolStripContainer1 = new ToolStripContainer();
            toolStrip1 = new ToolStrip();

            toolStrip1.Items.Add("One");
            toolStrip1.Items.Add("Two");
            toolStrip1.Items.Add("Three");

            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);

            this.LayoutFill(toolStripContainer1);


        }

        public void Add(ToolStripItem item)
        {
            toolStrip1.Items.Add(item);
        }
    }
}
