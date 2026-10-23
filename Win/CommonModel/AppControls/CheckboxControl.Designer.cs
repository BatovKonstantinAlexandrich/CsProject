namespace WinFormsApp.AppViewModules.CheckboxModule
{
    partial class CheckboxControl
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
            checkbox = new MaterialSkin.Controls.MaterialCheckbox();
            SuspendLayout();
            // 
            // checkbox
            // 
            checkbox.AutoSize = true;
            checkbox.Depth = 0;
            checkbox.Dock = DockStyle.Fill;
            checkbox.Location = new Point(0, 0);
            checkbox.Margin = new Padding(0);
            checkbox.MouseLocation = new Point(-1, -1);
            checkbox.MouseState = MaterialSkin.MouseState.HOVER;
            checkbox.Name = "checkbox";
            checkbox.ReadOnly = false;
            checkbox.Ripple = true;
            checkbox.Size = new Size(179, 38);
            checkbox.TabIndex = 0;
            checkbox.Text = "materialCheckbox1";
            checkbox.UseVisualStyleBackColor = true;
            // 
            // CheckboxControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(checkbox);
            Name = "CheckboxControl";
            Size = new Size(179, 38);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public MaterialSkin.Controls.MaterialCheckbox checkbox;
    }
}
