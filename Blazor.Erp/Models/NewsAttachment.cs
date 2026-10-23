using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class NewsAttachment
{
    public int Id { get; set; }
    public int NewsItemId { get; set; }
    public NewsItem NewsItem { get; set; } = null!;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public NewsAttachmentType MediaType { get; set; }
    public long FileSize { get; set; }
}
