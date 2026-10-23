[Label("Номер кредитной карты")]
[Description(
    "Номер карты состоит из 4-групп " +
    "по 4-цифры разделённых знаком тирэ." +
    "Пример: 3306-3305-4440-5555")]
public class InputCreditCardAttribute : BaseInputAttribute
{
    public const string EmptyMask = "0000-0000-0000-0000";

    public InputCreditCardAttribute() : base(InputTypes.CreditCard) { }

    public InputCreditCardAttribute(string argument) : base(InputTypes.CreditCard) { }

    /// <summary>Приводит строку к виду XXXX-XXXX-XXXX-XXXX (до 16 цифр).</summary>
    public static string FormatCardNumber(string text)
    {
        var digits = new string((text ?? string.Empty).Where(char.IsDigit).Take(16).ToArray());
        if (digits.Length == 0)
            return string.Empty;

        var parts = new List<string>();
        for (var i = 0; i < digits.Length; i += 4)
            parts.Add(digits.Substring(i, Math.Min(4, digits.Length - i)));
        return string.Join("-", parts);
    }

    /// <summary>Проверка формата XXXX-XXXX-XXXX-XXXX. Пустая строка допустима.</summary>
    public static bool TryValidate(string text, out string error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(text))
            return true;

        var value = text.Trim();
        if (value.Length != EmptyMask.Length)
        {
            error = "Номер карты должен иметь вид XXXX-XXXX-XXXX-XXXX";
            return false;
        }

        if (value[4] != '-' || value[9] != '-' || value[14] != '-')
        {
            error = "Группы цифр должны быть разделены тире";
            return false;
        }

        for (var i = 0; i < value.Length; i++)
        {
            if (i == 4 || i == 9 || i == 14)
                continue;
            if (!char.IsDigit(value[i]))
            {
                error = $"Символ №{i + 1} должен быть цифрой";
                return false;
            }
        }

        return true;
    }

    public override bool IsValidValue(object value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return true;
        return TryValidate(value.ToString(), out _);
    }

    public override string OnValidate(object model, string property, object value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return null;

        if (TryValidate(value.ToString(), out var error))
            return null;

        return $"{GetMessage(model, property, value)}. {error}";
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        return "Номер карты имеет формат XXXX-XXXX-XXXX-XXXX где X-цифра";
    }
}
