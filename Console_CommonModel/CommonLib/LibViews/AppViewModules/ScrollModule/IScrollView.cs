public interface IScrollControl: IScrollView, IControlBase
{
 
}
public interface IScrollView
{
    Action<object, ScrollEventArgs> OnScrollChanged { get; set; }
}