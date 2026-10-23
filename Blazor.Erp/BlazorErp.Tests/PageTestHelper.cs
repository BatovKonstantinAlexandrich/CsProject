using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BlazorErp.Tests;

public static class PageTestHelper
{
    public static async Task<HttpStatusCode> GetStatusCodeAsync(HttpClient client, string url)
    {
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        return response.StatusCode;
    }

    public static async Task AssertPageOpensAsync(HttpClient client, string url)
    {
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode,
            $"Страница {url} вернула {(int)response.StatusCode} {response.StatusCode}");
    }

    public static async Task<string> ReadInitialHtmlAsync(HttpClient client, string url, int maxBytes = 16384)
    {
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        var buffer = new byte[maxBytes];
        var read = await stream.ReadAsync(buffer.AsMemory(0, maxBytes));
        return Encoding.UTF8.GetString(buffer, 0, read);
    }
}
