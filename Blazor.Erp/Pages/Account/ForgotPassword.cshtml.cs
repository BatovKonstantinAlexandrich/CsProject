using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using BlazorErp.Models;
using BlazorErp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorErp.Pages.Account;

public class ForgotPasswordModel(
    UserManager<ApplicationUser> userManager,
    IAccountEmailSender emailSender,
    IWebHostEnvironment environment) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user is not { EmailConfirmed: true })
            return RedirectToPage("./ForgotPasswordConfirmation");

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { code, email = Input.Email },
            protocol: Request.Scheme)!;

        await emailSender.SendPasswordResetLinkAsync(
            Input.Email,
            "Восстановление пароля BlazorErp",
            $"Для сброса пароля перейдите по ссылке: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>сбросить пароль</a>.");

        if (environment.IsDevelopment())
            TempData["DevResetLink"] = callbackUrl;

        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
