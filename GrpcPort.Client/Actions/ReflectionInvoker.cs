using System.Globalization;
using System.Reflection;

namespace GrpcPort.Client.Actions;

internal static class ReflectionInvoker
{
    public static object? Call(object instance, string methodName, Dictionary<string, object> stringArgs)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new MissingMethodException(type.FullName, methodName);

        var parameters = method.GetParameters();
        var args = new object?[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            if (!stringArgs.TryGetValue(p.Name!, out var text) || text is null)
            {
                if (p.HasDefaultValue)
                {
                    args[i] = p.DefaultValue;
                    continue;
                }

                throw new ArgumentException($"Не передан параметр '{p.Name}'.");
            }

            args[i] = ConvertParameter($"{(text is string? text: text.ToJson())}", p.ParameterType);
        }

        return method.Invoke(instance, args);
    }

    public static IEnumerable<MethodInfo> GetPublishableMethods(Type type) =>
        type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName && !m.IsStatic);

    private static object? ConvertParameter(string text, Type targetType)
    {
        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (underlying == typeof(string))
            return text;
        if (underlying == typeof(bool))
            return bool.Parse(text);
        if (underlying.IsEnum)
            return Enum.Parse(underlying, text, ignoreCase: true);

        return Convert.ChangeType(text, underlying, CultureInfo.InvariantCulture);
    }
}
