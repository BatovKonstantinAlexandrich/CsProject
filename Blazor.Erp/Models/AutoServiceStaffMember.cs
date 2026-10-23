namespace BlazorErp.Models;

public class AutoServiceStaffMember
{
    public int Id { get; set; }
    public int AutoServiceId { get; set; }
    public AutoService AutoService { get; set; } = null!;
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
}
