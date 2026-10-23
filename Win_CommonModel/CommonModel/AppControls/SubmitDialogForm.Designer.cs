

using WinFormsApp.AppViews.InputModule;

namespace MaterialApp.Win_MaterialModule.DialogModule
{
    partial class SubmitDialogControl
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
            contentPanel = new InputPanel(typeof(InputFormField), (s, m) => { 
            });
            materialButton1 = new MaterialSkin.Controls.MaterialButton() { Size = new(120, 33) };
            materialButton2 = new MaterialSkin.Controls.MaterialButton() { Size = new(120, 33) };
            SuspendLayout();
            // 
            // contentPanel
            // 
            contentPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            contentPanel.Location = new System.Drawing.Point(13, 81);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new System.Drawing.Size(348, 326);
            contentPanel.TabIndex = 0;
            // 
            // materialButton1
            // 
            materialButton1.AutoSize = false;
            materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.Location = new System.Drawing.Point(211, 422);
            materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            materialButton1.Size = new System.Drawing.Size(150, 50);
            materialButton1.TabIndex = 1;
            materialButton1.Text = "Отмена";
            materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = true;
            // 
            // materialButton2
            // 
            materialButton2.AutoSize = false;
            materialButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton2.Depth = 0;
            materialButton2.HighEmphasis = true;
            materialButton2.Icon = null;
            materialButton2.Location = new System.Drawing.Point(13, 422);
            materialButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton2.Name = "materialButton2";
            materialButton2.NoAccentTextColor = System.Drawing.Color.Empty;
            materialButton2.Size = new System.Drawing.Size(150, 50);
            materialButton2.TabIndex = 2;
            materialButton2.Text = "Готово";
            materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton2.UseAccentColor = false;
            materialButton2.UseVisualStyleBackColor = true;
            // 
            // SubmitDialogForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(374, 467);
            Controls.Add(materialButton1);
            Controls.Add(materialButton2);
            Controls.Add(contentPanel);
            Name = "SubmitDialogForm";
            Text = "SubmitDialogForm";
            Load += SubmitDialogForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private InputPanel contentPanel;
        protected MaterialSkin.Controls.MaterialButton materialButton1;
        protected MaterialSkin.Controls.MaterialButton materialButton2;
    }
}