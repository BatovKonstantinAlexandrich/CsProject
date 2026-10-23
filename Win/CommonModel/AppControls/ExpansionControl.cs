using MaterialSkin.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Win_CommonModel.AppExtensions;

using WinFormsApp.AppViews.ColumnsModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>WinForms-контрол для <see cref="ExpansionView"/> (MaterialCard).</summary>
    public class ExpansionControl : ControlBase, ILayoutControl
    {
        public MaterialCard materialCard { get; }

        public ExpansionControl()
        {
            this.LayoutFill(this.materialCard = new MaterialCard());
            this.BindMouseEvents(this.materialCard);
            this.AutoSize = true;
            this.materialCard.AutoSize = true;
        }

        public void Add(Control item)
        {
            this.materialCard.Controls.Add(item);
        }

        public void Remove(Control item)
        {
            this.materialCard.Controls.Remove(item);
        }
    }


    /// <summary>WinForms-контрол для <see cref="CardView"/> (MaterialExpansionPanel).</summary>
    public class CardControl : ControlBase, ILayoutControl
    {

        [Label("Развернуто")]
        public bool Expanded { get => Get<bool>("Expanded"); set => Set<bool>("Expanded", value); }
        public CardControl()
        {
            InitializeComponent();

            this.ControlAdded += CardControl_ControlAdded;
            this.AddPropertyChangeListener(nameof(Expanded), evt => {
                if ((bool)evt.After)
                {
                    this.materialExpansionPanel1.Collapse = true;
                }
                else
                {
                    this.materialExpansionPanel1.Collapse = false;
                }
            });
            materialExpansionPanel1.Expand();
            materialExpansionPanel1.PanelCollapse += (s, e) =>
            {
                
            };
            materialExpansionPanel1.Controls.Add(flowLayoutPanel1);
        }

        private void CardControl_ControlAdded(object sender, ControlEventArgs e)
        {
            if(e.Control != this.materialExpansionPanel1)
            {

            }
            this.materialExpansionPanel1.Controls.Add(e.Control);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardControl));
            materialExpansionPanel1 = new MaterialSkin.Controls.MaterialExpansionPanel();
            flexControl1 = new FlexControl();
            flowLayoutPanel1 = new FlowLayoutPanel();
            materialExpansionPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // materialExpansionPanel1
            // 
            materialExpansionPanel1.BackColor = Color.FromArgb(255, 255, 255);
            materialExpansionPanel1.Controls.Add(flowLayoutPanel1);
            materialExpansionPanel1.Depth = 0;
            materialExpansionPanel1.Dock = DockStyle.Fill;
            materialExpansionPanel1.ExpandHeight = 307;
            materialExpansionPanel1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialExpansionPanel1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialExpansionPanel1.Location = new Point(0, 0);
            materialExpansionPanel1.Margin = new Padding(3, 16, 3, 16);
            materialExpansionPanel1.MouseState = MaterialSkin.MouseState.HOVER;
            materialExpansionPanel1.Name = "materialExpansionPanel1";
            materialExpansionPanel1.Padding = new Padding(24, 64, 24, 16);
            materialExpansionPanel1.Size = new Size(405, 307);
            materialExpansionPanel1.TabIndex = 0;
            // 
            // flexControl1
            // 
            flexControl1.AutoSize = true;
            flexControl1.BackColor = Color.White;
            flexControl1.BorderColor = Color.FromArgb(220, 220, 220);
            flexControl1.BorderRadius = 8;
            flexControl1.BorderWidth = 1;
            flexControl1.Changed = false;
            flexControl1.Font = new Font("Segoe UI", 9F);
            flexControl1.ForeColor = Color.FromArgb(33, 33, 33);
            flexControl1.Location = new Point(0, 0);
            flexControl1.Name = "flexControl1";
            flexControl1.ShowShadow = true;
            flexControl1.Size = new Size(150, 150);
            flexControl1.TabIndex = 0;
            flexControl1.UseModernStyle = true;
            flexControl1.ValidateOnInput = false;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Location = new Point(27, 56);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(351, 170);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // CardControl
            // 
            Controls.Add(materialExpansionPanel1);
            Name = "CardControl";
            Size = new Size(405, 307);
            materialExpansionPanel1.ResumeLayout(false);
            materialExpansionPanel1.PerformLayout();
            ResumeLayout(false);
        }

        public MaterialSkin.Controls.MaterialExpansionPanel materialExpansionPanel1;
        public FlowLayoutPanel flowLayoutPanel1;
        public FlexControl flexControl1;

        public void Add(Control item)
        {
            flowLayoutPanel1.Controls.Add(item);
        }

        public void Clear()
        {
            flowLayoutPanel1.Controls.Clear();
        }

        public int Count()
        {
            return flowLayoutPanel1.Controls.Count;
        }

        public void Remove(Control item)
        {
            flowLayoutPanel1.Controls.Remove(item);
        }
    }
}
