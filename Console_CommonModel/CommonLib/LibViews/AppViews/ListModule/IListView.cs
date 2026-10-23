
public interface IListView
{
    bool Enabled { get; set; }
    Action<Option> OnSelected { get; set; }
    ChangingList<Option> Options { get; set; }
    Option SelectedOption { get; set; }
    string Text { get; set; }

    void Add(ViewNode item);
    void OnResultSetChanged(PropertyChangedMessage message);
    void Remove(ViewNode item);
}