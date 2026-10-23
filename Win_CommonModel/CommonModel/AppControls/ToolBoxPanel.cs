using System;
using System.Windows.Forms;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ToolBoxPanel : Panel
    {
        private readonly ControlsFactory materialComponentsFactory = new();
        public void AddButton(string text, Action onclick)
        {
            this.LayoutRight(new Panel() { Width = 10 });
            this.LayoutRight(materialComponentsFactory.CreateButton(text, onclick));
        }
    }
}
