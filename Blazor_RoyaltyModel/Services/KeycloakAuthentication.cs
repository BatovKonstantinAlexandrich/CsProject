using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Blazor_SiteModel.Services;

public static class KeycloakAuthentication
{
    public static void AddKeycloak(this WebApplicationBuilder builder)
    {
        var authority = ResolveAuthority(builder.Configuration);
        var clientId = First(builder.Configuration["Keycloak:ClientId"], builder.Configuration["KEYCLOAK_CLIENT_ID"]);
        var clientSecret = First(builder.Configuration["Keycloak:ClientSecret"], builder.Configuration["KEYCLOAK_CLIENT_SECRET"]);
        var requiredRole = builder.Configuration["Keycloak:RequiredRole"];

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.AccessDeniedPath = "/access-denied";
        })
        .AddOpenIdConnect(options =>
        {
            options.Authority = authority;
            options.MetadataAddress = authority + "/.well-known/openid-configuration";
            options.ConfigurationManager = new KeycloakConfigurationManager(options.MetadataAddress, authority);
            options.ClientId = clientId;
            if (!string.IsNullOrWhiteSpace(clientSecret))
                options.ClientSecret = clientSecret;

            options.ResponseType = OpenIdConnectResponseType.Code;
            options.ResponseMode = OpenIdConnectResponseMode.Query;
            options.UsePkce = true;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.MapInboundClaims = false;
            options.DisableTelemetry = true;
            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "role";

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");

            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.NonceCookie.SameSite = SameSiteMode.Lax;
            options.NonceCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            // ASP.NET Core после code-flow безусловно валидирует id_token.
            // Если Keycloak вернул 200 без id_token (только access_token) — IDX10000.
            options.Events.OnAuthorizationCodeReceived = context =>
                RedeemAuthorizationCodeAsync(context, requiredRole);

            options.Events.OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is ClaimsIdentity identity)
                    EnsureDisplayName(identity);

                AddRealmRoles(context.Principal);
                if (!string.IsNullOrWhiteSpace(requiredRole) && context.Principal?.IsInRole(requiredRole) != true)
                {
                    context.HandleResponse();
                    context.Response.Redirect("/access-denied");
                }

                return Task.CompletedTask;
            };
            options.Events.OnAccessDenied = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/access-denied");
                return Task.CompletedTask;
            };
            options.Events.OnRemoteFailure = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/access-denied");
                return Task.CompletedTask;
            };
        });

        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();
    }

    public static void MapKeycloakEndpoints(this WebApplication app)
    {
        app.MapGet("/account/login", (string? returnUrl) => Results.Challenge(
            new AuthenticationProperties { RedirectUri = LocalReturnUrl(returnUrl) },
            [OpenIdConnectDefaults.AuthenticationScheme]))
            .AllowAnonymous();

        app.MapGet("/account/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/access-denied");
        }).AllowAnonymous();
    }

    private static async Task RedeemAuthorizationCodeAsync(AuthorizationCodeReceivedContext context, string? requiredRole)
    {
        var request = context.TokenEndpointRequest
            ?? throw new InvalidOperationException("TokenEndpointRequest не создан.");

        var configuration = await context.Options.ConfigurationManager!
            .GetConfigurationAsync(context.HttpContext.RequestAborted);
        var tokenEndpoint = request.TokenEndpoint ?? configuration.TokenEndpoint;
        if (string.IsNullOrWhiteSpace(tokenEndpoint))
        {
            context.Fail("Не задан token endpoint Keycloak.");
            return;
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(request.Parameters),
            Version = context.Backchannel.DefaultRequestVersion
        };

        using var response = await context.Backchannel.SendAsync(httpRequest, context.HttpContext.RequestAborted);
        var content = await response.Content.ReadAsStringAsync(context.HttpContext.RequestAborted);

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
        }
        catch (JsonException ex)
        {
            context.Fail($"Ответ token endpoint не JSON ({(int)response.StatusCode}). {ex.Message}");
            return;
        }

        using (document)
        {
            var root = document.RootElement;
            if (!response.IsSuccessStatusCode)
            {
                var error = GetString(root, "error") ?? response.StatusCode.ToString();
                var description = GetString(root, "error_description");
                context.Fail(string.IsNullOrWhiteSpace(description) ? error : $"{error}: {description}");
                return;
            }

            if (root.TryGetProperty("error", out _))
            {
                var error = GetString(root, "error") ?? "invalid_token_response";
                var description = GetString(root, "error_description");
                context.Fail(string.IsNullOrWhiteSpace(description) ? error : $"{error}: {description}");
                return;
            }

            var idToken = GetString(root, "id_token");
            var accessToken = GetString(root, "access_token");
            var refreshToken = GetString(root, "refresh_token");
            var tokenType = GetString(root, "token_type");
            var expiresIn = root.TryGetProperty("expires_in", out var expires)
                ? expires.ToString()
                : null;

            if (!string.IsNullOrWhiteSpace(idToken))
            {
                context.HandleCodeRedemption(new OpenIdConnectMessage
                {
                    IdToken = idToken,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    TokenType = tokenType,
                    ExpiresIn = expiresIn
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                context.Fail("Ответ token endpoint не содержит id_token и access_token.");
                return;
            }

            // Без id_token стандартный handler падает с IDX10000 — входим через userinfo.
            var principal = await CreatePrincipalFromUserInfoAsync(
                context, configuration, accessToken, context.HttpContext.RequestAborted);
            if (principal is null)
            {
                context.Fail("Не удалось получить сведения пользователя из userinfo.");
                return;
            }

            if (principal.Identity is ClaimsIdentity identity)
                AddClaimsFromAccessToken(identity, accessToken);

            AddRealmRoles(principal);
            context.Principal = principal;

            if (!string.IsNullOrWhiteSpace(requiredRole) && !principal.IsInRole(requiredRole))
            {
                context.HandleResponse();
                context.Response.Redirect("/access-denied");
                return;
            }

            if (context.Options.SaveTokens)
            {
                var tokens = new List<AuthenticationToken>
                {
                    new() { Name = OpenIdConnectParameterNames.AccessToken, Value = accessToken }
                };
                if (!string.IsNullOrWhiteSpace(refreshToken))
                    tokens.Add(new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = refreshToken });
                if (!string.IsNullOrWhiteSpace(tokenType))
                    tokens.Add(new AuthenticationToken { Name = OpenIdConnectParameterNames.TokenType, Value = tokenType });
                if (!string.IsNullOrWhiteSpace(expiresIn) && int.TryParse(expiresIn, out var seconds))
                {
                    var expiresAt = DateTimeOffset.UtcNow.AddSeconds(seconds).ToUnixTimeSeconds().ToString();
                    tokens.Add(new AuthenticationToken { Name = "expires_at", Value = expiresAt });
                }

                context.Properties!.StoreTokens(tokens);
            }

            context.Success();
        }
    }

    private static async Task<ClaimsPrincipal?> CreatePrincipalFromUserInfoAsync(
        AuthorizationCodeReceivedContext context,
        OpenIdConnectConfiguration configuration,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var userInfoEndpoint = configuration.UserInfoEndpoint;
        if (string.IsNullOrWhiteSpace(userInfoEndpoint))
            return null;

        using var request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Version = context.Backchannel.DefaultRequestVersion;

        using var response = await context.Backchannel.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        using var user = JsonDocument.Parse(body);
        var identity = new ClaimsIdentity(
            authenticationType: context.Scheme.Name,
            nameType: context.Options.TokenValidationParameters.NameClaimType,
            roleType: context.Options.TokenValidationParameters.RoleClaimType);

        foreach (var property in user.RootElement.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.String)
            {
                var value = property.Value.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    identity.AddClaim(new Claim(property.Name, value));
            }
            else if (property.Value.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
            {
                identity.AddClaim(new Claim(property.Name, property.Value.ToString()));
            }
            else if (property.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
            {
                identity.AddClaim(new Claim(property.Name, property.Value.GetRawText()));
            }
        }

        EnsureDisplayName(identity);

        return new ClaimsPrincipal(identity);
    }

    private static void AddClaimsFromAccessToken(ClaimsIdentity identity, string accessToken)
    {
        var parts = accessToken.Split('.');
        if (parts.Length < 2)
            return;

        try
        {
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            using var document = JsonDocument.Parse(Convert.FromBase64String(payload));
            foreach (var name in new[]
                     {
                         "realm_access", "resource_access", "preferred_username", "ad_login",
                         "email", "name", "given_name", "family_name", "sub"
                     })
            {
                if (identity.HasClaim(c => c.Type == name) || !document.RootElement.TryGetProperty(name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.String)
                {
                    var text = value.GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                        identity.AddClaim(new Claim(name, text));
                }
                else if (value.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
                {
                    identity.AddClaim(new Claim(name, value.GetRawText()));
                }
            }

            EnsureDisplayName(identity);
        }
        catch (FormatException)
        {
            // access_token не JWT — роли останутся только из userinfo.
        }
        catch (JsonException)
        {
        }
    }

    private static void EnsureDisplayName(ClaimsIdentity identity)
    {
        if (identity.NameClaimType is null || identity.HasClaim(c => c.Type == identity.NameClaimType))
            return;

        var given = identity.FindFirst("given_name")?.Value;
        var family = identity.FindFirst("family_name")?.Value;
        var composed = string.Join(' ', new[] { family, given }.Where(v => !string.IsNullOrWhiteSpace(v)));

        var fallback = identity.FindFirst("name")?.Value
            ?? (string.IsNullOrWhiteSpace(composed) ? null : composed)
            ?? identity.FindFirst("preferred_username")?.Value
            ?? identity.FindFirst("email")?.Value
            ?? identity.FindFirst("sub")?.Value;

        if (!string.IsNullOrWhiteSpace(fallback))
            identity.AddClaim(new Claim(identity.NameClaimType, fallback));
    }

    private static void AddRealmRoles(ClaimsPrincipal? principal)
    {
        var realmAccess = principal?.FindFirst("realm_access")?.Value;
        if (principal?.Identity is not ClaimsIdentity identity || string.IsNullOrWhiteSpace(realmAccess))
            return;

        using var document = JsonDocument.Parse(realmAccess);
        if (!document.RootElement.TryGetProperty("roles", out var roles))
            return;

        foreach (var role in roles.EnumerateArray())
        {
            var value = role.GetString();
            if (!string.IsNullOrWhiteSpace(value))
                identity.AddClaim(new Claim("role", value));
        }
    }

    private static string? GetString(JsonElement root, string name) =>
        root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static string ResolveAuthority(IConfiguration configuration)
    {
        var authority = configuration["Keycloak:Authority"];
        if (!string.IsNullOrWhiteSpace(authority))
            return authority.TrimEnd('/');

        var url = configuration["KEYCLOAK_URL"]?.TrimEnd('/');
        var realm = configuration["KEYCLOAK_REALM"];
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(realm))
            throw new InvalidOperationException("Не задан адрес Keycloak. Укажите Keycloak:Authority или переменные KEYCLOAK_URL и KEYCLOAK_REALM.");

        return url.Contains("/realms/", StringComparison.OrdinalIgnoreCase)
            ? url
            : $"{url}/realms/{realm}";
    }

    private static string LocalReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return "/";
        if (returnUrl.StartsWith('/') && !returnUrl.StartsWith("//") && !returnUrl.Contains('\\'))
            return returnUrl;
        return "/";
    }

    private static string First(string? primary, string? fallback) =>
        !string.IsNullOrWhiteSpace(primary) ? primary : fallback ?? "";

    private sealed class KeycloakConfigurationManager(string metadataAddress, string authority) : IConfigurationManager<OpenIdConnectConfiguration>
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> inner = new(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever { RequireHttps = metadataAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase) });

        private readonly string authorizationEndpoint = authority.TrimEnd('/') + "/protocol/openid-connect/auth";
        private readonly string tokenEndpoint = authority.TrimEnd('/') + "/protocol/openid-connect/token";
        private readonly string userInfoEndpoint = authority.TrimEnd('/') + "/protocol/openid-connect/userinfo";
        private readonly string endSessionEndpoint = authority.TrimEnd('/') + "/protocol/openid-connect/logout";

        public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        {
            var configuration = await inner.GetConfigurationAsync(cancel);
            // IdentityModel 7.x иногда не заполняет endpoints из документа Keycloak.
            if (string.IsNullOrEmpty(configuration.AuthorizationEndpoint))
                configuration.AuthorizationEndpoint = authorizationEndpoint;
            if (string.IsNullOrEmpty(configuration.TokenEndpoint))
                configuration.TokenEndpoint = tokenEndpoint;
            if (string.IsNullOrEmpty(configuration.UserInfoEndpoint))
                configuration.UserInfoEndpoint = userInfoEndpoint;
            if (string.IsNullOrEmpty(configuration.EndSessionEndpoint))
                configuration.EndSessionEndpoint = endSessionEndpoint;
            return configuration;
        }

        public void RequestRefresh() => inner.RequestRefresh();
    }
}
