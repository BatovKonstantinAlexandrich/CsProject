using Newtonsoft.Json.Linq;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace Blazor_RoyaltyModel.Apis
{
    public class EuroautoApi(HttpClient http) : IAuthCodeApi
    {
        public async Task SendAuthCode(string phone)
        {
            using var request = CreateRequest($"https://v1.euroauto.ru/api/courier/auth/phone?phone={Uri.EscapeDataString(phone)}");
            using var response = await http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> SubmitAuthCode(string phone, string code)
        {
            using var request = CreateRequest($"https://v1.euroauto.ru/api/courier/auth/phone?phone={Uri.EscapeDataString(phone)}&code={Uri.EscapeDataString(code)}");
            using var response = await http.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(responseText);
            if (string.IsNullOrWhiteSpace(tokenResponse?.Token))
                throw new InvalidOperationException("Сервер не вернул токен.");

            return tokenResponse.Token;
        }

        private static HttpRequestMessage CreateRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "YjJjc2l0ZTo0Rm5KSlo2UllyUGU=");
            return request;
        }

        private sealed class TokenResponse
        {
            [JsonPropertyName("token")]
            public string? Token { get; set; }
        }

        public Dictionary<string, object> DecodeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Токен не задан.", nameof(token));

            var value = token.Trim();
            const string bearerPrefix = "Bearer ";
            if (value.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                value = value[bearerPrefix.Length..].Trim();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(value))
                throw new InvalidOperationException("Токен не является JWT.");

            var jwt = handler.ReadJwtToken(value);
            var payload = JObject.Parse(jwt.Payload.SerializeToJson());
            return payload.Properties().ToDictionary(
                property => property.Name,
                property => ClaimValue(property.Value),
                StringComparer.Ordinal);
        }

        private static object ClaimValue(JToken token) => token.Type switch
        {
            JTokenType.Object => ((JObject)token).Properties().ToDictionary(property => property.Name, property => ClaimValue(property.Value), StringComparer.Ordinal),
            JTokenType.Array => token.Select(ClaimValue).ToList(),
            JTokenType.Integer => token.Value<long>(),
            JTokenType.Float => token.Value<double>(),
            JTokenType.Boolean => token.Value<bool>(),
            JTokenType.Date => token.Value<DateTime>(),
            JTokenType.Null => string.Empty,
            _ => token.Value<string>() ?? string.Empty
        };
    }
}
