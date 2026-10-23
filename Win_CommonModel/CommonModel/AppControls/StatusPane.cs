using MaterialSkin.Controls;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppForms;

namespace WinFormsApp.AppViews.StatusModule
{
    public partial class StatusPane : ProgramBackgroundControl
    {
        private readonly MaterialProgressBar materialProgressBar1;
        private readonly ProgramBackgroundControl background = new();


        

        public StatusPane()
        {
            InitializeComponent();
            this.AutoSize = true;   
            this.LayoutBottom(this.materialProgressBar1 = new MaterialProgressBar());
            this.materialProgressBar1.Value = 100;
            bool active = true;
            Action next = null;
            next = () => {
                if (this.materialProgressBar1.Value == 100)
                {
                    this.materialProgressBar1.Value = 0;
                }
                else
                {
                    this.materialProgressBar1.Value++;
                }                
                if(active)
                {
                    this.PutAction("Ожидание", next);
                }
            };
            this.PutAction("Ожидание", next);
            this.StartAsync();
        }
    }
}
