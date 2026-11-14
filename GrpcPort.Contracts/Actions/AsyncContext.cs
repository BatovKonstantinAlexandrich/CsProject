using System.Collections.Concurrent;

namespace GrpcPort.Contracts.Actions;

/// <summary>Пул обработчиков ответа по SerialKey.</summary>
public class AsyncContext
{
    private readonly ConcurrentDictionary<string, ActionHandler> _pool = new();
    private readonly Random _random = new();
    private readonly int _serialKeyLength = 32;

    public string Put(ActionHandler handle)
    {
        var key = GenerateSerialKey();
        _pool[key] = handle;
        return key;
    }

    public ActionHandler? Take(string serialKey)
    {
        _pool.TryRemove(serialKey, out var handle);
        return handle;
    }

    private string GenerateSerialKey()
    {
        Span<char> chars = stackalloc char[_serialKeyLength];
        string key;
        do
        {
            for (var i = 0; i < _serialKeyLength; i++)
                chars[i] = (char)('0' + _random.Next(0, 10));
            key = new string(chars);
        } while (_pool.ContainsKey(key));

        return key;
    }
}
