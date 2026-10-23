namespace WinFormsApp.SharedViewCollections.CheckedListModule
{
    partial class CheckedListControl
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
            checkedListBox = new MaterialSkin.Controls.MaterialCheckedListBox();
            materialCheckbox1 = new MaterialSkin.Controls.MaterialCheckbox();
            materialCheckbox2 = new MaterialSkin.Controls.MaterialCheckbox();
            SuspendLayout();
            // 
            // checkedListBox
            // 
            checkedListBox.AutoScroll = true;
            checkedListBox.BackColor = SystemColors.Control;
            checkedListBox.Depth = 0;
            checkedListBox.Dock = DockStyle.Fill;
            checkedListBox.ImeMode = ImeMode.On;
            checkedListBox.Location = new Point(0, 0);
            checkedListBox.MouseState = MaterialSkin.MouseState.HOVER;
            checkedListBox.Name = "checkedListBox";
            checkedListBox.Size = new Size(297, 264);
            checkedListBox.Striped = false;
            checkedListBox.StripeDarkColor = Color.Empty;
            checkedListBox.TabIndex = 0;
            // 
            // materialCheckbox1
            // 
            materialCheckbox1.AutoSize = true;
            materialCheckbox1.Depth = 0;
            materialCheckbox1.Location = new Point(0, 0);
            materialCheckbox1.Margin = new Padding(0);
            materialCheckbox1.MouseLocation = new Point(-1, -1);
            materialCheckbox1.MouseState = MaterialSkin.MouseState.HOVER;
            materialCheckbox1.Name = "materialCheckbox1";
            materialCheckbox1.ReadOnly = false;
            materialCheckbox1.Ripple = true;
            materialCheckbox1.Size = new Size(10, 10);
            materialCheckbox1.TabIndex = 0;
            materialCheckbox1.Text = "materialCheckbox1";
            materialCheckbox1.UseVisualStyleBackColor = true;
            // 
            // materialCheckbox2
            // 
            materialCheckbox2.AutoSize = true;
            materialCheckbox2.Depth = 0;
            materialCheckbox2.Location = new Point(0, 0);
            materialCheckbox2.Margin = new Padding(0);
            materialCheckbox2.MouseLocation = new Point(-1, -1);
            materialCheckbox2.MouseState = MaterialSkin.MouseState.HOVER;
            materialCheckbox2.Name = "materialCheckbox2";
            materialCheckbox2.ReadOnly = false;
            materialCheckbox2.Ripple = true;
            materialCheckbox2.Size = new Size(10, 10);
            materialCheckbox2.TabIndex = 0;
            materialCheckbox2.Text = "materialCheckbox2";
            materialCheckbox2.UseVisualStyleBackColor = true;
            // 
            // CheckedListControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(checkedListBox);
            Name = "CheckedListControl";
            Size = new Size(297, 264);
            ResumeLayout(false);
        }

        #endregion

        public MaterialSkin.Controls.MaterialCheckedListBox checkedListBox;
        private MaterialSkin.Controls.MaterialCheckbox materialCheckbox1;
        private MaterialSkin.Controls.MaterialCheckbox materialCheckbox2;
    }
}
