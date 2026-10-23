namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{
    public interface IBind<TView> where TView : ViewItem
    {
        void Bind(TView view);
    }
}