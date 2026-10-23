using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Вертикальная прокрутка содержимого: внутренний FlowLayoutPanel растёт по высоте,
    /// иначе при Fill высота контента равна клиентской области и поля обрезаются.
    /// </summary>
    public class ScrollControl : ControlBase, ILayoutControl
    {
        private readonly Panel scrollHost;
        private readonly Control contentPanel;

        public Action<object, ScrollEventArgs> OnScrollChanged { get; set; } = (s, e) => { };


        public ScrollControl()
        {
            Controls.Add(scrollHost = new Panel() { Dock = DockStyle.Fill });
            scrollHost.SetVerticalScroll();
            scrollHost.Scroll += (s, e) => OnScrollChanged(s, e);

             
        }

        public void SuspendContentLayout() => scrollHost.SuspendLayout();

        public void ResumeContentLayout(bool performLayout) => scrollHost.ResumeLayout(performLayout);

        public void BeginBatchUpdate() => scrollHost.SuspendLayout();

        public void EndBatchUpdate() => scrollHost.ResumeLayout(performLayout: true);

        public void Add(Control item)
        {
            if (item is null)
                return;
            item.Margin = new Padding(0, 0, 0, 4);
            scrollHost.Controls.Add(item);
        }

        public void Remove(Control item)
        {
            if (item is null)
                return;
            scrollHost.Controls.Remove(item);
        }
    }
}
