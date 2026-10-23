using MaterialSkin.Controls;

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

namespace WinFormsApp.AppViews.ListModule
{
    public partial class ListViewControl : ControlBase
    {
        public readonly MaterialLabel titleText;

        public ListViewControl()
        {
            InitializeComponent();
            this.LayoutTop(this.titleText = new MaterialLabel());
            this.titleText.Margin = new(10);
            this.titleText.TextAlign = ContentAlignment.MiddleCenter;
            this.titleText.Font = new Font(this.titleText.Font.Name, 28);
            this.MinimumSize = new(320, 320);
            this.AutoSize = true;                       
            this.SetVerticalScroll();            
        }
    }
}
