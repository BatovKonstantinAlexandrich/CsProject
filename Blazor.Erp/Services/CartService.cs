using BlazorErp.Data;
using BlazorErp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class CartService(ApplicationDbContext db)
{
    public Task<List<CartItem>> GetCartAsync(string userId) =>
        db.CartItems
            .Include(c => c.Product)
            .ThenInclude(p => p.Category)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.AddedAt)
            .ToListAsync();

    public async Task AddToCartAsync(string userId, int productId, int quantity = 1)
    {
        var existing = await db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            db.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            });
        }

        await db.SaveChangesAsync();
    }

    public async Task UpdateQuantityAsync(string userId, int cartItemId, int quantity)
    {
        var item = await db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        if (item == null) return;

        if (quantity <= 0)
            db.CartItems.Remove(item);
        else
            item.Quantity = quantity;

        await db.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(string userId, int cartItemId)
    {
        var item = await db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        if (item != null)
        {
            db.CartItems.Remove(item);
            await db.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var items = await db.CartItems.Where(c => c.UserId == userId).ToListAsync();
        db.CartItems.RemoveRange(items);
        await db.SaveChangesAsync();
    }

    public Task<int> GetCartCountAsync(string userId) =>
        db.CartItems.Where(c => c.UserId == userId).SumAsync(c => c.Quantity);
}
