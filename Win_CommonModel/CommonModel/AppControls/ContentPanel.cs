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

namespace WinFormsApp.AppViews.SideModule
{
    /// <summary>
    /// Управление контентом выдвигающимися панелями слева, справа сверху с боку
    /// </summary>
    public partial class ContentPanel : ControlBase
    {
        public ContentPanel()
        {
            InitializeComponent();
            ComponentsExtensions.LayoutFill(this, View);
            this.LayoutBottom(Bottom);
            this.LayoutTop(Top);
            this.LayoutRight(Right);
            this.LayoutLeft(Left);
            View.SetVerticalScroll();
        }

        public ColumnsPanel Left = new ColumnsPanel() { Dock = DockStyle.Fill };
        public ColumnsPanel Right = new ColumnsPanel() { Dock = DockStyle.Fill };
        public ColumnsPanel Top = new ColumnsPanel() { Dock = DockStyle.Fill, Size = new(30, 30) };
        public ColumnsPanel Bottom = new ColumnsPanel() { Dock = DockStyle.Fill, Size = new(30, 30) };
        public ColumnsPanel View = new ColumnsPanel() { Dock = DockStyle.Fill };

        public object Item { get; internal set; }
        public int topHeight = 30;
        public void ShowTop() {
            this.Top.Height = topHeight;
        }
        public void HideTop() {
            this.Top.Height = 0;
        }

        public void HideSides()
        {
            //this.leftPanel.AutoSize = this.rightPanel.AutoSize =
            //this.topPanel.AutoSize = this.bottomPanel.AutoSize = false;

            this.Left.Width = this.Right.Width = 0;
            this.Top.Height = this.Bottom.Height = 0;
        }

        public void SetView(Control ctrl)
        {
            View.Controls.Clear();
            View.LayoutFill(ctrl);
        }

        public void SetTop(Control ctrl)
        {
            this.Top.Clear();          
            this.Top.Add(ctrl);
            ctrl.Dock = DockStyle.Top;
        }
        public void SetLeft(Control ctrl)
        {
            ctrl.Dock= DockStyle.Fill;
            this.Left.Clear();
            var width = ctrl.Width;
            this.Left.Add(ctrl);
            this.Left.Width = (width + 100);
        }
        public void SetRight(Control ctrl)
        {
            ctrl.Dock = DockStyle.Fill;
            this.Right.Clear();
            var width = ctrl.Width;
            this.Right.Add(ctrl);
            this.Right.Width = (width + 100);
        }
        public void SetBottom(Control ctrl)
        {
            this.Bottom.Clear();
            var height = ctrl.Height;
            this.Bottom.Add(ctrl);
            this.Bottom.Height = (height + 20);
        }

        public void FillTop(Control ctrl)
        {

            this.Top.Clear();
            var height = ctrl.Height;
            this.Top.LayoutFill(ctrl);
            topHeight = this.Top.Height = height;

        }
        public void FillLeft(Control ctrl)
        {
            ctrl.Dock = DockStyle.Fill;
            this.Left.Clear();
            var width = ctrl.Width;
            this.Left.LayoutFill(ctrl);
            this.Left.Width = (width + 100);
        }
        public void FillRight(Control ctrl)
        {
            ctrl.Dock = DockStyle.Fill;
            this.Right.Clear();
            var width = ctrl.Width;
            this.Right.LayoutFill(ctrl);
            this.Right.Width = (width + 100);
        }
        public void FillBottom(Control ctrl)
        {
            this.Bottom.Clear();
            var height = ctrl.Height;
            this.Bottom.LayoutFill(ctrl);
            this.Bottom.Height = (height + 20);
        }

        private void ContentPanel_Load(object sender, EventArgs e)
        {

        }
        public void ToggleTop()
        {
            if (this.Top.AutoSize)
            {
                this.Top.AutoSize = false;
                this.Top.Height = 0;
            }
            else
            {
                this.Top.AutoSize = true;
            }
        }
        public void ToggleLeft()
        {
            if (this.Left.AutoSize)
            {
                this.Left.AutoSize = false;
                this.Left.Width = 0;
            }
            else
            {
                this.Left.AutoSize = true;
            }
        }
        public void ToggleRight()
        {
            if (this.Right.AutoSize)
            {
                this.Right.AutoSize = false;
                this.Right.Width = 0;
            }
            else
            {
                this.Right.AutoSize = true;
            }
        }
        public void ToggleBottom()
        {
            if(this.Bottom.AutoSize)
            {
                this.Bottom.AutoSize = false;
                this.Bottom.Height = 0;
            }
            else
            {
                this.Bottom.AutoSize = true;
            }
        }
        public void SetLeftWidth(int v)
        {
            this.Left.Width = v;
            
        }
        public void SetRightWidth(int v)
        {
            this.Right.Width = v;
        }
        public void SetTopHeight(int v)
        {
            this.Top.Height = v;
        }
        public void SetBottomHeight(int v)
        {
            this.Bottom.Height = v;
        }
 
    }
}
