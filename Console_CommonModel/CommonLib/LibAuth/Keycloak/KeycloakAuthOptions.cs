using System.IO;

using Newtonsoft.Json.Linq;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak
{
    /// <summary>
    /// Параметры клиента Keycloak (OAuth 2.0 / OpenID Connect, PKCE).
    /// </summary>
    public sealed class KeycloakAuthOptions
    {
        /// <summary>Включить окно входа при старте. Если в JSON не указано — включается при наличии Authority и ClientId.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Либо только хост: <c>https://auth.example.com</c> (тогда нужен <see cref="Realm"/>),
        /// либо уже полный префикс реалма Keycloak: <c>https://auth.example.com/realms/myrealm</c>.
        /// </summary>
        public string Authority { get; set; } = "";

        /// <summary>Имя реалма; используется только если <see cref="Authority"/> не содержит <c>/realms/</c>.</summary>
        public string Realm { get; set; } = "";

        public string ClientId { get; set; } = "";

        /// <summary>Для confidential client — обязателен при обмене code на токены.</summary>
        public string ClientSecret { get; set; } = "";

        /// <summary>
        /// Зарегистрируйте этот URI в клиенте Keycloak (Valid redirect URIs), например <c>http://localhost:4200/callback</c>.
        /// WinForms поднимает <see cref="System.Net.HttpListener"/> на этом хосте/порту на время входа (порт не должен быть занят другим процессом).
        /// </summary>
        public string RedirectUri { get; set; } = "http://localhost:4200/callback";

        public string Scopes { get; set; } = "openid profile email";

        /// <summary>Не проверять подпись id_token (только для отладки).</summary>
        public bool SkipIdTokenValidation { get; set; }

        /// <summary>Authority уже указывает на корень реалма (…/realms/имя).</summary>
        public bool IsRealmBaseUrl =>
            !string.IsNullOrWhiteSpace(Authority)
            && Authority.IndexOf("/realms/", StringComparison.OrdinalIgnoreCase) >= 0;

        public static KeycloakAuthOptions LoadFromAppSettings(string path = null)
        {
            path ??= Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path))
            {
                return new KeycloakAuthOptions { Enabled = false };
            }

            var json = File.ReadAllText(path);
            var root = JObject.Parse(json);
            var section = root["Keycloak"] as JObject;
            if (section is null)
            {
                return new KeycloakAuthOptions { Enabled = false };
            }

            var opts = section.ToObject<KeycloakAuthOptions>() ?? new KeycloakAuthOptions();
            if (!section.TryGetValue("Enabled", out _))
            {
                opts.Enabled = !string.IsNullOrWhiteSpace(opts.Authority)
                    && !string.IsNullOrWhiteSpace(opts.ClientId);
            }

            return opts;
        }

        public string GetRealmBaseUrl()
        {
            var a = (Authority ?? "").TrimEnd('/');
            if (string.IsNullOrEmpty(a))
            {
                return a;
            }

            if (IsRealmBaseUrl)
            {
                return a;
            }

            var r = (Realm ?? "").Trim('/');
            if (string.IsNullOrEmpty(r))
            {
                return a;
            }

            return $"{a}/realms/{r}";
        }

        public string GetAuthorizationEndpoint()
            => $"{GetRealmBaseUrl()}/protocol/openid-connect/auth";

        public string GetTokenEndpoint()
            => $"{GetRealmBaseUrl()}/protocol/openid-connect/token";

        public string GetMetadataAddress()
            => $"{GetRealmBaseUrl()}/.well-known/openid-configuration";

        /// <summary>Стандартный OIDC userinfo (профиль при scope profile / email).</summary>
        public string GetUserInfoEndpoint()
            => $"{GetRealmBaseUrl()}/protocol/openid-connect/userinfo";
    }
}
