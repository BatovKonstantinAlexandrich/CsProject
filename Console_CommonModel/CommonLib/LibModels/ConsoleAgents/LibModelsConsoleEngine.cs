using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Console_CommonModel.CommonLib.Others;
using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibModels.ConsoleAgents;

/// <summary>
/// Универсальный консольный интерфейс для модулей LibModels: CRUD по DbContext и вызов методов сервисов через InputConsole.
/// </summary>
public static class LibModelsConsoleEngine
{
    public static string[] args = new string[0];
    public static void RunModule(string moduleFolderName, Type anchorType, Action? dedicatedProgram = null)
    {
        var moduleTypes = DiscoverModuleTypes(moduleFolderName, anchorType).ToList();
        var dbContexts = moduleTypes
            .Where(t => typeof(DbContext).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
            .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
            .Distinct()
            .OrderBy(t => t.Name)
            .ToList();

        var services = DiscoverServiceTypes(moduleTypes).ToList();

        while (true)
        {
            var menu = new List<string>();
            if (dbContexts.Count > 0)
                menu.Add("Контексты данных");
            if (services.Count > 0)
                menu.Add("Сервисы и методы");
            if (dedicatedProgram != null)
                menu.Add("Специальная программа модуля");
            menu.Add("Назад");

            var choice = InputConsole.SingleSelect($"Модуль {moduleFolderName}", menu, ref args);
            if (choice == "Назад")
                return;

            switch (choice)
            {
                case "Контексты данных":
                    RunDbContextsMenu(dbContexts);
                    break;
                case "Сервисы и методы":
                    RunServicesMenu(services);
                    break;
                case "Специальная программа модуля":
                    RunDedicatedProgramSafely(dedicatedProgram);
                    break;
            }
        }
    }

    static void RunDedicatedProgramSafely(Action? dedicatedProgram)
    {
        if (dedicatedProgram == null)
            return;

        InputConsole.Write("Запуск специальной программы модуля (консольный режим)...");
        try
        {
            dedicatedProgram();
        }
        catch (Exception ex)
        {
            InputConsole.Error(ex.InnerException?.Message ?? ex.Message);
        }

        InputConsole.ConfirmContinue("Enter — вернуться в меню модуля.");
    }

    static IEnumerable<Type> DiscoverModuleTypes(string moduleFolderName, Type anchorType)
    {
        var prefix = $"Console_CommonModel.CommonLib.LibModels.{moduleFolderName}";
        return anchorType.Assembly.GetTypes().Where(t =>
        {
            if (t.Namespace == null)
                return t == anchorType;
            if (t.Namespace.StartsWith(prefix, StringComparison.Ordinal))
                return true;
            if (t.Namespace == anchorType.Namespace)
                return true;
            if (moduleFolderName == "ServiceBuilderModule"
                && t.Namespace == "Console_CommonModel.CommonLib.LibModels"
                && (t.Name.Contains("ServiceBuilder", StringComparison.Ordinal) || t.Name.Contains("RegisteredWsdl", StringComparison.Ordinal)))
                return true;
            return false;
        });
    }

    static IEnumerable<Type> DiscoverServiceTypes(IEnumerable<Type> moduleTypes)
    {
        return moduleTypes
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
            .Where(t => !typeof(DbContext).IsAssignableFrom(t))
            .Where(t => !typeof(Attribute).IsAssignableFrom(t))
            .Where(t => !t.Name.EndsWith("Test", StringComparison.OrdinalIgnoreCase) || GetCallableMethods(t).Any())
            .Where(t => GetCallableMethods(t).Any() || GetCallableStaticMethods(t).Any())
            .OrderBy(t => t.Name)
            .Distinct();
    }

    static void RunDbContextsMenu(List<Type> dbContextTypes)
    {
        while (true)
        {
            var names = dbContextTypes.Select(t => t.Name).ToList();
            names.Add("Назад");
            var picked = InputConsole.SingleSelect("Контексты данных", names, ref args);
            if (picked == "Назад")
                return;

            var ctxType = dbContextTypes.First(t => t.Name == picked);
            RunSingleDbContext(ctxType);
        }
    }

    static void RunSingleDbContext(Type dbContextType)
    {
        using var db = (DbContext)Activator.CreateInstance(dbContextType)!;
        if (InputConsole.Confirm($"Инициализировать хранилище для {dbContextType.Name}? (EnsureCreated)"))
        {
            db.Database.EnsureCreated();
            InputConsole.Write("База данных готова.");
        }

        var entitySets = dbContextType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .OrderBy(p => p.Name)
            .ToList();

        if (entitySets.Count == 0)
        {
            InputConsole.Warn($"В {dbContextType.Name} не найдено DbSet-сущностей.");
            InputConsole.ConfirmContinue();
            return;
        }

        while (true)
        {
            var entityNames = entitySets.Select(p => p.Name).ToList();
            entityNames.Add("Назад");
            var entityName = InputConsole.SingleSelect($"Сущности ({dbContextType.Name})", entityNames, ref args);
            if (entityName == "Назад")
                return;

            var setProperty = entitySets.First(p => p.Name == entityName);
            var entityType = setProperty.PropertyType.GetGenericArguments()[0];
            RunEntityCrud(db, setProperty, entityType);
        }
    }

    static void RunEntityCrud(DbContext db, PropertyInfo setProperty, Type entityType)
    {
        while (true)
        {
            var actions = new List<string>
            {
                "Просмотр списка",
                "Выбрать запись",
                "Зарегистрировать (добавить)",
                "Удалить запись",
                "Назад"
            };
            var action = InputConsole.SingleSelect($"Сущность {entityType.Name}", actions, ref args);
            if (action == "Назад")
                return;

            try
            {
                switch (action)
                {
                    case "Просмотр списка":
                        ListEntities(db, setProperty, entityType);
                        break;
                    case "Выбрать запись":
                        SelectEntity(db, setProperty, entityType);
                        break;
                    case "Зарегистрировать (добавить)":
                        RegisterEntity(db, setProperty, entityType);
                        break;
                    case "Удалить запись":
                        DeleteEntity(db, setProperty, entityType);
                        break;
                }
            }
            catch (Exception ex)
            {
                InputConsole.Error(ex.Message);
                InputConsole.ConfirmContinue();
            }
        }
    }

    static IList GetEntityList(DbContext db, PropertyInfo setProperty)
    {
        var dbSet = setProperty.GetValue(db);
        if (dbSet is not IEnumerable enumerable)
            return ArrayList.ReadOnly(new ArrayList());
        var list = new ArrayList();
        foreach (var item in enumerable)
            list.Add(item);
        return list;
    }

    static void ListEntities(DbContext db, PropertyInfo setProperty, Type entityType)
    {
        var list = GetEntityList(db, setProperty);
        if (list.Count == 0)
            InputConsole.Write("(пусто)");
        else
        {
            foreach (var item in list)
                InputConsole.Write(item.ToJsonOnScreen());
        }
        InputConsole.ConfirmContinue();
    }

    static object? SelectEntity(DbContext db, PropertyInfo setProperty, Type entityType)
    {
        var list = GetEntityList(db, setProperty);
        if (list.Count == 0)
        {
            InputConsole.Warn("Записей нет.");
            InputConsole.ConfirmContinue();
            return null;
        }

        var labels = new List<string>();
        foreach (object item in list)
            labels.Add(DescribeEntity(item));
        labels.Add("Отмена");
        var picked = InputConsole.SingleSelect($"Выбор {entityType.Name}", labels, ref args);
        if (picked == "Отмена")
            return null;

        var index = labels.IndexOf(picked);
        var entity = list[index];
        InputConsole.Write(entity.ToJsonOnScreen());
        InputConsole.ConfirmContinue();
        return entity;
    }

    static void RegisterEntity(DbContext db, PropertyInfo setProperty, Type entityType)
    {
        var entity = Activator.CreateInstance(entityType)!;
        FillEntityFromConsole(entity);
        var addMethod = setProperty.PropertyType.GetMethod("Add", new[] { entityType });
        addMethod?.Invoke(setProperty.GetValue(db), new[] { entity });
        db.SaveChanges();
        InputConsole.Write("Запись зарегистрирована:");
        InputConsole.Write(entity.ToJsonOnScreen());
        InputConsole.ConfirmContinue();
    }

    static void DeleteEntity(DbContext db, PropertyInfo setProperty, Type entityType)
    {
        var entity = SelectEntity(db, setProperty, entityType);
        if (entity == null)
            return;
        if (!InputConsole.Confirm("Удалить выбранную запись?"))
            return;

        var removeMethod = setProperty.PropertyType.GetMethod("Remove", new[] { entityType });
        removeMethod?.Invoke(setProperty.GetValue(db), new[] { entity });
        db.SaveChanges();
        InputConsole.Write("Запись удалена.");
        InputConsole.ConfirmContinue();
    }

    static void FillEntityFromConsole(object entity)
    {
        var args = Array.Empty<string>();
        foreach (var property in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.CanWrite)
                continue;
            if (property.GetIndexParameters().Length > 0)
                continue;
            if (IsNavigationProperty(property))
                continue;

            var label = AttrsUtil.LabelFor(entity.GetType(), property.Name);
            var value = ReadPropertyValue(property.PropertyType, label, ref args);
            if (value == null && property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
                continue;
            try
            {
                property.SetValue(entity, value);
            }
            catch
            {
                InputConsole.Warn($"Не удалось установить {property.Name}");
            }
        }
    }

    static bool IsNavigationProperty(PropertyInfo property)
    {
        var t = property.PropertyType;
        if (t == typeof(string))
            return false;
        if (typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string))
            return true;
        if (t.IsClass && t != typeof(string) && !t.IsPrimitive && t != typeof(decimal) && t != typeof(DateTime) && t != typeof(DateTimeOffset) && t != typeof(Guid))
        {
            if (typeof(DbContext).IsAssignableFrom(t))
                return true;
            if (t.Namespace?.Contains("LibModels") == true && !t.IsEnum)
                return true;
        }
        return false;
    }

    static object? ReadPropertyValue(Type type, string label, ref string[] args)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        if (underlying == typeof(string))
            return InputConsole.InputString(label, _ => new List<string>(), ref args);
        if (underlying == typeof(int))
            return InputConsole.InputNumber(label, _ => new List<string>(), ref args);
        if (underlying == typeof(long))
            return long.Parse(InputConsole.InputString(label, _ => new List<string>(), ref args), CultureInfo.InvariantCulture);
        if (underlying == typeof(bool))
            return InputConsole.Confirm(label);
        if (underlying == typeof(decimal))
            return decimal.Parse(InputConsole.InputString(label, _ => new List<string>(), ref args), CultureInfo.InvariantCulture);
        if (underlying == typeof(double))
            return double.Parse(InputConsole.InputString(label, _ => new List<string>(), ref args), CultureInfo.InvariantCulture);
        if (underlying == typeof(DateTime))
            return InputConsole.InputDateTime(label, null, ref args);
        if (underlying == typeof(Guid))
            return Guid.Parse(InputConsole.InputString(label, _ => new List<string>(), ref args));
        if (underlying.IsEnum)
        {
            var names = Enum.GetNames(underlying).ToList();
            var picked = InputConsole.SingleSelect(label, names, ref args);
            return Enum.Parse(underlying, picked);
        }

        var json = InputConsole.InputString($"{label} ({underlying.Name}, JSON)", _ => new List<string>(), ref args);
        if (string.IsNullOrWhiteSpace(json))
            return underlying.IsValueType ? Activator.CreateInstance(underlying) : null;
        return json.FromJson(underlying);
    }

    static void RunServicesMenu(List<Type> serviceTypes)
    {
        while (true)
        {
            var names = serviceTypes.Select(t => t.Name).ToList();
            names.Add("Назад");
            var picked = InputConsole.SingleSelect("Сервисы", names, ref args);
            if (picked == "Назад")
                return;

            var serviceType = serviceTypes.First(t => t.Name == picked);
            RunServiceMethods(serviceType);
        }
    }

    static void RunServiceMethods(Type serviceType)
    {
        var staticMethods = GetCallableStaticMethods(serviceType).ToList();
        var instanceMethods = GetCallableMethods(serviceType).ToList();

        while (true)
        {
            var entries = staticMethods.Select(m => $"[static] {m.Name}").ToList();
            entries.AddRange(instanceMethods.Select(m => m.Name));
            entries.Add("Назад");

            var picked = InputConsole.SingleSelect($"Методы {serviceType.Name}", entries, ref args);
            if (picked == "Назад")
                return;

            var isStatic = picked.StartsWith("[static] ", StringComparison.Ordinal);
            var methodName = isStatic ? picked["[static] ".Length..] : picked;
            var method = isStatic
                ? staticMethods.First(m => m.Name == methodName)
                : instanceMethods.First(m => m.Name == methodName);

            InvokeMethodInteractive(serviceType, method, isStatic);
        }
    }

    static void InvokeMethodInteractive(Type serviceType, MethodInfo method, bool isStatic)
    {
        object? target = null;
        if (!isStatic)
        {
            try
            {
                target = Activator.CreateInstance(serviceType);
            }
            catch (Exception ex)
            {
                InputConsole.Error($"Не удалось создать {serviceType.Name}: {ex.Message}");
                InputConsole.ConfirmContinue();
                return;
            }
        }

        var args = Array.Empty<string>();
        var argv = new List<object?>();
        foreach (var p in method.GetParameters())
        {
            if (p.ParameterType == typeof(string[]))
            {
                argv.Add(args);
                continue;
            }
            argv.Add(ReadPropertyValue(p.ParameterType, p.Name ?? "param", ref args));
        }

        try
        {
            var result = method.Invoke(isStatic ? null : target, argv.ToArray());
            if (method.ReturnType != typeof(void))
            {
                InputConsole.Write("Результат:");
                InputConsole.Write(result?.ToJsonOnScreen() ?? "(null)");
            }
            else
                InputConsole.Write("Метод выполнен.");
        }
        catch (TargetInvocationException tie)
        {
            InputConsole.Error(tie.InnerException?.Message ?? tie.Message);
        }
        catch (Exception ex)
        {
            InputConsole.Error(ex.Message);
        }

        InputConsole.ConfirmContinue();
    }

    static IEnumerable<MethodInfo> GetCallableMethods(Type type) =>
        type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Where(m => !m.Name.StartsWith("get_", StringComparison.Ordinal) && !m.Name.StartsWith("set_", StringComparison.Ordinal));

    static IEnumerable<MethodInfo> GetCallableStaticMethods(Type type) =>
        type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName);

    static string DescribeEntity(object entity)
    {
        var type = entity.GetType();
        var idProp = type.GetProperty("Id") ?? type.GetProperty("Name") ?? type.GetProperty("Title");
        if (idProp != null)
        {
            var v = idProp.GetValue(entity);
            return $"{type.Name}: {idProp.Name}={v}";
        }
        return entity.ToJsonOnScreen();
    }
}
