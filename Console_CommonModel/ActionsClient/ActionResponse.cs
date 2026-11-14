
public class ActionResponse
{    
    public string SerialKey { get; set; }
    public object ActionResult { get; set; }
    public bool ActionSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public ActionRequest RequestMessage { get; set; }

    public static ActionResponse FromJson(string responseText)
    {
        throw new NotImplementedException();
    }
}