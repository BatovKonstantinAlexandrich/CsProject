using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BlazorErp.Tests;

[CollectionDefinition("WebApp", DisableParallelization = true)]
public class WebAppCollection : ICollectionFixture<BlazorErpWebApplicationFactory>;

[Collection("WebApp")]
public class PageSmokeTests
{
    private readonly BlazorErpWebApplicationFactory _factory;

    public PageSmokeTests(BlazorErpWebApplicationFactory factory) => _factory = factory;

    public static IEnumerable<object[]> PublicPages =>
    [
        ["/"],
        ["/Error"],
        ["/Account/Login"],
        ["/Account/ForgotPassword"],
        ["/Account/ForgotPasswordConfirmation"],
        ["/Account/ResetPasswordConfirmation"],
        ["/Account/Register"],
        ["/Account/AccessDenied"],
    ];

    [Theory]
    [MemberData(nameof(PublicPages))]
    public async Task PublicPage_OpensWithoutServerError(string url)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
        await PageTestHelper.AssertPageOpensAsync(client, url);
    }

    [Fact]
    public async Task LoginPage_ContainsSignInForm()
    {
        var client = _factory.CreateClient();
        var html = await PageTestHelper.ReadInitialHtmlAsync(client, "/Account/Login");
        Assert.Contains("Вход в систему", html);
    }

    [Theory]
    [InlineData("/cart")]
    [InlineData("/checkout")]
    [InlineData("/orders")]
    [InlineData("/catalog")]
    [InlineData("/catalog/1")]
    [InlineData("/account")]
    [InlineData("/messages")]
    public async Task ClientPage_OpensWithoutServerError(string url)
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(
            _factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword);
        await PageTestHelper.AssertPageOpensAsync(client, url);
    }

    [Fact]
    public async Task OrderPayPage_OpensWithoutServerError()
    {
        var orderId = await TestDataHelper.CreatePendingPaymentOrderAsync(_factory.Services);
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(
            _factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword);
        await PageTestHelper.AssertPageOpensAsync(client, $"/orders/{orderId}/pay");
    }

    [Fact]
    public async Task OrderReviewPage_OpensWithoutServerError()
    {
        var orderId = await TestDataHelper.CreateDeliveredOrderAsync(_factory.Services);
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(
            _factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword);
        await PageTestHelper.AssertPageOpensAsync(client, $"/orders/{orderId}/review");
    }

    [Fact]
    public async Task TransportPage_OpensWithoutServerError()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(
            _factory, TestDataHelper.TransportEmail, TestDataHelper.TransportPassword);
        await PageTestHelper.AssertPageOpensAsync(client, "/transport");
    }

    [Fact]
    public async Task StorePage_OpensWithoutServerError()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(
            _factory, TestDataHelper.StoreEmail, TestDataHelper.StorePassword);
        await PageTestHelper.AssertPageOpensAsync(client, "/store");
    }

    [Fact]
    public async Task SupplierPage_OpensWithoutServerError()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(
            _factory, TestDataHelper.SupplierEmail, TestDataHelper.SupplierPassword);
        await PageTestHelper.AssertPageOpensAsync(client, "/supplier");
    }

    [Fact]
    public async Task AllRegisteredPages_OpenWithoutServerError()
    {
        var payOrderId = await TestDataHelper.CreatePendingPaymentOrderAsync(_factory.Services);
        var reviewOrderId = await TestDataHelper.CreateDeliveredOrderAsync(_factory.Services);

        var pages = new (string Url, Func<Task<HttpClient>> ClientFactory)[]
        {
            ("/", () => Task.FromResult(_factory.CreateClient())),
            ("/Error", () => Task.FromResult(_factory.CreateClient())),
            ("/Account/Login", () => Task.FromResult(_factory.CreateClient())),
            ("/Account/Register", () => Task.FromResult(_factory.CreateClient())),
            ("/Account/AccessDenied", () => Task.FromResult(_factory.CreateClient())),
            ("/catalog", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ("/cart", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ("/checkout", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ("/orders", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ($"/orders/{payOrderId}/pay", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ($"/orders/{reviewOrderId}/review", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ("/account", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ("/messages", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.ClientEmail, TestDataHelper.ClientPassword)),
            ("/transport", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.TransportEmail, TestDataHelper.TransportPassword)),
            ("/store", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.StoreEmail, TestDataHelper.StorePassword)),
            ("/supplier", () => AuthTestHelper.CreateAuthenticatedClientAsync(_factory, TestDataHelper.SupplierEmail, TestDataHelper.SupplierPassword)),
        };

        var failures = new List<string>();

        foreach (var (url, clientFactory) in pages)
        {
            var httpClient = await clientFactory();
            var status = await PageTestHelper.GetStatusCodeAsync(httpClient, url);
            if (status == HttpStatusCode.InternalServerError)
                failures.Add($"{url} → 500");
            else if (status is < HttpStatusCode.OK or >= HttpStatusCode.MultipleChoices)
                failures.Add($"{url} → {(int)status} {status}");
        }

        Assert.True(failures.Count == 0, "Страницы с ошибками:\n" + string.Join("\n", failures));
    }
}
