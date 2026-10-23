
using WinFormsApp.AppComponents.AppControlsModule;
using WinFormsApp.AppViews.ColumnsModule;

using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Корневой элемент управления
    /// </summary>
    public class AppControlRoot : LayoutControl, ILayoutControl
    {
        public AppControlRoot()
        {
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // AppControlRoot
            // 
            Name = "AppControlRoot";
            Controls.SetChildIndex(contentPane, 0);
            ResumeLayout(false);
        }
    }
}
