using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Microsoft.Extensions.Logging;


/// <summary>
/// Исключает из JSON поля представлений, которые нельзя или не нужно сохранять
/// (делегаты, WinForms-контролы, runtime-графы, [NotInput]).
/// </summary>
public class ViewJsonContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (ShouldIgnoreMember(member, property.PropertyType))
            property.ShouldSerialize = _ => false;

        return property;
    }

    private static bool ShouldIgnoreMember(MemberInfo member, Type propertyType)
    {
        if (member.GetCustomAttribute<NotInputAttribute>() != null)
            return true;

        var type = propertyType ?? GetMemberType(member);
        if (type is null)
            return false;

        if (typeof(Delegate).IsAssignableFrom(type))
            return true;

        //if (typeof(Control).IsAssignableFrom(type)
        //    || typeof(Component).IsAssignableFrom(type)
        //    || typeof(ComponentBase).IsAssignableFrom(type)
        //    || typeof(DesignerBase).IsAssignableFrom(type)
        //    || typeof(ILogger).IsAssignableFrom(type))
        //    return true;

        //if (type == typeof(ReportComponent) || typeof(ReportComponent).IsAssignableFrom(type))
        //    return true;

        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();
            if (def.Name.StartsWith("XmlNode", StringComparison.Ordinal))
                return true;

            if (def == typeof(Dictionary<,>)
                && type.GenericTypeArguments.Length == 2
                && typeof(Delegate).IsAssignableFrom(type.GenericTypeArguments[1]))
                return true;
        }

        return false;
    }

    private static Type GetMemberType(MemberInfo member) => member switch
    {
        PropertyInfo p => p.PropertyType,
        FieldInfo f => f.FieldType,
        _ => null
    };
}
