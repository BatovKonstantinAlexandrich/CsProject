using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Serialization;


using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

/// <summary>
/// Параметры компонента, описанные как XML-элемент
/// </summary>
[Label("Параметры компонента, описанные как XML-элемент")]
public class ComponentParams
{

    /// <summary>
    /// Идентификатор типа связанного с элементом (записывается в KebabStyle например: table-view)
    /// </summary>
    [Label("Код XML элемента")]
    [NotNullNotEmpty]
    [InputEngWord]
    public string Tag { get; set; }

    /// <summary>
    /// Свойства XML элемента, исп. для связывания свойств с данными, функциями 
    /// выполнениям выражений, родительскими элементами и глобальной облатсью видимости.  
    /// </summary>
    [Label("Правила связывания")]
    [InputCollectionDictionary]
    public ChangingDictionary<string, string> Attrs { get; set; } = new ChangingDictionary<string, string>();


    /// <summary>
    /// Текст внутри элемента XML
    /// </summary>
    [XmlIgnore]    
    public string Text { get; set; }


    public string BeginText()
    {
        string attrsstr = "";
        foreach (var attr in Attrs)
        {
            attrsstr += $" {attr.Key}='{attr.Value}'";
        }
        return $"<{Tag}{attrsstr}>";
    }


    public string EndText()
    {
        string attrsstr = "";
        foreach (var attr in Attrs)
        {
            attrsstr += $" {attr.Key}='{attr.Value}'";
        }
        return $"</{Tag}{attrsstr}>";
    }


    public string GetNameAttribute()
    {
        if (Attrs.ContainsKey("Name"))
        {
            return Attrs["Name"];
        }
        return null;
    }
}