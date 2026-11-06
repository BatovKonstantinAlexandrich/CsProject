using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;


/// <summary>
/// Область регистрации функций обработки ответных сообщений
/// </summary>
public class ActionsContext: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Динамическая область памяти для регистрации ссылок на функции обрабатывающии ответное сообщение
    /// </summary>
    private ConcurrentDictionary<string, ActionHandler> Pool = new ConcurrentDictionary<string, ActionHandler>();

    /// <summary>
    /// Длина ключа доступа к функции обрабатывающей ответное сообщение
    /// </summary>
    private int SerialKeyLength = 32;
    private Random Random = new Random();

    /// <summary>
    /// Режим логирования
    /// </summary>
    private bool _Logging = true;

    /// <summary>
    /// Регистрация функции обработчика
    /// </summary>
    /// <param name="Handle">ссылка на функцию</param>
    /// <returns>ключ доступа</returns>
    protected string Put(ActionHandler Handle)
    {
        if (_Logging)
            Console.WriteLine("Регистрация функции обработчика");
        lock (this.Pool)
        {
            string SerialKey = GenerateSerialKey();
            Pool[SerialKey] = Handle;
            if (_Logging)
                Console.WriteLine($"Функция обработчика зарегистрирована по номеру: {SerialKey}");
            return SerialKey;
        }
    }

    /// <summary>
    /// Извлечение функции обработчика
    /// </summary>
    /// <param name="SerialKey"> ключ доступа</param>
    /// <returns>функция обработчика</returns>
    protected ActionHandler Take(string SerialKey)
    {
        if (_Logging)
            Console.WriteLine($"Получение функции обработчика по номеру: {SerialKey}");
        lock (this.Pool)
        {
            ActionHandler Handle = null;
            Pool.TryRemove(SerialKey, out Handle);
            if (_Logging)
            {
                if (Handle != null)
                {
                    Console.WriteLine($"Получена функции обработчика по номеру: {SerialKey}");
                }
                else 
                {
                    Console.WriteLine($"Не найдена функци обработчика по номеру: {SerialKey}");
                }
            }
            return Handle;
        }
    }

    /// <summary>
    /// Генерация уникального ключа доступа
    /// </summary>
    /// <returns></returns>
    private string GenerateSerialKey()
    {
        if (_Logging)
            Console.WriteLine($"Генерация уникального ключа доступа");
        string key;
        lock (Pool)
        {
            
            do
            {
                key = "";
                for (int i = 0; i < SerialKeyLength; i++)
                {
                    key += (Math.Floor(Random.NextDouble() * 10)).ToString();
                }
                
            } while (Pool.ContainsKey(key));
            Pool[key] = null;
        }
        if (_Logging)
            Console.WriteLine($"Сгенерирован уникальный ключ доступа {key}");
        return key;
    }

    public void Dispose()
    {
        if (_Logging)
            Console.WriteLine($"Dispose");
    }

    public ValueTask DisposeAsync()
    {
        if (_Logging)
            Console.WriteLine($"DisposeAsync");
        return ValueTask.CompletedTask;
    }
}