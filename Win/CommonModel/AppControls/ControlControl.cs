using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp.AppComponents.AppControls
{
    public partial class ControlControl : ControlBase, IControlView
    {

        [Label("Надпись")]
        [NotNullNotEmpty()]
        public virtual string Text { get => Get<string>("Text"); set => Set<string>("Text", value); }

        [InputBool]
        [Label("Включена/Выключена")]
        public virtual bool Enabled { get => Get<bool>("Enabled"); set => Set<bool>("Enabled", value); }


        public ControlControl()
        {
            InitializeComponent();
        }

    }
}
