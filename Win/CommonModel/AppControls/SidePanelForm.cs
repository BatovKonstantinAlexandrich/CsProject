using MaterialSkin.Controls;





namespace WinFormsDesignerApp
{
    public partial class SidePanelForm : MaterialForm
    {
        public SidePanelForm()
        {
            InitializeComponent();
        }


        private void Setup()
        {
            this.checkedListBox1.Padding = this.checkedListBox1.Margin =
            this.centerPanel.Padding = this.centerPanel.Margin =
            this.bottomPanel.Padding = this.bottomPanel.Margin =
            this.topPanel.Padding = this.topPanel.Margin =
            this.leftPanel.Padding = this.leftPanel.Margin =
            this.cardContentPanel.Padding = this.cardContentPanel.Margin =
            this.materialCard1.Padding = this.materialCard1.Margin =
            this.rightPanel.Padding = this.rightPanel.Margin =
            this.panel1.Padding = this.panel1.Margin = new(0, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormLoad(object sender, EventArgs e)
        {
            this.Text = e.GetType().Name;
            
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            var factory = new ContextMenuFactory();
            this.sidePanel1.ContextMenuStrip = factory.CreateContextMenu(
                new List<object>() { sidePanel1 }, 
                target => target.GetType().Name,
                target => target.GetType().GetMethods().Select(m => m.Name).ToList(),
                (target,action) => action,
                (target, action) =>
                {
                    var method = target.GetType().GetMethods().FirstOrDefault(m => m.Name == action);
                    var result = method.Invoke(target, new object[] { });
                    return result;
                }
            );
            
        }
    }
}
