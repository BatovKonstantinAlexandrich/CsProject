public class MyRouteAttribute: Attribute
{
    public string Template { get; set; }

    public MyRouteAttribute(string template)
    {
        Template = template;
    }
}