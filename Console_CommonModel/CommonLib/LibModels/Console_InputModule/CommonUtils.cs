
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.AttributeView;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// Реализует методы работы с типами
/// </summary>
public static class CommonUtils
{

    public static object BindingsFor(string entity)
    {
        return BindingsFor(ReflectionService.TypeForName(entity));

    }


    public static object BindingsFor(Type type)
    {
        var attrs = ForType(type);
        if (attrs.ContainsKey(nameof(ViewBindingsAttribute)))
        {
            return attrs[nameof(ViewBindingsAttribute)];
        }
        else
        {
            return MyExpression.GetDefaultBindingsFor(type.Name);
        }


    }


    /// <summary>
    /// Возвращает значение атрибута установленного для типа объекта
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attrName"></param>
    /// <returns></returns>
    public static string GetTypeAttrValue(object target, string attrName)
    {
        var attrs = ForType(target.GetType());
        if (attrs.ContainsKey(attrName) == false)
        {
            return null;
        }
        else
        {
            return attrs[attrName];
        }
    }

    /// <summary>
    /// Возвращаент имя свойства помеченного заданным атриюбутом
    /// </summary>
    /// <param name="target"></param>
    /// <param name="nameOfInputTypeAttribute"></param>
    /// <returns></returns>
    public static object GetValueMarkedByAttribute(object target, string nameOfInputTypeAttribute)
    {
        return CommonUtils.ForAllPropertiesInType(target.GetType()).Where(p => p.Value.ContainsKey(nameOfInputTypeAttribute)).Select(p => p.Key).Single();

    }
    public static List<string> GetSearchTerms(string entity)
    {
        Type entityType = ReflectionService.TypeForShortName(entity);
        List<string> terms = CommonUtils.SearchTermsForType(entityType);
        return terms;
    }

    /*
    public static bool HasInputImage(Type type)
    {
        string prop = GetInputImagePropertyName(type);
        if( prop == null)
        {
            foreach (var nav in GetNavigation(type))
            {
                if(Types.IsCollectionType(nav.GetType()) == false)
                {
                    prop = GetInputImagePropertyName(nav.GetType());
                    if (prop != null)
                    {
                        break;
                    }
                }
            }
        }
        return prop == null ? false : true;
    }*/

    public static string GetInputImageUrlExpression()
    {
        return @"/api/Resource/Image?entity={{GetType().Name}}&id={{Id}}";
    }
    /*
    public static string GetInputImageUrl(object target)
    {
        string prop = GetInputImagePropertyName(target.GetType());
        if (prop != null)
        {
            string entity = target.GetType().Name;
            int id = int.Parse(ReflectionService.GetValueFor(target, "Id").ToString());
            return $"/api/Resource/Image?entity={entity}&id={id}";
        }
        else 
        {
            foreach (var nav in GetNavigation(target.GetType()))
            {
                if (Types.IsCollectionType(nav.GetType()) == false)
                {
                    prop = GetInputImagePropertyName(nav.GetType());
                    if (prop != null)
                    {
                        target = ReflectionService.GetValueFor(target, nav.Name);
                        if(target != null)
                        {
                            string entity = target.GetType().Name;
                            int id = int.Parse(ReflectionService.GetValueFor(target, "Id").ToString());
                            return $"/api/Resource/Image?entity={entity}&id={id}";
                        }
                        else
                        {
                            return $"/api/Resource/Image";
                        }
                        
                    }
                }
            }
        }
        throw new Exception("Не удалось найти изображение");

    }*/

    internal static string ForHelp(object target)
    {
        return GetTypeAttrValue(target, nameof(HelpMessageAttribute));
    }

    internal static string ForDescription(object target)
    {
        return GetTypeAttrValue(target, nameof(DescriptionAttribute));
    }
    /*
    internal static INavigation GetNavigationKeyFor(string instance, Type targetEntityType)
    {
        var navs = Attrs.GetNavigation(ReflectionService.TypeForName(instance));
        foreach(var nav in navs)
        {
            if(nav.TargetEntityType.Name == targetEntityType.FullName)
            {
                return nav;
            }
        }
        throw new Exception("Не найдено свойство навигации");
    }*/

    public static string GetInputImagePropertyName(Type type)
    {
        return GetInputImagePropertyName(ForAllPropertiesInType(type));
    }

    public static string GetInputImagePropertyName(Dictionary<string, Dictionary<string, string>> attrs)
    {
        foreach (var p in attrs)
        {
            if (attrs[p.Key].ContainsKey(nameof(InputImageAttribute)))
            {
                return p.Key;
            }
        }
        return null;
    }

    public static bool IsManyToManyRelation(Type type, string propertyName)
    {
        return CommonUtils.ForProperty(type, propertyName).ContainsKey(nameof(ManyToManyAttribute));
    }
    public static bool HasManyToManyRelation(Type type, string propertyName)
    {
        return CommonUtils.ForProperty(type, propertyName).ContainsKey(nameof(ManyToManyAttribute));
    }
    /*
    public static IEnumerable<INavigation> GetNavigation(Type type)
    {
        IEnumerable<INavigation> result = null;
        using (var _context = new ApplicationDbContext())
        {
            result = _context.GetNavigationPropertiesForType(type);
        }
        if(result == null)
        {
            return new List<INavigation>();
        }
        else
        {
            return result;
        }
    }*/

    internal static List<string> GetVisibleProperties(Type type)
    {
        List<string> props = new List<string>();
        foreach (string propertyName in ReflectionService.GetPropertyNames(type))
        {
            if (IsVisible(type, propertyName))
            {
                props.Add(propertyName);
            }
        }
        return props;
    }


    public static string[] GetCollectionTypePropertyNames(Type type, string propName)
    {
        return (from p in new List<PropertyInfo>(GetCollectionTypeProperties(type, propName)) select p.Name).ToArray();
    }
    public static PropertyInfo[] GetCollectionTypeProperties(Type type, string propName)
    {
        return ReflectionService.TypeForShortName(GetCollectionType(type, propName)).GetProperties();
    }
    public static Type GetCollectionSystemType(Type type, string propName)
    {
        return ReflectionService.TypeForShortName(GetCollectionType(type, propName));
    }

    public static bool IsCollectionType(Type type, string propName)
    {
        var property = type.GetProperty(propName);
        string TypeName = property.PropertyType.Name;
        bool res = property.PropertyType.Name.StartsWith("List");
        if (res == false)
        {
            Type p = property.PropertyType;
            while (p != typeof(Object) && p != null)
            {
                if ((from pinterface in new List<Type>(p.GetInterfaces()) where pinterface.Name.StartsWith("ICollection") select p).Count() > 0)
                {
                    return true;
                }
                p = p.BaseType;
            }
        }
        return res;
    }

    internal static bool IsInput(Type type, string name)
    {
        return IsInput(ForProperty(type, name));
    }
    internal static bool IsInput(Dictionary<string, string> attrs)
    {
        return attrs.ContainsKey(nameof(NotInputAttribute)) ? false : true;
    }

    public static string GetCollectionType(Type type, string propName)
    {
        var property = type.GetProperty(propName);
        string TypeName = property.PropertyType.Name;

        if (property.PropertyType.Name.StartsWith("List"))
        {

            string text = property.PropertyType.AssemblyQualifiedName;
            text = text.Substring(text.IndexOf("[[") + 2);
            text = text.Substring(0, text.IndexOf(","));
            TypeName = text.Substring(text.LastIndexOf(".") + 1);
            property.Info(property.Name + " " + text);
        }
        return TypeName;
    }

    public static bool IsCollection(Type type, string propName)
    {
        var property = type.GetProperty(propName);
        string TypeName = property.PropertyType.Name;
        bool IsCollection = false;
        if (property.PropertyType.Name.StartsWith("List"))
        {
            IsCollection = true;
            string text = property.PropertyType.AssemblyQualifiedName;
            text = text.Substring(text.IndexOf("[[") + 2);
            text = text.Substring(0, text.IndexOf(","));
            TypeName = text.Substring(text.LastIndexOf(".") + 1);
            type.Info(property.Name + " " + text);
        }
        return IsCollection;
    }


    /// <summary>
    /// Подпись элемента визуализации ассоциированного со заданным свойством 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetInputType(Type type, string name)
    {
        Dictionary<string, string> attrs = CommonUtils.ForProperty(type, name);
        return GetInputType(attrs);
    }




    /// <summary>
    /// Получение атрибута типа поля ввода
    /// </summary>
    /// <param name="attrs"></param>
    /// <returns></returns>
    public static string GetInputType(Dictionary<string, string> attrs)
    {
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
            return key.Replace("Attribute", "").Replace("Input", "");
        }
        else
        {
            return null;
        }
    }









    /// <summary>
    /// Получение атрибутов для объекта
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static Dictionary<string, string> ForObject(object p)
    {

        return ForType(p.GetType());
    }

    public static List<string> SearchTermsForType(Type p)
    {
        List<string> terms = new List<string>();
        Dictionary<string, string> attrs = ForType(p);
        if (attrs.ContainsKey(nameof(SearchTermsAttribute)))
        {
            terms.AddRange(attrs[nameof(SearchTermsAttribute)].Split(","));
        }
        return terms;
    }

    public static Dictionary<string, string> ForType(string p)
    {
        return ForType(ReflectionService.TypeForName(p));
    }
    public static Dictionary<string, string> ForType(Type p)
    {
        Dictionary<string, string> attrs = new Dictionary<string, string>();
        foreach (var data in p.GetCustomAttributesData())
        {
            string key = data.AttributeType.Name;
            foreach (var arg in data.ConstructorArguments)
            {
                string value = arg.Value.ToString();
                attrs[key] = value;
            }

        }
        return attrs;
    }

    internal static bool IsTrueValue(string v)
    {
        return v.ToLower() == "true";
    }

    public static string HelpFor(Type type, string property)
    {
        IDictionary<string, string> attrs = ForProperty(type, property);
        return attrs.ContainsKey(nameof(HelpMessageAttribute)) ? attrs[nameof(HelpMessageAttribute)] : "";

    }

    /// <summary>
    /// Подпись элемента визуализации ассоциированного со заданным свойством 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string LabelFor(Type model, string name)
    {
        Dictionary<string, string> attrs = CommonUtils.ForProperty(model, name);
        if (attrs.ContainsKey(nameof(LabelAttribute)) == false)
        {
            return name;
            //throw new Exception($"Для создания надписи с именем поля ввода " +
            //    $"установите атрибут Label на свойство {name} в классе {model.GetType().Name}");
        }
        else
        {
            return attrs[nameof(LabelAttribute)];
        }
    }


    /// <summary>
    /// Получение значения атрибута для текста надписи
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string LabelFor(Type type)
    {
        Dictionary<string, string> attrs = ForType(type);
        return attrs.ContainsKey(nameof(LabelAttribute)) ? attrs[nameof(LabelAttribute)] :
            attrs.ContainsKey(nameof(LabelAttribute)) ? attrs[nameof(LabelAttribute)] : null;
    }



    public static string DescriptionFor(Type type, string property)
    {
        Dictionary<string, string> attrs = ForProperty(type, property);
        return  
            attrs.ContainsKey(nameof(DescriptionAttribute)) ? attrs[nameof(DescriptionAttribute)] : null;
    }

    public static string LabelFor(object p)
    {
        Dictionary<string, string> attrs = ForObject(p);
        return attrs.ContainsKey(nameof(LabelAttribute)) ? attrs[nameof(LabelAttribute)] :
            attrs.ContainsKey(nameof(DisplayAttribute)) ? attrs[nameof(DisplayAttribute)] : p.GetType().Name;
    }

    public static string DescriptionFor(object p)
    {
        Dictionary<string, string> attrs = ForObject(p);
        return attrs.ContainsKey(nameof(DescriptionAttribute)) ? attrs[nameof(DescriptionAttribute)] : "";
    }

    public static string IconFor(Type type, string property)
    {
        Dictionary<string, string> attrs = ForProperty(type, property);
        return attrs.ContainsKey(nameof(IconAttribute)) ? attrs[nameof(IconAttribute)] : "person";

    }

  

    public static string GetControlType(Type type, string property)
    {
        if (property == nameof(MessageProperty.MessageProtocolId)) { 
            int x = 0;
        }
        var attrs = CommonUtils.ForProperty(type, property);
        var names = (from p in attrs.Keys
                 where p.ToType().IsExtends(typeof(ControlAttribute))
                 select p);
        return names.FirstOrDefault();
    }

    public static string IconFor(string type)
    {
        return IconFor(ReflectionService.TypeForName(type));
    }



    public static string IconFor(Type type)
    {
        Dictionary<string, string> attrs = ForType(type);
        return attrs.ContainsKey(nameof(IconAttribute)) ? attrs[nameof(IconAttribute)] :
            
            null;
    }

    /// <summary>
    /// Проверка флага отображением
    /// </summary>
    /// <param name="type"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public static bool IsVisible(Type type, string property)
    {
        string hidden = ForPropertyValue(type, typeof(InputHiddenAttribute), property);
        return "True" == hidden ? false : true;
    }


    /// <summary>
    /// Получить значения атрибуf заданного для свойства
    /// </summary>
    /// <param name="type"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public static string ForPropertyValue(Type type, Type attr, String property)
    {
        if (type == null)
        {
            throw new Exception("Аргумент " + type + " содержить ссылку на null");
        }
        Dictionary<string, string> attrs = new Dictionary<string, string>();
        if (type == null || type.GetProperty(property) == null)
        {
            throw new Exception("Свойство не найдено либо не задан тип");
        }
        foreach (var data in type.GetProperty(property).CustomAttributes)
        {
            string key = data.AttributeType.Name;
            if (key == attr.Name)
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


    /// <summary>
    /// Получить значения всех атрибутов заданных для свойства
    /// </summary>
    /// <param name="type"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public static Dictionary<string, string> ForProperty2(Type type, String property)
    {
        if (type == null)
        {
            throw new ArgumentNullException();
        }


        Dictionary<string, string> attrs = null;
        PropertyInfo info = null;

        try
        {
            attrs = new Dictionary<string, string>();
            info = type.GetProperty(property);
        }
        catch (AmbiguousMatchException ex)
        {
            ex.Info(ex.Message);
        }

        if (info == null)
        {
            throw new Exception($"Свойство {property} не найдено в объекте типа {type.Name}");
        }
        var datas = info.GetCustomAttributesData();
        if (datas != null)
            foreach (var data in datas)
            {

                string key = data.AttributeType.Name;
                //ParameterInfo[] pars = data.AttributeType.GetConstructors()[0].GetParameters();
                if (data.ConstructorArguments == null || data.ConstructorArguments.Count == 0)
                {
                    attrs[key] = "";
                }
                else
                {
                    foreach (var arg in data.ConstructorArguments)
                    {

                        string value = arg.Value == null ? "" : arg.Value.ToString();
                        attrs[key] = value;
                    }
                }

                //model.Attributes[data.AttributeType] = null;

            }

        if (attrs == null)
        {
            throw new Exception($"Не удалось получить атрибуты свойсва {property} класса {type.Name}");
        }
        return attrs;
    }

    public static Dictionary<string, string> ForMethod(Type controllerType, string name)
    {
        Dictionary<string, string> attrs = new Dictionary<string, string>();
        foreach (var method in controllerType.GetMethods())
        {
            foreach (var data in method.GetCustomAttributesData())
            {
                string key = data.AttributeType.Name;
                foreach (var arg in data.ConstructorArguments)
                {
                    string value = arg.Value.ToString();
                    attrs[key] = value;
                }

            }
        }
        return attrs;
    }


    /// <summary>
    /// Извлечение метода HTTP из атрибутов
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static string ParseHttpMethod(Dictionary<string, string> attributes)
    {
        foreach (var p in attributes)
        {
            switch (p.Key)
            {
                case "HttpPostAttribute":
                    return "GET";
                case "HttpPutAttribute":
                    return "PUT";
                case "HttpPatchAttribute":
                    return "PATCH";
                case "HttpDeleteAttribute":
                    return "DELETE";
                default: return "GET";
            }
        }
        return "GET";
    }




    /// <summary>
    /// Выбор значения атрибута DataType
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static string GetDataType(Dictionary<string, string> attributes)
    {
        foreach (var p in attributes)
        {
            switch (p.Key)
            {
                case "DataTypeAttribute":
                    switch (p.Value)
                    {
                        case "0": return "custom";
                        case "1": return "datetime";
                        case "2": return "date";
                        case "3": return "time";
                        case "4": return "duration";
                        case "5": return "phone";
                        case "6": return "currency";
                        case "7": return "text";
                        case "8": return "html";
                        case "9": return "textarea";
                        case "10": return "email";
                        case "11": return "password";
                        case "12": return "url";
                        case "13": return "image";
                        case "14": return "creditCard";
                        case "15": return "postalCode";
                        case "16": return "upload";
                        default: throw new Exception("Неизвестный тип данных");
                    }

            }
        }
        return null;
    }

    internal static Dictionary<string, Dictionary<string, string>> ForAllPropertiesInType2(Type type)
    {
        Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
        foreach (var prop in type.GetProperties())
        {
            Dictionary<string, string> forProperty = CommonUtils.ForProperty(type, prop.Name);
            result[prop.Name] = forProperty;
        }
        return result;
    }


    public static bool IsUniq(Dictionary<string, string> attributes)
    {
        return attributes.ContainsKey(nameof(UniqValueAttribute));
    }

    internal static string GetUniqProperty(Dictionary<string, Dictionary<string, string>> attrs)
    {
        foreach (var p in attrs)
        {
            if (IsUniq(attrs[p.Key]))
            {
                return p.Key;
            }
        }
        return null;
    }

    internal static Attribute[] ForPropertyLikeAttrubtes(Type type, string property)
    {
        var attrs = new List<Attribute>();
        if (type == null)
        {
            throw new ArgumentNullException();
        }



        PropertyInfo info = type.GetProperty(property);
        if (info == null)
        {
            throw new Exception($"Свойство {property} не найдено в объекте типа {type.Name}");
        }
        foreach (var data in info.GetCustomAttributesData())
        {
            string key = data.AttributeType.Name;

            if (data.ConstructorArguments == null || data.ConstructorArguments.Count == 0)
            {
                Attribute attr = ReflectionService.Create<Attribute>(key, new object[0]);
                attrs.Add(attr);
            }
            else
            {
                List<object> parameters = new List<object>();
                foreach (CustomAttributeTypedArgument arg in data.ConstructorArguments)
                {
                    parameters.Add(arg.Value);
                }
                Attribute attr = ReflectionService.Create<Attribute>(key, parameters.ToArray());
                attrs.Add(attr);

            }

            //model.Attributes[data.AttributeType] = null;

        }
        return attrs.ToArray();
    }

    internal static string ForManyToMany(Type type, string bindingGroup)
    {
        return CommonUtils.ForProperty(type, bindingGroup)[nameof(ManyToManyAttribute)];
    }
    internal static Dictionary<string, Dictionary<string, string>> ForAllPropertiesInType(Type type)
    {
        Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
        foreach (var prop in type.GetProperties())
        {
            Dictionary<string, string> forProperty = CommonUtils.ForProperty(type, prop.Name);
            result[prop.Name] = forProperty;
        }
        return result;
    }
    /*
   public static bool IsActiveObject(Type type)
   {
       return IsExtendedFrom(type, nameof(ActiveObject));
   }


   public static bool IsDictionaryTable(Type type)
   {
       return IsExtendedFrom(type, nameof(DictionaryTable));
   }

   public static bool IsDimensionTable(Type type)
   {
       return IsExtendedFrom(type, nameof(DictionaryTable ));
   }

   public static bool IsFactsTable(Type type)
   {
       return IsExtendedFrom(type, nameof(EventLog));
   }





   public static bool IsHierDictinary(Type entityType )
   {
       bool isHier = false;
       Type p = entityType;
       while (p != typeof(Object) && p != null)
       {
           if (p.Name.StartsWith("HierDictionaryTable"))
           {
               isHier = true;
               break;
           }
           p = p.BaseType;
       }

       return isHier;
   }*/
    public static string ParseCollectionType(Type type)
    {
        if(type.IsArray)
        {
            return type.GetTypeName().Replace("[]", "");
        }
        
        var types = type.GetGenericArguments();
        //types = types.Count() == 0 ? type.GetExtendedTypes().FirstOrDefault(t => t.GetGenericArguments().Count() > 0)?.GetGenericArguments() : types;
        if (types is null || types.Length == 0 || types.Length > 1)
        {

            var name = type.GetTypeName();
            var s = name.IndexOf("<");
            var e = name.LastIndexOf(">");
            if(s >0 && e > 0)
            {
                var gtypes = name.Substring(s + 1, e - s - 2);
                return gtypes.Split(",").First();
            }
            
            type.Error($"Тип {type.GetTypeName()} не является обобщением.");
            throw new ArgumentException(nameof(type), $"Тип {type.GetTypeName()} не является обобщением.");
        }
        else
        {
            return types[0].GetTypeName();
        }
        
    }
    public static HashSet<string> PRIMITIVE_TYPES = new HashSet<string>() {
            "Byte[]", "System.Byte[]", "Single","String", "Boolean", "System.String", "string", "int","long","float",
        "Nullable<System.Boolean>", "Double", "Nullable<System.Double>",
        "Int16", "Nullable<Int16>", "Int32", "Nullable<System.Int32>",
        "Int64", "Nullable<System.Int64>", "UInt16", "UInt32", "UInt64",
        "DateTime", "Nullable<System.DateTime>" };
    public static readonly IEnumerable<string> INPUT_TYPES = new HashSet<string>(ReflectionService.GetPublicStaticFieldNames(typeof(InputTypes)));

    public static readonly IEnumerable<string> NUBMER_TYPES = new HashSet<string>() {
              "System.Decimal",  "Decimal", "Nullable<System.Decimal>", "System.Float",
        "Float", "Nullable<System.Float>", "System.Double",  "Double", "Nullable<System.Double>",
        "Int16", "System.Int16", "Nullable<System.Int16>",
        "Int32", "System.Int32", "Nullable<System.Int32>",
        "Int64", "System.Int64", "Nullable<System.Int64>",
        "UInt16", "System.UInt16", "Nullable<System.UInt16>",
        "UInt32", "System.UInt32", "Nullable<System.UInt32>",
        "UInt64", "System.UInt64", "Nullable<System.UInt64>"  };
    public static readonly IEnumerable<string> TEXT_TYPES = new HashSet<string>() {
            "String,System.String" };

    static object CreateCollectionType(Type type)
    {
        throw new NotImplementedException( );
    }

    static object CreaCollectionType(Type type)
    {
        return new List<object>();
    }

    public static readonly IEnumerable<string> LOGICAL_TYPES = new HashSet<string>() {
            "Boolean","System.Boolean","Nullable<System.Boolean>", };


    public static bool IsExtendedFrom(Type targetType, string baseType)
    {
        return IsExtendedFromType(targetType, ReflectionService.TypeForName(baseType));
    }

    public static bool IsExtendedFromType(Type targetType, Type baseType)
    {
        if (targetType == null)
        {
            //Api.Utils.Info(Environment.StackTrace);
            throw new ArgumentNullException("targetType");
        }
        if (baseType == null)
        {
            //Api.Utils.Info(Environment.StackTrace);
            throw new ArgumentNullException("baseType");
        }
        Type typeOfObject = new object().GetType();
        Type p = targetType;
        while (p != typeOfObject && p != null)
        {
            if (p.Name == baseType.Name)
            {
                return true;
            }
            p = p.BaseType;
        }
        return false;
    }

    public static bool IsCollectionType(Type type)
    {
        Type p = type;
        while (p != typeof(Object) && p != null)
        {
            if ((from pinterface in new List<Type>(p.GetInterfaces())
                 where pinterface.IsExtends(typeof(IEnumerable))
                 select p).Count() > 0)
            {
                return true;
            }
            p = p.BaseType;
        }
        return false;
    }
    public static object ParseProperty(Type type, PropertyInfo property)
    {
        string TypeName = property.PropertyType.Name;
        bool IsCollection = false;
        if (property.PropertyType.Name.StartsWith("List"))
        {
            IsCollection = true;
            string text = property.PropertyType.AssemblyQualifiedName;
            text = text.Substring(text.IndexOf("[[") + 2);
            text = text.Substring(0, text.IndexOf(","));
            TypeName = text.Substring(text.LastIndexOf(".") + 1);
            type.Info(property.Name + " " + text);
        }
        object prop = new
        {
            Name = property.Name,
            IsCollection = IsCollection,
            Type = TypeName,
            Attributes = type.GetPropertyAttributes( property.Name)
        };
        return prop;
    }
    public static bool HasBaseType(Type targetType, string baseType)
    {
        return HasBaseType(targetType, ReflectionService.TypeForName(baseType));
    }
    public static bool HasBaseType(string targetType, Type baseType)
    {
        return HasBaseType(ReflectionService.TypeForName(targetType), baseType);
    }
    public static bool HasBaseType(string targetType, string baseType)
    {
        return HasBaseType(ReflectionService.TypeForName(targetType), ReflectionService.TypeForName(baseType));
    }

    public static bool HasBaseType( Type targetType, Type baseType )
    {
        if (targetType == null)
            throw new Exception("Тип не определён");
        Type p = targetType.BaseType;
        while (p != typeof(Object) && p != null)
        {
            if (p.Name == baseType.Name)
            {
                return true;                
            }
            p = p.BaseType;
        }
        return false;
    }



    public static bool IsDateTime( PropertyInfo property )
    {
        string propertyType = ParsePropertyType(property.PropertyType);
        if(propertyType == "System.DateTime" || propertyType == "DateTime" || propertyType == "Nullable<DateTime>" || propertyType == "Nullable<System.DateTime>")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsNullable(PropertyInfo property)
    {
        string propertyType = ParsePropertyType(property.PropertyType);
        return propertyType.StartsWith("Nullable");
    }

     

    public static string ParsePropertyType(Type propertyType)
    {
        string name = propertyType.Name;
        if( name.Contains("`"))
        {
            string text = propertyType.AssemblyQualifiedName;
            text = text.Substring(text.IndexOf("[[") + 2);
            text = text.Substring(0, text.IndexOf(","));
            name = name.Substring(0, name.IndexOf("`"))+"<"+text+">";

        }
        return name;
    }



    /// <summary>
    /// Метод получения описателя вызова статических методов 
    /// </summary>
    /// <param name="type"> тип </param>
    /// <returns> описание статических методов </returns>
    public static Dictionary<string, object> GetStaticMethods(Type type)
    {     
        Dictionary<string, object> actionMetadata = new Dictionary<string, object>();
        foreach (MethodInfo info in type.GetMethods())
        {
            if (info.IsPublic && info.IsStatic)
            {
                Dictionary<string, object> args = new Dictionary<string, object>();
                foreach (ParameterInfo pinfo in info.GetParameters())
                {
                    args[pinfo.Name] = new
                    {
                        type = pinfo.ParameterType.Name,
                        optional = pinfo.IsOptional,
                        name = pinfo.Name
                    };
                }
            }
        }
        return actionMetadata;
    }
    public static List<string> GetEventListeners(this Type p)
    {
        List<string> listeners = new List<string>();
        foreach (EventInfo evt in p.GetEvents())
        {
            listeners.Add(evt.Name.ToLower());
        }
        return listeners;
    }
    public static bool IsNumber(this PropertyInfo propertyInfo)
    {
        return NUBMER_TYPES.Contains(ParsePropertyType(propertyInfo.PropertyType));
    }
    public static bool IsNumber(this Type propertyInfo)
    {
        return NUBMER_TYPES.Contains(ParsePropertyType(propertyInfo));
    }
    public static bool IsText(PropertyInfo propertyInfo)
    {
        return TEXT_TYPES.Contains(ParsePropertyType(propertyInfo.PropertyType));
    }

    public static bool IsPrimitive(string propertyType)
    {
        Type type = ReflectionService.TypeForName(propertyType);
        
        return PRIMITIVE_TYPES.Contains(ParsePropertyType(type));
    }

    public static bool IsPrimitive(Type propertyType)
    {
        return PRIMITIVE_TYPES.Contains(ParsePropertyType(propertyType));
    }

    public static bool IsPrimitive(Type modelType, string property)
    {
        return PRIMITIVE_TYPES.Contains(ParsePropertyType(modelType.GetProperty(property).PropertyType));
    }

    public static bool IsBoolean(PropertyInfo propertyInfo)
    {
        return LOGICAL_TYPES.Contains(ParsePropertyType(propertyInfo.PropertyType));
    }

    public static bool ReferenceIsDictionary(object properties)
    {
        return properties.GetType().Name.Contains("Dictionary");
    }

    public static Dictionary<string, string> ForProperty(Type type, string key)
    {
        return type.GetPropertyAttributes(key);
    }

    
}
