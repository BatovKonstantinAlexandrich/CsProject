using BlazorErp.Hubs;
using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class NewsNotifier(
    IHubContext<UsersHub> hubContext,
    UserManager<ApplicationUser> userManager) : INewsNotifier
{
    public async Task NotifyNewsPublishedAsync(NewsItem newsItem)
    {
        var dto = NewsNotificationDto.FromNewsItem(newsItem);
        var userIds = await userManager.Users
            .Where(u => u.Id != newsItem.AuthorId)
            .Select(u => u.Id)
            .ToListAsync();

        foreach (var userId in userIds)
        {
            await hubContext.Clients
                .Group(userId)
                .SendAsync(UsersHub.ReceiveNewsNotification, dto);
        }
    }
}
