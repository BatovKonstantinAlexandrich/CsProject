public class ActionResponse
{    
    public string SerialKey { get; set; }
    public object MessageObject { get; set; }
    public string ActionStatus { get; set; }
    public string StatusText { get; set; }
    public bool Success { get; set; }
    public ActionRequest RequestMessage { get; set; }
    public bool Confirmed { get; set; }
}