namespace BlazorErp.Models;

public class StoreCashier
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
