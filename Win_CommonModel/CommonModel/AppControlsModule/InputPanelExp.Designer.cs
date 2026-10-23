namespace WinDesigner
{
    partial class InputPanelExp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            materialExpansionPanel1 = new MaterialSkin.Controls.MaterialExpansionPanel();
            materialCard1 = new MaterialSkin.Controls.MaterialCard();
            materialExpansionPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // materialExpansionPanel1
            // 
            materialExpansionPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            materialExpansionPanel1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            materialExpansionPanel1.CancelButtonText = "Отменить";
            materialExpansionPanel1.Controls.Add(materialCard1);
            materialExpansionPanel1.Depth = 0;
            materialExpansionPanel1.Description = "Редактирование персональных данных";
            materialExpansionPanel1.ExpandHeight = 552;
            materialExpansionPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            materialExpansionPanel1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
            materialExpansionPanel1.Location = new System.Drawing.Point(0, 0);
            materialExpansionPanel1.Margin = new System.Windows.Forms.Padding(3, 16, 3, 16);
            materialExpansionPanel1.MouseState = MaterialSkin.MouseState.HOVER;
            materialExpansionPanel1.Name = "materialExpansionPanel1";
            materialExpansionPanel1.Padding = new System.Windows.Forms.Padding(24, 64, 24, 16);
            materialExpansionPanel1.Size = new System.Drawing.Size(948, 552);
            materialExpansionPanel1.TabIndex = 0;
            materialExpansionPanel1.Title = "Учетные данные";
            materialExpansionPanel1.ValidationButtonText = "Ок";
            // 
            // materialCard1
            // 
            materialCard1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            materialCard1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            materialCard1.Depth = 0;
            materialCard1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
            materialCard1.Location = new System.Drawing.Point(23, 49);
            materialCard1.Margin = new System.Windows.Forms.Padding(14);
            materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new System.Windows.Forms.Padding(14);
            materialCard1.Size = new System.Drawing.Size(905, 416);
            materialCard1.TabIndex = 2;
            // 
            // InputPanelExp
            // 
            ClientSize = new System.Drawing.Size(951, 548);
            Controls.Add(materialExpansionPanel1);
            Name = "InputPanelExp";
            Text = "Form1";
            //Load += InputPanelExp_Load;
            materialExpansionPanel1.ResumeLayout(false);
            materialExpansionPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private MaterialSkin.Controls.MaterialExpansionPanel materialExpansionPanel1;
        private MaterialSkin.Controls.MaterialCard materialCard1;
    }
}