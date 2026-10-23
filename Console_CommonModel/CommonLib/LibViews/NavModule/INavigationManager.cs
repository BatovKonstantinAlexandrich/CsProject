namespace Console_CommonModel.CommonLib.LibViews.NavModule
{

    public interface INavigationManager<TControl>
    {
        void RemovePage(string state);
        void AddPage(string state, TControl panel);
        void AddPage(string state, Task<TControl> panel);
        IEnumerable<TControl> GetNavComponents();
        List<string> GetNavOptions();
        TControl Navigate(string state);
        void Refresh();
    }
}