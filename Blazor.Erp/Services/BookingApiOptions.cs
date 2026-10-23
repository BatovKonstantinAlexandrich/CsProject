namespace BlazorErp.Services;

public class BookingApiOptions
{
    public const string SectionName = "BookingApi";

    public string BaseUrl { get; set; } = "https://sps.euroauto.ru";
    public string ApiToken { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}
