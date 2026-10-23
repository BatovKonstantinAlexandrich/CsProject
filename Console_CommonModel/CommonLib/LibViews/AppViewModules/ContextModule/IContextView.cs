
public interface IContextView
{
    public bool Disabled { get; set; }
    public string Icon { get; set; }
    public ChangingList<ContextView> Items { get; set; }
    public string Label { get; set; }
    public string Text { get; }
}