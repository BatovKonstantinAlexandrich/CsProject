using WinFormsApp.Console.Console_ClassModule.ClassDataModel;
using System.Reflection;
using GrpcPort.Contracts.Actions;

namespace GrpcPort.Client.Actions;

/// <summary>Загрузка DLL и публикация публичных методов как ActionModel.</summary>
public sealed class AssemblyPublisher
{
    private readonly GrpcActionsClient _client;
    private readonly List<Action> _rollbacks = new();

    public AssemblyPublisher(GrpcActionsClient client) => _client = client;

    public IReadOnlyList<object> LoadedServices { get; } = new List<object>();

    public async Task<object> LoadAndPublishAsync(string assemblyPath, string? typeName = null)
    {
        var bytes = await File.ReadAllBytesAsync(assemblyPath);
        return await LoadAndPublishAsync(bytes, typeName, Path.GetFileName(assemblyPath));
    }

    public async Task<object> LoadAndPublishAsync(byte[] assemblyBytes, string? typeName = null, string? displayName = null)
    {
        var assembly = Assembly.Load(assemblyBytes);
        var type = ResolveType(assembly, typeName);
        var service = Activator.CreateInstance(type)
            ?? throw new InvalidOperationException($"Не удалось создать {type.FullName}");

        var rollback = await RegisterServiceReferenceAsync(service);
        _rollbacks.Add(rollback);
        ((List<object>)LoadedServices).Add(service);

        Console.WriteLine($"Загружено: {displayName ?? type.FullName} ({type.Assembly.GetName().Name})");
        return service;
    }

    public async Task LoadDirectoryAsync(string directory, string searchPattern = "*.dll")
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException(directory);

        foreach (var file in Directory.GetFiles(directory, searchPattern))
        {
            // пропускаем служебные runtime-сборки
            var name = Path.GetFileName(file);
            if (name.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("Grpc.", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("Google.", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("Newtonsoft.", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                await LoadAndPublishAsync(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Пропуск {name}: {ex.Message}");
            }
        }
    }

    /// <summary>По аналогии с ActionsConsumer.RegisterServiceReference.</summary>
    public async Task<Action> RegisterServiceReferenceAsync(object service)
    {
        var methods = ReflectionInvoker.GetPublishableMethods(service.GetType()).ToList();
        var published = new List<MethodModel>();

        foreach (var methodInfo in methods)
        {
            var model = MethodModelFactory.FromMethod(methodInfo);
            await _client.PublishAsync(model, parameters =>
                ReflectionInvoker.Call(service, methodInfo.Name, parameters));
            published.Add(model);
            Console.WriteLine($"  + publish {model.Name}");
        }

        return () =>
        {
            foreach (var method in published)
                _ = _client.UnpublishAsync(method);
        };
    }

    public void UnpublishAll()
    {
        foreach (var rollback in _rollbacks.AsEnumerable().Reverse())
            rollback();
        _rollbacks.Clear();
    }

    private static Type ResolveType(Assembly assembly, string? typeName)
    {
        var candidates = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(t => t.GetConstructors().Any(c => c.GetParameters().Length == 0))
            .ToList();

        if (!string.IsNullOrWhiteSpace(typeName))
        {
            return candidates.FirstOrDefault(t =>
                       t.FullName == typeName ||
                       t.Name == typeName ||
                       t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                   ?? throw new TypeLoadException($"Тип '{typeName}' не найден в {assembly.GetName().Name}");
        }

        return candidates.FirstOrDefault()
               ?? throw new TypeLoadException($"В сборке {assembly.GetName().Name} нет публичных классов.");
    }
}
