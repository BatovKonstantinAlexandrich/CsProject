
namespace Console_CommonModel.CommonLib.Others
{
    public interface IComponentBase<TViewModel, TViewControl>
        where TViewModel : AppViewBase
        where TViewControl : IControlBase
    {
        ChangingList<DesignerBase> viewServices { get; set; }

        void Bind();
        void BindContextMenu();
        void BindControlEvents();
        void BindSides(PanelsView view, IControlBase side);
        TViewControl GetControl();
        TViewModel GetViewModel();
        void InitViewServices();
    }
}