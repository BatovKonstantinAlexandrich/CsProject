using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class SellerRegistrationService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    MessageService messageService)
{
    public Task<List<StoreRegistrationOption>> GetStoresForRegistrationAsync() =>
        db.Stores
            .OrderBy(s => s.Name)
            .Select(s => new StoreRegistrationOption
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address
            })
            .ToListAsync();

    public async Task<StoreSellerRegistrationRequest> SubmitRegistrationRequestAsync(string userId, int storeId)
    {
        var store = await db.Stores
            .Include(s => s.Owner)
            .FirstOrDefaultAsync(s => s.Id == storeId)
            ?? throw new InvalidOperationException("Магазин не найден.");

        var existingPending = await db.StoreSellerRegistrationRequests
                .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending)
            || await db.StoreCashierRegistrationRequests
                .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending)
            || await db.AutoServiceEmployeeRegistrationRequests
                .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending);
        if (existingPending)
            throw new InvalidOperationException("У вас уже есть заявка на рассмотрении.");

        var alreadySeller = await db.StoreSellers.AnyAsync(s => s.UserId == userId);
        if (alreadySeller)
            throw new InvalidOperationException("Вы уже зарегистрированы как продавец.");

        if (await db.StoreCashiers.AnyAsync(s => s.UserId == userId))
            throw new InvalidOperationException("Вы уже зарегистрированы как кассир.");

        if (await db.AutoServiceEmployees.AnyAsync(e => e.UserId == userId))
            throw new InvalidOperationException("Вы уже работаете в автосервисе.");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Пользователь не найден.");

        var request = new StoreSellerRegistrationRequest
        {
            UserId = userId,
            StoreId = storeId,
            Status = RegistrationRequestStatus.Pending
        };

        db.StoreSellerRegistrationRequests.Add(request);
        await db.SaveChangesAsync();

        var address = string.IsNullOrWhiteSpace(store.Address) ? "" : $", {store.Address}";
        await messageService.SendMessageAsync(
            userId,
            store.OwnerId,
            $"Заявка продавца: {user.FullName}",
            $"""
            Пользователь {user.FullName} ({user.Email}) подал заявку на роль продавца в магазине «{store.Name}»{address}.

            Подтвердите или отклоните заявку в разделе «Мой магазин» → «Заявки сотрудников».
            """);

        return request;
    }

    public Task<StoreSellerRegistrationRequest?> GetPendingRequestForUserAsync(string userId) =>
        db.StoreSellerRegistrationRequests
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending);

    public Task<StoreSellerRegistrationRequest?> GetLatestRequestForUserAsync(string userId) =>
        db.StoreSellerRegistrationRequests
            .Include(r => r.Store)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

    public Task<List<StoreSellerRegistrationRequest>> GetPendingRequestsForStoreOwnerAsync(string ownerId) =>
        db.StoreSellerRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.Store)
            .Where(r => r.Store.OwnerId == ownerId && r.Status == RegistrationRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> ApproveRequestAsync(int requestId, string ownerId)
    {
        var request = await db.StoreSellerRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.Store.OwnerId == ownerId);

        if (request == null || request.Status != RegistrationRequestStatus.Pending)
            return false;

        if (await db.StoreSellers.AnyAsync(s => s.UserId == request.UserId))
            return false;

        request.Status = RegistrationRequestStatus.Approved;
        request.ProcessedAt = DateTime.UtcNow;

        db.StoreSellers.Add(new StoreSeller
        {
            UserId = request.UserId,
            StoreId = request.StoreId
        });

        await userManager.AddToRoleAsync(request.User, RoleNames.Seller);
        await db.SaveChangesAsync();

        await messageService.SendMessageAsync(
            ownerId,
            request.UserId,
            "Заявка продавца одобрена",
            $"""
            Ваша заявка на работу продавцом в магазине «{request.Store.Name}» одобрена.

            Теперь вы можете войти в систему и работать в разделе «Мой магазин».
            """);

        return true;
    }

    public async Task<bool> RejectRequestAsync(int requestId, string ownerId)
    {
        var request = await db.StoreSellerRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.Store.OwnerId == ownerId);

        if (request == null || request.Status != RegistrationRequestStatus.Pending)
            return false;

        request.Status = RegistrationRequestStatus.Rejected;
        request.ProcessedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        await messageService.SendMessageAsync(
            ownerId,
            request.UserId,
            "Заявка продавца отклонена",
            $"""
            Ваша заявка на работу продавцом в магазине «{request.Store.Name}» отклонена владельцем.

            Вы можете связаться с владельцем магазина через сообщения или подать заявку в другой магазин (потребуется новая регистрация с другим email).
            """);

        return true;
    }
}
