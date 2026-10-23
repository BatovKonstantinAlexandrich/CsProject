using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class MailingConfigurationService(ApplicationDbContext db)
{
    public Task<List<MailingConfiguration>> GetAllAsync(
        string? search = null,
        MailingConfigurationStatus? status = null,
        DateTime? startFrom = null,
        DateTime? startTo = null) =>
        db.MailingConfigurations
            .Include(m => m.TestAudience)
            .Where(m =>
                (string.IsNullOrWhiteSpace(search)
                 || m.SettingsName.Contains(search)
                 || m.MarketerName.Contains(search))
                && (status == null || m.Status == status)
                && (startFrom == null || m.MailingStartDate >= startFrom)
                && (startTo == null || m.MailingStartDate <= startTo))
            .OrderByDescending(m => m.LastEditDate)
            .ToListAsync();

    public Task<MailingConfiguration?> GetByIdAsync(int id) =>
        db.MailingConfigurations
            .Include(m => m.TestAudience)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<MailingConfiguration> CreateAsync(MailingConfiguration model)
    {
        model.LastEditDate = DateTime.UtcNow;
        model.CreatedAt = DateTime.UtcNow;
        db.MailingConfigurations.Add(model);
        await db.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateAsync(MailingConfiguration model)
    {
        var existing = await db.MailingConfigurations.FindAsync(model.Id);
        if (existing == null)
            return false;

        existing.SettingsName = model.SettingsName;
        existing.MarketerName = model.MarketerName;
        existing.Status = model.Status;
        existing.MailingStartDate = model.MailingStartDate;
        existing.AudienceTotal = model.AudienceTotal;
        existing.AudienceSent = model.AudienceSent;
        existing.TimeoutMinutes = model.TimeoutMinutes;
        existing.ExportFileName = model.ExportFileName;
        existing.LastEditDate = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await db.MailingConfigurations
            .Include(m => m.TestAudience)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null)
            return false;

        db.MailingTestAudiences.RemoveRange(item.TestAudience);
        db.MailingConfigurations.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<MailingTestAudience> AddTestAudienceAsync(MailingTestAudience audience)
    {
        db.MailingTestAudiences.Add(audience);
        await db.SaveChangesAsync();
        return audience;
    }

    public async Task<bool> UpdateTestAudienceAsync(MailingTestAudience audience)
    {
        var existing = await db.MailingTestAudiences.FindAsync(audience.Id);
        if (existing == null)
            return false;

        existing.Phone = audience.Phone;
        existing.MessengersOrder = audience.MessengersOrder;
        existing.FallbackToSms = audience.FallbackToSms;
        existing.IsActive = audience.IsActive;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTestAudienceAsync(int id)
    {
        var item = await db.MailingTestAudiences.FindAsync(id);
        if (item == null)
            return false;

        db.MailingTestAudiences.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }

    public static string GetStatusLabel(MailingConfigurationStatus status) => status switch
    {
        MailingConfigurationStatus.Draft => "Черновик",
        MailingConfigurationStatus.Active => "Активна",
        MailingConfigurationStatus.Paused => "Приостановлена",
        MailingConfigurationStatus.Completed => "Завершена",
        MailingConfigurationStatus.Archived => "Архив",
        _ => status.ToString()
    };
}
