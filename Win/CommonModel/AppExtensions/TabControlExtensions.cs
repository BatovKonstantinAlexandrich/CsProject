public static class TabControlExtensions
{
    /// <summary>
    /// Добавление новой вкладки, связывает расмер компонента с размером вкладки 
    /// </summary>
    public static TabPage AddTab(this TabControl tabs, string title, Control tab)
    {
        TabPage page = new TabPage(title);
        tabs.TabPages.Add(page);
        page.Controls.Add(tab);
        page.Dock = DockStyle.Fill;
        page.Anchor = AnchorStyles.Left;
        tab.Size = page.Size;
        page.Resize += (sender, evt) => tab.Size = page.Size;
        return page;
    }


    /// <summary>
    /// Удаление всех вкладок
    /// </summary>
    /// </summary>
    public static int ClearTabs(this TabControl tabs)
    {
        int res = tabs.TabPages.Count;
        tabs.TabPages.Remove(tabs.SelectedTab);
        tabs.TabPages.Clear();
        return res;
    }


    /// <summary>
    /// Удаление всех вкладок
    /// </summary>
    public static void RemoveTabs(this TabControl tabs, string title)
    {
        tabs.TabPages.Remove(tabs.TabPages[title]);
    }
}
