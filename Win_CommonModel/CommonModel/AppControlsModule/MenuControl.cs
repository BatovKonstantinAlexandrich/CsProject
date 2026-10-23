using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppViewModules.MenuModule
{
    public partial class MenuControl : ControlBase
    {
        public List<int> path = new();


        public MenuControl()
        {
            InitializeComponent();
            this.AutoSize = true;
            this.ContextMenuStrip = new();

        }


        public void Clear()
        {
            menuStrip.Items.Clear();
        }


        public void AddMenu(ToolStripDropDownItem item)
        {
            menuStrip.Items.Add(item);
        }

        public void AddMenu(IEnumerable<ToolStripDropDownItem> items)
        {
            menuStrip.Items.AddRange(items.ToArray());
        }
    }
}
