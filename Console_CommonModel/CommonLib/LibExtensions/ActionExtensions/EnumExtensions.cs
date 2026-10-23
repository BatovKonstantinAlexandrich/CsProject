using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EnumExtensions
{
    public static TEnum CreateEnumValue<TEnum>(this string name)
        where TEnum: Enum
    {
        var dictionary = ConvertEnumToDictionary(typeof(TEnum));
        return (TEnum)dictionary.FirstOrDefault(kv => kv.Value.ToLower() == name.ToLower()).Key;
    }
    public static TEnum CreateEnumValue<TEnum>(this string name, TEnum dafaults)
        where TEnum : Enum
    {
        var dictionary = ConvertEnumToDictionary(typeof(TEnum));
        var p = dictionary.Where(kv => kv.Value.ToLower() == name.ToLower()).Select(kv => kv.Key).FirstOrDefault();
        return p is null ? dafaults : ((TEnum)p);
    }



    public static Dictionary<object, string> ConvertEnumToDictionary(this Type ptype)
    {
        if(!ptype.IsEnum)
        {
            throw new Exception($"Тип {ptype.GetTypeName()} не является перечислением и не может быть преобразовать в справочник.");
        }
        var values = Enum.GetValues(ptype);
        var names = Enum.GetNames(ptype);
        var result = new Dictionary<object, string>();
        int i = 0;
        foreach (object pval in values)
        {            
            result[pval] = names[i++];
        }
        return result;
    }
}
