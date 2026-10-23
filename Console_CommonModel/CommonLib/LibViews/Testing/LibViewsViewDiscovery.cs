using System.Reflection;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryDataServices;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Microsoft.EntityFrameworkCore;
using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibViews.Testing;

internal static class LibViewsViewDiscovery
{
    const string LibViewsPrefix = "Console_CommonModel.CommonLib.LibViews";

    static readonly Type AppViewBaseType = typeof(AppViewBase);
    static readonly Type ViewItemType = typeof(ViewItem);

    internal static IReadOnlyList<Type> GetConcreteViewTypes()
    {
        var result = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var type in GetLoadableTypes(AppViewBaseType.Assembly))
        {
            if (type == null || type.IsInterface || type.IsAbstract)
                continue;
            if (type.Namespace == null || !type.Namespace.StartsWith(LibViewsPrefix, StringComparison.Ordinal))
                continue;
            if (IsExcluded(type))
                continue;
            if (!IsViewType(type))
                continue;

            if (type.IsGenericTypeDefinition)
            {
                var closed = LibViewsViewInstantiator.TryCloseGenericType(type);
                if (closed != null)
                    result[closed.FullName ?? closed.Name] = closed;
                continue;
            }

            result[type.FullName ?? type.Name] = type;
        }

        return result.Values.OrderBy(t => t.FullName, StringComparer.Ordinal).ToList();
    }

    internal static IReadOnlyDictionary<string, IReadOnlyList<Type>> GetViewTypesByArea()
    {
        return GetConcreteViewTypes()
            .GroupBy(GetAreaName)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<Type>)g.ToList());
    }

    static string GetAreaName(Type type)
    {
        var ns = type.Namespace!;
        if (ns.Length == LibViewsPrefix.Length)
            return "LibViews";
        var suffix = ns[(LibViewsPrefix.Length + 1)..];
        var dot = suffix.IndexOf('.');
        return dot < 0 ? suffix : suffix[..dot];
    }

    static bool IsViewType(Type type) =>
        AppViewBaseType.IsAssignableFrom(type) || ViewItemType.IsAssignableFrom(type);

    static bool IsExcluded(Type type)
    {
        if (typeof(TestingElement).IsAssignableFrom(type))
            return true;
        if (type.Name.EndsWith("Tests", StringComparison.Ordinal))
            return true;
        if (type.Name.EndsWith("Test", StringComparison.Ordinal) && type.Name != "ViewTestControl")
            return true;
        if (type.Name.EndsWith("Unit", StringComparison.Ordinal))
            return true;
        if (typeof(TestingUnit).IsAssignableFrom(type))
            return true;
        if (type == typeof(AppViewBase))
            return false;
        return false;
    }

    static IEnumerable<Type?> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types;
        }
    }
}

internal static class LibViewsViewInstantiator
{
    static readonly Type DefaultDbContext = typeof(DeliveryDbContext);
    static readonly Type DefaultEntity = typeof(Product);
    static readonly Type DefaultAppView = typeof(TextView);

    internal static bool TryCreate(Type viewType, out object? instance, out string? error)
    {
        instance = null;
        error = null;
        try
        {
            var type = viewType;
            if (type.IsGenericTypeDefinition)
            {
                type = TryCloseGenericType(type);
                if (type == null)
                {
                    error = "Не удалось закрыть обобщённый тип";
                    return false;
                }
            }

            var ctor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                Type.EmptyTypes,
                modifiers: null);
            if (ctor == null)
            {
                error = "Отсутствует public parameterless конструктор";
                return false;
            }

            instance = Activator.CreateInstance(type);
            return instance != null;
        }
        catch (Exception ex)
        {
            error = ex.InnerException?.Message ?? ex.Message;
            return false;
        }
    }

    internal static Type? TryCloseGenericType(Type openGeneric)
    {
        if (!openGeneric.IsGenericTypeDefinition)
            return openGeneric;

        var args = openGeneric.GetGenericArguments()
            .Select(ResolveGenericArgument)
            .ToArray();

        try
        {
            return openGeneric.MakeGenericType(args);
        }
        catch
        {
            return null;
        }
    }

    static Type ResolveGenericArgument(Type parameter)
    {
        if (!parameter.IsGenericParameter)
            return parameter;

        var constraints = parameter.GetGenericParameterConstraints();
        foreach (var constraint in constraints)
        {
            if (constraint == typeof(AppViewBase))
                return DefaultAppView;
            if (typeof(DbContext).IsAssignableFrom(constraint) && !constraint.IsAbstract)
                return PickFirstAssignable(DefaultDbContext, constraint) ?? DefaultDbContext;
            if (constraint.IsClass && constraint != typeof(object))
            {
                var pick = PickFirstAssignable(DefaultEntity, constraint)
                           ?? PickFirstAssignable(DefaultAppView, constraint);
                if (pick != null)
                    return pick;
            }
        }

        if (parameter.Name.Contains("DbContext", StringComparison.OrdinalIgnoreCase))
            return DefaultDbContext;
        if (parameter.Name.Contains("Entity", StringComparison.OrdinalIgnoreCase)
            || parameter.Name is "T" or "TItem" or "TModel")
            return DefaultEntity;
        if (parameter.Name.Contains("View", StringComparison.OrdinalIgnoreCase))
            return DefaultAppView;
        if (parameter.Name.Contains("MultiView", StringComparison.OrdinalIgnoreCase))
            return typeof(MultiViewConstructor<>).MakeGenericType(DefaultEntity);

        return typeof(object);
    }

    static Type? PickFirstAssignable(Type candidate, Type constraint)
    {
        if (constraint.IsAssignableFrom(candidate))
            return candidate;
        if (constraint.IsGenericTypeDefinition && candidate.IsGenericType)
        {
            try
            {
                var closed = constraint.MakeGenericType(candidate.GetGenericArguments());
                if (closed.IsAssignableFrom(candidate))
                    return candidate;
            }
            catch
            {
                // ignore
            }
        }

        return constraint.IsAssignableFrom(candidate) ? candidate : null;
    }
}
