using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class AutoServiceService(ApplicationDbContext db)
{
    public Task<AutoService?> GetAutoServiceByOwnerAsync(string ownerId) =>
        db.AutoServices.FirstOrDefaultAsync(s => s.OwnerId == ownerId);

    public async Task<AutoService?> GetAutoServiceForUserAsync(string userId)
    {
        var owned = await GetAutoServiceByOwnerAsync(userId);
        if (owned != null)
            return owned;

        return await db.AutoServiceEmployees
            .Include(e => e.AutoService)
            .Where(e => e.UserId == userId)
            .Select(e => e.AutoService)
            .FirstOrDefaultAsync();
    }

    public Task<bool> IsAutoServiceOwnerAsync(string userId) =>
        db.AutoServices.AnyAsync(s => s.OwnerId == userId);

    public async Task<bool> UpdateProfileAsync(string ownerId, AutoServiceProfileUpdate update)
    {
        var service = await GetAutoServiceByOwnerAsync(ownerId);
        if (service == null || string.IsNullOrWhiteSpace(update.Name))
            return false;

        service.Name = update.Name.Trim();
        service.Address = string.IsNullOrWhiteSpace(update.Address) ? null : update.Address.Trim();
        service.Description = string.IsNullOrWhiteSpace(update.Description) ? null : update.Description.Trim();
        service.Phone = string.IsNullOrWhiteSpace(update.Phone) ? null : update.Phone.Trim();
        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<AutoServiceOffering>> GetOfferingsAsync(int autoServiceId) =>
        db.AutoServiceOfferings
            .Where(o => o.AutoServiceId == autoServiceId)
            .OrderBy(o => o.Name)
            .ToListAsync();

    public async Task<AutoServiceOffering?> SaveOfferingAsync(int autoServiceId, AutoServiceOffering offering)
    {
        if (string.IsNullOrWhiteSpace(offering.Name) || offering.Price < 0 || offering.DurationMinutes <= 0)
            return null;

        if (offering.Id == 0)
        {
            offering.AutoServiceId = autoServiceId;
            offering.Name = offering.Name.Trim();
            offering.Description = string.IsNullOrWhiteSpace(offering.Description) ? null : offering.Description.Trim();
            db.AutoServiceOfferings.Add(offering);
        }
        else
        {
            var existing = await db.AutoServiceOfferings
                .FirstOrDefaultAsync(o => o.Id == offering.Id && o.AutoServiceId == autoServiceId);
            if (existing == null)
                return null;

            existing.Name = offering.Name.Trim();
            existing.Description = string.IsNullOrWhiteSpace(offering.Description) ? null : offering.Description.Trim();
            existing.Price = offering.Price;
            existing.DurationMinutes = offering.DurationMinutes;
            existing.IsActive = offering.IsActive;
            offering = existing;
        }

        await db.SaveChangesAsync();
        return offering;
    }

    public async Task<bool> DeleteOfferingAsync(int autoServiceId, int offeringId)
    {
        var offering = await db.AutoServiceOfferings
            .FirstOrDefaultAsync(o => o.Id == offeringId && o.AutoServiceId == autoServiceId);
        if (offering == null)
            return false;

        db.AutoServiceOfferings.Remove(offering);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<AutoServiceStaffMember>> GetStaffAsync(int autoServiceId) =>
        db.AutoServiceStaffMembers
            .Where(s => s.AutoServiceId == autoServiceId)
            .OrderBy(s => s.FullName)
            .ToListAsync();

    public async Task<AutoServiceStaffMember?> SaveStaffMemberAsync(int autoServiceId, AutoServiceStaffMember member)
    {
        if (string.IsNullOrWhiteSpace(member.FullName) || string.IsNullOrWhiteSpace(member.Position))
            return null;

        if (member.Id == 0)
        {
            member.AutoServiceId = autoServiceId;
            member.FullName = member.FullName.Trim();
            member.Position = member.Position.Trim();
            member.Phone = string.IsNullOrWhiteSpace(member.Phone) ? null : member.Phone.Trim();
            member.Email = string.IsNullOrWhiteSpace(member.Email) ? null : member.Email.Trim();
            db.AutoServiceStaffMembers.Add(member);
        }
        else
        {
            var existing = await db.AutoServiceStaffMembers
                .FirstOrDefaultAsync(s => s.Id == member.Id && s.AutoServiceId == autoServiceId);
            if (existing == null)
                return null;

            existing.FullName = member.FullName.Trim();
            existing.Position = member.Position.Trim();
            existing.Phone = string.IsNullOrWhiteSpace(member.Phone) ? null : member.Phone.Trim();
            existing.Email = string.IsNullOrWhiteSpace(member.Email) ? null : member.Email.Trim();
            existing.IsActive = member.IsActive;
            member = existing;
        }

        await db.SaveChangesAsync();
        return member;
    }

    public async Task<bool> DeleteStaffMemberAsync(int autoServiceId, int staffId)
    {
        var member = await db.AutoServiceStaffMembers
            .FirstOrDefaultAsync(s => s.Id == staffId && s.AutoServiceId == autoServiceId);
        if (member == null)
            return false;

        db.AutoServiceStaffMembers.Remove(member);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<AutoServiceAppointment>> GetAppointmentsAsync(int autoServiceId, DateTime from, DateTime to) =>
        db.AutoServiceAppointments
            .Include(a => a.Offering)
            .Include(a => a.StaffMember)
            .Where(a => a.AutoServiceId == autoServiceId
                        && a.ScheduledAt >= from
                        && a.ScheduledAt < to)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync();

    public async Task<AutoServiceAppointment?> SaveAppointmentAsync(int autoServiceId, AutoServiceAppointment appointment)
    {
        if (string.IsNullOrWhiteSpace(appointment.ClientName))
            return null;

        if (appointment.OfferingId != null)
        {
            var offering = await db.AutoServiceOfferings
                .FirstOrDefaultAsync(o => o.Id == appointment.OfferingId && o.AutoServiceId == autoServiceId);
            if (offering == null)
                return null;

            if (appointment.DurationMinutes <= 0)
                appointment.DurationMinutes = offering.DurationMinutes;
        }

        if (appointment.StaffMemberId != null)
        {
            var staffExists = await db.AutoServiceStaffMembers
                .AnyAsync(s => s.Id == appointment.StaffMemberId && s.AutoServiceId == autoServiceId && s.IsActive);
            if (!staffExists)
                appointment.StaffMemberId = null;
        }

        if (appointment.Id == 0)
        {
            appointment.AutoServiceId = autoServiceId;
            appointment.ClientName = appointment.ClientName.Trim();
            appointment.ClientPhone = string.IsNullOrWhiteSpace(appointment.ClientPhone) ? null : appointment.ClientPhone.Trim();
            appointment.Notes = string.IsNullOrWhiteSpace(appointment.Notes) ? null : appointment.Notes.Trim();
            db.AutoServiceAppointments.Add(appointment);
        }
        else
        {
            var existing = await db.AutoServiceAppointments
                .FirstOrDefaultAsync(a => a.Id == appointment.Id && a.AutoServiceId == autoServiceId);
            if (existing == null)
                return null;

            existing.OfferingId = appointment.OfferingId;
            existing.StaffMemberId = appointment.StaffMemberId;
            existing.ClientName = appointment.ClientName.Trim();
            existing.ClientPhone = string.IsNullOrWhiteSpace(appointment.ClientPhone) ? null : appointment.ClientPhone.Trim();
            existing.ScheduledAt = appointment.ScheduledAt;
            existing.DurationMinutes = appointment.DurationMinutes;
            existing.Status = appointment.Status;
            existing.Notes = string.IsNullOrWhiteSpace(appointment.Notes) ? null : appointment.Notes.Trim();
            appointment = existing;
        }

        await db.SaveChangesAsync();
        return appointment;
    }

    public async Task<bool> UpdateAppointmentStatusAsync(int autoServiceId, int appointmentId, AutoServiceAppointmentStatus status)
    {
        var appointment = await db.AutoServiceAppointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.AutoServiceId == autoServiceId);
        if (appointment == null)
            return false;

        appointment.Status = status;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAppointmentAsync(int autoServiceId, int appointmentId)
    {
        var appointment = await db.AutoServiceAppointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.AutoServiceId == autoServiceId);
        if (appointment == null)
            return false;

        db.AutoServiceAppointments.Remove(appointment);
        await db.SaveChangesAsync();
        return true;
    }
}
