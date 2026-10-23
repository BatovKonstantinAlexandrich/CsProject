using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule
{
    [Label("Многострочный текст")]
    public class MultiLineTextView : TextBoxView
    {
        public MultiLineTextView()
        {
            AutoSize = true;
            MinHeight = 200;
        }
    }


}
