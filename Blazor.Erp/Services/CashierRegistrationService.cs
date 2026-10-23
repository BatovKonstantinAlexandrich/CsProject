using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class CashierRegistrationService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    MessageService messageService)
{
    public async Task<StoreCashierRegistrationRequest> SubmitRegistrationRequestAsync(string userId, int storeId)
    {
        var store = await db.Stores
            .Include(s => s.Owner)
            .FirstOrDefaultAsync(s => s.Id == storeId)
            ?? throw new InvalidOperationException("Магазин не найден.");

        if (await db.StoreCashierRegistrationRequests
                .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending)
            || await db.StoreSellerRegistrationRequests
                .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending)
            || await db.AutoServiceEmployeeRegistrationRequests
                .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending))
            throw new InvalidOperationException("У вас уже есть заявка на рассмотрении.");

        if (await db.StoreCashiers.AnyAsync(s => s.UserId == userId))
            throw new InvalidOperationException("Вы уже зарегистрированы как кассир.");

        if (await db.StoreSellers.AnyAsync(s => s.UserId == userId))
            throw new InvalidOperationException("Вы уже зарегистрированы как продавец.");

        if (await db.AutoServiceEmployees.AnyAsync(e => e.UserId == userId))
            throw new InvalidOperationException("Вы уже работаете в автосервисе.");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Пользователь не найден.");

        var request = new StoreCashierRegistrationRequest
        {
            UserId = userId,
            StoreId = storeId,
            Status = RegistrationRequestStatus.Pending
        };

        db.StoreCashierRegistrationRequests.Add(request);
        await db.SaveChangesAsync();

        var address = string.IsNullOrWhiteSpace(store.Address) ? "" : $", {store.Address}";
        await messageService.SendMessageAsync(
            userId,
            store.OwnerId,
            $"Заявка кассира: {user.FullName}",
            $"""
            Пользователь {user.FullName} ({user.Email}) подал заявку на роль кассира в магазине «{store.Name}»{address}.

            Подтвердите или отклоните заявку в разделе «Мой магазин» → «Заявки сотрудников».
            """);

        return request;
    }

    public Task<StoreCashierRegistrationRequest?> GetPendingRequestForUserAsync(string userId) =>
        db.StoreCashierRegistrationRequests
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending);

    public Task<StoreCashierRegistrationRequest?> GetLatestRequestForUserAsync(string userId) =>
        db.StoreCashierRegistrationRequests
            .Include(r => r.Store)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

    public Task<List<StoreCashierRegistrationRequest>> GetPendingRequestsForStoreOwnerAsync(string ownerId) =>
        db.StoreCashierRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.Store)
            .Where(r => r.Store.OwnerId == ownerId && r.Status == RegistrationRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> ApproveRequestAsync(int requestId, string ownerId)
    {
        var request = await db.StoreCashierRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.Store.OwnerId == ownerId);

        if (request == null || request.Status != RegistrationRequestStatus.Pending)
            return false;

        if (await db.StoreCashiers.AnyAsync(s => s.UserId == request.UserId))
            return false;

        request.Status = RegistrationRequestStatus.Approved;
        request.ProcessedAt = DateTime.UtcNow;

        db.StoreCashiers.Add(new StoreCashier
        {
            UserId = request.UserId,
            StoreId = request.StoreId
        });

        await userManager.AddToRoleAsync(request.User, RoleNames.Cashier);
        await db.SaveChangesAsync();

        await messageService.SendMessageAsync(
            ownerId,
            request.UserId,
            "Заявка кассира одобрена",
            $"""
            Ваша заявка на работу кассиром в магазине «{request.Store.Name}» одобрена.

            Теперь вы можете войти в систему и работать в разделе «Мой магазин».
            """);

        return true;
    }

    public async Task<bool> RejectRequestAsync(int requestId, string ownerId)
    {
        var request = await db.StoreCashierRegistrationRequests
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
            "Заявка кассира отклонена",
            $"""
            Ваша заявка на работу кассиром в магазине «{request.Store.Name}» отклонена владельцем.

            Вы можете связаться с владельцем магазина через сообщения или подать заявку в другой магазин (потребуется новая регистрация с другим email).
            """);

        return true;
    }
}
