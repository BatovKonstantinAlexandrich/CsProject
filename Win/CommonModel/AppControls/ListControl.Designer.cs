using System.Text.Json.Serialization;

namespace WinFormsApp.AppViews.ListModule
{
    partial class ListViewControl
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
            list = new MaterialSkin.Controls.MaterialListBox();
            SuspendLayout();
            // 
            // materialListView1
            // 
          
            list.BackColor = Color.FromArgb(255, 255, 255);
      
            list.Depth = 0;
            list.Dock = DockStyle.Fill;
         
            list.Location = new Point(0, 0);
            list.MinimumSize = new Size(200, 100);
           
            list.MouseState = MaterialSkin.MouseState.OUT;
            list.Name = "materialListView1";
        
            list.Size = new Size(256, 209);
            list.TabIndex = 0;
        
            // 
            // ListViewControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(list);
            Name = "ListViewControl";
            Size = new Size(256, 209);
            ResumeLayout(false);
        }

        #endregion

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public MaterialSkin.Controls.MaterialListBox list;
    }
}
