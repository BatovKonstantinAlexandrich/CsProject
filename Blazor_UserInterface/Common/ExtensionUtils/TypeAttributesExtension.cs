using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TypeAttributesExtension
{

    public static string Label(this Type type)
    {
        return Utils.LabelFor(type);
    }
    public static string Label(this Type type, string property)
    {
        return Utils.LabelFor(type, property);
    }    
    public static string Description(this Type type)
    {
        return Utils.DescriptionFor(type);
    }
    public static string Icon(this Type type)
    {
        return Utils.IconFor(type);
    }
    public static bool IsEntity(this Type type)
    {
        return Typing.IsExtendedFrom(type, "BaseEntity");
    }
}