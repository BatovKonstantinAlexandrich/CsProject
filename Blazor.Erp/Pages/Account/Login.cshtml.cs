using System.ComponentModel.DataAnnotations;
using BlazorErp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorErp.Pages.Account;

public class LoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public bool ShowExternalLogin { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        ShowExternalLogin = IsGitHubConfigured();
    }

    public string ReturnUrl { get; set; } = "/";

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= ReturnUrl;
        ShowExternalLogin = IsGitHubConfigured();

        if (!ModelState.IsValid)
            return Page();

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ErrorMessage = "Неверный email или пароль.";
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(
            user.UserName!, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return LocalRedirect(returnUrl);

        if (result.RequiresTwoFactor)
            return RedirectToPage("/Account/LoginWith2fa", new { returnUrl, Input.RememberMe });

        if (result.IsLockedOut)
        {
            ErrorMessage = "Аккаунт заблокирован. Попробуйте позже.";
            return Page();
        }

        ErrorMessage = "Неверный email или пароль.";
        return Page();
    }

    public IActionResult OnPostExternalLogin(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        var redirectUrl = Url.Page("/Account/ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);
        return Challenge(properties, "GitHub");
    }

    private bool IsGitHubConfigured()
    {
        var clientId = configuration["Authentication:GitHub:ClientId"];
        var clientSecret = configuration["Authentication:GitHub:ClientSecret"];
        return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret);
    }
}
