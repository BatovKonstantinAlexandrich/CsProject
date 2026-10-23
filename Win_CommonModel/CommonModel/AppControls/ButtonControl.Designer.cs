namespace WinFormsApp.AppViewModules.ButtonModule
{
    partial class ButtonControl
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
            button = new MaterialSkin.Controls.MaterialButton();
            SuspendLayout();
            // 
            // button
            // 
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            button.Depth = 0;
            button.HighEmphasis = true;
            button.Icon = null;
            button.Location = new Point(0, 0);
            button.Margin = new Padding(4, 6, 4, 6);
            button.MouseState = MaterialSkin.MouseState.HOVER;
            button.Name = "button";
            button.NoAccentTextColor = Color.Empty;
            button.AutoSize = false;
            button.Dock = DockStyle.Fill;
            button.Size = new Size(158, 36);
            button.TabIndex = 0;
            button.Text = "materialButton1";
            button.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            button.UseAccentColor = false;
            button.UseVisualStyleBackColor = true;
            // 
            // ButtonControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = false;
            Controls.Add(button);
            Name = "ButtonControl";
            Size = new Size(162, 42);
            ResumeLayout(false);
            // без PerformLayout — иначе AutoSize/PreferredSize MaterialButton снова раздует контейнер
        }

        #endregion

        public MaterialSkin.Controls.MaterialButton button = new();
    }
}
