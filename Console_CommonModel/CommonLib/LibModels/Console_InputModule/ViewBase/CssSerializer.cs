public class CssSerializer
{

    /// <summary>
    /// Форматированный вывод свойств CSS
    /// </summary>
    public string Seriallize(object target, Dictionary<string, object> valuesMap, Dictionary<string, Dictionary<string,string>> attrsMap)
    {
        string text = "";        
        foreach(var p in valuesMap)
        {
            if (p.Key == "ClassList") continue;
            if (p.Key == nameof(StyledItem.FontSize)) {
                text += $"{p.Key.ToKebabStyle("-")}: {p.Value}px; ";
            }
            else
            {
                text += $"{p.Key.ToKebabStyle("-")}: {ToQualifiedString(target, p.Value, !attrsMap.ContainsKey(p.Key) ? new() : attrsMap[p.Key])}; ";
            }
            
        }
        return text;
    }
 


    public static string ToQualifiedString(object target, object value, Dictionary<string, string> attrs)
    {
        if (attrs.ContainsKey(nameof(InputPercentAttribute)))
        {
            return value?.ToString() + "%";
        }
        else if (attrs.ContainsKey(nameof(InputSelectAttribute)))
        {
            return value?.ToString();
        }
        else if (attrs.ContainsKey(nameof(UnitsAttribute)))
        {
            if(attrs[nameof(UnitsAttribute)].IndexOf("{{")!=-1)
            {
                return value?.ToString() + target.Interpolate(attrs[nameof(UnitsAttribute)]);
            }
            else
            {
                return value?.ToString() + attrs[nameof(UnitsAttribute)];
            }
            
        }
        else if (attrs.ContainsKey(nameof(InputColorAttribute)))
        {
            return value?.ToString();
        }
        else
        {
            return value?.ToString();
        }

    }

}