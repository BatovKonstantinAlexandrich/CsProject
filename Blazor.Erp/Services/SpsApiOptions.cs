namespace BlazorErp.Services;

public class SpsApiOptions
{
    public const string SectionName = "SpsApi";

    public string BaseUrl { get; set; } = "https://sps.euroauto.ru";
    public string TokenUrl { get; set; } = "https://sps.euroauto.ru/api/auth/Login";
    public string ApiToken { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string[] Roles { get; set; } = ["UserDesktop"];
}
