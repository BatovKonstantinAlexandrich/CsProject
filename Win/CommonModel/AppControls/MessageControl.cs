using WinFormsApp.AppComponents.AppControls;

namespace WinForms
{

    /// <summary>
    /// Вывод сообщения в отдельной форме.
    /// </summary>
    public partial class MessageControl : ControlBase
    {
        public string Title { get => this.Message; set => this.Message = value; }
        public string Chapter { get => label1.Text; set => label1.Text = value; }
        public string Message { get => richTextBox1.Text; set => richTextBox1.Text = value; }

        public MessageControl()
        {
            InitializeComponent();
        }

        private void СloseButtonClicked(object sender, EventArgs e)
        {
            this.GetAppForm().Close();
        }
    }
}
