using System.Security.Claims;
using BlazorErp.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlazorErp.Services;

public class IdentityRevalidatingAuthStateProvider(
    ILoggerFactory loggerFactory,
    IServiceScopeFactory scopeFactory,
    IOptions<IdentityOptions> options,
    IHttpContextAccessor httpContextAccessor)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    public void NotifyPreviewRoleChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = await base.GetAuthenticationStateAsync();
        var user = ApplyDeveloperRolePreview(state.User);
        return new AuthenticationState(user);
    }

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(authenticationState.User);
        if (user == null)
            return false;

        var securityStamp = authenticationState.User.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
        if (securityStamp == null)
            return true;

        var dbUser = await userManager.FindByIdAsync(user.Id);
        return dbUser?.SecurityStamp == securityStamp;
    }

    internal ClaimsPrincipal ApplyDeveloperRolePreview(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true || !user.IsInRole(RoleNames.Developer))
            return user;

        var previewRole = httpContextAccessor.HttpContext?.Session.GetString(DeveloperRolePreview.SessionKey);
        if (string.IsNullOrEmpty(previewRole) || !RoleNames.PreviewableRoles.Contains(previewRole))
            return user;

        var claims = user.Claims
            .Where(c => c.Type is not (ClaimTypes.Role or DeveloperRolePreview.PreviewRoleClaimType))
            .ToList();

        claims.Add(new Claim(ClaimTypes.Role, RoleNames.Developer));
        claims.Add(new Claim(ClaimTypes.Role, previewRole));
        claims.Add(new Claim(DeveloperRolePreview.PreviewRoleClaimType, previewRole));

        var identity = user.Identity as ClaimsIdentity;
        var newIdentity = new ClaimsIdentity(
            claims,
            identity?.AuthenticationType,
            identity?.NameClaimType ?? ClaimTypes.Name,
            identity?.RoleClaimType ?? ClaimTypes.Role);
        return new ClaimsPrincipal(newIdentity);
    }
}
