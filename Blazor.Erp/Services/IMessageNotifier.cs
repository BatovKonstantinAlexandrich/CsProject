using BlazorErp.Models;
using Microsoft.AspNetCore.SignalR;

namespace BlazorErp.Services;

public interface IMessageNotifier
{
    Task NotifyNewMessageAsync(Message message);
    Task SendPendingNotificationsAsync(string userId, IClientProxy client);
}
