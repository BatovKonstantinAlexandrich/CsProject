public class Option
{
    public string Value { get; set; }
    public object Item { get;  set; }

    public Option() { }
    public Option(string Value)
    {
        this.Value = Value;
    }
}
