using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews
{
    public class ViewTest: AppViewBase
    {
        public ViewTest()
        {
        }

        internal void SetViewModel<TView>(TView view) where TView : AppViewBase
        {
             
        }
    }
}