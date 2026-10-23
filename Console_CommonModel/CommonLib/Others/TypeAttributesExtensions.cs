using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;

using System.Collections.Concurrent;



/// <summary>
/// Расширения для работы с атрибутами типов
/// </summary>
public static class TypeAttributesExtensions
{
    private static readonly TypeHelperStrict SharedTypeHelper = new();
    private static readonly ConcurrentDictionary<(Type Type, string Property), Dictionary<string, string>> PropertyAttributesCache = new();

    public static bool useClassDbContext = false;


    public static TypeDescription GetTypeDescription(this object p)
    {
        Type ptype = p is Type ? (Type)p : p.GetType();
        return TypeDescription.Get(ptype);
    }
    public static Dictionary<string, string> GetMethodAttributes(this object p, string property)
    {
        Type ptype = p is Type ? (Type)p : p.GetType();
        if (useClassDbContext)
            return TypeInfoProvider.GetMethodAttributes(ptype, property);
        else return SharedTypeHelper.GetMethodAttributes(ptype, property);
    }

    public static Dictionary<string, string> GetPropertyAttributes(this object p, string property)
    {
        Type ptype = p is Type ? (Type)p : p.GetType();
        if (useClassDbContext)
            return TypeInfoProvider.GetPropertyAttributes(ptype, property);

        return PropertyAttributesCache.GetOrAdd((ptype, property), static key =>
            SharedTypeHelper.GetPropertyAttributes(key.Type, key.Property));
    }

    public static string GetPropertyAttribute(this Type ptype, string property, string name)
        => useClassDbContext ? TypeInfoProvider.GetPropertyAttribute(ptype, property, name)
            : SharedTypeHelper.GetPropertyAttributes(ptype, property)[name];

    public static string GetMethodAttribute(this Type ptype, string method, string name)
        => useClassDbContext ? TypeInfoProvider.GetMethodAttribute(ptype, method, name)
            : SharedTypeHelper.GetMethodAttributes(ptype, method)[name];

    public static void AddAttribute(this Type ptype, string name, string value)
    {
        if (useClassDbContext)
            TypeInfoProvider.AddOrReplaceAttribute(ptype, name, value);
        else
            TypeHelperStrict.AddOrReplaceTypeAttribute(ptype, name, value);
    }

    public static void RemoveAttribute(this Type ptype, string name)
    {
        if (useClassDbContext)
            TypeInfoProvider.RemoveAttribute(ptype, name);
        else
            TypeHelperStrict.RemoveTypeAttribute(ptype, name);
    }

    public static IDictionary<string, string> GetMethodParameterAttributes(this Type ptype, string method, string param)
    {
        if (useClassDbContext)
            return TypeInfoProvider.GetMethodParameterAttributes(ptype, method, param);

        var paramInfo = ptype.GetMethods().FirstOrDefault(m => m.Name == method)?.GetParameters()
            .FirstOrDefault(p => p.Name == param);
        return SharedTypeHelper.GetParameterAttributes(paramInfo);
    }

    public static void AddPropertyAttribute(this Type ptype, string property, string name, string value)
    {
        if (useClassDbContext)
            TypeInfoProvider.AddOrReplacePropertyAttribute(ptype, property, name, value);
        else
            TypeHelperStrict.AddOrReplacePropertyAttribute(ptype, property, name, value);

        PropertyAttributesCache.TryRemove((ptype, property), out _);
    }

    public static void RemovePropertyAttribute(this Type ptype, string property, string name)
    {
        if (useClassDbContext)
            TypeInfoProvider.RemovePropertyAttribute(ptype, property, name);
        else
            TypeHelperStrict.RemovePropertyAttribute(ptype, property, name);

        PropertyAttributesCache.TryRemove((ptype, property), out _);
    }

    public static void AddMethodAttribute(this Type ptype, string method, string name, string value)
    {
        if (useClassDbContext)
            TypeInfoProvider.AddOrReplaceMethodAttribute(ptype, method, name, value);
        else
            TypeHelperStrict.AddOrReplaceMethodAttribute(ptype, method, name, value);
    }

    public static void RemoveMethodAttribute(this Type ptype, string method, string name)
    {
        if (useClassDbContext)
            TypeInfoProvider.RemoveMethodAttribute(ptype, method, name);
        else
            TypeHelperStrict.RemoveMethodAttribute(ptype, method, name);
    }

    public static void AddParameterAttribute(this Type ptype, string method, string parameter, string name, string value)
    {
        if (useClassDbContext)
            TypeInfoProvider.AddOrReplaceParameterAttribute(ptype, method, parameter, name, value);
        else
            TypeHelperStrict.AddOrReplaceParameterAttribute(ptype, method, parameter, name, value);
    }

    public static void RemoveParameterAttribute(this Type ptype, string method, string parameter, string name)
    {
        if (useClassDbContext)
            TypeInfoProvider.RemoveParameterAttribute(ptype, method, parameter, name);
        else
            TypeHelperStrict.RemoveParameterAttribute(ptype, method, parameter, name);
    }

    public static Dictionary<string, string> GetAttributes(this Type ptype)
        => useClassDbContext ? TypeInfoProvider.GetAttributes(ptype)
            : SharedTypeHelper.GetAttributes(ptype);

    public static Dictionary<string, Dictionary<string, string>> @GetPropertiesAttributesMap(this Type type)
    {
        Dictionary<string, Dictionary<string, string>> result = new();
        foreach (var name in type.GetPropertyNames())
            result[name] = type.GetPropertyAttributes(name);
        return result;
    }

    public static List<string> GetExtendedTypeNames(this Type type)
        => TypeInfoProvider.GetExtendedTypes(type);
}
