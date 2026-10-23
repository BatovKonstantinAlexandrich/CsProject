namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule
{
  
    public interface ILayout<TViewItem>
    {
        void Add(TViewItem item);
        void Remove(TViewItem item);
    }
    public interface ILayoutView : ILayout<ViewNode>
    {
        void Add(ViewNode item);
        void Remove(ViewNode item);
        public bool IsVertical { get; set; }
        public bool WrapContent { get; set; }
        public bool FillSize { get; set; }
    }
    public interface ILayoutControl : ILayout<IControlBase>
    {
        void Add(IControlBase item);
        void Remove(IControlBase item);
    }

    public interface IDropable<AppViewBase>
    {
        void Drop(AppViewBase item);
    }
    public interface ILayoutViewSlot : IDropable<AppViewBase>
    {
        void Set(IControlBase item);
        void Hover();

        IControlBase Get();
    }
}