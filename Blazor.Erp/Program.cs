using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using BlazorErp.Components;
using BlazorErp.Data;
using BlazorErp.Shared.DependencyInjection;
using BlazorErp.Models;
using BlazorErp.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.ComponentModel;

namespace BlazorErp;
 
public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=blazorerp.db";
        using (var db = new ApplicationDbContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        var githubClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        var githubClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
        var authBuilder = builder.Services.AddAuthentication();

        if (!string.IsNullOrEmpty(githubClientId) && !string.IsNullOrEmpty(githubClientSecret))
        {
            authBuilder.AddOAuth("GitHub", options =>
            {
                options.ClientId = githubClientId;
                options.ClientSecret = githubClientSecret;
                options.CallbackPath = "/signin-github";
                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                options.Scope.Add("read:user");
                options.Scope.Add("user:email");
                options.SaveTokens = true;
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var userRequest = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        userRequest.Headers.UserAgent.ParseAdd("BlazorErp");
                        var userResponse = await context.Backchannel.SendAsync(userRequest);
                        if (!userResponse.IsSuccessStatusCode)
                            return;

                        await using var userStream = await userResponse.Content.ReadAsStreamAsync();
                        using var userDoc = await JsonDocument.ParseAsync(userStream);
                        var root = userDoc.RootElement;

                        var identity = new ClaimsIdentity(context.Scheme.Name);
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, root.GetProperty("id").GetInt64().ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.Name, root.GetProperty("login").GetString()!));

                        if (root.TryGetProperty("email", out var emailProp) && emailProp.ValueKind == JsonValueKind.String)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Email, emailProp.GetString()!));
                        }
                        else
                        {
                            var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                            emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            emailRequest.Headers.UserAgent.ParseAdd("BlazorErp");
                            var emailResponse = await context.Backchannel.SendAsync(emailRequest);
                            if (emailResponse.IsSuccessStatusCode)
                            {
                                await using var emailStream = await emailResponse.Content.ReadAsStreamAsync();
                                using var emailDoc = await JsonDocument.ParseAsync(emailStream);
                                foreach (var email in emailDoc.RootElement.EnumerateArray())
                                {
                                    if (email.TryGetProperty("primary", out var primary) && primary.GetBoolean()
                                        && email.TryGetProperty("email", out var primaryEmail))
                                    {
                                        identity.AddClaim(new Claim(ClaimTypes.Email, primaryEmail.GetString()!));
                                        break;
                                    }
                                }
                            }
                        }

                        context.Principal = new ClaimsPrincipal(identity);
                    }
                };
            });
        }

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("ClientOnly", p => p.RequireRole(RoleNames.Client))
            .AddPolicy("TransportOnly", p => p.RequireRole(RoleNames.TransportCompany))
            .AddPolicy("StoreOwnerOnly", p => p.RequireRole(RoleNames.StoreOwner))
            .AddPolicy("StoreStaffOnly", p => p.RequireRole(RoleNames.StoreOwner, RoleNames.Seller, RoleNames.Cashier))
            .AddPolicy("CashierOnly", p => p.RequireRole(RoleNames.Cashier))
            .AddPolicy("MarketingOnly", p => p.RequireRole(RoleNames.Marketing, RoleNames.Analyst, RoleNames.Admin))
            .AddPolicy("AnalystOnly", p => p.RequireRole(RoleNames.Analyst))
            .AddPolicy("SupplierOnly", p => p.RequireRole(RoleNames.Supplier))
            .AddPolicy("AutoServiceOwnerOnly", p => p.RequireRole(RoleNames.AutoServiceOwner))
            .AddPolicy("AutoServiceStaffOnly", p => p.RequireRole(
                RoleNames.AutoServiceOwner,
                RoleNames.AutoMechanic,
                RoleNames.AutoElectrician,
                RoleNames.TireServiceWorker,
                RoleNames.CarWashWorker))
            .AddPolicy("AdminOnly", p => p.RequireRole(RoleNames.Admin));

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthStateProvider>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
        builder.Services.Configure<PaymentOptions>(builder.Configuration.GetSection(PaymentOptions.SectionName));
        builder.Services.Configure<ExternalAppsOptions>(builder.Configuration.GetSection(ExternalAppsOptions.SectionName));
        builder.Services.Configure<BookingApiOptions>(builder.Configuration.GetSection(BookingApiOptions.SectionName));
        builder.Services.Configure<SpsApiOptions>(builder.Configuration.GetSection(SpsApiOptions.SectionName));
        builder.Services.Configure<PayGwApiOptions>(builder.Configuration.GetSection(PayGwApiOptions.SectionName));
        builder.Services.Configure<VendorsSetupApiOptions>(builder.Configuration.GetSection(VendorsSetupApiOptions.SectionName));
        builder.Services.AddSingleton<IAccountEmailSender, SmtpAccountEmailSender>();
        builder.Services.AddScoped<IPaymentNotifier, PaymentNotifierService>();
        builder.Services.AddSignalR();
        builder.Services.AddScoped<IMessageNotifier, MessageNotifier>();
        builder.Services.AddScoped<INewsNotifier, NewsNotifier>();
        builder.Services.AddScoped<IOrderNotifier, OrderNotifier>();
        builder.Services.AddScoped<OrderFulfillmentService>();
        builder.Services.AddScoped<IMessageComposeDraftService, MessageComposeDraftService>();
        builder.Services.AddScoped<UsersHubConnectionService>();

        builder.Services.AddScoped<CatalogService>();
        builder.Services.AddScoped<CartService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<ReviewService>();
        builder.Services.AddScoped<StoreService>();
        builder.Services.AddScoped<SellerRegistrationService>();
        builder.Services.AddScoped<CashierRegistrationService>();
        builder.Services.AddScoped<CashierService>();
        builder.Services.AddScoped<MailingConfigurationService>();
        builder.Services.AddScoped<ReportService>();
        builder.Services.AddScoped<SupplierService>();
        builder.Services.AddScoped<AutoServiceService>();
        builder.Services.AddScoped<AutoServiceEmployeeRegistrationService>();
        builder.Services.AddScoped<BlazorErp.Services.MessageService>();
        builder.Services.AddScoped<NewsService>();
        builder.Services.AddScoped<NewsMediaStorageService>();
        builder.Services.AddScoped<StoreMediaStorageService>();
        builder.Services.AddScoped<FeedbackService>();
        builder.Services.AddScoped<DeveloperRolePreviewService>();

        builder.Services.AddHttpClient();
        builder.Services.AddHttpClient<StoBookingApiService>();
        builder.Services.AddHttpClient<SpsAuthService>();
        builder.Services.AddHttpClient<PayGwApiService>();
        builder.Services.AddHttpClient<VendorsSetupApiService>();
        builder.Services.AddMudServices();
        builder.Services.AddBlazorErpSharedViews();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddRazorPages();

        var app = builder.Build();

        await DbInitializer.InitializeAsync(app.Services);

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        if (!app.Environment.IsEnvironment("Testing"))
            app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapHub<BlazorErp.Hubs.UsersHub>("/hubs/users");
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}

public partial class Program { }
