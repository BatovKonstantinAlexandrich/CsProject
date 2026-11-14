namespace GrpcPort.SamplePlugin;

/// <summary>Пример сервиса, методы которого публикуются как remote actions.</summary>
public class HelloService
{
    public string Greet(string name) => $"Hello, {name}!";

    public string Add(string a, string b)
    {
        if (!int.TryParse(a, out var x) || !int.TryParse(b, out var y))
            throw new ArgumentException("a и b должны быть числами");
        return (x + y).ToString();
    }

    public string Echo(string message) => message;

    public string Now() => DateTimeOffset.Now.ToString("O");
}
