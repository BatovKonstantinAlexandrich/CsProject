using BlazorErp.Models;

namespace BlazorErp.Services;

public interface INewsNotifier
{
    Task NotifyNewsPublishedAsync(NewsItem newsItem);
}
