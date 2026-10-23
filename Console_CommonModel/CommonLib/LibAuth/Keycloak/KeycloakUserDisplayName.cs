using System.Security.Claims;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak
{
    public static class KeycloakUserDisplayName
    {
        public static string FromPrincipal(ClaimsPrincipal principal)
        {
            if (principal is null)
            {
                return "(нет данных пользователя)";
            }

            if (principal.Identity?.IsAuthenticated == true
                && !string.IsNullOrWhiteSpace(principal.Identity.Name))
            {
                return principal.Identity.Name;
            }

            static string Claim(ClaimsPrincipal p, string type) =>
                p.FindFirst(type)?.Value;

            return Claim(principal, "preferred_username")
                ?? Claim(principal, "name")
                ?? Claim(principal, ClaimTypes.Name)
                ?? Claim(principal, "email")
                ?? Claim(principal, ClaimTypes.Email)
                ?? Claim(principal, "sub")
                ?? "(имя не найдено в claims — проверьте scope profile)";
        }
    }
}
