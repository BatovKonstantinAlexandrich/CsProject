namespace Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule
{
    public interface IFlexControl: IFlexView, IControlBase
    {

    }
    public interface IFlexView
    {
        string Direction { get; set; }
        bool FillDirection { get; set; }
        bool WrapContents { get; set; }
         
    }
}