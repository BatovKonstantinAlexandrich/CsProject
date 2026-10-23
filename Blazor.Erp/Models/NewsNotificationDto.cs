namespace BlazorErp.Models;

public class NewsNotificationDto
{
    public int NewsId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string FullBody { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }

    public static NewsNotificationDto FromNewsItem(NewsItem item)
    {
        var preview = item.Content.Length > 120 ? item.Content[..120] + "…" : item.Content;
        return new NewsNotificationDto
        {
            NewsId = item.Id,
            Title = item.Title,
            Body = preview,
            FullBody = item.Content,
            AuthorName = item.Author?.FullName ?? "Пользователь",
            AuthorId = item.AuthorId,
            PublishedAt = item.PublishedAt
        };
    }
}
