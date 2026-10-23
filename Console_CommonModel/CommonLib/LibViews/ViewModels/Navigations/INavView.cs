using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

public interface INavView
{
    public List<string> GetNavOptions();
    public void OnNavSelected(string selected);


    public void AddNavOption(string selected, AppViewBase view);
    public void RemoveNavOption(string selected);


}
