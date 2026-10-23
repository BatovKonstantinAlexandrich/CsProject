

using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;

using WinFormsApp.AppComponents.AppControlsModule;

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
