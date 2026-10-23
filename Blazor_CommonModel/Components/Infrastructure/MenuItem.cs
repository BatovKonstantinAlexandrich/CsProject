public class MenuItemItem<T>
{
    public T? Value { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool Disabled { get; set; }
    public Action<object, EventArgs> Todo { get; set; }
    public Action<object, EventArgs> OnClick { get; set; }
    public List<MenuItemItem<T>>? Children { get; set; }
}