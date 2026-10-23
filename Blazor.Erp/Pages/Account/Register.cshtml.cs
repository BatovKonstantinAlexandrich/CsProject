using System.ComponentModel.DataAnnotations;
using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorErp.Pages.Account;

public class RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext db,
    SellerRegistrationService sellerRegistrationService,
    CashierRegistrationService cashierRegistrationService,
    AutoServiceEmployeeRegistrationService autoServiceEmployeeRegistrationService,
    IConfiguration configuration) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public bool ShowExternalLogin { get; set; }
    public List<StoreRegistrationOption> Stores { get; set; } = [];
    public List<AutoServiceRegistrationOption> AutoServices { get; set; } = [];

    public class InputModel
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = RoleNames.Client;

        [Display(Name = "Магазин")]
        public int? StoreId { get; set; }

        [Display(Name = "Автосервис")]
        public int? AutoServiceId { get; set; }
    }

    public async Task OnGetAsync()
    {
        ShowExternalLogin = IsGitHubConfigured();
        Stores = await sellerRegistrationService.GetStoresForRegistrationAsync();
        AutoServices = await autoServiceEmployeeRegistrationService.GetAutoServicesForRegistrationAsync();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ShowExternalLogin = IsGitHubConfigured();
        Stores = await sellerRegistrationService.GetStoresForRegistrationAsync();
        AutoServices = await autoServiceEmployeeRegistrationService.GetAutoServicesForRegistrationAsync();

        if ((Input.Role == RoleNames.Seller || Input.Role == RoleNames.Cashier)
            && (Input.StoreId == null || Input.StoreId <= 0))
            ModelState.AddModelError(nameof(Input.StoreId), "Выберите магазин.");

        if (AutoServiceStaffRoles.IsStaffRole(Input.Role)
            && (Input.AutoServiceId == null || Input.AutoServiceId <= 0))
            ModelState.AddModelError(nameof(Input.AutoServiceId), "Выберите автосервис.");

        if (!ModelState.IsValid)
            return Page();

        var role = Input.Role switch
        {
            RoleNames.TransportCompany => RoleNames.TransportCompany,
            RoleNames.StoreOwner => RoleNames.StoreOwner,
            RoleNames.Supplier => RoleNames.Supplier,
            RoleNames.AutoServiceOwner => RoleNames.AutoServiceOwner,
            RoleNames.AutoMechanic => RoleNames.AutoMechanic,
            RoleNames.AutoElectrician => RoleNames.AutoElectrician,
            RoleNames.TireServiceWorker => RoleNames.TireServiceWorker,
            RoleNames.CarWashWorker => RoleNames.CarWashWorker,
            RoleNames.Seller => RoleNames.Seller,
            RoleNames.Cashier => RoleNames.Cashier,
            RoleNames.TechnicalSupport => RoleNames.TechnicalSupport,
            RoleNames.CallCenter => RoleNames.CallCenter,
            RoleNames.Marketing => RoleNames.Marketing,
            _ => RoleNames.Client
        };

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }

        if (role == RoleNames.Seller)
        {
            try
            {
                await sellerRegistrationService.SubmitRegistrationRequestAsync(user.Id, Input.StoreId!.Value);
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(user);
                ErrorMessage = ex.Message;
                return Page();
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            TempData["StaffRegistrationPending"] = "Seller";
            return LocalRedirect("/account");
        }

        if (role == RoleNames.Cashier)
        {
            try
            {
                await cashierRegistrationService.SubmitRegistrationRequestAsync(user.Id, Input.StoreId!.Value);
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(user);
                ErrorMessage = ex.Message;
                return Page();
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            TempData["StaffRegistrationPending"] = "Cashier";
            return LocalRedirect("/account");
        }

        if (AutoServiceStaffRoles.IsStaffRole(role))
        {
            try
            {
                await autoServiceEmployeeRegistrationService.SubmitRegistrationRequestAsync(
                    user.Id, Input.AutoServiceId!.Value, role);
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(user);
                ErrorMessage = ex.Message;
                return Page();
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            TempData["AutoServiceStaffRegistrationPending"] = role;
            return LocalRedirect("/account");
        }

        await userManager.AddToRoleAsync(user, role);

        if (role == RoleNames.StoreOwner)
        {
            db.Stores.Add(new Store
            {
                Name = $"Магазин {Input.FullName}",
                OwnerId = user.Id
            });
            await db.SaveChangesAsync();
        }
        else if (role == RoleNames.Supplier)
        {
            db.Suppliers.Add(new Supplier
            {
                Name = Input.FullName,
                OwnerId = user.Id
            });
            await db.SaveChangesAsync();
        }
        else if (role == RoleNames.AutoServiceOwner)
        {
            db.AutoServices.Add(new AutoService
            {
                Name = $"Автосервис {Input.FullName}",
                OwnerId = user.Id
            });
            await db.SaveChangesAsync();
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        return LocalRedirect(returnUrl);
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
