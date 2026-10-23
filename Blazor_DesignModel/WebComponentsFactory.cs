

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

public sealed class WebComponentsFactory : IComponentsFactory<IControlBase>
{
    public IControlBase CreateControl(string name, ViewItem view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateControl(ViewItem view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateControlRoot(AppViewRoot view)
    {
        throw new NotImplementedException();
    }
}
