namespace Console_CommonModel.CommonLib.LibModels;

/// <summary>
/// Регистрация SOAP/WCF по WSDL и чтение списка из базы ServiceBuilder.
/// </summary>
public sealed class SoapServiceBuilder
{
    private static readonly Lazy<SoapServiceBuilder> _instance = new(() => new SoapServiceBuilder());
    public static SoapServiceBuilder Instance => _instance.Value;

    private SoapServiceBuilder()
    {
    }

    /// <summary>
    /// Проверяет доступность WSDL, сохраняет запись в БД.
    /// </summary>
    public RegisteredWsdlService RegisterFromWsdlUrl(string wsdlUrl)
    {
        if (string.IsNullOrWhiteSpace(wsdlUrl))
            throw new ArgumentException("URL не задан.", nameof(wsdlUrl));
        wsdlUrl = wsdlUrl.Trim();
        if (!Uri.TryCreate(wsdlUrl, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("Нужен абсолютный URL (http/https).", nameof(wsdlUrl));

        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        using var response = http.GetAsync(wsdlUrl).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidOperationException("Пустой ответ по указанному URL.");
        if (body.IndexOf("definitions", StringComparison.OrdinalIgnoreCase) < 0 &&
            body.IndexOf("wsdl", StringComparison.OrdinalIgnoreCase) < 0)
            throw new InvalidOperationException("Ответ не похож на WSDL (нет типичных элементов).");

        var display = string.IsNullOrEmpty(uri.PathAndQuery) || uri.PathAndQuery == "/"
            ? uri.Host
            : $"{uri.Host}{uri.PathAndQuery}";
        if (display.Length > 500)
            display = display[..500];

        using var db = new ServiceBuilderDbContext();
        db.EnsureStorageCreated();

        var existing = db.RegisteredWsdlServices.FirstOrDefault(s => s.WsdlUrl == wsdlUrl);
        if (existing is not null)
        {
            existing.DisplayName = display;
            existing.RegisteredAtUtc = DateTime.UtcNow;
            db.SaveChanges();
            return existing;
        }

        var row = new RegisteredWsdlService
        {
            WsdlUrl = wsdlUrl,
            DisplayName = display,
            RegisteredAtUtc = DateTime.UtcNow
        };
        db.RegisteredWsdlServices.Add(row);
        db.SaveChanges();
        return row;
    }

    public IReadOnlyList<RegisteredWsdlService> ListRegisteredWsdlServices()
    {
        using var db = new ServiceBuilderDbContext();
        db.EnsureStorageCreated();
        return db.RegisteredWsdlServices
            .OrderBy(s => s.DisplayName)
            .ThenBy(s => s.WsdlUrl)
            .ToList();
    }

    public bool TryDelete(int id)
    {
        using var db = new ServiceBuilderDbContext();
        var row = db.RegisteredWsdlServices.Find(id);
        if (row is null)
            return false;
        db.RegisteredWsdlServices.Remove(row);
        db.SaveChanges();
        return true;
    }
}
