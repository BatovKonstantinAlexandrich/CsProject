using ActionModels;

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
using WinFormsApp.Icons;

namespace WinFormsApp.AppViewModules.CheckboxModule
{
    public partial class CheckboxControl : ControlBase
    {
      

        public CheckboxControl()
        {
            InitializeComponent();
            var manager = new IconsManager();
            Controls.Add(checkbox);
            IsChecked = checkbox.Checked;
            AutoSize = true;
            Resize += OnResized;
            this.Height = this.checkbox.Height;
            checkbox.Text = "test";
            checkbox.CheckedChanged += (s, e) => {
                OnCheckedChanged(IsChecked = checkbox.Checked);

            };
            checkbox.Image = manager.GetImageByName("application_xp.png");

            OnResized(this, new());        
        }

        public string Text { get => this.checkbox.Text; set => this.checkbox.Text = value; }
        public bool IsChecked { get => changeSupport.Get<bool>(nameof(IsChecked)); set => changeSupport.Set(nameof(IsChecked), value); }


        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<bool> OnCheckedChanged { get; set; } = text => { };

      

        private void OnResized(object sender, EventArgs e)
        {

            checkbox.Size = Size;
        }


    }
}
