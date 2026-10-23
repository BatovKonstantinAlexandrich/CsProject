using BlazorErp.Models;
using BlazorErp.Models.Enums;
using BlazorErp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorErp.Pages.Account;

[Authorize]
public class RegisterPendingModel(
    SellerRegistrationService sellerRegistrationService,
    CashierRegistrationService cashierRegistrationService,
    UserManager<ApplicationUser> userManager) : PageModel
{
    public string StoreName { get; set; } = "";
    public string StaffRoleLabel { get; set; } = "сотрудника";
    public bool IsRejected { get; set; }
    public bool IsApproved { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToPage("/Account/Login");

        if (await userManager.IsInRoleAsync(user, RoleNames.Seller))
        {
            IsApproved = true;
            StaffRoleLabel = "продавца";
            var request = await sellerRegistrationService.GetLatestRequestForUserAsync(user.Id);
            StoreName = request?.Store?.Name ?? "магазин";
            return Page();
        }

        if (await userManager.IsInRoleAsync(user, RoleNames.Cashier))
        {
            IsApproved = true;
            StaffRoleLabel = "кассира";
            var request = await cashierRegistrationService.GetLatestRequestForUserAsync(user.Id);
            StoreName = request?.Store?.Name ?? "магазин";
            return Page();
        }

        var sellerLatest = await sellerRegistrationService.GetLatestRequestForUserAsync(user.Id);
        var cashierLatest = await cashierRegistrationService.GetLatestRequestForUserAsync(user.Id);

        if (sellerLatest?.Status == RegistrationRequestStatus.Pending)
        {
            StaffRoleLabel = "продавца";
            StoreName = sellerLatest.Store?.Name ?? "магазин";
            return Page();
        }

        if (cashierLatest?.Status == RegistrationRequestStatus.Pending)
        {
            StaffRoleLabel = "кассира";
            StoreName = cashierLatest.Store?.Name ?? "магазин";
            return Page();
        }

        if (sellerLatest?.Status == RegistrationRequestStatus.Rejected)
        {
            IsRejected = true;
            StaffRoleLabel = "продавца";
            StoreName = sellerLatest.Store?.Name ?? "магазин";
            return Page();
        }

        if (cashierLatest?.Status == RegistrationRequestStatus.Rejected)
        {
            IsRejected = true;
            StaffRoleLabel = "кассира";
            StoreName = cashierLatest.Store?.Name ?? "магазин";
            return Page();
        }

        return RedirectToPage("/");
    }
}
