using MaterialSkin.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ComboboxControl : ControlBase
    {

        public ChangingList<string> Options { get; set; } = new();


        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<string> OnSelected { get; set; }


        public ComboboxControl()
        {
            InitializeComponent();
            MinimumSize = _comboBox.Size;
            AutoSize = true;
            Options.OnChanged += OnOptionsChanged;
            OnSelected = text => { };
            this.LayoutFill(_comboBox);
            _comboBox.SelectedValueChanged += (s, e) =>
            {

                OnSelected(_comboBox.SelectedIndex >= 0 && _comboBox.SelectedIndex <= Options.Count() - 1 ? Options[_comboBox.SelectedIndex] : null);
            };
        }

        private void OnOptionsChanged(object sender, IChangingListEvent e)
        {
            switch (e.Target.Action)
            {
                case "added": _comboBox.Items.Add(e.Target.Item); break;
                case "removed": _comboBox.Items.Remove(e.Target.Item); break;
            }
        }


        public void SetText(string text) => _comboBox.Text = text;

        private void InitializeComponent()
        {
            _comboBox = new MaterialComboBox();
            SuspendLayout();
            // 
            // _comboBox
            // 
            _comboBox.AutoResize = false;
            _comboBox.BackColor = Color.FromArgb(255, 255, 255);
            _comboBox.Depth = 0;
            _comboBox.Dock = DockStyle.Top;
            _comboBox.DrawMode = DrawMode.OwnerDrawVariable;
            _comboBox.DropDownHeight = 174;
            _comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _comboBox.DropDownWidth = 121;
            _comboBox.Font = new Font("Roboto Medium", 14F, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);
            _comboBox.ForeColor = Color.FromArgb(222, 0, 0, 0);
            _comboBox.FormattingEnabled = true;
            _comboBox.IntegralHeight = false;
            _comboBox.ItemHeight = 43;
            _comboBox.Location = new System.Drawing.Point(0, 0);
            _comboBox.MaxDropDownItems = 4;
            _comboBox.MouseState = MaterialSkin.MouseState.OUT;
            _comboBox.Name = "_comboBox";
            _comboBox.Size = new System.Drawing.Size(150, 49);
            _comboBox.StartIndex = 0;
            _comboBox.TabIndex = 0;
            // 
            // ComboboxControl
            // 
            AutoSize = true;
            Controls.Add(_comboBox);
            Name = "ComboboxControl";
            Size = new System.Drawing.Size(150, 51);
            ResumeLayout(false);
        }

        internal void SetSelectedIndex(int selectedIndex)
        {
            if (selectedIndex <= _comboBox.Items.Count && selectedIndex >= 0)
            {
                _comboBox.SelectedIndex = selectedIndex;
            }
        }

        public MaterialComboBox _comboBox;
        private MaterialComboBox materialComboBox1;
    }
}
