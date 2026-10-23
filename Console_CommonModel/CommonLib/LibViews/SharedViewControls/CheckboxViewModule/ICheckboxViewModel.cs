
public interface ICheckboxViewModel: IChangeSupport
{
    public bool Checked { get; set; }
    public bool Enabled { get; set; }
    public string Text { get; set; }
    Action<object, PropertyChangedMessage<bool>> OnCheckedChanged { get; set; }

}