namespace BlazorErp.Services;

public sealed class ExternalAppsOptions
{
    public const string SectionName = "ExternalApps";

    public ExternalAppEntry PayGwManager { get; set; } = new();
    public ExternalAppEntry VendorsSetup { get; set; } = new();
    public ExternalAppEntry MarketerWeb { get; set; } = new();
}

public sealed class ExternalAppEntry
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}
