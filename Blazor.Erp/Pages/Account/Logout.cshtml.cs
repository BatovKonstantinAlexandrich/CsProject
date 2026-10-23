using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorErp.Pages.Account;

[IgnoreAntiforgeryToken]
public class LogoutModel(SignInManager<ApplicationUser> signInManager) : PageModel
{
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        await signInManager.SignOutAsync();
        returnUrl ??= Url.Content("~/");
        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        await signInManager.SignOutAsync();
        returnUrl ??= Url.Content("~/");
        return LocalRedirect(returnUrl);
    }
}
