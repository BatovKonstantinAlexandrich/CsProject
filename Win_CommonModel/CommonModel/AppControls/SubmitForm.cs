using MaterialSkin.Controls;
using System;
using System.Drawing;

namespace WinForms
{

    /// <summary>
    /// Вывод сообщения в отдельной форме для подтверждения пользователем.
    /// </summary>
    public partial class SubmitForm : MaterialForm
    {
        private readonly Action<bool, string> completed;

        public string Title { get => this.Message; set => this.Message = value; }
        public string Chapter { get => this.Text; set => this.Text = value; }
        public string Message { get => richTextBox1.Text; set => richTextBox1.Text = value; }

        
        public SubmitForm(Action<bool, string> completed)
        {
            InitializeComponent();
            this.completed = completed;
            this.MaximumSize = this.MinimumSize;
            this.Resize += (s, e) => this.MaximumSize = this.MinimumSize = new Size(464, 358);
            button1.Click += OnSubmitButtonClicked;
            this.richTextBox1.Enabled = false;
        }


        private void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            this.completed(true, Text);
            this.Close();
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            this.completed(false, Text);
            this.Close();
        }
    }
}
