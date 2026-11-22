namespace Blazor_SiteModel.Services;

public sealed class UserSession
{
    public string? Name { get; private set; }

    public string? Phone { get; private set; }

    public string? Token { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Name);

    public event Action? Changed;

    public void SignIn(string name, string phone, string? token = null)
    {
        Name = name.Trim();
        Phone = phone.Trim();
        Token = string.IsNullOrWhiteSpace(token) ? null : token;
        Changed?.Invoke();
    }

    public void SignOut()
    {
        Name = null;
        Phone = null;
        Token = null;
        Changed?.Invoke();
    }
}
