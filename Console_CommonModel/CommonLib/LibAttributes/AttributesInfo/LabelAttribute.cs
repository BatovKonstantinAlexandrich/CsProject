 

[Label("Метка")]
public class LabelAttribute: Attribute
{
    public string Text { get; set; }
    public LabelAttribute(string text) 
    {
        this.Text = text;         
    }
}