using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using System.Collections.Concurrent;
using System.Reflection;

using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

/// <summary>
/// Вспомогательные методы для работы с типами.
/// Метаданные типа кэшируются в потокобезопасном справочнике <see cref="ClassModels"/>.
/// </summary>
public class TypeHelperStrict
{
    private static readonly ConcurrentDictionary<Type, ClassModel> ClassModels = new();

    /// <summary>
    /// Атрибуты типа параметра метода (ключ: Type + метод + имя параметра).
    /// </summary>
    private static readonly ConcurrentDictionary<(Type Type, string Method, string Parameter), Dictionary<string, string>>
        ArgumentTypeAttributes = new();

    /// <summary>Потокобезопасный справочник метаданных типов.</summary>
    public static IReadOnlyDictionary<Type, ClassModel> ClassModelCache => ClassModels;

    /// <summary>
    /// Возвращает <see cref="ClassModel"/> из кэша или создаёт и сохраняет новую модель.
    /// </summary>
    public static ClassModel GetOrCreateClassModel(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        return ClassModels.GetOrAdd(type, BuildClassModel);
    }

    /// <summary>
    /// Удаляет модель типа из кэша (для тестов и горячей перезагрузки).
    /// </summary>
    public static bool InvalidateClassModel(Type type)
    {
        if (type is null)
            return false;

        var removed = ClassModels.TryRemove(type, out _);
        foreach (var key in ArgumentTypeAttributes.Keys.Where(k => k.Type == type).ToList())
            ArgumentTypeAttributes.TryRemove(key, out _);
        return removed;
    }

    /// <summary>
    /// Получение аттрибутов
    /// </summary>
    public Dictionary<string, string> GetAttributes(Type p)
    {
        if (p is null)
        {
            System.Console.WriteLine($"Вам следует передать ссылку на Type в метод Utils.GetEntityContrainsts() вместо null");
            System.Console.WriteLine($"{new ArgumentNullException("p")}");
            return new Dictionary<string, string>();
        }

        var model = GetOrCreateClassModel(p);
        return model.Attributes
            .GroupBy(a => a.Name)
            .ToDictionary(g => g.Key, g => g.Last().Value ?? string.Empty);
    }

    /// <summary>
    /// Получение имени типа
    /// </summary>
    public string GetTypeName(object target)
    {
        Type targetType = target is Type ? ((Type)target) : target.GetType();
        return GetNameOfType(targetType);
    }

    /// <summary>
    /// Получение имени типа
    /// </summary>
    public string GetNameOfType(Type propertyType)
    {
        if (propertyType is null)
            throw new ArgumentNullException("type");

        string name = propertyType.Name;
        if (name is null)
            return string.Empty;

        if (name.IndexOf('`') != -1)
            name = name.Substring(0, name.IndexOf('`'));

        var arr = propertyType.GetGenericArguments();
        if (arr.Length > 0)
        {
            name += '<';
            foreach (var arg in arr)
                name += GetNameOfType(arg) + ",";
            name = name.Substring(0, name.Length - 1);
            name += '>';
        }

        return name;
    }

    /// <summary>
    /// Получение имен свойств
    /// </summary>
    public List<string> GetOwnPropertyNames(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        var model = GetOrCreateClassModel(type);
        return model.Properties.Select(p => p.Name).ToList();
    }

    /// <summary>
    /// Получение аттрибутов свойств
    /// </summary>
    public Dictionary<string, string> GetPropertyAttributes(Type ptype, string property)
    {
        if (ptype is null)
            throw new ArgumentNullException(nameof(ptype));

        var model = GetOrCreateClassModel(ptype);
        var prop = model.Properties.FirstOrDefault(p => p.Name == property);
        if (prop is not null)
            return prop.Attributes
                .GroupBy(a => a.Name)
                .ToDictionary(g => g.Key, g => g.Last().Value ?? string.Empty);

        return ReadPropertyAttributesUncached(ptype, property);
    }

    /// <summary>
    /// Получение имени методов
    /// </summary>
    public List<string> GetOwnMethodNames(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        var model = GetOrCreateClassModel(type);
        return model.Methods.Select(m => m.Name).ToList();
    }

    /// <summary>
    /// Получение аттрибутов параметров
    /// </summary>
    public IDictionary<string, string> GetParameterAttributes(ParameterInfo par)
    {
        try
        {
            if (par is null)
                throw new ArgumentException(nameof(par));

            var declaringType = par.Member?.DeclaringType;
            if (declaringType is not null)
            {
                var model = GetOrCreateClassModel(declaringType);
                var methodModel = model.Methods.FirstOrDefault(m => m.Name == par.Member.Name);
                var parameterModel = methodModel?.Parameters.FirstOrDefault(p => p.Name == par.Name);
                if (parameterModel?.Attributes.Count > 0)
                    return parameterModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
            }

            return ReadParameterAttributesUncached(par);
        }
        catch (Exception ex)
        {
            throw new Exception($"Исключение при получении атрибутов параметра {par.Name} типа {par.ParameterType.GetTypeName()} => {ex.Message}");
        }
    }

    /// <summary>
    /// Получение аттрибутов метода
    /// </summary>
    public Dictionary<string, string> GetMethodAttributes(Type ptype, string method)
    {
        if (ptype is null)
            throw new ArgumentNullException(nameof(ptype));

        var model = GetOrCreateClassModel(ptype);
        var methodModel = model.Methods.FirstOrDefault(m => m.Name == method);
        if (methodModel is not null)
            return methodModel.Attributes
                .GroupBy(a => a.Name)
                .ToDictionary(g => g.Key, g => g.Last().Value ?? string.Empty);

        return ReadMethodAttributesUncached(ptype, method);
    }

    /// <summary>
    /// Получение аттрибутов аргументов (атрибуты типа параметра)
    /// </summary>
    public Dictionary<string, string> GetArgumentAttributes(Type ptype, string method, string parameter)
    {
        if (ptype is null)
            throw new ArgumentNullException(nameof(ptype));

        GetOrCreateClassModel(ptype);

        if (ArgumentTypeAttributes.TryGetValue((ptype, method, parameter), out var cached))
            return new Dictionary<string, string>(cached);

        return ReadArgumentAttributesUncached(ptype, method, parameter);
    }

    /// <summary>
    /// Добавляет или заменяет атрибут типа в кэше <see cref="ClassModels"/>.
    /// </summary>
    public static Dictionary<string, string> AddOrReplaceTypeAttribute(Type type, string name, string value)
    {
        var model = GetOrCreateClassModel(type);
        UpsertAttribute(model.Attributes, name, value);
        return model.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    /// <summary>
    /// Удаляет атрибут типа из кэша <see cref="ClassModels"/>.
    /// </summary>
    public static Dictionary<string, string> RemoveTypeAttribute(Type type, string name)
    {
        var model = GetOrCreateClassModel(type);
        RemoveTypeAttribute(model.Attributes, name);
        return model.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    private static void RemoveTypeAttribute(List<ClassModelAttribute> attributes, string name)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Добавляет или заменяет атрибут свойства в кэше <see cref="ClassModels"/>.
    /// </summary>
    public static Dictionary<string, string> AddOrReplacePropertyAttribute(Type type, string property, string name, string value)
    {
        var model = GetOrCreateClassModel(type);
        var propertyModel = model.Properties.FirstOrDefault(p => p.Name == property)
            ?? throw new ArgumentException(nameof(property), $"Свойство {property} не найдено в кэше типа {type.GetTypeName()}");

        UpsertAttribute(propertyModel.Attributes, name, value);
        return propertyModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    /// <summary>
    /// Удаляет атрибут свойства из кэша <see cref="ClassModels"/>.
    /// </summary>
    public static Dictionary<string, string> RemovePropertyAttribute2(Type type, string property, string name)
    {
        var model = GetOrCreateClassModel(type);
        var propertyModel = model.Properties.FirstOrDefault(p => p.Name == property)
            ?? throw new ArgumentException(nameof(property), $"Свойство {property} не найдено в кэше типа {type.GetTypeName()}");

        RemovePropertyModelAttribute(propertyModel.Attributes, name);
        return propertyModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    /// <summary>
    /// Добавляет или заменяет атрибут метода в кэше <see cref="ClassModels"/>.
    /// </summary>
    public static Dictionary<string, string> AddOrReplaceMethodAttribute(Type type, string method, string name, string value)
    {
        var model = GetOrCreateClassModel(type);
        var methodModel = model.Methods.FirstOrDefault(m => m.Name == method)
            ?? throw new ArgumentException(nameof(method), $"Метод {method} не найден в кэше типа {type.GetTypeName()}");

        UpsertAttribute(methodModel.Attributes, name, value);
        return methodModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    /// <summary>
    /// Удаляет атрибут метода из кэша <see cref="ClassModels"/>.
    /// </summary>
    
    public static Dictionary<string, string> RemoveMethodAttribute(Type type, string method, string name)
    {
        var model = GetOrCreateClassModel(type);
        var methodModel = model.Methods.FirstOrDefault(m => m.Name == method)
            ?? throw new ArgumentException(nameof(method), $"Метод {method} не найден в кэше типа {type.GetTypeName()}");

        RemoveMethodModelAttribute(methodModel.Attributes, name);
        return methodModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }
    public static Dictionary<string, string> RemovePropertyAttribute(Type type, string method, string name)
    {
        var model = GetOrCreateClassModel(type);
        var methodModel = model.Properties.FirstOrDefault(m => m.Name == method)
            ?? throw new ArgumentException(nameof(method), $"Свойство {method} не найден в кэше типа {type.GetTypeName()}");

        RemovePropertyModelAttribute(methodModel.Attributes, name);
        return methodModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    private static void RemoveMethodModelAttribute(List<MethodModelAttribute> attributes, string name)
    {
        throw new NotImplementedException();
    }
    private static void RemoveMethodParameterModelAttribute(List<MethodParameterModelAttribute> attributes, string name)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Добавляет или заменяет атрибут параметра метода в кэше <see cref="ClassModels"/>.
    /// </summary>
    public static Dictionary<string, string> AddOrReplaceParameterAttribute(
        Type type, string method, string parameter, string name, string value)
    {
        var parameterModel = GetCachedParameterModel(type, method, parameter);
        UpsertAttribute(parameterModel.Attributes, name, value);
        return parameterModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    /// <summary>
    /// Удаляет атрибут параметра метода из кэша <see cref="ClassModels"/>.
    /// </summary>
    public static Dictionary<string, string> RemoveParameterAttribute(
        Type type, string method, string parameter, string name)
    {
        var parameterModel = GetCachedParameterModel(type, method, parameter);
        RemoveMethodParameterModelAttribute(parameterModel.Attributes, name);
        return parameterModel.Attributes.ToDictionary(a => a.Name, a => a.Value ?? string.Empty);
    }

    private static MethodParameterModel GetCachedParameterModel(Type type, string method, string parameter)
    {
        var model = GetOrCreateClassModel(type);
        var methodModel = model.Methods.FirstOrDefault(m => m.Name == method)
            ?? throw new ArgumentException(nameof(method), $"Параметер {method} не найден в кэше типа {type.GetTypeName()}");

        return methodModel.Parameters.FirstOrDefault(p => p.Name == parameter)
            ?? throw new ArgumentException(nameof(parameter),
                $"Параметр {parameter} не найден в кэше метода {type.GetTypeName()}.{method}");
    }
    private static void UpsertAttribute(List<MethodModelAttribute> attributes, string name, string value)
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            existing.Value = value;
        else
            attributes.Add(new MethodModelAttribute { Name = name, Value = value });
    }
    private static void UpsertAttribute(List<ClassModelAttribute> attributes, string name, string value)
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            existing.Value = value;
        else
            attributes.Add(new ClassModelAttribute { Name = name, Value = value });
    }
    private static void UpsertAttribute(List<MethodParameterModelAttribute> attributes, string name, string value)
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            existing.Value = value;
        else
            attributes.Add(new MethodParameterModelAttribute { Name = name, Value = value });
    }
    private static void UpsertAttribute(List<PropertyModelAttribute> attributes, string name, string value)    
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            existing.Value = value;
        else
            attributes.Add(new PropertyModelAttribute { Name = name, Value = value });
    }
    private static void RemovePropertyModelAttribute(List<PropertyModelAttribute> attributes, string name)
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            attributes.Remove(existing);
    }
    private static void RemoveMethodParameterModelAttribute<T>(List<MethodParameterModelAttribute> attributes, string name)
        where T : BaseEntity
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            attributes.Remove(existing);
    }

    private static void RemoveAttribute<T>(List<ClassModelAttribute> attributes, string name)
        where T : BaseEntity
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            attributes.Remove(existing);
    }
    private static void RemoveAttribute<T>(List<PropertyModelAttribute> attributes, string name)
        where T : BaseEntity
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            attributes.Remove(existing);
    }
    private static void RemoveAttribute<T>(List<MethodModelAttribute> attributes, string name)
        where T : BaseEntity
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            attributes.Remove(existing);
    }
    private static void RemoveClassModelAttribute<T>(List<ClassModelAttribute> attributes, string name)
        where T : BaseEntity
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            attributes.Remove(existing);
    }
    private static ClassModel BuildClassModel(Type type)
    {
        var helper = new TypeHelperStrict();
        var typeName = type.GetTypeName();
        var model = new ClassModel
        {
            Name = typeName,
            Label = typeName,
            Description = string.Empty,
        };

        // Только reflection: GetLabel/GetDescription идут через TypeAttributesExtensions → TypeHelper и дают рекурсию.
        foreach (var data in type.GetCustomAttributesData())
        {
            var attrName = data.AttributeType.Name;
            var value = data.ConstructorArguments.Count > 0
                ? data.ConstructorArguments[0].Value?.ToString()?.Replace("\"", string.Empty) ?? string.Empty
                : string.Empty;

            UpsertAttribute(model.Attributes, attrName, value);

            if (attrName == nameof(LabelAttribute))
                model.Label = value;
            else if (attrName == nameof(DescriptionAttribute))
                model.Description = value;
        }

        const BindingFlags memberFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        foreach (var propertyInfo in type.GetProperties(memberFlags))
        {
            var propertyModel = new PropertyModel
            {
                Name = propertyInfo.Name,
                Type = propertyInfo.PropertyType.GetTypeName(),
                Label = propertyInfo.Name,
                Description = propertyInfo.Name,
            };

            foreach (var kv in propertyInfo.GetCustomAttributesData())
            {
                propertyModel.Attributes.Add(new()
                {
                    Name = helper.GetTypeName(kv.AttributeType),
                    Value = FormatPropertyAttributeValue(kv),
                });
            }

            model.Properties.Add(propertyModel);
        }

        var processedMethods = new HashSet<string>();
        var methods = type.GetOwnMethodNames();
        if(type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            methods.AddRange(type.GetGenericTypeDefinition().GetMethods(memberFlags).Select(m=>m.Name));
        }
        foreach (var name in methods)
        {
            var methodInfo = type.GetMethodByName(name);
            if (methodInfo.IsSpecialName || methodInfo.IsStatic)
                continue;
            if (!processedMethods.Add(methodInfo.Name))
                continue;
            var methodModel = new MethodModel
            {
                Name = methodInfo.Name,
                Label = methodInfo.Name,
                Description = methodInfo.Name,
                Type = methodInfo.ReturnType.GetTypeName(),
            };

            foreach (var kv in methodInfo.GetCustomAttributesData())
            {
                methodModel.Attributes.Add(new ()
                {
                    Name = helper.GetTypeName(kv.AttributeType),
                    Value = FormatMethodAttributeValue(kv),
                });
            }

            var labelAttr = methodModel.Attributes.FirstOrDefault(a => a.Name == nameof(LabelAttribute));
            if (labelAttr is not null)
                methodModel.Label = labelAttr.Value;
            var iconAttr = methodModel.Attributes.FirstOrDefault(a => a.Name == nameof(IconAttribute));
            if (iconAttr is not null)
                methodModel.Icon = iconAttr.Value;
            var descAttr = methodModel.Attributes.FirstOrDefault(a => a.Name == nameof(DescriptionAttribute));
            if (descAttr is not null)
                methodModel.Description = descAttr.Value;
            var helpAttr = methodModel.Attributes.FirstOrDefault(a => a.Name == nameof(HelpMessageAttribute));
            if (helpAttr is not null)
                methodModel.Help = helpAttr.Value;

            foreach (var par in methodInfo.GetParameters())
            {
                var parameterModel = new MethodParameterModel
                {
                    Name = par.Name,
                    Type = par.ParameterType.GetTypeName(),
                };

                foreach (var data in par.GetCustomAttributesData())
                {
                    parameterModel.Attributes.Add(new MethodParameterModelAttribute
                    {
                        Name = helper.GetTypeName(data.AttributeType),
                        Value = data.ConstructorArguments.Count > 0
                            ? data.ConstructorArguments.First().Value?.ToString()?.Replace("\"", string.Empty) ?? string.Empty
                            : string.Empty,
                    });
                }

                var parLabel = parameterModel.Attributes.FirstOrDefault(a => a.Name == nameof(LabelAttribute));
                if (parLabel is not null)
                    parameterModel.Label = parLabel.Value;
                var parIcon = parameterModel.Attributes.FirstOrDefault(a => a.Name == nameof(IconAttribute));
                if (parIcon is not null)
                    parameterModel.Icon = parIcon.Value;

                methodModel.Parameters.Add(parameterModel);

                var argumentTypeAttrs = new Dictionary<string, string>();
                foreach (var kv in par.ParameterType.GetCustomAttributesData())
                {
                    argumentTypeAttrs[helper.GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count > 0
                        ? kv.ConstructorArguments.First().ToString().Replace("\"", string.Empty)
                        : string.Empty;
                }

                if (argumentTypeAttrs.Count > 0)
                    ArgumentTypeAttributes[(type, methodInfo.Name, par.Name)] = argumentTypeAttrs;
            }

            model.Methods.Add(methodModel);
        }

        return model;
    }

    private static string FormatPropertyAttributeValue(CustomAttributeData kv)
    {
        var value = kv.ConstructorArguments.Count > 0
            ? $"{kv.ConstructorArguments.First().Value}".Replace("\"", string.Empty)
            : string.Empty;
        if (value.StartsWith("\""))
            value = value.Substring(1);
        if (value.EndsWith("\""))
            value = value.Substring(0, value.Length - 1);
        return value;
    }

    private static string FormatMethodAttributeValue(CustomAttributeData kv)
        => kv.ConstructorArguments.Count > 0
            ? kv.ConstructorArguments.First().ToString().Replace("\"", string.Empty)
            : string.Empty;

    private Dictionary<string, string> ReadPropertyAttributesUncached(Type ptype, string property)
    {
        var propertyInfo = ptype.GetProperty(property)
            ?? throw new ArgumentException($"Тип {ptype.GetTypeName()} не содержит свойство {property}", nameof(property));

        return new Dictionary<string, string>(propertyInfo.GetCustomAttributesData().Select(data =>
            new KeyValuePair<string, string>(
                data.AttributeType.GetTypeName().Contains("Json")?
                    data.AttributeType.FullName:
                    GetTypeName(data.AttributeType),
                FormatPropertyAttributeValue(data))));
    }

    private Dictionary<string, string> ReadMethodAttributesUncached(Type ptype, string method)
    {
        var methodInfo = ptype.GetMethods().FirstOrDefault(p => p.Name == method)
            ?? throw new ArgumentException(nameof(method));

        return new Dictionary<string, string>(methodInfo.GetCustomAttributesData().Select(data =>
            new KeyValuePair<string, string>(
                           data.AttributeType.GetTypeName().Contains("Json") ?
                    data.AttributeType.FullName :
                    GetTypeName(data.AttributeType),
                FormatMethodAttributeValue(data))));
    }

    private Dictionary<string, string> ReadParameterAttributesUncached(ParameterInfo par)
        => new(par.GetCustomAttributesData().Select(data =>
            new KeyValuePair<string, string>(
                    data.AttributeType.GetTypeName().Contains("Json") ?
                    data.AttributeType.FullName :
                    GetTypeName(data.AttributeType),
                data.ConstructorArguments.Count > 0
                    ? data.ConstructorArguments.First().Value?.ToString()?.Replace("\"", string.Empty) ?? string.Empty
                    : string.Empty)));

    private Dictionary<string, string> ReadArgumentAttributesUncached(Type ptype, string method, string parameter)
    {
        var methodInfo = ptype.GetMethods().First(p => p.Name == method)
            ?? throw new ArgumentException(nameof(method));
        var parInfo = methodInfo.GetParameters().FirstOrDefault(p => p.Name == parameter)
            ?? throw new ArgumentException("parameter", $"Не найден параметр {parameter} в методе {GetNameOfType(ptype)}.{method}");

        var res = new Dictionary<string, string>();
        foreach (var kv in parInfo.ParameterType.GetCustomAttributesData())
        {
            res[GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count > 0
                ? kv.ConstructorArguments.First().ToString().Replace("\"", string.Empty)
                : string.Empty;
        }

        return res;
    }
}
