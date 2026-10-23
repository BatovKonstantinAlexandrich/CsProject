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

namespace MaterialApp.Win_MaterialModule.DialogModule
{
    public partial class SubmitDialogForm : MaterialForm
    {
        public SubmitDialogForm(string title )
        {
            InitializeComponent();
            this.Text = title;
        }
        public SubmitDialogForm(string title, Control content, Action<bool> complete)
        {
            InitializeComponent();
            this.Text = title;
            content.Dock = DockStyle.Fill;
            this.contentPanel.LayoutFill(content);
            SetCompleteAction(complete);
        }
        
        public void SetContentPanel(Control content)
        {
            content.Dock = DockStyle.Fill;
            this.contentPanel.Controls.Clear();
            this.contentPanel.Controls.Add(content);
        }
        public void SetCompleteAction(Action<bool> complete)
        {
            if (complete is not null)
            {
                this.materialButton1.Click += (sender, evt) => complete(true);
                this.materialButton2.Click += (sender, evt) => complete(false);

            }
        }

        private void SubmitDialogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
