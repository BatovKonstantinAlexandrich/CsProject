using Console_CommonModel.CommonLib.LibViews.NavModule;


/// <summary>
/// Переключает панели интерфейса, аналогично переходу по страницам
/// </summary>
public abstract class NavigationManagerBase<TControl> : INavigationManager<TControl>
{
    protected readonly ChangingDictionary<string, Task<TControl>> pages = new ChangingDictionary<string, Task<TControl>>();

    /// <summary>
    /// Последняя страница
    /// </summary>
    public TControl last;

    /// <summary>
    /// Панель размещения страниц
    /// </summary>
    public TControl panel;

    public NavigationManagerBase(TControl panel)
    {
        this.panel = panel;
        if (panel is null)
            throw new ArgumentNullException(nameof(panel));
    }
    public abstract TControl Navigate(string state);
    public abstract void Refresh();
    public abstract IEnumerable<TControl> GetNavComponents();
    public abstract void RemovePage(string state);
    public abstract List<string> GetNavOptions();
    public abstract void AddPage(string state, TControl panel);
    public abstract void AddPage(string state, Task<TControl> panel);
}