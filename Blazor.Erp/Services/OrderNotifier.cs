using BlazorErp.Hubs;
using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace BlazorErp.Services;

public class OrderNotifier(
    IHubContext<UsersHub> hubContext,
    UserManager<ApplicationUser> userManager,
    MessageService messageService) : IOrderNotifier
{
    public Task NotifyOrderTakenForDeliveryAsync(Order order, ApplicationUser? transportCompany)
    {
        var dto = OrderDeliveryNotificationDto.FromOrder(order, transportCompany);
        return hubContext.Clients
            .Group(order.ClientId)
            .SendAsync(UsersHub.ReceiveOrderDeliveryNotification, dto);
    }

    public async Task NotifyPrepareOrderForDeliveryAsync(Order order, FulfillmentAssignment assignment)
    {
        if (assignment.RecipientUserIds.Count == 0)
            return;

        var dto = OrderPreparationNotificationDto.FromOrder(order, assignment);
        foreach (var userId in assignment.RecipientUserIds.Distinct())
        {
            await hubContext.Clients
                .Group(userId)
                .SendAsync(UsersHub.ReceiveOrderPreparationNotification, dto);

            await messageService.SendMessageAsync(
                order.ClientId,
                userId,
                dto.ToMessageSubject(),
                dto.ToMessageBody(),
                notifyRecipient: false);
        }
    }

    public async Task NotifyOrderReadyForTransportAsync(Order order)
    {
        var dto = OrderReadyForTransportNotificationDto.FromOrder(order);
        var transportUsers = await userManager.GetUsersInRoleAsync(RoleNames.TransportCompany);

        foreach (var user in transportUsers)
        {
            await hubContext.Clients
                .Group(user.Id)
                .SendAsync(UsersHub.ReceiveOrderReadyForTransportNotification, dto);
        }
    }
}
