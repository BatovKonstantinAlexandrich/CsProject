namespace BlazorErp.Models;

public class MessageNotificationDto
{
    public int MessageId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsReply { get; set; }
    public bool IsRead { get; set; }

    public static MessageNotificationDto FromMessage(Message message) => new()
    {
        MessageId = message.Id,
        Subject = message.Subject,
        Body = message.Body.Length > 120 ? message.Body[..120] + "…" : message.Body,
        SenderName = message.Sender?.FullName ?? "Пользователь",
        SenderId = message.SenderId,
        SentAt = message.SentAt,
        IsReply = message.ParentMessageId.HasValue,
        IsRead = message.IsRead
    };
}
