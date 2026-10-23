using BlazorErp.Models;
using BlazorErp.Models.Enums;

namespace BlazorErp.Services;

public class NewsMediaStorageService(IWebHostEnvironment environment)
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024;
    public const int MaxFilesPerNews = 10;

    private static readonly HashSet<string> AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private static readonly HashSet<string> AllowedVideoExtensions = [".mp4", ".webm", ".mov"];
    private static readonly HashSet<string> AllowedFileExtensions =
        [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip", ".txt", ".csv"];

    public static NewsAttachmentType ClassifyMediaType(string contentType, string fileName)
    {
        if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
            || AllowedImageExtensions.Contains(Path.GetExtension(fileName)))
            return NewsAttachmentType.Image;

        if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
            || AllowedVideoExtensions.Contains(Path.GetExtension(fileName)))
            return NewsAttachmentType.Video;

        return NewsAttachmentType.File;
    }

    public static bool IsAllowedFile(string contentType, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (AllowedImageExtensions.Contains(extension)
            || AllowedVideoExtensions.Contains(extension)
            || AllowedFileExtensions.Contains(extension))
            return true;

        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
            || contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
            || contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("document", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("spreadsheet", StringComparison.OrdinalIgnoreCase)
            || contentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase)
            || contentType.Equals("application/zip", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<NewsAttachment> SaveAsync(int newsItemId, NewsUploadInput upload)
    {
        if (upload.Content.Length == 0)
            throw new InvalidOperationException("Файл пустой.");

        if (upload.Content.Length > MaxFileSizeBytes)
            throw new InvalidOperationException($"Файл «{upload.FileName}» превышает 10 МБ.");

        if (!IsAllowedFile(upload.ContentType, upload.FileName))
            throw new InvalidOperationException($"Тип файла «{upload.FileName}» не поддерживается.");

        var directory = GetNewsDirectory(newsItemId);
        Directory.CreateDirectory(directory);

        var extension = Path.GetExtension(upload.FileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(directory, storedFileName);

        await File.WriteAllBytesAsync(fullPath, upload.Content);

        return new NewsAttachment
        {
            NewsItemId = newsItemId,
            OriginalFileName = Path.GetFileName(upload.FileName),
            StoredFileName = storedFileName,
            ContentType = string.IsNullOrWhiteSpace(upload.ContentType) ? "application/octet-stream" : upload.ContentType,
            MediaType = ClassifyMediaType(upload.ContentType, upload.FileName),
            FileSize = upload.Content.Length
        };
    }

    public void DeleteNewsAttachments(IEnumerable<NewsAttachment> attachments)
    {
        foreach (var group in attachments.GroupBy(a => a.NewsItemId))
        {
            var directory = GetNewsDirectory(group.Key);
            foreach (var attachment in group)
            {
                var path = Path.Combine(directory, attachment.StoredFileName);
                if (File.Exists(path))
                    File.Delete(path);
            }

            if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
                Directory.Delete(directory);
        }
    }

    public static string GetPublicUrl(NewsAttachment attachment) =>
        $"/uploads/news/{attachment.NewsItemId}/{attachment.StoredFileName}";

    private string GetNewsDirectory(int newsItemId) =>
        Path.Combine(environment.WebRootPath, "uploads", "news", newsItemId.ToString());
}
