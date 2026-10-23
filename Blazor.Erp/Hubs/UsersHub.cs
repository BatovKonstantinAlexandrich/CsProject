using System.Security.Claims;
using BlazorErp.Models;
using BlazorErp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BlazorErp.Hubs;

[Authorize]
public class UsersHub(IServiceScopeFactory scopeFactory) : Hub
{
    public const string ReceiveMessageNotification = "ReceiveMessageNotification";
    public const string ReceiveNewsNotification = "ReceiveNewsNotification";
    public const string ReceiveOrderDeliveryNotification = "ReceiveOrderDeliveryNotification";
    public const string ReceiveOrderPreparationNotification = "ReceiveOrderPreparationNotification";
    public const string ReceiveOrderReadyForTransportNotification = "ReceiveOrderReadyForTransportNotification";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnConnectedAsync();
    }

    public async Task RequestPendingNotifications()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return;

        await using var scope = scopeFactory.CreateAsyncScope();
        var notifier = scope.ServiceProvider.GetRequiredService<IMessageNotifier>();
        await notifier.SendPendingNotificationsAsync(userId, Clients.Caller);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

        await base.OnDisconnectedAsync(exception);
    }
}
