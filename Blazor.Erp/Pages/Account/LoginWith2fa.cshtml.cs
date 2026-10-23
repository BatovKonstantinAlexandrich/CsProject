using System.ComponentModel.DataAnnotations;
using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorErp.Pages.Account;

public class LoginWith2faModel(SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Введите код")]
        [StringLength(7, MinimumLength = 6)]
        [DataType(DataType.Text)]
        public string TwoFactorCode { get; set; } = string.Empty;

        public bool RememberMachine { get; set; }
    }

    public void OnGet(bool rememberMe, string? returnUrl = null)
    {
        Input.RememberMachine = rememberMe;
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return Page();

        var code = Input.TwoFactorCode.Replace(" ", "").Replace("-", "");
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(
            code, rememberMe, Input.RememberMachine);

        if (result.Succeeded)
            return LocalRedirect(returnUrl);

        ErrorMessage = "Неверный код.";
        return Page();
    }
}
