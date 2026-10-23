using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class ScrollComponent: ComponentBase<ScrollView, ScrollControl>
    {
        public ScrollComponent([NotNullNotEmpty] ScrollView ViewModel, [NotNullNotEmpty] ScrollControl Control) : base(ViewModel, Control)
        {
        }

        private static Control GetOrCreateControl(AppViewBase view)
        {
            if (view.control is Control existing && !existing.IsDisposed)
                return existing;
            return view.ToControl();
        }

        public override void Bind()
        {
            var scrollView = GetViewModel();
            var scrollControl = GetControl();

            scrollView.Controls.OnAdded((i, p) => {
                scrollControl.Add(GetOrCreateControl((AppViewBase)p));
            });
            scrollView.Controls.OnRemoved((i, p) => {
                var av = (AppViewBase)p;
                if (av.control is Control ctrl && !ctrl.IsDisposed)
                    scrollControl.Remove(ctrl);
            });

            scrollControl.BeginBatchUpdate();
            try
            {
                foreach (var control in scrollView.Controls)
                {
                    var ctrl = GetOrCreateControl((AppViewBase)control);
                    if (!ctrl.IsDisposed)
                        scrollControl.Add(ctrl);
                }
            }
            finally
            {
                scrollControl.EndBatchUpdate();
            }

            scrollControl.OnScrollChanged += scrollView.OnScrollChanged;
        }
    }
}
