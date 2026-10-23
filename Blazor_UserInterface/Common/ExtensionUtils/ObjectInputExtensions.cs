using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class ObjectInputExtensions
{
    public static string[] GetUserInputPropertyNames(this Type type)
    {
        var navs = Utils.GetRefTypPropertiesNames(type);        
        return ReflectionService
              .GetPropertyNames(type).
               Where(n => 
                   Utils.IsInput(type, n) &&
                   Attrs.IsVisible(type, n) &&
                   navs.Contains(n) == false && 
                   navs.Contains(n+"ID") == false).ToArray();
    }
    public static bool IsCollectionType(this Type type)
    {
        return Typing.IsCollectionType(type);
    }

    public static Dictionary<string, string> GetAttrs(this object p)
    {
        if(p is Type)return Utils.ForType((Type)p);
        return Utils.ForType(p.GetType());
    }
    

    public static string[] GetOwnPropertyNames(this Type type)
    {
        return (from p in new List<PropertyInfo>((type).GetProperties()) where p.DeclaringType == type select p.Name).ToArray();
    }

    /// <summary>
    /// Возвращает значение свойства
    /// </summary> 
    public static object GetProperty(this object target, string property )
    {
        return target.GetType().GetProperty(property).GetValue(target);
    }

    /// <summary>
    /// Возвращает значение свойства
    /// </summary> 
    public static void SetProperty(this object target, string property, object value)
    {
        target.GetType().GetProperty(property).SetValue(target,value);
    }

    /// <summary>
    /// Преобразование к формату JSON
    /// </summary>
    public static string ToJson(this object target )
    {
        return Formating.ToJson(target);
    }
    /// <summary>
    /// Преобразование к формату JSON
    /// </summary>
    public static string ToJsonOnScreen(this object target)
    {
        
        return JsonConvert.SerializeObject(target,new JsonSerializerSettings() { Formatting=Formatting.Indented});
    }
   
}