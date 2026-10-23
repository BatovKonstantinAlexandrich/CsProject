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

namespace WinFormsApp.AppViews.CalendarModule
{
    public partial class DateTimeControl : ControlBase

    {
        public DateTimeControl()
        {
            InitializeComponent();
            this.MinimumSize = this.dateTimePicker1.Size;
            this.AutoSize = true;            
        }
    }
}
