using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class StoreCashierRegistrationRequest
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public RegistrationRequestStatus Status { get; set; } = RegistrationRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
