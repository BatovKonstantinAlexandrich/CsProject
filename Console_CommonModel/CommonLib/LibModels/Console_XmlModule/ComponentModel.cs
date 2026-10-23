using System.ComponentModel.Design;
using System.Text.Json.Serialization;

/// <summary>
/// Модель компонента, управление через параметры
/// </summary>
public class ComponentModel : IComponentModel
{
    public ComponentParams Parameters { get; set; }
    
    event EventHandler<ComponentEventArgs> Input = (sender, evt) => { };
    event EventHandler<ComponentEventArgs> Output = (sender, evt) => { };

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public object Target { get; set; }


    /// <summary>
    /// Инициализация компонента - связывание разметки
    /// </summary>
    public void OnInit()
    {
        //this.Info(this.ToJson());
    }

    public void OnChanges()
    {
    }

    public void OnUpdate()
    {
    }

    public void OnDestroy()
    {
    }

    public void PreLinking()
    {
    }

    public void PostLinking()
    {
    }

    public void OnError()
    {
    }

    public void OnMessage()
    {
    }

    internal object GetTarget() => Target;

    internal void SetTarget(object targetComponent)
    {
        this.Target = targetComponent;
    }

    public string GetXml(string inner)
    {
        return GetXmlHeader() + inner + GetXmlFooter();
    }

    public string GetXml(string inner, int level)
    {
        if(this.Parameters.Tag == "columns-view")
        {
            int x = 0;
        }
        return GetXmlHeader(level) + inner + GetXmlFooter(level);
    }

    public string GetXmlHeader(int level = 0)
    {
        string before = "";
        for (int i = 0; i < level; i++)
            before += "   ";
        string result = "<" + this.Parameters.Tag;
        foreach(var kv in Parameters.Attrs)
        {
            result += $" {kv.Key}=\"{kv.Value}\"";
        }
        result +=">\n";
        return before+result;
    }

    public string GetXmlFooter(int level = 0)
    {
        string before = "";
        for (int i = 0; i < level; i++)
            before += "   ";
        string result = "</" + this.Parameters.Tag;        
        result += ">\n";
        return before + result;
    }
}