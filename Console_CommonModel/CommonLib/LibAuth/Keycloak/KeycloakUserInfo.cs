using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak
{
    /// <summary>
    /// Загрузка профиля через OIDC UserInfo (Bearer access_token).
    /// </summary>
    public static class KeycloakUserInfo
    {
        private static readonly HttpClient Http = new HttpClient();

        /// <summary>
        /// Запрос к Keycloak userinfo; claims с источником <c>KeycloakUserInfo</c>.
        /// </summary>
        public static async Task<ClaimsIdentity> FetchClaimsIdentityAsync(
            KeycloakAuthOptions options,
            string accessToken,
            CancellationToken cancellationToken = default)
        {
            if (options is null || string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException("Нужны options и access_token.");
            }

            using var req = new HttpRequestMessage(HttpMethod.Get, options.GetUserInfoEndpoint());
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var resp = await Http.SendAsync(req, cancellationToken).ConfigureAwait(false);
            var json = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"UserInfo {(int)resp.StatusCode}: {json}");
            }

            return ParseUserInfoJson(json);
        }

        /// <summary>
        /// Объединяет principal из id_token с claims из UserInfo.
        /// UserInfo добавляется первым — <see cref="ClaimsPrincipal.FindFirst"/> отдаёт приоритет полям профиля с сервера.
        /// </summary>
        public static ClaimsPrincipal Merge(ClaimsPrincipal idTokenPrincipal, ClaimsIdentity userInfoIdentity)
        {
            var merged = new ClaimsPrincipal();
            if (userInfoIdentity != null)
            {
                merged.AddIdentity(userInfoIdentity);
            }

            if (idTokenPrincipal != null)
            {
                foreach (var identity in idTokenPrincipal.Identities)
                {
                    merged.AddIdentity(new ClaimsIdentity(identity));
                }
            }

            return merged;
        }

        private static ClaimsIdentity ParseUserInfoJson(string json)
        {
            var id = new ClaimsIdentity("KeycloakUserInfo");
            using var doc = JsonDocument.Parse(json);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                AddJsonPropertyAsClaims(id, prop.Name, prop.Value);
            }

            return id;
        }

        private static void AddJsonPropertyAsClaims(ClaimsIdentity id, string name, JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var nested in el.EnumerateObject())
                    {
                        AddJsonPropertyAsClaims(id, name + "." + nested.Name, nested.Value);
                    }

                    break;
                case JsonValueKind.Array:
                    var i = 0;
                    foreach (var item in el.EnumerateArray())
                    {
                        AddJsonPropertyAsClaims(id, $"{name}[{i}]", item);
                        i++;
                    }

                    break;
                case JsonValueKind.String:
                    id.AddClaim(new Claim(name, el.GetString() ?? ""));
                    break;
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    id.AddClaim(new Claim(name, el.GetRawText()));
                    break;
                case JsonValueKind.Null:
                    break;
                default:
                    id.AddClaim(new Claim(name, el.GetRawText()));
                    break;
            }
        }
    }
}
