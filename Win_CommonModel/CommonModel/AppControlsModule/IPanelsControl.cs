
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

public interface IPanelsControl
{
    IControlBase Bottom { get; set; }
    IControlBase Left { get; set; }
    IControlBase Right { get; set; }
    IControlBase Top { get; set; }
    public void SetLeft(IControlBase leftButton);
    public void SetRight(IControlBase leftButton);
    public void SetTop(IControlBase leftButton);
    public void SetBottom(IControlBase leftButton); 
}