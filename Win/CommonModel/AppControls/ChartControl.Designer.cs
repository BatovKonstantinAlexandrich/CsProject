namespace WinFormsDesignerApp
{
    partial class ChartControl
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
            //cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            SuspendLayout();
            // 
            // cartesianChart1
            // 
            //cartesianChart1.Location = new Point(29, 57);
            //cartesianChart1.Name = "cartesianChart1";
            ////cartesianChart1.Size = new Size(200, 100);
            //cartesianChart1.TabIndex = 0;
            //cartesianChart1.Text = "cartesianChart1";
            // 
            // ChartControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cartesianChart1);
            Name = "ChartControl";
            Size = new Size(267, 230);
            ResumeLayout(false);
        }

        #endregion

        private LiveCharts.WinForms.CartesianChart cartesianChart1;
    }
}
