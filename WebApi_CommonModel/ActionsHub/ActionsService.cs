using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Namotion.Reflection;
using System.Globalization;
using System.Reflection;


/// <summary>
/// Расширяет <see cref="ActionsContext"/>: регистрация публичных методов экземпляра с привязкой
/// <see cref="Dictionary{TKey,TValue}"/> строковых параметров к аргументам метода через рефлексию.
/// </summary>
public class ActionsService  
{
    private ActionsContext actionsContext;  //память с ссылками на функции 

    public ActionsService(ActionsContext actionsContext)
    {
        this.actionsContext = actionsContext;
    }

    private const BindingFlags ServiceMethodFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod;

    /// <summary>
    /// Регистрирует все публичные методы экземпляра <paramref name="service"/> как операции контекста.
    /// Имя операции: <c>{ИмяТипа}.{ИмяМетода}_{ТипПараметра1}_...</c> (суффикс с типами только если у метода есть параметры).
    /// </summary>
    /// <returns>Действие, снимающее регистрацию всех добавленных операций для этого вызова.</returns>
    public Action AddService<TService>(TService service)
    {
        if (service is null)
            throw new ArgumentNullException(nameof(service));

        var runtimeType = service.GetType();
        var registeredNames = new List<string>();
        List<Action> rollback = new();
        foreach (var method in GetServiceMethods(runtimeType))
        {
            var actionName = BuildActionName(runtimeType, method);
            var methodInfo = method;
            var methodModel = CreateMethodModel(runtimeType, actionName);
            rollback.Add(actionsContext.AddAction(actionName, methodModel, (args,onsuccess,onerror) =>
            {
                try
                {
                    this.Info($"{service.GetTypeName()}.{method}(...)");
                    var invokeArgs = BuildInvokeArguments(methodInfo, args);
                    
                    var methodResult = methodInfo.Invoke(service, invokeArgs);
                    if (methodResult is Task t)
                        t.Wait();
                    
                    onsuccess(methodResult.HasProperty("Result")? ((dynamic)methodResult).Result : null);
                }
                catch(Exception ex)
                {
                    onerror(ex.Message);
                }
            }));
            registeredNames.Add(actionName);
        }
        return () =>
        {
            foreach (var todo in rollback)
                todo();
        };
    }

    private MethodModel CreateMethodModel(Type type, string name)
    {
        var typeHelper = new TypeHelper();
        var info = type.GetMethods().FirstOrDefault(m => m.Name==name);
        var methodAttributes = typeHelper.GetMethodAttributes(type, name);
        var method = new MethodModel()
        {
            Name = name,
            Label = methodAttributes.ContainsKey("LabelAttribute")? methodAttributes["LabelAttribute"]: name
        };
        method.Type = info.ReturnType.GetTypeName();               
        foreach (var attr in methodAttributes.Select(attr => new MethodModelAttribute()
        {
         
            Name = attr.Key,
            Value = attr.Value
        }))
        {
            attr.Parent = method;
            method.Attributes.Add(attr);
        }
        var methodParameters = info.GetParameters();
        foreach (var par in methodParameters)
        {
            var parameterAttributes = typeHelper.GetParameterAttributes(par);

            var mpm = new MethodParameterModel()
            {
                Parent = method,
                Name = par.Name,
                Type = par.ParameterType.GetTypeName(),
                Label = parameterAttributes.ContainsKey("LabelAttribute") ? parameterAttributes["LabelAttribute"] : name
            };
            method.Parameters.Add(mpm);
            mpm.Parent = method;

            foreach (var attr in parameterAttributes.Select(attr => new MethodParameterModelAttribute()
            {
                Parent = mpm,
                Name = attr.Key,
                Value = attr.Value
            }))
            {
                attr.Parent = mpm;
                mpm.Attributes.Add(attr);             
            }
        }
        return method;
    }

    private static IEnumerable<MethodInfo> GetServiceMethods(Type runtimeType)
    {
        return runtimeType.GetMethods(ServiceMethodFlags)
            .Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object) && !m.ContainsGenericParameters);
    }

    private static string BuildActionName(Type type, MethodInfo method)
    {
        var baseName = $"{type.Name}.{method.Name}";
        var parameters = method.GetParameters();
        if (parameters.Length == 0)
            return baseName;

        var sig = string.Join("_", parameters.Select(p => p.ParameterType.Name));
        return $"{baseName}_{sig}";
    }

    private static object[] BuildInvokeArguments(MethodInfo method, Dictionary<string, object> args)
    {
        var parameters = method.GetParameters();
        var result = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            if (p.ParameterType == typeof(CancellationToken))
            {
                result[i] = default(CancellationToken);
                continue;
            }

            if (!TryGetArgValue(args, p.Name, out var raw))
            {
                if (p.HasDefaultValue)
                {
                    result[i] = p.DefaultValue;
                    continue;
                }

                throw new Exception(
                    $"Для метода [{method.Name}] не указан параметр [{p.Name}]. Передайте ключ словаря, совпадающий с именем параметра (без учёта регистра).");
            }

            result[i] = ConvertStringToParameterType(raw, p.ParameterType, p.Name, method.Name);
        }

        return result;
    }

    private static bool TryGetArgValue(Dictionary<string, object> args, string parameterName, out object value)
    {
        if (args.TryGetValue(parameterName, out value))
            return true;

        foreach (var kv in args)
        {
            if (string.Equals(kv.Key, parameterName, StringComparison.OrdinalIgnoreCase))
            {
                value = kv.Value;
                return true;
            }
        }

        value = null;
        return false;
    }

    private static object ConvertStringToParameterType(object value, Type parameterType, string parameterName, string methodName)
    {
        var targetType = parameterType;
        var nullableInner = Nullable.GetUnderlyingType(targetType);
        if (nullableInner != null)
        {
            if (string.IsNullOrWhiteSpace($"{value}"))
                return null;
            targetType = nullableInner;
        }

        try
        {
            if (targetType == typeof(string))
                return value;

            if (targetType == typeof(bool))
            {
                if (string.Equals($"{value}", "1", StringComparison.OrdinalIgnoreCase)) return true;
                if (string.Equals($"{value}", "0", StringComparison.OrdinalIgnoreCase)) return false;
                return bool.Parse($"{value}");
            }

            if (targetType.IsEnum)
                return Enum.Parse(targetType, $"{value}", ignoreCase: true);

            if (targetType == typeof(Guid))
                return Guid.Parse($"{value}");

            if (targetType == typeof(DateTime))
                return DateTime.Parse($"{value}", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            if (targetType == typeof(DateTimeOffset))
                return DateTimeOffset.Parse($"{value}", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            if (targetType == typeof(TimeSpan))
                return TimeSpan.Parse($"{value}", CultureInfo.InvariantCulture);

            if (targetType == typeof(byte))
                return byte.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(sbyte))
                return sbyte.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(short))
                return short.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(ushort))
                return ushort.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(int))
                return int.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(uint))
                return uint.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(long))
                return long.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(ulong))
                return ulong.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(float))
                return float.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(double))
                return double.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(decimal))
                return decimal.Parse($"{value}", CultureInfo.InvariantCulture);
            if (targetType == typeof(char))
                return $"{value}".Length == 1 ? $"{value}"[0] : char.Parse($"{value}");

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Не удалось преобразовать значение параметра [{parameterName}] для метода [{methodName}] к типу [{parameterType.Name}]: {ex.Message}",
                ex);
        }
    }
}
