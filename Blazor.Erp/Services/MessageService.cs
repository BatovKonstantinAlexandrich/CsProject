using BlazorErp.Data;
using BlazorErp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class MessageService(ApplicationDbContext db, IMessageNotifier messageNotifier)
{
    public async Task<Message> SendMessageAsync(
        string senderId,
        string recipientId,
        string subject,
        string body,
        int? parentMessageId = null,
        bool notifyRecipient = true)
    {
        var message = new Message
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Subject = subject,
            Body = body,
            ParentMessageId = parentMessageId
        };

        db.Messages.Add(message);
        await db.SaveChangesAsync();

        await db.Entry(message).Reference(m => m.Sender).LoadAsync();

        if (notifyRecipient)
            await messageNotifier.NotifyNewMessageAsync(message);

        return message;
    }

    public Task<List<Message>> GetInboxAsync(string userId) =>
        db.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Where(m => m.RecipientId == userId && m.ParentMessageId == null)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

    public Task<List<Message>> GetSentAsync(string userId) =>
        db.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Where(m => m.SenderId == userId && m.ParentMessageId == null)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

    public async Task<Message?> GetMessageThreadAsync(int messageId, string userId)
    {
        var root = await db.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.Replies).ThenInclude(r => r.Sender)
            .FirstOrDefaultAsync(m => m.Id == messageId && (m.SenderId == userId || m.RecipientId == userId));

        if (root == null)
            return null;

        var markedRead = false;

        if (root.RecipientId == userId && !root.IsRead)
        {
            root.IsRead = true;
            markedRead = true;
        }

        foreach (var reply in root.Replies.Where(r => r.RecipientId == userId && !r.IsRead))
        {
            reply.IsRead = true;
            markedRead = true;
        }

        if (markedRead)
            await db.SaveChangesAsync();

        return root;
    }

    public Task<int> GetUnreadCountAsync(string userId) =>
        db.Messages.CountAsync(m => m.RecipientId == userId && !m.IsRead && m.ParentMessageId == null);

    public Task<List<ApplicationUser>> GetUsersForMessagingAsync(string currentUserId) =>
        db.UserContext
            .Where(u => u.Id != currentUserId)
            .OrderBy(u => u.FullName)
            .ToListAsync();

    public async Task<bool> DeleteMessageAsync(int messageId, string userId)
    {
        var root = await db.Messages
            .Include(m => m.Replies)
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                m.ParentMessageId == null &&
                (m.SenderId == userId || m.RecipientId == userId));

        if (root == null)
            return false;

        if (root.Replies.Count > 0)
            db.Messages.RemoveRange(root.Replies);

        db.Messages.Remove(root);
        await db.SaveChangesAsync();
        return true;
    }
}
