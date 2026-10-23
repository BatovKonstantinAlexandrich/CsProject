using BlazorErp.Data;
using BlazorErp.Hubs;
using BlazorErp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class MessageNotifier(
    IHubContext<UsersHub> hubContext,
    IServiceScopeFactory scopeFactory) : IMessageNotifier
{
    public async Task NotifyNewMessageAsync(Message message)
    {
        var dto = MessageNotificationDto.FromMessage(message);
        await hubContext.Clients
            .Group(message.RecipientId)
            .SendAsync(UsersHub.ReceiveMessageNotification, dto);
    }

    public async Task SendPendingNotificationsAsync(string userId, IClientProxy client)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var unread = await db.Messages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Where(m => m.RecipientId == userId && !m.IsRead && m.ParentMessageId == null)
            .OrderByDescending(m => m.SentAt)
            .Take(10)
            .ToListAsync();

        foreach (var message in unread)
        {
            await client.SendAsync(
                UsersHub.ReceiveMessageNotification,
                MessageNotificationDto.FromMessage(message));
        }
    }
}
