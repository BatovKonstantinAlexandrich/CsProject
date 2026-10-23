using BlazorErp.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorErp.Services;

public class DeveloperRolePreviewService(
    IHttpContextAccessor httpContextAccessor,
    AuthenticationStateProvider authenticationStateProvider)
{
    public string? GetPreviewRole()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        return session?.GetString(DeveloperRolePreview.SessionKey);
    }

    public async Task SetPreviewRoleAsync(string? role)
    {
        var context = httpContextAccessor.HttpContext;
        if (context?.Session == null)
            return;

        if (string.IsNullOrEmpty(role))
            context.Session.Remove(DeveloperRolePreview.SessionKey);
        else
            context.Session.SetString(DeveloperRolePreview.SessionKey, role);

        await context.Session.CommitAsync();

        if (authenticationStateProvider is IdentityRevalidatingAuthStateProvider provider)
            provider.NotifyPreviewRoleChanged();
    }
}
