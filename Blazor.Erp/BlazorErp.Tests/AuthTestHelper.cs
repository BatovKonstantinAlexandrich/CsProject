using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlazorErp.Tests;

public static class AuthTestHelper
{
    public static async Task LoginAsync(HttpClient client, string email, string password)
    {
        var loginPage = await client.GetAsync("/Account/Login");
        loginPage.EnsureSuccessStatusCode();
        var html = await loginPage.Content.ReadAsStringAsync();
        var token = ExtractAntiforgeryToken(html);

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Input.Email"] = email,
            ["Input.Password"] = password,
            ["Input.RememberMe"] = "false"
        };

        var response = await client.PostAsync("/Account/Login", new FormUrlEncodedContent(form));
        if (response.StatusCode is HttpStatusCode.Redirect or HttpStatusCode.Found or HttpStatusCode.OK)
            return;

        var body = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(
            $"Не удалось войти как {email}. Статус: {(int)response.StatusCode}. {body[..Math.Min(body.Length, 300)]}");
    }

    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        BlazorErpWebApplicationFactory factory,
        string email,
        string password)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });
        await LoginAsync(client, email, password);
        return client;
    }

    private static string ExtractAntiforgeryToken(string html)
    {
        var match = Regex.Match(html, @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""");
        if (!match.Success)
            throw new InvalidOperationException("Antiforgery-токен не найден на странице входа.");
        return match.Groups[1].Value;
    }
}
