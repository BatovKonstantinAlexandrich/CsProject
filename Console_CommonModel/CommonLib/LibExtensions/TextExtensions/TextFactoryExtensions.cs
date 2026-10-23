
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


/// <summary>
/// Расширения к строкового типа
/// </summary>
public static class TextFactoryExtensions
{

    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static void ForEachProperty(this object target, Func<string, object, bool> todo)
    {
        target.GetType().GetProperties().ToList().ForEach(p => todo(  p.Name, target.GetValue(p.Name)  )  );
    }
    public static void ForEachInputProperty(this object target, Func<string, object, bool> todo)
    {
        target.GetType().GetInputProperties().ToList().ForEach(p => todo(p, target.GetValue(p)));
    }
    public static void ForEachJsonProperty(this object target, Func<string, object, bool> todo)
    {
        target.GetType().GetInputProperties().Where(p => target.GetType().GetPropertyAttributeValue<JsonIgnoreAttribute>(p, null) is null).ToList().ForEach(p => todo(p, target.GetValue(p)));
    }
    public static Dictionary<string, string> ToTextDictionary(this object target)
    {
        var result = new Dictionary<string, string>();
        if(target.IsExtends(typeof(Dictionary<string,object>)))
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)target;
            foreach(var p in dict.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value?.ToString())))
            {
                result[p.Key] = p.Value;
            }
            return result;
        }
        else
        {
            var properties = target.GetInputJsonProperties().ToList();
            target.Info(properties.ToJsonOnScreen());
            foreach(var property in properties)
            {
                result[property] = $"{target.GetValue(property)}";
            }
            target.Info(result.ToJsonOnScreen());
            return result;
        }
        
    }

    public static string ParseSubstring(this string text, string start, string end)
    {
        if (text == null)
            throw new ArgumentNullException("text");
        if (text.IndexOf(start) != -1)
        {
            var substr = text.Substring(text.IndexOf(start) + start.Length, text.IndexOf(end) - text.IndexOf(start) - start.Length);
            return substr;
        }
        throw new ArgumentException($"Строка: {text} не содержит подстроки {start}");
    }

    public static Type ToType(this string text)
    {
        try
        {
            if (text.IndexOf(".") != -1)
                text = text.Substring(text.LastIndexOf(".") + 1);
            if (text == null)
                throw new ArgumentNullException("text");
            if (text.IndexOf("<") != -1)
            {
                var genericArgs = text.Substring(text.IndexOf("<") + 1, text.LastIndexOf(">") - text.IndexOf("<")-1).Split(",");
                var genericArgsTypes = genericArgs.Select(a => a.ToType()).ToArray();

                var mainType = text.Substring(0, text.IndexOf("<"));

                mainType += $"`{genericArgs.Count()}";
                return mainType.ToType().MakeGenericType(genericArgsTypes);
                 
            }
            /*Type ptype = text.Contains(".")? ReflectionService.TypeForName(text): 

                ReflectionService.TypeForShortName(text);

            if(ptype == null)
            {
                ptype = text.Contains(".") ? ServiceFactory.Get().TypeForName(text) : ServiceFactory.Get().TypeForShortName(text);  
            }
            if(ptype is null)
            {
                throw new ArgumentException($"Не удалось найти тип: {text}");
            }*/
            var result = ServiceFactory.Get().GetType(text);
            return result;
            //return ptype;
        }
        catch (Exception e)
        {
            e.Error(e);
            return null;
            //throw new Exception($"Не удалось получить тип по имени {text} => {e.Message}\n{e.StackTrace}");
        }
    }
    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static T New<T>(this string text)
    {
        if (text == null)
            throw new ArgumentNullException("text");
        return (T)ReflectionService.CreateWithDefaultConstructor<object>(text.ToType());
    }
    public static T New<T>(this string text, params object[] args)
    {
        if (text == null)
            throw new ArgumentNullException("text");
        return (T)ReflectionService.Create<T>(text.ToType(),args);
    }
    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static object New(this string text)
    {
        try
        {
            if (text == null)
                throw new ArgumentNullException("text");
            var ptype = text.ToType();
            return ReflectionService.CreateWithDefaultConstructor<object>(ptype);
        }
        catch(Exception ex) 
        {
            text.Error("Не удалось создать экземпляр типа "+text+" "+ex.Message);
            throw;
        }
    }
    public static object New(this string text, params object[] args)
    {
        try
        {
            if (text == null)
                throw new ArgumentNullException("text");
            var ptype = text.ToType();
            return ReflectionService.Create<object>(ptype, args);
        }
        catch (Exception ex)
        {
            text.Error("Не удалось создать экземпляр типа " + text + " " + ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static object New(this Type type)
    {
        //type.Debug($"{type.GetTypeName()}");
        if (type == null)
            throw new ArgumentNullException("type");
        if(type.IsInterface)
        {
            return ServiceFactory.Get().GetTypesImplemented(type).FirstOrDefault(t => !t.IsAbstract && t.IsPublic && t.GetConstructors().Any(c => c.GetParameters().Count()==0)).New();
        }
        else
        {
            return ReflectionService.CreateWithDefaultConstructor<object>(type);
        }
        
    }

    public static T New<T>(this Type type) where T: class
    {

        if (type == null)
            throw new ArgumentNullException("type");

        return ReflectionService.CreateWithDefaultConstructor<T>(type);
    }

    public static List<PropertyInfo> GetPropertiesByType<T>(this Type ptype) where T : class
    {
        return ptype.GetProperties().Where(p => p.PropertyType == typeof(T) || p.PropertyType.IsExtends(typeof(T)) || p.PropertyType.IsImplements(typeof(T))).ToList();
    }

    public static List<PropertyInfo> GetPropertiesByType(this Type ptype, Type baseType)  
    {
        return ptype.GetProperties().Where(p => p.PropertyType == baseType || p.PropertyType.IsExtends(baseType) || p.PropertyType.IsImplements(baseType)).ToList();
    }

    public static T New<T>(this Type type, params object[] args) where T : class
    {
        if (type == null)
            throw new ArgumentNullException("type");
        return ReflectionService.Create<T>(type, args);
    }


    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static object New(this Type type, object[] args)
    {
        if (type == null)
            throw new ArgumentNullException("type");
        return ReflectionService.Create<object>(type, args);
    }


    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static object Copy(this object type, IDictionary<string,object> keyValues)
    {
        var names = type.GetType().GetProperties().Select(p => p.Name);
        foreach (var kv in keyValues)
        {
            if (names.Contains(kv.Key))
            {
                Setter.SetValue(type,kv.Key, kv.Value); 

            }
        }
        return type;
    }

    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static object CopyObject(this object type, object keyValues)
    {
        var names = type.GetType().GetProperties().Select(p => p.Name);
        foreach (var kv in keyValues.ToJson().FromJson<Dictionary<string,object>>())        {
            if (names.Contains(kv.Key))
            {
                try
                {
                    if (Typing.IsCollectionType(type.GetType().GetProperty(kv.Key).PropertyType) == false)
                        Setter.SetValue(type, kv.Key, kv.Value);
                }
                catch(Exception ex)
                {
                    ex.Info($"Исключение при копировании данных в поле с ключом {kv.Key} => {ex.Message}");
                }

            }
        }
        return type;
    }

    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static object Create(this Type type, object[] parameters)
    {
        return ReflectionService.Create<object>(type, parameters);
    }
    public static object CallGeneric(this object p, Type generic, string action, IEnumerable<object> parameters)
    {
        try
        {
            object ActionResult = null;
            Type type = p.GetType();
            var method = type.GetMethods().Where(m => m.Name == action && m.GetGenericArguments().Count()>0).FirstOrDefault();
            method = method.MakeGenericMethod(generic);
            try
            {
                ActionResult = method.Invoke(p, parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение проброшено из метода {TypeExtensions2.GetTypeName(type)}.{action}: {ex.Message}", ex);
            }
            return ActionResult;
        }
        catch (Exception ex)
        {
            throw new Exception($"Искулючение при выполнении динамического вызова {nameof(Call)}({p.GetType().GetTypeName()},{action},{parameters.ToJson()})");
        }
    }
    public static object Call(this object p, string action, IEnumerable<object> parameters)
    {
        try
        {
            object ActionResult = null;
            Type type = p.GetType();
            var method = type.GetMethods().Where(m => m.Name == action).FirstOrDefault();
            try
            {
                ActionResult = method.Invoke(p, parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение проброшено из метода {TypeExtensions2.GetTypeName(type)}.{action}: {ex.Message}", ex);
            }
            return ActionResult;
        }
        catch(Exception ex)
        {
            throw new Exception($"Искулючение при выполнении динамического вызова {nameof(Call)}({p.GetType().GetTypeName()},{action},{parameters.ToJson()})");
        }
    }
    public static object Call(this object p, string action,  Dictionary<string,object> parameters)
    {
        object ActionResult = null;
        Type type = p.GetType();


        var setter = new TextDataSetter();
        
     
        var method = type.GetMethods().Where(m => m.Name == action).FirstOrDefault();        
        try
        {
            var parr = new List<object>();
            foreach(var name in method.GetParameters().Select(p=>p.Name))
            {
                if(parameters.ContainsKey(name))
                {
                    var typeName = method.GetParameters().First(p => p.Name == name).ParameterType.GetTypeName();
                    if (parameters[name] is null)
                    {
                        parr.Add(null);
                    }
                    else
                    {
                        var textValue = parameters[name].ToString();
                        var argValue = TextDataSetter.FromText(textValue, typeName);
                        parr.Add(argValue);
                    }
                }
                else
                {
                    throw new ArgumentException(name, "Отсутсвует фактическое значение параметры выпол");
                }                
            }
            ActionResult = method.Invoke(p, parr.ToArray());
        }
        catch (Exception ex)
        {
            throw new Exception($"Исключение проброшено из метода {TypeExtensions2.GetTypeName(type)}.{action}: {ex.Message}", ex);
        }
        return ActionResult;
    }
    public static T Create<T>(this Type type, object[] parameters)
    {
        return (T)ReflectionService.Create<T>(type, parameters);
    }
    public static T Create<T>(this Type type)
    {
        return ReflectionService.Create<T>(type, new object[0]);
    }
    public static T Invoke<T>(this Type type, string action, Dictionary<string,string> args)
    {
        return ReflectionService.Create<T>(type, new object[0]);
    }

    /// <summary>
    /// Поиск типа по имени
    /// </summary>
    public static object New(this string text, string deps)
    {
        var constructor = text.ToType().GetConstructors()[0];
        return constructor.Invoke(new object[] { deps });
    }

}