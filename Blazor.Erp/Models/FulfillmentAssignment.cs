namespace BlazorErp.Models;

public record FulfillmentAssignment(
    int? StoreId,
    string LocationName,
    string? LocationAddress,
    bool IsCentralWarehouse,
    IReadOnlyList<string> RecipientUserIds);
