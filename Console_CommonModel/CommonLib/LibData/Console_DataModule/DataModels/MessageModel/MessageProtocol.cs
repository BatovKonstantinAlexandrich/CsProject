
using System.Collections.Generic;

[Icon("message")]
[Label("Информационные характеристики сообщений")]
public class MessageProtocol : BusinessEntity<MessageProtocol>
{             
    [Label("Бизнес-ресурс")]
    [InputDictionary(nameof(BusinessResource) + ",Name")]
    public int? BusinessResourceId { get; set; } = null;

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    public virtual BusinessResource BusinessResource { get; set; }

    [Label("Источник")]     
    [InputDictionary("BusinessFunction" + ",Name")]
    [InputOrder(2)]
    [NotInput]
    public int? FromBusinessFunctionId { get; set; } = null;

    [Label("Приёмник")]
    [SelectDictionary("BusinessFunction,Name")]
    [InputOrder(2)]
    [NotInput]
    public int? ToBusinessFunctionId { get; set; } = null;
 
    [Label("Свойства")]
    [InputStructureCollection(nameof(MessageProperty))]
    [InputOrder(3)]
    public virtual List<MessageProperty> Properties { get; set; } = new List<MessageProperty>();
    
    public void ResetPropertiesMessageAttributeRef()
    {
        foreach(var property in Properties)
        {
            property.Attribute = null;
        }
    }
    public string GetInputTableName()
    {
        return "[" + RusEngTranslite.TransliteToLatine(this.Name).ToUpper() + "]";
    }
}
