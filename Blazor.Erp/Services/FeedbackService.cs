using BlazorErp.Data;
using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlazorErp.Services;

public class FeedbackService(
    ApplicationDbContext db,
    MessageService messageService,
    UserManager<ApplicationUser> userManager)
{
    public async Task SubmitAsync(string name, string email, string subject, string body, string? userId = null)
    {
        var feedback = new Feedback
        {
            Name = name.Trim(),
            Email = email.Trim(),
            Subject = subject.Trim(),
            Body = body.Trim(),
            UserId = userId
        };

        db.Feedbacks.Add(feedback);
        await db.SaveChangesAsync();

        if (userId == null)
            return;

        var admin = await userManager.FindByEmailAsync("admin@demo.ru");
        if (admin == null || admin.Id == userId)
            return;

        await messageService.SendMessageAsync(
            userId,
            admin.Id,
            $"[Обратная связь] {feedback.Subject}",
            $"От: {feedback.Name} ({feedback.Email})\n\n{feedback.Body}");
    }
}
