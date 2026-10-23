namespace BlazorErp.Models;

public class AutoServiceEmployee
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int AutoServiceId { get; set; }
    public AutoService AutoService { get; set; } = null!;
    public string StaffRole { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
