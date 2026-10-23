//using Newtonsoft.Json;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//public static class Extensions2
//{
//    public static List<string> GetOwnMethodNames(this object target)
//    {
//        Type type = target is Type ? (Type)target : target.GetType();
//        return type.GetMethods().Where(m => m.Name.StartsWith("get_") == false && m.Name.StartsWith("set_") == false && (type.BaseType is not null && type.BaseType.GetMethods().Select(m2 => m2.Name).Contains(m.Name) == false)).Select(m => m.Name).ToList();
//    }
//    public static IEnumerable<Type> GetClassTypes(this Assembly assembly) =>
//        assembly.GetTypes().Where(t => t.IsClass == true && ("" + t.Name[0]).IsEng());
//    public static object InvokeGenericMethod(this object target, string name, List<Type> geneticTypes, List<object> args)
//    {
//        MethodInfo method = target.GetType().GetMethods().First(m => m.Name == name);
//        method = method.MakeGenericMethod(geneticTypes.ToArray());

//        object[] pars = args.ToArray();
//        return method.Invoke(target, pars);
//    }
//    public static object InvokeGenericMethod(this object target, string name, List<Type> geneticTypes, Dictionary<string, object> args)
//    {
//        MethodInfo method = target.GetType().GetMethods().First(m => m.Name == name);
//        method = method.MakeGenericMethod(geneticTypes.ToArray());

//        object[] pars = method.GetParameters().Select(par => args[name]).ToArray();
//        return method.Invoke(target, pars);
//    }
//    public static IEnumerable<Type> GetClassTypes(this Assembly assembly) =>
//        assembly.GetTypes().Where(t => t.IsClass == true && (("" + t.Name[0])).IsEng());

//    public static TTable FromJson<TTable>(this string target)
//    {
//        return JsonConvert.DeserializeObject<TTable>(target);
//    }



//    ///// <summary>
//    ///// Сериаллизация объекта в JSON
//    ///// </summary>    
//    //public static string ToJson(this object target)
//    //{
//    //    try
//    //    {
//    //        if (target == null)
//    //            throw new ArgumentNullException("target");
//    //        return JsonConvert.SerializeObject(target, CreateSafeJsonSettings(Formatting.None));
//    //    }
//    //    catch (JsonException ex)
//    //    {
//    //        throw new Exception($"Ошибка при сериаллизации в json объекта" +
//    //            $" {(target == null ? "null" : target.GetTypeName())} => ", ex);
//    //    }
//    //}
//    //public static string ToJsonOnScreen(this object target)
//    //{
//    //    string result = string.Empty;
//    //    try
//    //    {
//    //        result = JsonConvert.SerializeObject(target, CreateSafeJsonSettings(Newtonsoft.Json.Formatting.Indented));
//    //    }

//    //    catch (Exception ex)
//    //    {
//    //        result = ex.ToString();
//    //        Console.WriteLine(result);
//    //        throw new Exception($"Не удалось сериаллизовать объект типа {target.GetTypeName()} в JSON");
//    //    }
//    //    return result;
//    //}
//    //public static void Error(this object target, params object[] messages)
//    //{
//    //    Console.ForegroundColor = ConsoleColor.Yellow;
//    //    if (messages.Length > 0 && messages[0] is Exception)
//    //    {
//    //        var list = messages.ToList();
//    //        Exception ex = (Exception)messages[0];
//    //        list.RemoveAt(0);
//    //        target.Error(ex, list.ToArray());
//    //    }
//    //    else if (messages.Length > 0 && messages[messages.Length - 1] is Exception)
//    //    {
//    //        var list = messages.ToList();
//    //        Exception ex = (Exception)messages[messages.Length - 1];
//    //        list.RemoveAt(list.Count - 1);
//    //        target.Error(ex, list.ToArray());
//    //    }
//    //    else
//    //    {
//    //        string message = "";
//    //        foreach (var line in messages)
//    //            message += line + "\n";
//    //        target.Info(message);
//    //    }


//    //    Console.ResetColor();
//    //    string[] args = new string[] { };



//    //}


//    public static void Error(this object target, Exception ex, params object[] messages)
//    {
//        Console.Clear();


//        System.Console.WriteLine(ex);
//        while (ex != null)
//        {


//            Console.ForegroundColor = ConsoleColor.Cyan;
//            System.Console.WriteLine($"\n[{ex.Source}][{ex.TargetSite}] => {ex.Message}");
//            Console.ResetColor();
//            System.Console.WriteLine();
//            ex = ex.InnerException;
//        }

//        target.Info(messages);

//    }
//    //public static void Info(this object p, params object[] arr)
//    //{
//    //    foreach (object o in arr)
//    //    {
//    //        Console.WriteLine(o);
//    //    }
//    //}
//    private static JsonSerializerSettings CreateSafeJsonSettings(Newtonsoft.Json.Formatting formatting)
//    {
//        return new JsonSerializerSettings()
//        {
//            Formatting = formatting,
//            NullValueHandling = NullValueHandling.Ignore,
//            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
//            Error = (sender, args) =>
//            {
//                args.ErrorContext.Handled = true;
//            }
//        };
//    }

//    //public static List<string> GetOwnPropertyNames(this Type type)
//    //{
//    //    var properties = type.GetProperties();
//    //    var own = properties.Where(p => p.DeclaringType.GetTypeName() == type.GetTypeName()).Select(p => p.Name).ToList();
//    //    return own;
//    //}
//    //public static string GetNameOfType(this Type propertyType)
//    //{
//    //    try
//    //    {
//    //        if (propertyType == null)
//    //            throw new ArgumentNullException("type");
//    //        string name = propertyType.Name;
//    //        if (name == null) return "";
//    //        if (name.IndexOf("`") != -1)
//    //            name = name.Substring(0, name.IndexOf("`"));

//    //        var arr = propertyType.GetGenericArguments();
//    //        if (arr.Length > 0)
//    //        {
//    //            name += '<';
//    //            foreach (var arg in arr)
//    //            {
//    //                name += arg.GetNameOfType() + ",";
//    //            }
//    //            name = name.Substring(0, name.Length - 1);
//    //            name += '>';
//    //        }
//    //        return name;

//    //    }
//    //    catch (Exception)
//    //    {
//    //        throw;
//    //    }
//    //}

//    //public static string GetTypeName(this object target)
//    //{
//    //    if (target is null) return "";
//    //    Type targetType = target is Type ? ((Type)target) : target.GetType();
//    //    string result = targetType.GetNameOfType();
//    //    return result;
//    //}
//    public static string GetOwnTypeName(this object target)
//    {
//        Type targetType = target is Type ? ((Type)target) : target.GetType();
//        string result = targetType.GetNameOfType();
//        int? iend = Enumerable.Range(0, result.Length).FirstOrDefault(i => result[i].ToString().IsEng() == false);
//        iend = iend is null ? result.Length : iend;
//        result.Substring(0, (int)iend);
//        return result;
//    }

//    private static readonly string ENG_CHARS = "qwertyuiopasdfghjklzxcvbnm" + "qwertyuiopasdfghjklzxcvbnm".ToUpper();
//    //public static bool IsEng(this string word)
//    //{
//    //    string alf = ENG_CHARS;
//    //    string text = word;
//    //    for (int i = 0; i < text.Length; i++)
//    //    {
//    //        if (!alf.Contains(text[i]))
//    //        {
//    //            return false;
//    //        }
//    //    }
//    //    return true;
//    //}
//    /// <summary>
//    /// Возвращает существительное во множественном числе
//    /// </summary>
//    /// <param name="table"></param>
//    /// <returns></returns>
//    public static string GetMultiCountName(this string table)
//    {
//        //определение наименования в множественном числе и единственном                        
//        string tableName = table;
//        string multicount_name = null;
//        if (tableName.EndsWith("s"))
//        {
//            if (tableName.EndsWith("ies"))
//            {
//                multicount_name = tableName;
//            }
//            else
//            {
//                multicount_name = tableName;
//            }
//        }
//        else
//        {
//            if (tableName.EndsWith("y"))
//            {
//                multicount_name = tableName.Substring(0, tableName.Length - 1) + "ies";
//            }
//            else
//            {
//                multicount_name = tableName + "s";
//            }
//        }
//        return multicount_name;
//    }
//    //public static bool IsType(this string text)
//    //{
//    //    try
//    //    {
//    //        return ServiceFactory.Get().Has(text);
//    //    }
//    //    catch (Exception)
//    //    {
//    //        return false;
//    //    }
//    //}

//    //public static Type ToType(this string text)
//    //{
//    //    try
//    //    {
//    //        if (text == null)
//    //            throw new ArgumentNullException("text");
//    //        if (text.IndexOf("<") != -1)
//    //        {
//    //            var genericArgs = text.Substring(text.IndexOf("<") + 1, text.IndexOf(">") - text.IndexOf("<") - 1).Split(",");

//    //            var mainType = text.Substring(0, text.IndexOf("<"));

//    //            mainType += $"`{genericArgs.Count()}";
//    //            if (genericArgs.All(a => a.IsType()))
//    //            {
//    //                var genericTypes = genericArgs.Select(parg => parg.ToType()).ToArray();
//    //                return mainType.ToType().MakeGenericType(genericTypes);

//    //            }
//    //            else
//    //            {
//    //                return mainType.ToType();
//    //            }
//    //        }
//    //        /*Type ptype = text.Contains(".")? ReflectionService.TypeForName(text): 

//    //            ReflectionService.TypeForShortName(text);

//    //        if(ptype == null)
//    //        {
//    //            ptype = text.Contains(".") ? ServiceFactory.Get().TypeForName(text) : ServiceFactory.Get().TypeForShortName(text);  
//    //        }
//    //        if(ptype is null)
//    //        {
//    //            throw new ArgumentException($"Не удалось найти тип: {text}");
//    //        }*/
//    //        var result = ServiceFactory.Get().GetType(text);
//    //        return result;
//    //        //return ptype;
//    //    }
//    //    catch (Exception e)
//    //    {
//    //        System.Console.WriteLine(e);
//    //        throw new Exception($"Не удалось получить тип по имени {text} => {e.Message}\n{e.StackTrace}");
//    //    }
//    //}
//    //public static bool IsImplementsFrom(this Type targetType, Type baseType)
//    //{
//    //    Type typeOfObject = new object().GetType();
//    //    Type p = targetType;
//    //    while (p != typeOfObject && p != null)
//    //    {
//    //        if (p.Name == baseType.Name || p.GetInterfaces().Select(i => i.GetTypeName()).Contains(baseType.GetTypeName()))
//    //        {
//    //            return true;
//    //        }
//    //        p = p.BaseType;
//    //    }
//    //    return false;
//    //}
//    //public static bool IsExtends(this System.Object type, Type baseType)
//    //{
//    //    if (type is Type)
//    //        return ((Type)type).IsExtendsFrom(baseType);
//    //    else return type.GetType().IsExtendsFrom(baseType);
//    //}
//    //public static bool IsImplements(this System.Object type, Type baseType)
//    //{
//    //    if (type is Type)
//    //        return ((Type)type).IsImplementsFrom(baseType);
//    //    else return type.GetType().IsImplementsFrom(baseType);
//    //}
//    //public static bool IsExtendsFrom(this Type type, Type baseType)
//    //{

//    //    Type typeOfObject = new object().GetType();
//    //    Type p = type;
//    //    while (p != typeOfObject && p != null)
//    //    {
//    //        var n1 = p.GetTypeName();
//    //        var n2 = baseType.GetTypeName();
//    //        //System.Console.WriteLine($"{n1}={n2}");
//    //        if (n1 == n2)
//    //        {
//    //            return true;
//    //        }
//    //        if (baseType.IsGenericTypeDefinition)
//    //        {

//    //        }
//    //        p = p.BaseType;
//    //    }
//    //    return false;
//    //}

//    public static List<Type> GetExtendedTypes(this Type ptype)
//    {
//        Type typeOfObject = new object().GetType();
//        Type p = ptype;
//        List<Type> extTypes = new();
//        while (p != typeOfObject && p != null)
//        {
//            p = p.BaseType;
//            if (p is not null)
//            {
//                extTypes.Add(p);
//            }
//        }
//        return extTypes;
//    }
//    public static bool IsFile(this string path)
//     => System.IO.File.Exists(path);
//    public static IEnumerable<object> GetArgumentsFromDictionary(this MethodBase methodInfo, Dictionary<string, object> arguments)
//    {
//        var parNames = methodInfo.GetParameters().Select(p => p.Name).ToList();
//        var argsInvoke = parNames.Select(name => arguments.ContainsKey(name) ? arguments[name] : null).ToArray();
//        return argsInvoke;
//    }
//    //public static List<string> GetOwnMethodNames(this Type type)
//    //{
//    //    return type.GetMethods().Where(m => m.Name.StartsWith("get_") == false && m.Name.StartsWith("set_") == false && type.BaseType.GetMethods().Select(m2 => m2.Name).Contains(m.Name) == false).Select(m => m.Name).ToList();
//    //}

//    //public static IDictionary<string, IEnumerable<string>> Validate(this MethodInfo methodInfo, Dictionary<string, object> arguments)
//    //{
//    //    var parNames = methodInfo.GetParameters().Select(p => p.Name).ToList();
//    //    var argsInvoke = parNames.Select(name => arguments.ContainsKey(name) ? arguments[name] : null).ToArray();
//    //    return methodInfo.Validate(argsInvoke);
//    //}

//    //public static IDictionary<string, IEnumerable<string>> Validate(this MethodBase methodInfo, object[] arguments)
//    //{
//    //    //TODO;
//    //    return new Dictionary<string, IEnumerable<string>>();
//    //}

//    public static T New<T>(this Type type, params object[] args) where T : class
//    {
//        if (type == null)
//            throw new ArgumentNullException("type");
//        return RCreate<T>(type, args);
//    }
//    public static bool IsExtends(this System.Object type, Type baseType)
//    {
//        if (type is Type)
//            return ((Type)type).IsExtendsFrom(baseType);
//        else return type.GetType().IsExtendsFrom(baseType);
//    }
//    public static object FromJsonType(this string target, Type ptype)
//    {
//        return typeof(JsonConvert).GetMethods().FirstOrDefault(m => m.Name == "DeserializeObject" && m.IsGenericMethodDefinition).MakeGenericMethod(ptype).Invoke(null, new object[] { target });
//    }
//    public static bool IsType(this string text)
//    {
//        try
//        {
//            return ServiceFactory.Get().Has(text);
//        }
//        catch (Exception)
//        {
//            return false;
//        }
//    }
//    public static string GetNameOfType(this Type propertyType)
//    {
//        try
//        {
//            if (propertyType == null)
//                throw new ArgumentNullException("type");
//            string name = propertyType.Name;
//            if (name == null) return "";
//            if (name.IndexOf("`") != -1)
//                name = name.Substring(0, name.IndexOf("`"));

//            var arr = propertyType.GetGenericArguments();
//            if (arr.Length > 0)
//            {
//                name += '<';
//                foreach (var arg in arr)
//                {

//                    name += arg.GetTypeName().IsType() ?
//                        arg.GetTypeName() :
//                        arg.BaseType is not null ? arg.BaseType.Name : arg.Name;
//                    name += ",";
//                }
//                name = name.Substring(0, name.Length - 1);
//                name += '>';
//            }
//            return name;

//        }
//        catch (Exception ex)
//        {
//            throw;
//        }
//    }


//    public static string GetTypeName(this object target)
//    {
//        if (target is null) return "";
//        Type targetType = target is Type ? ((Type)target) : target.GetType();
//        string result = targetType.GetNameOfType();
//        return result;
//    }
//    public static bool IsExtendsFrom(this Type type, Type baseType)
//    {

//        Type typeOfObject = new object().GetType();
//        Type p = type;
//        while (p != typeOfObject && p != null)
//        {
//            var n1 = p.GetTypeName();
//            var n2 = baseType.GetTypeName();
//            //System.Console.WriteLine($"{n1}={n2}");
//            if (n1 == n2)
//            {
//                return true;
//            }
//            if (baseType.IsGenericTypeDefinition)
//            {
//                if (p.GetGenericArguments().Count() > 0)
//                {
//                    if (p.GetGenericTypeDefinition().GetTypeName() == n2)
//                        return true;
//                }
//            }
//            p = p.BaseType;
//        }
//        return false;
//    }
//}
//    public static object Call(this object p, string action, Dictionary<string, object> parameters)
//    {
//        try
//        {
//            object ActionResult = null;
//            Type type = p.GetType();
//            var method = type.GetMethods().Where(m => m.Name == action).FirstOrDefault();
//            try
//            {
//                List<object> args = new();
//                foreach (var par in method.GetParameters())
//                {
//                    if (parameters.ContainsKey(par.Name))
//                    {
//                        if (parameters[par.Name].IsExtends(par.ParameterType))
//                        {
//                            args.Add(parameters[par.Name]);
//                        }
//                        else if (parameters[par.Name] is string s)
//                        {
//                            args.Add(s.FromJsonType(par.ParameterType));
//                        }
//                        else
//                        {
//                            throw new Exception("Не поддерживаемый тип");
//                        }
//                    }
//                    else
//                    {
//                        args.Add(null);
//                    }
//                }
//                ActionResult = method.Invoke(p, args.ToArray());
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Исключение проброшено из метода {type.GetTypeName()}.{action}: {ex.Message}", ex);
//            }
//            return ActionResult;
//        }
//        catch (Exception ex)
//        {
//            throw new Exception($"Исключение при выполнении динамического вызова {nameof(Call)}({p.GetType().GetTypeName()},{action},{parameters.ToJson()})");
//        }
//    }

//    public static T RCreate<T>(Type type, object[] vs)
//    {
//        ConstructorInfo constructor = (from c in new List<ConstructorInfo>(type.GetConstructors()) where c.GetParameters().Length == vs.Length select c).FirstOrDefault();
//        if (constructor is null)
//            throw new ArgumentException($"Не удалось найти конструктор типа {type.GetTypeName()} c {vs.Length} аргументами"); ;
//        return (T)constructor.Invoke(vs);
//        //return (T)(constructor==null? constructor.Invoke(new object[type.GetConstructors().First().GetParameters().Count()]):constructor.Invoke(vs));
//    }

//    /// <summary>
//    /// Поиск типа по имени
//    /// </summary>
//    public static object Create(this Type type, object[] parameters)
//    {
//        return RCreate<object>(type, parameters);
//    }

//}