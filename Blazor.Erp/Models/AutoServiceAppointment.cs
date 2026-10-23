using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class AutoServiceAppointment
{
    public int Id { get; set; }
    public int AutoServiceId { get; set; }
    public AutoService AutoService { get; set; } = null!;
    public int? OfferingId { get; set; }
    public AutoServiceOffering? Offering { get; set; }
    public int? StaffMemberId { get; set; }
    public AutoServiceStaffMember? StaffMember { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ClientPhone { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public AutoServiceAppointmentStatus Status { get; set; } = AutoServiceAppointmentStatus.Scheduled;
    public string? Notes { get; set; }
}
