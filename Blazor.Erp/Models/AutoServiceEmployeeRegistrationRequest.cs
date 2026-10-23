using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class AutoServiceEmployeeRegistrationRequest
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int AutoServiceId { get; set; }
    public AutoService AutoService { get; set; } = null!;
    public string StaffRole { get; set; } = string.Empty;
    public RegistrationRequestStatus Status { get; set; } = RegistrationRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
