using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{
    public interface IDesignerFactory
    {
        List<object> CreateServices(AppViewBase view);
    }
}