[Label("Адрес TCP/IP")]
public class InputTcpIpAddressAttribute : BaseInputAttribute
{
    public string Mask { get; }

    public InputTcpIpAddressAttribute() : base(InputTypes.TcpIpAddress)
    {
        Mask = "255.255.255.0";
    }

    public InputTcpIpAddressAttribute(string mask = "255.255.255.0") : base(InputTypes.TcpIpAddress)
    {
        Mask = string.IsNullOrWhiteSpace(mask) ? "255.255.255.0" : mask;
    }

    /// <summary>Проверка формата IPv4: 4 сегмента 0–254 через точку.</summary>
    public static bool TryValidateIpv4(string text, out string error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(text))
            return true;

        var parts = text.Trim().Split('.');
        if (parts.Length != 4)
        {
            error = "IPv4 состоит из 4 чисел, разделённых точками";
            return false;
        }

        for (var i = 0; i < parts.Length; i++)
        {
            var word = parts[i];
            if (string.IsNullOrEmpty(word) || !word.All(char.IsDigit))
            {
                error = $"Сегмент №{i + 1}: не является числом";
                return false;
            }
            if (!int.TryParse(word, out var number))
            {
                error = $"Сегмент №{i + 1}: не является числом";
                return false;
            }
            if (number < 0 || number > 254)
            {
                error = $"Сегмент №{i + 1}: число вне диапазона [0, 254]";
                return false;
            }
        }

        return true;
    }

    public override bool IsValidValue(object value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return true;
        return TryValidateIpv4(value.ToString(), out _);
    }

    public override string OnValidate(object model, string property, object value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return null;

        if (TryValidateIpv4(value.ToString(), out var error))
            return null;

        return $"{GetMessage(model, property, value)}. {error}";
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        return "TCP/IP v4: четыре числа из [0, 254], разделённых '.'";
    }
}
