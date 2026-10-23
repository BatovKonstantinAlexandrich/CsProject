using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak
{
    /// <summary>
    /// Авторизация через браузер: Authorization Code + PKCE, локальный redirect, обмен кода на токены, проверка id_token.
    /// </summary>
    public static class KeycloakPkceLogin
    {
        private static readonly HttpClient Http = new HttpClient();

        private static void Log(string message) => Console.WriteLine("[Keycloak] " + message);

        public sealed class Result
        {
            public bool Success { get; init; }
            public string Error { get; init; }
            public string AccessToken { get; init; }
            public string IdToken { get; init; }
            public string RefreshToken { get; init; }
            public DateTimeOffset? ExpiresAt { get; init; }
            public ClaimsPrincipal User { get; init; }
        }

        public static async Task<Result> LoginAsync(Action<Result> oncompleted, KeycloakAuthOptions options, CancellationToken cancellationToken = default)
            => await LoginAsync(options, cancellationToken, oncompleted);


        public static async Task<Result> LoginAsync(KeycloakAuthOptions options, CancellationToken cancellationToken = default, Action<Result> oncompleted = null)
        {
            if (options is null || string.IsNullOrWhiteSpace(options.Authority)
                || string.IsNullOrWhiteSpace(options.ClientId)
                || string.IsNullOrWhiteSpace(options.RedirectUri))
            {
                Log("Ошибка: не заданы Authority, ClientId или RedirectUri.");
                return new Result { Success = false, Error = "Не заданы параметры Keycloak (Authority, ClientId, RedirectUri)." };
            }

            if (!options.IsRealmBaseUrl && string.IsNullOrWhiteSpace(options.Realm))
            {
                Log("Ошибка: не задан Realm при Authority без /realms/…");
                return new Result { Success = false, Error = "Укажите Realm или задайте Authority как …/realms/имя_реалма." };
            }

            var redirect = new Uri(options.RedirectUri);
            if (!string.Equals(redirect.Scheme, "http", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(redirect.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                Log("Ошибка: RedirectUri должен быть http:// или https://.");
                return new Result { Success = false, Error = "RedirectUri должен быть http:// или https://." };
            }

            Log($"Вход: client_id={options.ClientId}, redirect={options.RedirectUri}");

            var codeVerifier = CreateCodeVerifier();
            var codeChallenge = CreateCodeChallenge(codeVerifier);
            var state = CreateState();

            using var listener = new HttpListener();
            foreach (var prefix in GetListenerPrefixes(redirect))
            {
                listener.Prefixes.Add(prefix);
            }

            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex)
            {
                Log($"Ошибка HttpListener.Start: {ex.Message}");
                var prefixes = string.Join(", ", listener.Prefixes);
                return new Result
                {
                    Success = false,
                    Error =
                        "Не удалось слушать порт для OAuth redirect. Частые причины: порт занят (например, ng serve на 4200), " +
                        "или нет прав резервирования URL. Остановите процесс на этом порту или выполните от администратора:\n" +
                        $"netsh http add urlacl url=http://localhost:{redirect.Port}/ user=%USERNAME%\n" +
                        $"netsh http add urlacl url=http://127.0.0.1:{redirect.Port}/ user=%USERNAME%\n" +
                        $"Префиксы: {prefixes}. Система: {ex.Message}",
                };
            }

            if (!listener.IsListening)
            {
                Log("Ошибка: после Start() IsListening = false.");
                try { listener.Stop(); } catch { /* ignore */ }
                return new Result
                {
                    Success = false,
                    Error = "HttpListener.Start() выполнен, но IsListening = false — порт не прослушивается.",
                };
            }

            var probeError = await VerifyLocalPortAcceptsTcpAsync(redirect.Port, cancellationToken).ConfigureAwait(false);
            if (probeError is not null)
            {
                Log("Ошибка: TCP-проб порта не прошёл: " + probeError);
                try { listener.Stop(); } catch { /* ignore */ }
                return new Result
                {
                    Success = false,
                    Error = probeError,
                };
            }

            Log($"HttpListener активен, порт {redirect.Port}. Префиксы: {string.Join("; ", listener.Prefixes)}");

            try
            {
                var scope = string.IsNullOrWhiteSpace(options.Scopes) ? "openid profile email" : options.Scopes;
                var authUrl = string.Join("&",
                    $"{options.GetAuthorizationEndpoint()}?",
                    $"client_id={Uri.EscapeDataString(options.ClientId)}",
                    $"response_type=code",
                    $"scope={Uri.EscapeDataString(scope)}",
                    $"redirect_uri={Uri.EscapeDataString(options.RedirectUri)}",
                    $"state={Uri.EscapeDataString(state)}",
                    $"code_challenge={Uri.EscapeDataString(codeChallenge)}",
                    $"code_challenge_method=S256");

                Log("Открываю системный браузер для авторизации.");
                Process.Start(new ProcessStartInfo
                {
                    FileName = authUrl,
                    UseShellExecute = true,
                });

                Log("Ожидаю redirect на локальный callback…");
                var context = await listener.GetContextAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
                Log($"Получен HTTP-запрос: {context.Request.HttpMethod} {context.Request.Url?.PathAndQuery}");
                var request = context.Request;
                var response = context.Response;

                if (!string.Equals(request.Url.AbsolutePath.TrimEnd('/'), redirect.AbsolutePath.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                {
                    Log($"Ошибка: неверный путь callback (ожидался {redirect.AbsolutePath}).");
                    await WriteResponseAsync(response, 400, "Неверный путь callback.").ConfigureAwait(false);
                    return new Result { Success = false, Error = "Ожидался redirect на " + options.RedirectUri };
                }

                var query = ParseQuery(request.Url.Query);
                if (query.TryGetValue("error", out var oauthError) && !string.IsNullOrEmpty(oauthError))
                {
                    query.TryGetValue("error_description", out var desc);
                    if (string.IsNullOrEmpty(desc))
                    {
                        desc = oauthError;
                    }

                    Log("Ошибка от IdP: " + desc);
                    await WriteResponseAsync(response, 400, WebUtility.HtmlEncode(desc)).ConfigureAwait(false);
                    return new Result { Success = false, Error = desc };
                }

                if (!query.TryGetValue("state", out var returnedState) || returnedState != state)
                {
                    Log("Ошибка: неверный state.");
                    await WriteResponseAsync(response, 400, "Неверный state.").ConfigureAwait(false);
                    return new Result { Success = false, Error = "Подмена state / устаревшая сессия." };
                }

                if (!query.TryGetValue("code", out var code) || string.IsNullOrEmpty(code))
                {
                    Log("Ошибка: в query нет authorization code.");
                    await WriteResponseAsync(response, 400, "Нет authorization code.").ConfigureAwait(false);
                    return new Result { Success = false, Error = "Сервер не вернул code." };
                }

                OpenIdConnectConfiguration oidcConfig;
                try
                {
                    var cm = new ConfigurationManager<OpenIdConnectConfiguration>(
                        options.GetMetadataAddress(),
                        new OpenIdConnectConfigurationRetriever(),
                        new HttpDocumentRetriever { RequireHttps = options.Authority.StartsWith("https", StringComparison.OrdinalIgnoreCase) });
                    oidcConfig = await cm.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log("Ошибка OpenID discovery: " + ex.Message);
                    await WriteResponseAsync(response, 500, "Ошибка загрузки OpenID metadata.").ConfigureAwait(false);
                    return new Result { Success = false, Error = $"Discovery: {ex.Message}" };
                }

                Log("Загрузка metadata завершена. Обмен code на токены…");
                string accessToken;
                string idToken;
                string refreshToken;
                int expiresIn;
                try
                {
                    (accessToken, idToken, refreshToken, expiresIn) = await ExchangeCodeAsync(
                        options,
                        code,
                        codeVerifier,
                        cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log("Ошибка token endpoint: " + ex.Message);
                    await WriteResponseAsync(response, 500, "Ошибка обмена кода на токены.").ConfigureAwait(false);
                    return new Result { Success = false, Error = ex.Message };
                }

                ClaimsPrincipal principal = null;
                if (!string.IsNullOrEmpty(idToken))
                {
                    if (options.SkipIdTokenValidation)
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(idToken);
                        principal = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "Keycloak"));
                    }
                    else
                    {
                        try
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var validationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = oidcConfig.Issuer,
                                ValidIssuers = oidcConfig.Issuer != null ? new[] { oidcConfig.Issuer, oidcConfig.Issuer.TrimEnd('/') } : null,
                                ValidateAudience = true,
                                ValidAudiences = new[] { options.ClientId, "account" },
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKeys = oidcConfig.SigningKeys,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.FromMinutes(2),
                            };
                            principal = handler.ValidateToken(idToken, validationParameters, out _);
                        }
                        catch (Exception ex)
                        {
                            Log("Ошибка проверки id_token: " + ex.Message);
                            await WriteResponseAsync(response, 400, "Токен не прошёл проверку.").ConfigureAwait(false);
                            return new Result { Success = false, Error = $"Проверка id_token: {ex.Message}" };
                        }
                    }
                }

                await WriteResponseAsync(response, 200,
                    "Авторизация выполнена. Это окно можно закрыть.").ConfigureAwait(false);

                var expiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(expiresIn, 0));
                Log("Вход выполнен успешно.");

                var r = new Result
                {
                    Success = true,
                    AccessToken = accessToken,
                    IdToken = idToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    User = principal,
                };
                if (oncompleted is not null)
                    oncompleted(r);
                return r;
            }
            finally
            {
                Log("HttpListener остановлен.");
                try { listener.Stop(); } catch { /* ignore */ }
            }
        }

        private static async Task<(string access, string id, string refresh, int expiresIn)> ExchangeCodeAsync(
            KeycloakAuthOptions options,
            string code,
            string codeVerifier,
            CancellationToken ct)
        {
            var body = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = options.RedirectUri,
                ["client_id"] = options.ClientId,
                ["code_verifier"] = codeVerifier,
            };

            if (!string.IsNullOrWhiteSpace(options.ClientSecret))
            {
                body["client_secret"] = options.ClientSecret;
            }

            using var content = new FormUrlEncodedContent(body);
            using var response = await Http.PostAsync(options.GetTokenEndpoint(), content, ct).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Token endpoint: {(int)response.StatusCode} {json}");
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var access = root.GetProperty("access_token").GetString();
            var id = root.TryGetProperty("id_token", out var idEl) ? idEl.GetString() : null;
            var refresh = root.TryGetProperty("refresh_token", out var refEl) ? refEl.GetString() : null;
            var expiresIn = root.TryGetProperty("expires_in", out var expEl) && expEl.TryGetInt32(out var sec) ? sec : 0;
            return (access, id, refresh, expiresIn);
        }

        private static async Task WriteResponseAsync(HttpListenerResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            response.ContentType = "text/html; charset=utf-8";
            var html = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>Keycloak</title></head><body><p>"
                + WebUtility.HtmlEncode(message)
                + "</p></body></html>";
            var bytes = Encoding.UTF8.GetBytes(html);
            response.ContentLength64 = bytes.Length;
            await response.OutputStream.WriteAsync(bytes).ConfigureAwait(false);
            response.OutputStream.Close();
        }

        /// <summary>
        /// Убеждаемся, что после <see cref="HttpListener.Start"/> порт реально слушает TCP (http.sys).
        /// </summary>
        private static async Task<string> VerifyLocalPortAcceptsTcpAsync(int port, CancellationToken cancellationToken)
        {
            static async Task<bool> TryOnce(IPAddress address, int p, CancellationToken ct)
            {
                try
                {
                    using var tcp = new TcpClient();
                    using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    timeout.CancelAfter(TimeSpan.FromSeconds(2));
                    await tcp.ConnectAsync(address, p, timeout.Token).ConfigureAwait(false);
                    return tcp.Connected;
                }
                catch
                {
                    return false;
                }
            }

            if (await TryOnce(IPAddress.Loopback, port, cancellationToken).ConfigureAwait(false))
            {
                return null;
            }

            if (await TryOnce(IPAddress.IPv6Loopback, port, cancellationToken).ConfigureAwait(false))
            {
                return null;
            }

            return
                $"После запуска HttpListener порт {port} не принимает TCP на 127.0.0.1 и ::1. " +
                "Часто порт занят другим приложением (например ng serve). Освободите порт или измените RedirectUri в appsettings и в Keycloak.";
        }

        /// <summary>
        /// Браузер после Keycloak открывает <c>RedirectUri</c> (часто <c>localhost</c>).
        /// На Windows запрос может пойти на 127.0.0.1 или ::1 — регистрируем оба, плюс имя localhost.
        /// </summary>
        private static IEnumerable<string> GetListenerPrefixes(Uri redirect)
        {
            var scheme = redirect.Scheme;
            var port = redirect.Port;
            var host = redirect.Host;

            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                yield return $"{scheme}://localhost:{port}/";
                yield return $"{scheme}://127.0.0.1:{port}/";
                if (string.Equals(scheme, "http", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scheme, "https", StringComparison.OrdinalIgnoreCase))
                {
                    yield return $"{scheme}://[::1]:{port}/";
                }
            }
            else
            {
                yield return $"{scheme}://{host}:{port}/";
            }
        }

        private static Dictionary<string, string> ParseQuery(string query)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(query))
            {
                return d;
            }

            var q = query.StartsWith("?") ? query.Substring(1) : query;
            foreach (var part in q.Split('&'))
            {
                var idx = part.IndexOf('=');
                if (idx < 0)
                {
                    d[Uri.UnescapeDataString(part)] = "";
                }
                else
                {
                    d[Uri.UnescapeDataString(part.Substring(0, idx))] =
                        Uri.UnescapeDataString(part.Substring(idx + 1));
                }
            }

            return d;
        }

        private static string CreateCodeVerifier()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        private static string CreateCodeChallenge(string verifier)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(verifier));
            return Base64UrlEncode(hash);
        }

        private static string CreateState()
        {
            var bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
