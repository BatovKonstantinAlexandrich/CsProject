using System.Globalization;

namespace Blazor_SiteModel.Services;

public static class PriceText
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    public static string Format(decimal value) =>
        value <= 0 ? "Бесплатно" : $"{value.ToString("N0", Ru)} ₽";

    public static string Hours(int minutes)
    {
        var hours = minutes / 60m;
        return hours.ToString("0.#", Ru) + " н/ч";
    }
}
