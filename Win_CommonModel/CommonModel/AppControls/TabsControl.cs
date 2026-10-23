
using MaterialSkin.Controls;

namespace WinFormsApp.AppComponents.AppControls
{
    public class TabsControl : ControlBase
    {
        private readonly TabControl _tabs;

        public TabsControl()
        {
            var content = new MaterialCard() { Dock = DockStyle.Fill, Margin = new(0, 0, 0, 0), Padding = new(0, 0, 0, 0) };
            Margin = new(0, 0, 0, 0);
            Padding = new(0, 0, 0, 0);
            this.LayoutFill(content);
            content.LayoutFill(_tabs = new TabControl() { Margin = new(0, 0, 0, 0), Padding = new(0) });
        }
        public void AddTab(string key, Control ctrl)
        {
            _tabs.AddTab(key, ctrl);
        }

        public void RemoveTab(string key)
        {
            _tabs.RemoveTabs(key);
        }
    }
}
