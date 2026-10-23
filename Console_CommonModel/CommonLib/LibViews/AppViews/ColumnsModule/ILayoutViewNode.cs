using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule
{
    public interface ILayout<TViewItem>
    {
        void Add(TViewItem item);
        void Remove(TViewItem item);
    }
    public interface ILayoutViewNode : ILayout<ViewNode>
    {
        void Add(ViewNode item);
        void Remove(ViewNode item);
    }

    public interface IDropable<AppViewBase>
    {
        void Drop(AppViewBase item);
    }

}