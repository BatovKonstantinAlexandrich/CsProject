using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TypeExtensions
{

    public static bool IsExtendsFrom(this Type type, Type baseType)
    {
        return Typing.IsExtendedFrom(type, baseType.Name);
    }
    public static bool IsExtendsFrom(this Type type, string baseType)
    {
        return Typing.IsExtendedFrom(type, baseType);
    }
}