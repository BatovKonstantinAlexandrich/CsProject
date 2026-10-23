public interface IButtonViewModel
{
    bool Enabled { get; set; }
    string Icon { get; set; }
    bool IconOrText { get; set; }
    string Text { get; set; }
    Action<object, EventArgs> OnClick { get; set; }
}