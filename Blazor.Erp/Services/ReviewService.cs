using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class ReviewService(ApplicationDbContext db)
{
    public async Task<Review?> CreateReviewAsync(int orderId, string authorId, int rating, string comment)
    {
        var order = await db.Orders
            .Include(o => o.Review)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ClientId == authorId);

        if (order == null || order.Status != OrderStatus.Delivered || order.Review != null)
            return null;

        if (rating is < 1 or > 5)
            return null;

        var review = new Review
        {
            OrderId = orderId,
            AuthorId = authorId,
            Rating = rating,
            Comment = comment
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync();
        return review;
    }
}
