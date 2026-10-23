using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Services;

public class AutoServiceEmployeeRegistrationService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    MessageService messageService)
{
    public Task<List<AutoServiceRegistrationOption>> GetAutoServicesForRegistrationAsync() =>
        db.AutoServices
            .OrderBy(s => s.Name)
            .Select(s => new AutoServiceRegistrationOption
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address
            })
            .ToListAsync();

    public async Task<AutoServiceEmployeeRegistrationRequest> SubmitRegistrationRequestAsync(
        string userId, int autoServiceId, string staffRole)
    {
        if (!AutoServiceStaffRoles.IsStaffRole(staffRole))
            throw new InvalidOperationException("Недопустимая роль сотрудника автосервиса.");

        var autoService = await db.AutoServices
            .Include(s => s.Owner)
            .FirstOrDefaultAsync(s => s.Id == autoServiceId)
            ?? throw new InvalidOperationException("Автосервис не найден.");

        if (await HasAnyPendingStaffRequestAsync(userId))
            throw new InvalidOperationException("У вас уже есть заявка на рассмотрении.");

        if (await db.AutoServiceEmployees.AnyAsync(e => e.UserId == userId))
            throw new InvalidOperationException("Вы уже зарегистрированы как сотрудник автосервиса.");

        if (await db.StoreSellers.AnyAsync(s => s.UserId == userId)
            || await db.StoreCashiers.AnyAsync(c => c.UserId == userId))
            throw new InvalidOperationException("Вы уже работаете в магазине.");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Пользователь не найден.");

        var request = new AutoServiceEmployeeRegistrationRequest
        {
            UserId = userId,
            AutoServiceId = autoServiceId,
            StaffRole = staffRole,
            Status = RegistrationRequestStatus.Pending
        };

        db.AutoServiceEmployeeRegistrationRequests.Add(request);
        await db.SaveChangesAsync();

        var roleLabel = AutoServiceStaffRoles.GetDisplayName(staffRole);
        var address = string.IsNullOrWhiteSpace(autoService.Address) ? "" : $", {autoService.Address}";
        await messageService.SendMessageAsync(
            userId,
            autoService.OwnerId,
            $"Заявка ({roleLabel}): {user.FullName}",
            $"""
            Пользователь {user.FullName} ({user.Email}) подал заявку на роль «{roleLabel}» в автосервисе «{autoService.Name}»{address}.

            Подтвердите или отклоните заявку в разделе «Автосервис» → «Заявки сотрудников».
            """);

        return request;
    }

    public Task<AutoServiceEmployeeRegistrationRequest?> GetPendingRequestForUserAsync(string userId) =>
        db.AutoServiceEmployeeRegistrationRequests
            .Include(r => r.AutoService)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending);

    public Task<AutoServiceEmployeeRegistrationRequest?> GetLatestRequestForUserAsync(string userId) =>
        db.AutoServiceEmployeeRegistrationRequests
            .Include(r => r.AutoService)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

    public Task<List<AutoServiceEmployeeRegistrationRequest>> GetPendingRequestsForOwnerAsync(string ownerId) =>
        db.AutoServiceEmployeeRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.AutoService)
            .Where(r => r.AutoService.OwnerId == ownerId && r.Status == RegistrationRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> ApproveRequestAsync(int requestId, string ownerId)
    {
        var request = await db.AutoServiceEmployeeRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.AutoService)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.AutoService.OwnerId == ownerId);

        if (request == null || request.Status != RegistrationRequestStatus.Pending)
            return false;

        if (!AutoServiceStaffRoles.IsStaffRole(request.StaffRole))
            return false;

        if (await db.AutoServiceEmployees.AnyAsync(e => e.UserId == request.UserId))
            return false;

        request.Status = RegistrationRequestStatus.Approved;
        request.ProcessedAt = DateTime.UtcNow;

        db.AutoServiceEmployees.Add(new AutoServiceEmployee
        {
            UserId = request.UserId,
            AutoServiceId = request.AutoServiceId,
            StaffRole = request.StaffRole
        });

        await userManager.AddToRoleAsync(request.User, request.StaffRole);
        await db.SaveChangesAsync();

        var roleLabel = AutoServiceStaffRoles.GetDisplayName(request.StaffRole);
        await messageService.SendMessageAsync(
            ownerId,
            request.UserId,
            "Заявка одобрена",
            $"""
            Ваша заявка на работу ({roleLabel}) в автосервисе «{request.AutoService.Name}» одобрена.

            Теперь вы можете войти в систему и работать в разделе «Автосервис».
            """);

        return true;
    }

    public async Task<bool> RejectRequestAsync(int requestId, string ownerId)
    {
        var request = await db.AutoServiceEmployeeRegistrationRequests
            .Include(r => r.User)
            .Include(r => r.AutoService)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.AutoService.OwnerId == ownerId);

        if (request == null || request.Status != RegistrationRequestStatus.Pending)
            return false;

        request.Status = RegistrationRequestStatus.Rejected;
        request.ProcessedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var roleLabel = AutoServiceStaffRoles.GetDisplayName(request.StaffRole);
        await messageService.SendMessageAsync(
            ownerId,
            request.UserId,
            "Заявка отклонена",
            $"""
            Ваша заявка на работу ({roleLabel}) в автосервисе «{request.AutoService.Name}» отклонена владельцем.

            Вы можете связаться с владельцем через сообщения или подать заявку в другой автосервис (потребуется новая регистрация с другим email).
            """);

        return true;
    }

    private async Task<bool> HasAnyPendingStaffRequestAsync(string userId) =>
        await db.AutoServiceEmployeeRegistrationRequests
            .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending)
        || await db.StoreSellerRegistrationRequests
            .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending)
        || await db.StoreCashierRegistrationRequests
            .AnyAsync(r => r.UserId == userId && r.Status == RegistrationRequestStatus.Pending);
}
