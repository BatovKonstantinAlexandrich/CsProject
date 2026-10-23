namespace BlazorErp.Services;

public class PayGwApiOptions
{
    public const string SectionName = "PayGwApi";

    public bool Enabled { get; set; } = true;
    public int LegalEntityId { get; set; } = 1;
}
