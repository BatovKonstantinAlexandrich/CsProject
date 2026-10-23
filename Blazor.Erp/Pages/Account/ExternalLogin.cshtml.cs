using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BlazorErp.Data;
using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorErp.Pages.Account;

public class ExternalLoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext db) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ProviderDisplayName { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = RoleNames.Client;
    }

    public IActionResult OnGet() => RedirectToPage("/Account/Login");

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!string.IsNullOrEmpty(remoteError))
        {
            ErrorMessage = $"Ошибка провайдера: {remoteError}";
            return RedirectToPage("/Account/Login");
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToPage("/Account/Login");

        var signInResult = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded)
            return LocalRedirect(returnUrl);

        if (signInResult.IsLockedOut)
            return RedirectToPage("/Account/Login");

        ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider;
        var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
        var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;

        Input = new InputModel { Email = email, FullName = name };
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
        {
            ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider;
            return Page();
        }

        var role = Input.Role switch
        {
            RoleNames.TransportCompany => RoleNames.TransportCompany,
            RoleNames.StoreOwner => RoleNames.StoreOwner,
            RoleNames.Supplier => RoleNames.Supplier,
            RoleNames.AutoServiceOwner => RoleNames.AutoServiceOwner,
            _ => RoleNames.Client
        };

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FullName = Input.FullName,
                EmailConfirmed = true
            };
            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                ErrorMessage = string.Join(" ", createResult.Errors.Select(e => e.Description));
                ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider;
                return Page();
            }

            await userManager.AddToRoleAsync(user, role);

            if (role == RoleNames.StoreOwner)
            {
                db.Stores.Add(new Store { Name = $"Магазин {Input.FullName}", OwnerId = user.Id });
                await db.SaveChangesAsync();
            }
            else if (role == RoleNames.Supplier)
            {
                db.Suppliers.Add(new Supplier { Name = Input.FullName, OwnerId = user.Id });
                await db.SaveChangesAsync();
            }
            else if (role == RoleNames.AutoServiceOwner)
            {
                db.AutoServices.Add(new AutoService { Name = $"Автосервис {Input.FullName}", OwnerId = user.Id });
                await db.SaveChangesAsync();
            }
        }

        await userManager.AddLoginAsync(user, info);
        await signInManager.SignInAsync(user, isPersistent: false);
        return LocalRedirect(returnUrl);
    }
}
