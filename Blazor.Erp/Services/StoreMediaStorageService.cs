namespace BlazorErp.Services;

public class StoreMediaStorageService(IWebHostEnvironment environment)
{
    public const long MaxImageSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedImageExtensions =
        [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public static bool IsAllowedImage(string contentType, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (AllowedImageExtensions.Contains(extension))
            return true;

        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> SaveStoreImageAsync(int storeId, byte[] content, string fileName, string contentType)
    {
        if (content.Length == 0)
            throw new InvalidOperationException("Файл пустой.");

        if (content.Length > MaxImageSizeBytes)
            throw new InvalidOperationException("Изображение не должно превышать 5 МБ.");

        if (!IsAllowedImage(contentType, fileName))
            throw new InvalidOperationException("Допустимы изображения: JPG, PNG, GIF, WebP.");

        var directory = GetStoreDirectory(storeId);
        Directory.CreateDirectory(directory);

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension) || !AllowedImageExtensions.Contains(extension.ToLowerInvariant()))
            extension = ".jpg";

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(directory, storedFileName);
        await File.WriteAllBytesAsync(fullPath, content);

        return $"/uploads/stores/{storeId}/{storedFileName}";
    }

    public void DeleteStoreImageIfLocal(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl) || !IsLocalStoreImage(imageUrl))
            return;

        var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(environment.WebRootPath, relativePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        var directory = Path.GetDirectoryName(fullPath);
        if (directory != null && Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
            Directory.Delete(directory);
    }

    public static bool IsLocalStoreImage(string? imageUrl) =>
        !string.IsNullOrWhiteSpace(imageUrl)
        && imageUrl.StartsWith("/uploads/stores/", StringComparison.OrdinalIgnoreCase);

    private string GetStoreDirectory(int storeId) =>
        Path.Combine(environment.WebRootPath, "uploads", "stores", storeId.ToString());
}
