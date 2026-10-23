using Newtonsoft.Json;


public interface ILink
{
    public string Icon { get; set; }
    public string Label { get; set; }
    public string Tooltip { get; set; }
    public string Href { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput()]
    public object Item { get; set; }
    public bool IsActive { get; set; }
    public List<ILink> ChildLinks { get; set; }
}