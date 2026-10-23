using WinFormsApp.AppComponents.AppControls;

namespace WinFormsDesignerApp.WinModule.InputModule
{
    partial class InputFormControl
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
            columnsPanel = new ColumnsPanel();
            SuspendLayout();
            // 
            // columnsPanel
            // 
            columnsPanel.AutoScroll = true;
            columnsPanel.BackColor = Color.FromArgb(255, 255, 255);
            columnsPanel.BorderStyle = BorderStyle.Fixed3D;
            columnsPanel.Depth = 0;
            columnsPanel.Dock = DockStyle.Fill;
            columnsPanel.Fixed = false;
            columnsPanel.ForeColor = Color.FromArgb(222, 0, 0, 0);
            columnsPanel.Location = new Point(0, 0);
            columnsPanel.Margin = new Padding(14);
            columnsPanel.Mode = false;
            columnsPanel.MouseState = MaterialSkin.MouseState.HOVER;
            columnsPanel.Name = "columnsPanel";
            columnsPanel.Padding = new Padding(14);
            columnsPanel.Size = new Size(252, 243);
            columnsPanel.TabIndex = 0;
            // 
            // InputFormControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(columnsPanel);
            Name = "InputFormControl";
            Size = new Size(252, 243);
            ResumeLayout(false);
        }

        #endregion

        private ColumnsPanel columnsPanel;
    }
}
