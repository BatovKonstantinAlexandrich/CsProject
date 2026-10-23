using System.Security.Claims;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak
{
    /// <summary>
    /// Результат входа и текущая сессия приложения после успешной авторизации Keycloak.
    /// </summary>
    public static class KeycloakAuthSession
    {
        public static bool IsAuthenticated { get; set; }

        public static string AccessToken { get; set; }

        public static string IdToken { get; set; }

        public static string RefreshToken { get; set; }

        public static DateTimeOffset? AccessTokenExpiresAt { get; set; }

        public static ClaimsPrincipal User { get; set; }

        /// <summary>Удобочитаемое имя из токена (для UI и логов).</summary>
        public static string DisplayName { get; set; }

        public static void Clear()
        {
            IsAuthenticated = false;
            AccessToken = null;
            IdToken = null;
            RefreshToken = null;
            AccessTokenExpiresAt = null;
            User = null;
            DisplayName = null;
        }
    }
}
