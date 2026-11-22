namespace Blazor_SiteModel.Services;

public sealed class CityState
{
    public string Current { get; private set; } = "Москва";

    public event Action? Changed;

    public void Select(string city)
    {
        if (string.IsNullOrWhiteSpace(city) || city == Current)
            return;

        Current = city;
        Changed?.Invoke();
    }
}
