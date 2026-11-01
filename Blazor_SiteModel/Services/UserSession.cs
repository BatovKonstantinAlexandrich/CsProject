using pickpoint_delivery_service;

namespace Blazor_SiteModel.Services;

public sealed class UserSession()
{
    public string? Name { get; private set; }

    public string? Phone { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Name);

    public event Action? Changed;

    public void SignIn(string name, string phone)
    {
        
        Name = name.Trim();
        Phone = phone.Trim();
        Changed?.Invoke();
    }

    public void SignOut()
    {
        Name = null;
        Phone = null;
        Changed?.Invoke();
    }
}
