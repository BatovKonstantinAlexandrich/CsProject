namespace BlazorErp.Models;

public class AutoService
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = null!;
    public List<AutoServiceOffering> Offerings { get; set; } = [];
    public List<AutoServiceStaffMember> Staff { get; set; } = [];
    public List<AutoServiceAppointment> Appointments { get; set; } = [];
}
