using BlazorErp.Models;

namespace BlazorErp.Services;

public interface IOrderNotifier
{
    Task NotifyOrderTakenForDeliveryAsync(Order order, ApplicationUser? transportCompany);
    Task NotifyPrepareOrderForDeliveryAsync(Order order, FulfillmentAssignment assignment);
    Task NotifyOrderReadyForTransportAsync(Order order);
}
