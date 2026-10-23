using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

/// <summary>Настройка рассылки (упрощённая модель из marketer-web).</summary>
public class MailingConfiguration
{
    public int Id { get; set; }
    public string SettingsName { get; set; } = string.Empty;
    public string MarketerName { get; set; } = string.Empty;
    public MailingConfigurationStatus Status { get; set; } = MailingConfigurationStatus.Draft;
    public DateTime? MailingStartDate { get; set; }
    public int AudienceTotal { get; set; }
    public int AudienceSent { get; set; }
    public int TimeoutMinutes { get; set; } = 60;
    public string? ExportFileName { get; set; }
    public DateTime LastEditDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<MailingTestAudience> TestAudience { get; set; } = [];
}

public class MailingTestAudience
{
    public int Id { get; set; }
    public int MailingConfigurationId { get; set; }
    public MailingConfiguration MailingConfiguration { get; set; } = null!;
    public string Phone { get; set; } = string.Empty;
    public string MessengersOrder { get; set; } = string.Empty;
    public bool FallbackToSms { get; set; }
    public bool IsActive { get; set; } = true;
}
