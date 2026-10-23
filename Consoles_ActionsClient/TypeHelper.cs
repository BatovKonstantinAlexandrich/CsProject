using ActionModels;

using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

using WinFormsApp.Console.Console_ClassModule.ClassDataModel;


/// <summary>
/// Вспомогательные методы для работы с типами
/// </summary>
internal class TypeHelper
{
    private static readonly ConcurrentDictionary<Type, ClassModel> ClassModels = new();
    /// <summary>
    /// Атрибуты типа параметра метода (ключ: Type + метод + имя параметра).
    /// </summary>
    private static readonly ConcurrentDictionary<(Type Type, string Method, string Parameter), Dictionary<string, string>>
        ArgumentTypeAttributes = new();
    /// <summary>
    /// Возвращает <see cref="ClassModel"/> из кэша или создаёт и сохраняет новую модель.
    /// </summary>
    public static ClassModel GetOrCreateClassModel(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        return ClassModels.GetOrAdd(type, BuildClassModel);
    }
    private static string FormatMethodAttributeValue(CustomAttributeData kv)
       => kv.ConstructorArguments.Count > 0
           ? kv.ConstructorArguments.First().ToString().Replace("\"", string.Empty)
           : string.Empty;

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

    
    private static ClassModel BuildClassModel(Type type)
    {
        var helper = new TypeHelper();
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

            UpsertAttribute<ClassModel>(model.Attributes.Select(p => (AttributeBase<ClassModel>)p).ToList(), attrName, value);

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
        foreach (var methodInfo in type.GetMethods(memberFlags))
        {
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
            //var iconAttr = methodModel.Attributes.FirstOrDefault(a => a.Name == nameof(IconAttribute));
            //if (iconAttr is not null)
            //    methodModel.Icon = iconAttr.Value;
            var descAttr = methodModel.Attributes.FirstOrDefault(a => a.Name == nameof(DescriptionAttribute));
            if (descAttr is not null)
                methodModel.Description = descAttr.Value;
            var helpAttr = methodModel.Attributes.FirstOrDefault(a => a.Name == "HelpMessageAttribute");
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
                    parameterModel.Attributes.Add(new ()
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
                //var parIcon = parameterModel.Attributes.FirstOrDefault(a => a.Name == nameof(IconAttribute));
                //if (parIcon is not null)
                //    parameterModel.Icon = parIcon.Value;

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
    private static void UpsertAttribute<T>(List<AttributeBase<T>> attributes, string name, string value)
    where T : BaseEntity
    {
        var existing = attributes.FirstOrDefault(a => a.Name == name);
        if (existing is not null)
            existing.Value = value;
        else
            attributes.Add(new AttributeBase<T> { Name = name, Value = value });
    }

    /// <summary>
    /// Получение аттрибутов
    /// </summary>
    public Dictionary<string, string> GetAttributes(Type p)
    {
        /*var provider = new CustomDataProvider();
        if (provider.GetTypes().Contains(ptype.GetTypeName()) == false)
        {
            provider.ToType(ptype);
        }*/
        Dictionary<string, string> attrs = new Dictionary<string, string>();
        if (p == null)
        {
            System.Console.WriteLine($"Вам слендует передать ссылку на Type в метод Utils.GetEntityContrainsts() вместо null");
            System.Console.WriteLine($"{new ArgumentNullException("p")}");
            return attrs;
        }
        foreach (var data in p.GetCustomAttributesData())
        {
            string key = data.AttributeType.Name;
            foreach (var arg in data.ConstructorArguments)
            {
                string value = arg.Value.ToString().Replace("\"","");
                attrs[key] = value;
            }

        }
        return attrs;
    }

    /// <summary>
    /// Получение имени типа
    /// </summary>
    public string GetTypeName(object target)
    {
        Type targetType = target is Type ? ((Type)target) : target.GetType();
        string result = GetNameOfType(targetType);
        return result;
    }

    /// <summary>
    /// Получение имени типа
    /// </summary>
    public string GetNameOfType(Type propertyType)
    {
        if (propertyType == null)
            throw new ArgumentNullException("type");
        string name = propertyType.Name;
        if (name == null) return "";
        if (name.IndexOf("`") != -1)
            name = name.Substring(0, name.IndexOf("`"));

        var arr = propertyType.GetGenericArguments();
        if (arr.Length > 0)
        {
            name += '<';
            foreach (var arg in arr)
            {
                name += GetNameOfType(arg) + ",";
            }
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
        var properties = type.GetProperties();
        var own = properties.Where(p => GetTypeName(p.DeclaringType) == GetTypeName(type)).Select(p => p.Name).ToList();
        return own;
    }

    /// <summary>
    /// Получение аттрибутов свойств 
    /// </summary>
    public Dictionary<string, string> GetPropertyAttributes(Type ptype, string property)
    {
        var res = new Dictionary<string, string>();
        var pproperty = ptype.GetProperties().FirstOrDefault(p => p.Name == property);
        if (pproperty == null)
        {
            
            throw new ArgumentException($"Тип {ptype.GetTypeName()} не содержит свойство {property}", nameof(property));
        }
        foreach (var kv in pproperty.GetCustomAttributesData())
        {
            res[GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count() > 0 ? 
                $"{kv.ConstructorArguments.First().Value}".Replace("\"","") : "";
            if (res[GetTypeName(kv.AttributeType)].StartsWith("\""))
                res[GetTypeName(kv.AttributeType)] = res[GetTypeName(kv.AttributeType)].Substring(1);
            if (res[GetTypeName(kv.AttributeType)].EndsWith("\""))
                res[GetTypeName(kv.AttributeType)] = res[GetTypeName(kv.AttributeType)].Substring(0, res[GetTypeName(kv.AttributeType)].Length-1);
        }
        return res;
    }

    /// <summary>
    /// Получение имени методов
    /// </summary>
    public List<string> GetOwnMethodNames(Type type)
    {
        return (from p in new List<MethodInfo>((type).GetMethods()) where p.DeclaringType == type select p.Name).ToList();
    }

    /// <summary>
    /// Получение аттрибутов параметров 
    /// </summary>
    public IDictionary<string, string> GetParameterAttributes( ParameterInfo par)
    {
        try
        {
            if (par == null)
                throw new ArgumentException(nameof(par));
          
            var res = new Dictionary<string, string>(par.GetCustomAttributesData().Select(data => 
                new KeyValuePair<string, string>(
                    GetTypeName(data.AttributeType),
                    data.ConstructorArguments.Count()>0?
                        data.ConstructorArguments.First().Value.ToString().Replace("\"", ""): ""
                )));
            return res;
        }
        catch (Exception ex)
        {
            throw new Exception($"Исключение при получении атрибутов параметра {par.Name} типа {par.ParameterType.GetTypeName()} => {ex.Message}");
        }
    }

    /// <summary>
    /// Получение аттрибутов метода 
    /// </summary>
    public Dictionary<string, string> GetMethodAttributes( Type ptype, string method)

    {
        var res = new Dictionary<string, string>();
        var pproperty = ptype.GetMethods().First(p => p.Name == method);
        if (pproperty == null)
            throw new ArgumentException(nameof(method));
        foreach (var kv in pproperty.GetCustomAttributesData())
        {
            res[GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count() > 0 ? 
                kv.ConstructorArguments.First().ToString().Replace("\"","") : "";
        }
        return res;
    }

    /// <summary>
    /// Получение аттрибутов аргументов 
    /// </summary>
    public Dictionary<string, string> GetArgumentAttributes(
        Type ptype, string method, string parameter)
    {
        var methodInfo = ptype.GetMethods().First(p => p.Name == method);
        if (methodInfo == null)
            throw new ArgumentException(nameof(method));
        var parInfo = methodInfo.GetParameters().FirstOrDefault(p => p.Name == parameter);
        if (parInfo is null)
            throw new ArgumentException("parameter", $"Не найден параметр {parameter} в методе {GetTypeName(ptype)}.{method}");
        var res = new Dictionary<string, string>();
        foreach (var kv in parInfo.ParameterType.GetCustomAttributesData())
        {
            res[GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count() > 0 ? 
                kv.ConstructorArguments.First().ToString().Replace("\"","") : "";
        }


        return res;
    }
}
