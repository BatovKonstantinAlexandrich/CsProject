using BlazorErp.Data;
using BlazorErp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class NewsService(
    ApplicationDbContext db,
    INewsNotifier newsNotifier,
    NewsMediaStorageService mediaStorage)
{
    public Task<List<NewsItem>> GetPublishedNewsAsync(int count = 10) =>
        db.NewsItems
            .Include(n => n.Author)
            .Include(n => n.Attachments)
            .Where(n => n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .Take(count)
            .ToListAsync();

    public Task<List<NewsItem>> GetUserNewsAsync(string authorId) =>
        db.NewsItems
            .Include(n => n.Attachments)
            .Where(n => n.AuthorId == authorId)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();

    public async Task<NewsItem> CreateNewsAsync(
        string authorId,
        string title,
        string content,
        IReadOnlyList<NewsUploadInput>? attachments = null)
    {
        if (attachments?.Count > NewsMediaStorageService.MaxFilesPerNews)
            throw new InvalidOperationException($"Можно прикрепить не более {NewsMediaStorageService.MaxFilesPerNews} файлов.");

        var item = new NewsItem
        {
            AuthorId = authorId,
            Title = title,
            Content = content
        };
        db.NewsItems.Add(item);
        await db.SaveChangesAsync();

        if (attachments is { Count: > 0 })
        {
            foreach (var upload in attachments)
            {
                var attachment = await mediaStorage.SaveAsync(item.Id, upload);
                db.NewsAttachments.Add(attachment);
            }

            await db.SaveChangesAsync();
            await db.Entry(item).Collection(n => n.Attachments).LoadAsync();
        }

        await db.Entry(item).Reference(n => n.Author).LoadAsync();
        await newsNotifier.NotifyNewsPublishedAsync(item);
        return item;
    }

    public async Task<bool> DeleteNewsAsync(int newsId, string userId, bool isAdmin = false)
    {
        var item = await db.NewsItems
            .Include(n => n.Attachments)
            .FirstOrDefaultAsync(n => n.Id == newsId);
        if (item == null)
            return false;

        if (item.AuthorId != userId && !isAdmin)
            return false;

        mediaStorage.DeleteNewsAttachments(item.Attachments);
        db.NewsItems.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }
}
