using WinFormsApp.AppViews.SideModule;

namespace WinFormsDesignerApp
{
    partial class SidePanelForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            materialCard1 = new MaterialSkin.Controls.MaterialCard();
            cardContentPanel = new Panel();
            panel1 = new Panel();
            centerPanel = new Panel();
            dataGridView1 = new DataGridView();
            leftPanel = new Panel();
            rightPanel = new Panel();
            checkedListBox1 = new CheckedListBox();
            bottomPanel = new Panel();
            progressBar = new MaterialSkin.Controls.MaterialProgressBar();
            topPanel = new Panel();
            sidePanel1 = new ContentPanel();
            materialButton1 = new MaterialSkin.Controls.MaterialButton();
            materialCard1.SuspendLayout();
            cardContentPanel.SuspendLayout();
            panel1.SuspendLayout();
            centerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            leftPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // materialCard1
            // 
            materialCard1.BackColor = Color.FromArgb(255, 255, 255);
            materialCard1.Controls.Add(cardContentPanel);
            materialCard1.Depth = 0;
            materialCard1.Dock = DockStyle.Fill;
            materialCard1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialCard1.Location = new Point(23, 84);
            materialCard1.Margin = new Padding(14);
            materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(14);
            materialCard1.Size = new Size(706, 693);
            materialCard1.TabIndex = 0;
            // 
            // cardContentPanel
            // 
            cardContentPanel.Controls.Add(panel1);
            cardContentPanel.Controls.Add(bottomPanel);
            cardContentPanel.Controls.Add(topPanel);
            cardContentPanel.Dock = DockStyle.Fill;
            cardContentPanel.Location = new Point(14, 14);
            cardContentPanel.Margin = new Padding(0);
            cardContentPanel.Name = "cardContentPanel";
            cardContentPanel.Size = new Size(678, 665);
            cardContentPanel.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(centerPanel);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 25);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(678, 600);
            panel1.TabIndex = 4;
            // 
            // centerPanel
            // 
            centerPanel.Controls.Add(dataGridView1);
            centerPanel.Controls.Add(leftPanel);
            centerPanel.Controls.Add(rightPanel);
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.Location = new Point(0, 0);
            centerPanel.Name = "centerPanel";
            centerPanel.Size = new Size(674, 596);
            centerPanel.TabIndex = 5;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(200, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(274, 596);
            dataGridView1.TabIndex = 4;
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(materialButton1);
            leftPanel.Controls.Add(sidePanel1);
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(200, 596);
            leftPanel.TabIndex = 1;
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(checkedListBox1);
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Location = new Point(474, 0);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(200, 596);
            rightPanel.TabIndex = 3;
            // 
            // checkedListBox1
            // 
            checkedListBox1.Dock = DockStyle.Fill;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(0, 0);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(200, 596);
            checkedListBox1.TabIndex = 0;
            // 
            // bottomPanel
            // 
            bottomPanel.BorderStyle = BorderStyle.Fixed3D;
            bottomPanel.Controls.Add(progressBar);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 625);
            bottomPanel.Margin = new Padding(0);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(678, 40);
            bottomPanel.TabIndex = 3;
            // 
            // progressBar
            // 
            progressBar.Depth = 0;
            progressBar.Dock = DockStyle.Bottom;
            progressBar.Location = new Point(0, 31);
            progressBar.MouseState = MaterialSkin.MouseState.HOVER;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(674, 5);
            progressBar.TabIndex = 0;
            // 
            // topPanel
            // 
            topPanel.BorderStyle = BorderStyle.Fixed3D;
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(678, 25);
            topPanel.TabIndex = 0;
            // 
            // sidePanel1
            // 
            sidePanel1.BackColor = Color.FromArgb(205, 255, 255);
            sidePanel1.Location = new Point(3, 63);
            sidePanel1.Name = "sidePanel1";
            sidePanel1.Size = new Size(194, 166);
            sidePanel1.TabIndex = 1;
            // 
            // materialButton1
            // 
            materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.Location = new Point(14, 252);
            materialButton1.Margin = new Padding(4, 6, 4, 6);
            materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = Color.Empty;
            materialButton1.Size = new Size(158, 36);
            materialButton1.TabIndex = 2;
            materialButton1.Text = "materialButton1";
            materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = true;
            materialButton1.Click += materialButton1_Click;
            // 
            // SidePanelForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(752, 800);
            Controls.Add(materialCard1);
            Name = "SidePanelForm";
            Padding = new Padding(23, 84, 23, 23);
            Text = "Form1";
            Load += OnFormLoad;
            materialCard1.ResumeLayout(false);
            cardContentPanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            centerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            rightPanel.ResumeLayout(false);
            bottomPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private MaterialSkin.Controls.MaterialCard materialCard1;
        private Panel cardContentPanel;
        private Panel topPanel;
        private Panel panel1;
        private Panel bottomPanel;
        private Panel centerPanel;
        private DataGridView dataGridView1;
        private Panel leftPanel;
        private Panel rightPanel;
        private CheckedListBox checkedListBox1;
        private MaterialSkin.Controls.MaterialProgressBar progressBar;
        private ContentPanel sidePanel1;
        private MaterialSkin.Controls.MaterialButton materialButton1;
    }
}
