namespace BlazorErp.Models;

public class AutoServiceOffering
{
    public int Id { get; set; }
    public int AutoServiceId { get; set; }
    public AutoService AutoService { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;
}
