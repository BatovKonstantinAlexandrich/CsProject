
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputComponents;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


[Label("Расширение типов информационными атрибутами")]
[Description("Предоставляет типу Type функции для получения значения атрибутов группы AttributesInfo")]
public static class AttributesInfoExtensions
{
  
    [Label("Получить метку к данному типу")]
    public static string GetLabel(this object target)
    {
        Type ptype = null;
        if (target is not Type)
        {
            ptype = target.GetType();
        }
        else
        {
            ptype = (Type)target;
        }
        string expression = ptype.GetAttribute<LabelAttribute>(ptype.GetTypeName()).Replace("\"", "").Replace("\\", "");
        if (target is not Type)
        {
            ptype = target.GetType();
        }
        return target.Interpolate(expression);
    }

    [Label("Получить иконку к данному типу")]
    public static string GetIcon(this Type ptype)
    {
        var icon = ptype.GetAttribute<IconAttribute>("help");
        return icon.Replace("\"", "");
    }

    [Label("Получить описание к данному типу")]
 
    public static string GetDescription(this object target)
    {
        Type ptype = null;
        if (target is not Type)
        {
            ptype = target.GetType();
        }
        else
        {
            ptype = (Type)target;
        }
        string expression = ptype.GetAttribute<DescriptionAttribute>("").Replace("\"", "").Replace("\\", "");
        if (target is not Type)
        {
            ptype = target.GetType();
        }
        return target.Interpolate(expression);
    }

    [Label("Получить вспомогательное сообщение к данному типу")]
    public static string GetHelp(this Type ptype) => ptype.GetAttribute<HelpMessageAttribute>("");

    [Label("Получить метку свойства")]
    public static string GetPropertyLabel(this Type ptype, string propertyName) {

        
        if (String.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException(nameof(propertyName), $"Не задано значение аргумента {nameof(propertyName)}");
        var attrs = ptype.GetPropertyAttributes(propertyName);
        if(attrs.ContainsKey(nameof(DisplayAttribute)))
        {
            return ptype.GetProperty(propertyName).GetCustomAttribute<DisplayAttribute>().Name;
        }    
        if(attrs.ContainsKey(nameof(LabelAttribute)))
        {
            return attrs[nameof(LabelAttribute)];
        }
        
        return propertyName;
        
    }

    public static ControlAttribute GetPropertyControl(this Type ptype, string property)
    {
        try
        {
            string controlType = CommonUtils.GetControlType(ptype, property);
            if (String.IsNullOrWhiteSpace(controlType))
            {
                return null;
            }

            var attr = controlType.New<ControlAttribute>(new object[] { ptype.GetPropertyAttributes(property)[controlType] });
            return attr;
        }
        catch (Exception ex)
        {
            throw new Exception($"Свойство типа {ptype.GetTypeName()}  с именем {property} исключено для получения элемента управления: " + ex.Message);
        }
    }

    [Label("Получить иконку свойства")]
    public static string GetPropertyIcon(this Type ptype, string property) => ptype.GetPropertyAttributeValue<IconAttribute>(property, "help");
    public static string GetPropertyInputType(this Type ptype, string property)
    {
        var attrs = ptype.GetPropertyAttributes(property);
        ptype.Info(attrs.ToJsonOnScreen());
        if (attrs.ContainsKey("Key") || attrs.ContainsKey("KeyAttribute"))
        {
            return "hidden";
        }
        string key = null;
        List<string> keys = new List<string>(attrs.Keys);
        BaseInputAttribute.GetInputTypes().ForEach((string name) =>
        {
            if (keys.Contains(name))
            {
                key = name;
            }
        });
        if (key != null)
        {
            var customDataType = key.ToType().New<BaseInputAttribute>().CustomDataType;
            return key.Replace("Attribute", "").Replace("Input", "");
        }
        else
        {
            var type = ptype.GetProperty(property).PropertyType;
            if (type.IsDateTime())
            {
                return "date";
            }
            else if (type.IsBoolean())
            {
                return "checkbox";
            }
            else if (type.IsNumber())
            {
                return "number";
            }
            else
            {
               
                if (type.IsImplementsFrom(typeof(IDictionary<,>)))
                {
                    return nameof(InputCollectionDictionaryAttribute).Replace("Input","").Replace("Attribute","");
                }
                else if (type.IsImplements(typeof(IEnumerable<>)))
                {
                    if (Typing.IsPrimitive(type.GetGenericArguments()[0]))
                    {
                        return nameof(InputPrimitiveCollectionAttribute).Replace("Input", "").Replace("Attribute", "");
                    }
                    else
                    {
                        return nameof(InputStructureCollectionAttribute).Replace("Input", "").Replace("Attribute", "");
                    }
                }
                else if (!Typing.IsPrimitive(type))
                {
                    return nameof(InputCustomAttribute).Replace("Input", "").Replace("Attribute", "");
                }
                else

                {
                    return "text";
                }
            }

        }
    }
    public static string GetPropertyControlType(this Type ptype, string property)
    {
        return ptype.GetPropertyAttributes(property).Keys.FirstOrDefault(key => key.ToType().IsExtends(typeof(ControlAttribute)));
    }
public static bool IsPropertyInput(this Type type, string property)
    {
        return type.GetPropertyAttributes(property).Keys.Contains(nameof(NotInputAttribute)) ? false : true;
    }
    public static bool IsPropertyVisible(this Type type, string property)
    {
        string hidden = type.GetPropertyAttributeValue<InputHiddenAttribute>(property);
        return "True" == hidden ? false : true;
    }
 
    public static string GetPropertyAttributeValue<TAttr>(this Type type, String property) where TAttr : Attribute
    {
        if (type == null)
        {
            throw new Exception("Аргумент " + type + " содержить ссылку на null");
        }
        Dictionary<string, string> attrs = new Dictionary<string, string>();
        if (type == null || type.GetProperty(property) == null)
        {
            throw new Exception($"Свойство {property} не найдено либо не задан тип {type.GetTypeName()}");
        }
        foreach (var data in type.GetProperty(property).CustomAttributes)
        {
            string key = data.AttributeType.Name;
            if (key == typeof(TAttr).Name)
            {
                foreach (var arg in data.ConstructorArguments)
                {
                    string value = arg.Value.ToString();
                    return value;
                }
            }
        }
        return null;
    }

    [Label("Получить описание свойства")]
    public static string GetPropertyDescription(this Type ptype, string property) => ptype.GetPropertyAttributeValue<DescriptionAttribute>(property, "");

    [Label("Получить вспомогательное сообщение к свойству")]
    public static string GetPropertyHelp(this Type ptype, string property) => ptype.GetPropertyAttributeValue<HelpMessageAttribute>(property, "");

    [Label("Получить метку к методу")]
    public static string GetMethodLabel(this Type ptype, string method) => ptype.GetMethodAttribute<LabelAttribute>(method, method);

    [Label("Получить иконку к методу")]
    public static string GetMethodIcon(this Type ptype, string method) => ptype.GetMethodAttribute<IconAttribute>(method, "help");

    [Label("Получить описание к методу")]
    public static string GetMethodDescription(this Type ptype, string method) => ptype.GetMethodAttribute<DescriptionAttribute>(method, "");

    [Label("Получить вспомогательное сообщение к методу")]
    public static string GetMethodHelp(this Type ptype, string method) => ptype.GetMethodAttribute<HelpMessageAttribute>(method, "");

    [Label("Получить атрибуты к методу")]
    public static string GetAttribute<TAttribute>(this Type ptype, string defaults = null) where TAttribute : Attribute
    {
        var attrs = ptype.GetAttributes();
        if(!attrs.ContainsKey(typeof(TAttribute).GetTypeName()))
        {
            return defaults;
        }
        else
        {
            return attrs[typeof(TAttribute).GetTypeName()];
        }
        /*var pattribute = ptype.GetCustomAttributesData().FirstOrDefault(data => data.AttributeType == typeof(TAttribute));
        if (pattribute == null)
        {
            return defaults;
        }
        else
        {
            if (pattribute != null && pattribute.ConstructorArguments != null && pattribute.ConstructorArguments.Count() > 0)
            {
                return pattribute.ConstructorArguments.First().ToString();
            }
            else
            {
                return "";
            }
        }*/
    }

    [Label("Получить атрибуты к свойству")]
    public static Dictionary<string, string> _GetPropertyAttributes( this Type ptype, string property)
    {
        var res = new Dictionary<string, string>();
        var pproperty = ptype.GetProperties().FirstOrDefault(p => p.Name == property);
        if (pproperty == null)
            throw new ArgumentException(nameof(property));
        foreach (var kv in pproperty.GetCustomAttributesData())
        {
            res[TypeExtensions2.GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count() > 0 ? kv.ConstructorArguments.First().ToString() : "";
        }
        return res;
    }


    public static Dictionary<string,Dictionary<string, string>> GetArgumentsAttributes(this Type ptype, string method)
    {
        Dictionary<string, Dictionary<string, string>> res = new();
        var methodInfo = ptype.GetMethods().First(p => p.Name == method);
        if (methodInfo == null)
            throw new ArgumentException(nameof(method));
        foreach(var par in methodInfo.GetParameters())
        {
            res[par.Name] = ptype.GetArgumentAttributes(methodInfo.Name, par.Name);
        }
        return res;
    }
    public static Dictionary<string, string> GetArgumentAttributes(
        this Type ptype, string method, string parameter)
    {
        var methodInfo = ptype.GetMethods().First(p => p.Name == method);
        if (methodInfo == null)
            throw new ArgumentException(nameof(method));
        var parInfo = methodInfo.GetParameters().FirstOrDefault(p => p.Name == parameter);
        if (parInfo is null)
            throw new ArgumentException("parameter",$"Не найден параметр {parameter} в методе {ptype.GetTypeName()}.{method}");
        var res = new Dictionary<string, string>();
        foreach (var kv in parInfo.ParameterType.GetCustomAttributesData())
        {
            res[TypeExtensions2.GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count() > 0 ? kv.ConstructorArguments.First().ToString() : "";
        }
         

        return res;
    }

    public static IDictionary<string, string> GetParameterAttributes(
        this ParameterInfo par)
    {
        if (par == null)
            throw new ArgumentException(nameof(par));
        
        var res = new Dictionary<string, string>(par.GetCustomAttributesData().Select(data => new KeyValuePair<string, string>(

            TypeExtensions2.GetTypeName(data.AttributeType),
            data.ConstructorArguments.Count()>0?data.ConstructorArguments.FirstOrDefault().Value.ToString(): null
        )));

        return res;
    }

    /*public static Dictionary<string, string> GetMethodAttributes(
        this Type ptype, string method)

    {
        var res = new Dictionary<string, string>();
        var pproperty = ptype.GetMethods().First(p => p.Name == method);
        if (pproperty == null)
            throw new ArgumentException(nameof(method));
        foreach (var kv in pproperty.GetCustomAttributesData())
        {
            res[TypeExtensions2.GetTypeName(kv.AttributeType)] = kv.ConstructorArguments.Count() > 0 ? kv.ConstructorArguments.First().ToString() : "";
        }
        return res;
    }*/
    public static bool HasPropertyAttribute<TAttribute>(this Type ptype, string property) where TAttribute : Attribute
    {
        return ptype.GetPropertyAttributes(property).ContainsKey(typeof(TAttribute).GetTypeName());
    }

    [Label("Получить значение атрибута к свойству")]
    public static TAttribute GetPropertyAttribute<TAttribute>(this Type ptype, string property, TAttribute defaults = null) where TAttribute : class
    {
        try
        {            
            var pproperty = ptype.GetProperties().FirstOrDefault(p => p.Name == property);
            if (pproperty == null)
                throw new ArgumentException(nameof(property));
            var attrs = pproperty.GetCustomAttributesData();
            attrs.Select(data => data.AttributeType.GetTypeName()).ToList().ToJsonOnScreen().WriteToConsole();
            var pattribute = attrs.FirstOrDefault(data => data.AttributeType == typeof(TAttribute));
            if (pattribute == null)
            {
                return defaults;
            }
            else
            {
                return pattribute.AttributeType.New<TAttribute>(pattribute.ConstructorArguments.Select(p => p.Value.ToString()).ToArray());
                
            }
        }
        catch (Exception ex)
        {
            //throw new Exception("Ошибка при получении значения атрибута " + TypeExtensions2.GetTypeName(typeof(TAttribute)), ex);
            return null;
        }

    }

    public static string GetPropertyAttributeValue<TAttribute>(this Type ptype, string property, string defaults = null) where TAttribute : Attribute
    {       
        var attributes = ptype.GetPropertyAttributes(property);
        if(!attributes.ContainsKey(typeof(TAttribute).GetTypeName()))
        {
            return defaults;
        }
        return attributes.First(kv => kv.Key == typeof(TAttribute).GetTypeName()).Value;       
    }

    [Label("Получить значение атрибута к методу")]
    public static string GetMethodAttribute<TAttribute>(this Type ptype, string method, string defaults = null) where TAttribute : Attribute
    {
        var pmethod = ptype.GetMethods().FirstOrDefault(p => p.Name == method);
        if (pmethod == null)
            throw new ArgumentException($"{TypeExtensions2.GetTypeName(ptype)} не определяет метод {method}", nameof(method));
        var pattribute = pmethod.GetCustomAttributesData().FirstOrDefault(data => data.AttributeType == typeof(TAttribute));
        if (pattribute == null)
        {
            return defaults;
        }
        else
        {
            if (pattribute.ConstructorArguments != null && pattribute.ConstructorArguments.Count() > 0)
            {
                string result = "";
                foreach (var arg in pattribute.ConstructorArguments)
                {
                    result += String.IsNullOrEmpty(result) ? arg.Value.ToString() : ("," + arg.Value.ToString());
                }
                return result;
            }
            else
            {
                return "";
            }
        }
    }
}
