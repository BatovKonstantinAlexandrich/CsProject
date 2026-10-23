namespace WinFormsApp.ClassViewModel
{
    partial class ClassModelControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            treeView1 = new TreeView();
            materialExpansionPanel1 = new MaterialSkin.Controls.MaterialExpansionPanel();
            materialExpansionPanel2 = new MaterialSkin.Controls.MaterialExpansionPanel();
            materialExpansionPanel3 = new MaterialSkin.Controls.MaterialExpansionPanel();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeView1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(materialExpansionPanel3);
            splitContainer1.Panel2.Controls.Add(materialExpansionPanel2);
            splitContainer1.Panel2.Controls.Add(materialExpansionPanel1);
            splitContainer1.Size = new Size(455, 442);
            splitContainer1.SplitterDistance = 151;
            splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(151, 442);
            treeView1.TabIndex = 0;
            // 
            // materialExpansionPanel1
            // 
            materialExpansionPanel1.BackColor = Color.FromArgb(255, 255, 255);
            materialExpansionPanel1.Collapse = true;
            materialExpansionPanel1.Depth = 0;
            materialExpansionPanel1.Dock = DockStyle.Top;
            materialExpansionPanel1.ExpandHeight = 200;
            materialExpansionPanel1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialExpansionPanel1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialExpansionPanel1.Location = new Point(0, 0);
            materialExpansionPanel1.Margin = new Padding(16, 1, 16, 0);
            materialExpansionPanel1.MouseState = MaterialSkin.MouseState.HOVER;
            materialExpansionPanel1.Name = "materialExpansionPanel1";
            materialExpansionPanel1.Padding = new Padding(24, 64, 24, 16);
            materialExpansionPanel1.Size = new Size(300, 48);
            materialExpansionPanel1.TabIndex = 0;
            // 
            // materialExpansionPanel2
            // 
            materialExpansionPanel2.BackColor = Color.FromArgb(255, 255, 255);
            materialExpansionPanel2.Collapse = true;
            materialExpansionPanel2.Depth = 0;
            materialExpansionPanel2.Dock = DockStyle.Top;
            materialExpansionPanel2.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialExpansionPanel2.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialExpansionPanel2.Location = new Point(0, 48);
            materialExpansionPanel2.Margin = new Padding(16, 1, 16, 0);
            materialExpansionPanel2.MouseState = MaterialSkin.MouseState.HOVER;
            materialExpansionPanel2.Name = "materialExpansionPanel2";
            materialExpansionPanel2.Padding = new Padding(24, 64, 24, 16);
            materialExpansionPanel2.Size = new Size(300, 48);
            materialExpansionPanel2.TabIndex = 1;
            // 
            // materialExpansionPanel3
            // 
            materialExpansionPanel3.BackColor = Color.FromArgb(255, 255, 255);
            materialExpansionPanel3.Collapse = true;
            materialExpansionPanel3.Depth = 0;
            materialExpansionPanel3.Dock = DockStyle.Top;
            materialExpansionPanel3.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialExpansionPanel3.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialExpansionPanel3.Location = new Point(0, 96);
            materialExpansionPanel3.Margin = new Padding(16, 1, 16, 0);
            materialExpansionPanel3.MouseState = MaterialSkin.MouseState.HOVER;
            materialExpansionPanel3.Name = "materialExpansionPanel3";
            materialExpansionPanel3.Padding = new Padding(24, 64, 24, 16);
            materialExpansionPanel3.Size = new Size(300, 48);
            materialExpansionPanel3.TabIndex = 2;
            // 
            // ClassModelControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "ClassModelControl";
            Size = new Size(455, 442);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        protected SplitContainer splitContainer1;
        protected TreeView treeView1;
        protected MaterialSkin.Controls.MaterialExpansionPanel materialExpansionPanel2;
        protected MaterialSkin.Controls.MaterialExpansionPanel materialExpansionPanel1;
        protected MaterialSkin.Controls.MaterialExpansionPanel materialExpansionPanel3;
    }
}
