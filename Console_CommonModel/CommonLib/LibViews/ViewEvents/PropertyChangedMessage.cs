using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Сообщение о изменения свойства объекта
/// </summary>
public class PropertyChangedMessage<T> : MyEvent
{


    [NotInput]
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    private PropertyChangedMessage p;


    [NotInput]
    [NotMapped]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public object Source { get; set; }
    public string Property { get; set; }
    public T Before { get; set; }
    public T After { get; set; }
    public PropertyChangedMessage() { }
    public PropertyChangedMessage(PropertyChangedMessage p)
    {
        this.p = p;
        this.Source = p.Source;
        this.Property = p.Property;
        this.Before = (T) p.Before;
        this.After = (T) p.After;
    }
}
public class PropertyChangedMessage: PropertyChangedMessage<object>
{
  
   


    public PropertyChangedMessage() { 
        Type = "change";
    }

    public string GetBindedActionName()
    {
        return "On" + Property + "Changed";
    }
}
public class PropertyGetMessage : MyEvent
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public object Source { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
  


    public PropertyGetMessage()
    {
        Type = "get";
    }

    public string GetBindedActionName()
    {
        return "On" + Property + "Get";
    }
}
public class PropertySetMessage : MyEvent
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public object Source { get; set; }
    public string Property { get; set; }
    public object Before { get; set; }
    public object After { get; set; }


    public PropertySetMessage()
    {
        Type = "set";
    }

    public string GetBindedActionName()
    {
        return "On" + Property + "Set";
    }
}